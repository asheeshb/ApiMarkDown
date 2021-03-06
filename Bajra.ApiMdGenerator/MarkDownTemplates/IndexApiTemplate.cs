﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bajra.ApiMdGenerator.MarkDownTemplates
{
    class IndexApiTemplate
    {
        private static void SetHeaderContent(ref StringBuilder sbrRef, string dateTimeStamp, string versionText, string imagePath = "")
        {
            string imgLogo = $@"
 ![Company Logo here]({imagePath})

";

            string templateMain = $@"

* **Documentation Generated On** : {dateTimeStamp}
* **DLL Version** : {versionText}

# API Index under _{TemplateIndexConsts.PLACEHOLDER_ASSEMBLY_NAME}_

{TemplateIndexConsts.PLACEHOLDER_MASTER_INDEX_BLOCK}

----
";
            if (!string.IsNullOrEmpty(imagePath))
                sbrRef.Append(imgLogo);

            sbrRef.Append(templateMain);

        }


        public static StringBuilder GetTemplateStringBuilder(string dateTimeStamp, string versionText, string imagePath = "")
        {
            string templateMain = $@"

{TemplateIndexConsts.PLACEHOLDER_MASTER_NAMESPACED_BLOCK}

";

            StringBuilder sbr = new StringBuilder();

            SetHeaderContent(ref sbr, dateTimeStamp, versionText, imagePath);

            sbr.Append(templateMain);

            return sbr;

        }

        private static void AppendSingleApiBlock(ref StringBuilder sbr, ApiControllerObj apiControllerObj, string requiredLinkExtension)
        {
            string controllerName = apiControllerObj.ControllerName;

            sbr.AppendLine();
            sbr.Append($"### {controllerName}");
            sbr.AppendLine();

            //string sadFace =  "_No Comments_";// "![No comments !!](worried.png)";//"  :worried:";
            string sadFace = $"![No comments !!]({Consts.URL.SadSmiley_URL})";//"

            foreach (var m in apiControllerObj.MethodArray)
            {
                string hyperlinkToMd = $"{apiControllerObj.GetFolderName()}/{ m.MDFileNameWithoutExtension}.{requiredLinkExtension}";
                sbr.Append($"1. [{m.MethodName}]({hyperlinkToMd})");

                if (m.IsCommentingMissing)
                    sbr.Append("  -  " + sadFace);
                sbr.AppendLine();
            }
        }

        public static StringBuilder GetSingleNamespaceBlock(ref StringBuilder sbr_mainContent, ref StringBuilder sbr_indexContent, string namespaceBlockName, List<ApiControllerObj> allControllersstring, string requiredLinkExtension)
        {
            sbr_mainContent.AppendLine();
            sbr_mainContent.Append($"## {namespaceBlockName}");
            sbr_mainContent.AppendLine();

            sbr_indexContent.Append($"   - **[{namespaceBlockName}](#{GetHyperlinkFormattedName(namespaceBlockName)})**");
            sbr_indexContent.AppendLine();

            foreach (var a in allControllersstring)
            {
                sbr_indexContent.Append($"     - [{a.ControllerName}](#{GetHyperlinkFormattedName(a.ControllerName)})");
                sbr_indexContent.AppendLine();

                AppendSingleApiBlock(ref sbr_mainContent, a, requiredLinkExtension);
            }

            sbr_mainContent.AppendLine();

            return sbr_mainContent;
        }

        private static string GetHyperlinkFormattedName(string namespaceBlockName)
        {
            return namespaceBlockName.ToLower();//.Replace(".", "-");
        }
    }
}
