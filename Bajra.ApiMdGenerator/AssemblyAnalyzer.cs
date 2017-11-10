using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MicrosoftStuff;
using System.Web.Http;
using System.Diagnostics;

namespace Bajra.ApiMdGenerator
{
    public class AssemblyAnalyzer
    {
        #region Static Methods
        static bool IsSimpleType(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                // nullable type, check if the nested type is simple.
                return IsSimpleType(type.GetGenericArguments()[0]);
            }
            return type.IsPrimitive
              || type.IsEnum
              || type.Equals(typeof(string))
              || type.Equals(typeof(decimal));
        }

        public static string GetVersion(string assemblyPath)
        {
            try
            {
                FileVersionInfo myFileVersionInfo = FileVersionInfo.GetVersionInfo(assemblyPath);

                return myFileVersionInfo.FileVersion;
            }
            catch
            {
                return string.Empty;
            }

        }

        #endregion Static Methods

        #region Public Methods

        public IEnumerable<ApiControllerObj> GetApiControllerListForAssembly(string assemblyPath, string xmlPath = null)
        {
            Assembly asm = Assembly.LoadFile(assemblyPath);

            Type[] asmType = asm.GetTypes();

            List<Type> controllerTypeList = asmType.Where(t => t.IsSubclassOf(typeof(ApiController))).ToList();

            List<ApiControllerObj> controllers = new List<ApiControllerObj>();

            foreach (Type controllerItem in controllerTypeList)
            {
                List<ApiMethodObj> apiMethods = ProcessApiMethods(controllerItem);

                if (apiMethods.Any())
                {
                    controllers.Add(new ApiControllerObj()
                    {
                        ControllerName = controllerItem.Name,
                        ControllerNamespace = controllerItem.Namespace,
                        MethodArray = apiMethods.ToList()
                    });
                }
            }

            return controllers.OrderBy(t => t.ControllerNamespace).ThenBy(t => t.ControllerName);
        }

        #endregion Public Methods

        #region Private Methods
        private List<ApiMethodObj> ProcessApiMethods(Type controllerItem)
        {
            List<ApiMethodObj> apiMethods = new List<ApiMethodObj>();

            //src: https://stackoverflow.com/questions/21583278/getting-all-controllers-and-actions-names-in-c-sharp

            List<MethodInfo> allPublicMethods = controllerItem.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public)
                .Where(m => !m.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), true).Any()).ToList();

            foreach (MethodInfo mInfo in allPublicMethods)
            {
                try
                {
                    var m = new ApiMethodObj()
                    {
                        ControllerName = mInfo.DeclaringType.Name
                    };

                    apiMethods.Add(m);
                    try
                    { m.ReturnType = MSMethods.GetReturnType(mInfo); }
                    catch (Exception ex)
                    {
                        Console.Write(ex);
                    }

                    var _attributeCache = mInfo.GetCustomAttributes(inherit: true);

                    m.MethodName = MSMethods.GetActionName(mInfo, _attributeCache);

                    m.FullMethodName = controllerItem.FullName + "." + m.MethodName;

                    var supportedMethods = MSMethods.GetSupportedHttpMethods(mInfo, _attributeCache);
                    if (supportedMethods.Any())
                        m.SupportedHttpMethodArray.AddRange(supportedMethods);

                    Process_MethodParams(mInfo, ref m);
                }
                catch (Exception ex)
                {
                    Console.Write(ex);
                }
            }
            
            return apiMethods;
        }

        private void Process_MethodParams(MethodInfo mInfo, ref ApiMethodObj m)
        {
            //m.Attributes = attribs.Select(a => a.GetType().Name.Replace("Attribute", "")).ToList();

            var allParams = mInfo.GetParameters();

            if (allParams.Length == 1)
            {
                var p = GenerateApiMethodParamObj(allParams.First());

                FromBodyAttribute fromBodyAttr = (FromBodyAttribute)Attribute.GetCustomAttribute(allParams.First(), typeof(FromBodyAttribute));

                //if it is exactly one param && it has the attribute or it is complex type param then by default it is FromBody
                if (fromBodyAttr != null || !IsSimpleType(allParams.First().ParameterType))
                    p.IsFromBody = true;

                m.ParameterArray.Add(p);
            }
            else
            {
                //only one FromBody will receive the body params even if the attribute is put for multiple params.
                //src: https://stackoverflow.com/questions/24874490/pass-multiple-complex-objects-to-a-post-put-web-api-method
                bool isFromBodyAlreadySet = false;

                foreach (var pInfo in mInfo.GetParameters().OrderBy(p => p.Position))
                {
                    var p = GenerateApiMethodParamObj(pInfo);

                    FromBodyAttribute fromBodyAttr = (FromBodyAttribute)Attribute.GetCustomAttribute(pInfo, typeof(FromBodyAttribute));

                    if (!isFromBodyAlreadySet && fromBodyAttr != null)
                        p.IsFromBody = true;

                    m.ParameterArray.Add(p);
                }
            }
        }

        private ApiMethodParamObj GenerateApiMethodParamObj(ParameterInfo pInfo)
        {
            ApiMethodParamObj returnObj = new ApiMethodParamObj()
            {
                ParamName = pInfo.Name,
                ParamTypeName = pInfo.ParameterType.FullName,
                Position = pInfo.Position,
                //IsIn = p.IsIn,
                //IsLcid = p.IsLcid,
                IsOptional = pInfo.IsOptional,
                //IsOut = p.IsOut,
                //IsRetval = p.IsRetval,
            };

            var pInfo_type = pInfo.ParameterType;
            var underlyingType = Nullable.GetUnderlyingType(pInfo_type);
            var actualType = underlyingType ?? pInfo_type;

            returnObj.ParamTypeName = actualType.FullName;

            if (actualType.FullName != null)
            {
                string lowerCaseName = actualType.FullName.ToLower();

                if (lowerCaseName.StartsWith("system.") || lowerCaseName.StartsWith("microsoft."))
                    returnObj.ParamTypeNameWithUrlLink = string.Format("[{0}]({1}{2})",
                        actualType.FullName, Consts.URL.MicrosoftAPI_Docs, lowerCaseName);
                else if (lowerCaseName.StartsWith("newtonsoft."))
                    returnObj.ParamTypeNameWithUrlLink = string.Format("[{0}]({1}{2})",
                      actualType.FullName, Consts.URL.NewtonsoftAPI_Docs, lowerCaseName.Replace(".", "_"));
            }

            return returnObj;
        }
        
        #endregion Private Methods
    }
}
