using System;
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

        private static void AppendSingleApiBlock(ref StringBuilder sbr, string controllerName, List<ApiMethodObj> methodObjList)
        {
            sbr.AppendLine();
            sbr.Append($"### {controllerName}");
            sbr.AppendLine();
            
            //string sadFace =  "_No Comments_";// "![No comments !!](worried.png)";//"  :worried:";
            string sadFace = "![No comments !!](http://asheesh.buzz/cdn/sad16.png)";//"

            foreach (var m in methodObjList)
            {
                string hyperlinkToMd = $"{controllerName}/{ m.MDFileNameWithoutExtension}.md";
                sbr.Append($"1. [{m.MethodName}]({hyperlinkToMd})");

                if (m.IsCommentingMissing)
                    sbr.Append("  -  " + sadFace);
                sbr.AppendLine();
            }
        }

        public static StringBuilder GetSingleNamespaceBlock(ref StringBuilder sbr_mainContent, ref StringBuilder sbr_indexContent, string namespaceBlockName, List<ApiControllerObj> allControllersstring)
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

                AppendSingleApiBlock(ref sbr_mainContent, a.ControllerName, a.MethodArray);
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
