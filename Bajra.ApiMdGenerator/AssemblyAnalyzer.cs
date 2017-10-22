using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Web.Mvc;
using System.Net.Http;
using System.Collections.ObjectModel;
using System.Web.Http.Controllers;
using System.Runtime.CompilerServices;
using MicrosoftStuff;
using System.Web.Http;

namespace Bajra.ApiMdGenerator
{
    public class AssemblyAnalyzer
    {


        public static List<ApiControllerObj> GetApiControllerListForAssembly(string assemblyPath, string xmlPath = null)
        {
            Assembly asm = Assembly.LoadFile(assemblyPath);

            Type[] t = asm.GetTypes();

            List<Type> controllerTypeList = t.Where(type => type.IsSubclassOf(typeof(ApiController))).ToList();

           
            List<ApiControllerObj> controllers = new List<ApiControllerObj>();

            foreach (Type controllerItem in controllerTypeList)
            {

                List<ApiMethodObj> apiMethods = new List<ApiMethodObj>();

                //src: https://stackoverflow.com/questions/21583278/getting-all-controllers-and-actions-names-in-c-sharp
                try
                {
                    List<MethodInfo> allPublicMethods = controllerItem.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public)
                        .Where(m => !m.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), true).Any()).ToList();

                    foreach (MethodInfo x in allPublicMethods)
                    {
                        var m = new ApiMethodObj() { ControllerName = x.DeclaringType.Name };
                        apiMethods.Add(m);

                        m.ReturnType = MSMethods.GetReturnType(x);//.ReturnType.Name,
                        var _attributeCache = x.GetCustomAttributes(inherit: true);
                        
                        m.MethodName = MSMethods.GetActionName(x, _attributeCache);

                        m.FullMethodName = controllerItem.FullName + "." + m.MethodName;

                        m.SupportedHttpMethodArray = MSMethods.GetSupportedHttpMethods(x, _attributeCache).ToArray();

                        //m.Attributes = attribs.Select(a => a.GetType().Name.Replace("Attribute", "")).ToList();

                        m.ParameterArray = x.GetParameters().OrderBy(p => p.Position).Select(p =>
                                                 new ApiMethodParamObj()
                                                 {
                                                     ParamName = p.Name,
                                                     ParamTypeName = p.ParameterType.FullName,
                                                     Position = p.Position,
                                                     //IsIn = p.IsIn,
                                                     //IsLcid = p.IsLcid,
                                                     IsOptional = p.IsOptional,
                                                     //IsOut = p.IsOut,
                                                     //IsRetval = p.IsRetval,
                                                 }).ToArray();
                       
                    }
                }
                catch (Exception e)
                {

                }

                if (apiMethods.Any())
                {
                    controllers.Add(new ApiControllerObj()
                    {
                        ControllerName = controllerItem.Name,
                        ControllerNamespace = controllerItem.Namespace,
                        MethodArray = apiMethods.ToArray()
                    });
                }
            }

            return controllers;
        }

    }
}
