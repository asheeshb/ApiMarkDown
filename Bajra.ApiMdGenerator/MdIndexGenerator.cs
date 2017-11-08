using Bajra.ApiMdGenerator.MarkDownTemplates;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bajra.ApiMdGenerator
{
    internal class MdIndexGenerator
    {
        private List<ApiControllerObj> _apControllerObjArray { get; set; }

        internal MdIndexGenerator(string path, List<ApiControllerObj> apControllerObjArray, string versionInfo, string assemblyName, string imagePath = "", bool isSortRequired = false)
        {
            this._apControllerObjArray = apControllerObjArray;

            if (isSortRequired)
                this._apControllerObjArray = apControllerObjArray = apControllerObjArray.OrderBy(t => t.ControllerNamespace).ThenBy(t => t.ControllerName).ToList();

            string dateTimeString = DateTime.Now.ToString("yyyy/MM/dd H:mm:ss");

            StringBuilder sbr_mdIndex = IndexApiTemplate.GetTemplateStringBuilder(dateTimeString, versionInfo, imagePath);

            var namespaceGroup = this._apControllerObjArray.GroupBy(t => t.ControllerNamespace).OrderBy(t => t.Key);

            StringBuilder sbr_mainIndex = new StringBuilder();
            StringBuilder sbr_indexOfIndex = new StringBuilder();
            foreach (var eachNamespaceApiGroupList in namespaceGroup)
            {
                IndexApiTemplate.GetSingleNamespaceBlock(ref sbr_mainIndex, ref sbr_indexOfIndex, eachNamespaceApiGroupList.Key, eachNamespaceApiGroupList.ToList());
            }

            sbr_mdIndex.Replace(TemplateIndexConsts.PLACEHOLDER_MASTER_INDEX_BLOCK, sbr_indexOfIndex.ToString());
            sbr_mdIndex.Replace(TemplateIndexConsts.PLACEHOLDER_MASTER_NAMESPACED_BLOCK, sbr_mainIndex.ToString());
            sbr_mdIndex.Replace(TemplateIndexConsts.PLACEHOLDER_ASSEMBLY_NAME, assemblyName);
            CreateMD_Index_File(path, sbr_mdIndex);
        }

        private string CreateMD_Index_File(string path, StringBuilder sbr)
        {
            string mdFileName = "ApiIndex.md";
            string fullMdFileAndPath = Path.Combine(path, mdFileName);

            File.WriteAllText(fullMdFileAndPath, sbr.ToString());

            return fullMdFileAndPath;
        }
    }
}
