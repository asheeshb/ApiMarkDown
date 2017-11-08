using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bajra.ApiMdGenerator.MarkDownTemplates
{
    class TemplateConsts
    {
        public const string PLACEHOLDER_API_NAME = "<<<API_NAME>>>";
        public const string PLACEHOLDER_CONTROLLER_NAME = "<<<CONTROLLER_NAME>>>";
        public const string PLACEHOLDER_ADDITIONAL_INFO = "<<<ADDITIONAL_INFORMATION>>>";
        public const string PLACEHOLDER_URL = "<<<URL>>>";
        public const string PLACEHOLDER_METHOD_TYPE = "<<<METHOD_TYPE>>>";
        public const string PLACEHOLDER_PARAM_LIST_REQUIRED = "<<<PARAM_LIST_REQUIRED>>>";
        public const string PLACEHOLDER_PARAM_LIST_OPTIONAL = "<<<PARAM_LIST_OPTIONAL>>>";
        public const string PLACEHOLDER_PARAM_FROM_BODY = "<<<PARAM_FROM_BODY>>>";
        public const string PLACEHOLDER_RETURN_GENERIC_RESPONSE = "<<<PARAM_GENERIC_RETURN>>>";
        public const string PLACEHOLDER_SUCCESS_RESPONSE = "<<<PARAM_IF_SUCCESS_DEFINED>>>";
        public const string PLACEHOLDER_FAIL_RESPONSE = "<<<PARAM_IF_FAILED_DEFINED>>>";
        public const string PLACEHOLDER_EXAMPLE = "<<<EXAMPLE>>>";
        public const string PLACEHOLDER_NOTES = "<<<NOTES>>>";

        public const string TEXT_NONE = "None";

        public const string NOTE_SIGNATURE = "### Extra Notes :\r\n";
    }

    class TemplateIndexConsts
    {
        public const string PLACEHOLDER_ASSEMBLY_NAME = "<<<ASSEMBLY_NAME>>>";

        public const string PLACEHOLDER_MASTER_INDEX_BLOCK = "<<<PLACEHOLDER_MASTER_INDEX_BLOCK>>>";
        public const string PLACEHOLDER_MASTER_NAMESPACED_BLOCK = "<<<PLACEHOLDER_PLACEHOLDER_MASTER_NAMESPACED_BLOCK>>>";
    }
}
