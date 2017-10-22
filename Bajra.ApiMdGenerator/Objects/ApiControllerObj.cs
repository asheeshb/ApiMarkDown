using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Bajra.ApiMdGenerator
{
    public class ApiControllerObj
    {
        public string ControllerName { get; set; }
        public string ControllerNamespace { get; set; }

        public bool IsSecured { get; set; }
        public bool IsSessionEnabled { get; set; }

        public ApiMethodObj[] MethodArray { get; set; }

        public override string ToString()
        {
            return ControllerName;
        }
    }
}
