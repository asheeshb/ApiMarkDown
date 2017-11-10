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

        public List<ApiMethodObj> MethodArray { get; set; }

        public override string ToString()
        {
            return ControllerName;
        }

        public string GetFolderName()
        {
            return GetValidFileName(this.ControllerNamespace + "." + this.ControllerName);
        }

        private string GetValidFileName(string currentName)
        {
            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
            {
                currentName = currentName.Replace(c, '_');
            }

            return currentName;
        }
    }
}
