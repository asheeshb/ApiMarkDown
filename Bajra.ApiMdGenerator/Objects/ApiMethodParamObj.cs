using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Bajra.ApiMdGenerator
{
    public class ApiMethodParamObj
    {
        public string ParamName { get; set; }

        public string ParamTypeName { get; set; }

        public string ParamTypeNameWithUrlLink { get; set; }

        public int Position { get; set; }

        //public bool IsIn { get; set; }

        //public bool IsLcid { get; set; }

        public bool IsOptional { get; set; }

        //public bool IsOut { get; set; }

        //public bool IsRetval { get; set; }

        public bool IsFromBody { get; set; }

        public bool IsDtoType { get; set; }

        public override string ToString()
        {
            return ParamName;
        }

        public string GetCorrectedParamTypeName()
        {
            if (!string.IsNullOrEmpty(this.ParamTypeNameWithUrlLink))
                return this.ParamTypeNameWithUrlLink;
            else
                return this.ParamTypeName;
        }
    }
}
