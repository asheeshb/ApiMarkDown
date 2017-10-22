using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Bajra.ApiMdGenerator
{
    public class ApiMethodObj
    {
        public string ControllerName { get; set; }
        public string FullMethodName { get; set; }
        public string MethodName { get; set; }
        public Type ReturnType { get; set; }

        public HttpMethod[] SupportedHttpMethodArray { get; set; }

        public ApiMethodParamObj[] ParameterArray { get; set; }

        public string MethodDescription { get; set; }
        
        public override string ToString()
        {
            return ControllerName + "." + MethodName;
        }
    }
}
