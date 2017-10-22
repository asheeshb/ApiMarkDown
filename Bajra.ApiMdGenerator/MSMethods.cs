using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace MicrosoftStuff
{
    //Src: https://aspnetwebstack.codeplex.com/SourceControl/latest#src/System.Web.Http/Controllers/ReflectedHttpActionDescriptor.cs

    internal static class MSMethods
    {
        private static readonly HttpMethod[] _supportedHttpMethodsByConvention =
        {
            HttpMethod.Get,
            HttpMethod.Post,
            HttpMethod.Put,
            HttpMethod.Delete,
            HttpMethod.Head,
            HttpMethod.Options,
            new HttpMethod("PATCH")
        };

        /// <summary>
        /// Fast implementation to get the subset of a given type.
        /// </summary>
        /// <typeparam name="T">type to search for</typeparam>
        /// <returns>subset of objects that can be assigned to T</returns>
        public static ReadOnlyCollection<T> OfType<T>(object[] objects) where T : class
        {
            int max = objects.Length;
            List<T> list = new List<T>(max);
            int idx = 0;
            for (int i = 0; i < max; i++)
            {
                T attr = objects[i] as T;
                if (attr != null)
                {
                    list.Add(attr);
                    idx++;
                }
            }
            list.Capacity = idx;

            return new ReadOnlyCollection<T>(list);
        }


        public static string GetActionName(MethodInfo methodInfo, object[] actionAttributes)
        {
            ActionNameAttribute nameAttribute = OfType<ActionNameAttribute>(actionAttributes).FirstOrDefault();
            return nameAttribute != null
                       ? nameAttribute.Name
                       : methodInfo.Name;
        }

        public static Collection<HttpMethod> GetSupportedHttpMethods(MethodInfo methodInfo, object[] actionAttributes)
        {
            Collection<HttpMethod> supportedHttpMethods = new Collection<HttpMethod>();
            ICollection<IActionHttpMethodProvider> httpMethodProviders = OfType<IActionHttpMethodProvider>(actionAttributes);
            if (httpMethodProviders.Count > 0)
            {
                // Get HttpMethod from attributes
                foreach (IActionHttpMethodProvider httpMethodSelector in httpMethodProviders)
                {
                    foreach (HttpMethod httpMethod in httpMethodSelector.HttpMethods)
                    {
                        supportedHttpMethods.Add(httpMethod);
                    }
                }
            }
            else
            {
                // Get HttpMethod from method name convention 
                for (int i = 0; i < _supportedHttpMethodsByConvention.Length; i++)
                {
                    if (methodInfo.Name.StartsWith(_supportedHttpMethodsByConvention[i].Method, StringComparison.OrdinalIgnoreCase))
                    {
                        supportedHttpMethods.Add(_supportedHttpMethodsByConvention[i]);
                        break;
                    }
                }
            }

            if (supportedHttpMethods.Count == 0)
            {
                // Use POST as the default HttpMethod
                supportedHttpMethods.Add(HttpMethod.Post);
            }

            return supportedHttpMethods;
        }

        private static readonly Type TaskGenericType = typeof(Task<>);

        public static Type GetTaskInnerTypeOrNull(Type type)
        {
            //Contract.Assert(type != null);
            if (type.IsGenericType && !type.IsGenericTypeDefinition)
            {
                Type genericTypeDefinition = type.GetGenericTypeDefinition();

                if (TaskGenericType == genericTypeDefinition)
                {
                    return type.GetGenericArguments()[0];
                }
            }

            return null;
        }

        internal static Type GetReturnType(MethodInfo methodInfo)
        {
            Type result = methodInfo.ReturnType;
            if (typeof(Task).IsAssignableFrom(result))
            {
                result = GetTaskInnerTypeOrNull(methodInfo.ReturnType);
            }
            if (result == typeof(void))
            {
                result = null;
            }
            return result;
        }
    }
}
