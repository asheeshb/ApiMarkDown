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

        public List<HttpMethod> SupportedHttpMethodArray { get; private set; } = new List<HttpMethod>();

        public List<ApiMethodParamObj> ParameterArray { get; private set; } = new List<ApiMethodParamObj>();

        public string MethodDescription { get; set; }
        
        public override string ToString()
        {
            return ControllerName + "." + MethodName;
        }
    }
}
