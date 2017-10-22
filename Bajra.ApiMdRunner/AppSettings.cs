using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bajra.ApiMdGenerator
{
    public static class AppSettings
    {
        public static string MdOutputPath
        {
            get
            {
                if (ConfigurationManager.AppSettings["MdOutputPath"] == null)
                    throw new Exception("Appsettings not found for MdOutputPath ");

                return ConfigurationManager.AppSettings["MdOutputPath"];
            }
        }
    }
}
