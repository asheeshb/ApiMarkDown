using Bajra.ApiMdGenerator.MarkDownTemplates;
using Bajra.ApiMdGenerator.XmlObjects;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Bajra.ApiMdGenerator
{
    public class MdGeneratorCore
    {
        private string DllFullFilePath { get; set; }
        private string XmlFullFilePath { get; set; }
        private string SavePath { get; set; }

        private string _mdFileCache = "";

        const string PADDING_PARAM = "    ";

        public MdGeneratorCore(string dllFullFilePath, string xmlFullFilePath, string mdOutputPath)
        {
            DllFullFilePath = dllFullFilePath;
            XmlFullFilePath = xmlFullFilePath;
            SavePath = mdOutputPath;
        }

        public DirectoryInfo GenerateMDFilesForAssembly()
        {
            List<ApiControllerObj> apiControllerList = AssemblyAnalyzer.GetApiControllerListForAssembly(this.DllFullFilePath);

            string finalFolderPath = Path.Combine(this.SavePath, GetFolderPathWithCurrentTimeStamp());

            DirectoryInfo dirInfo = null;

            if (!Directory.Exists(finalFolderPath))
                dirInfo = Directory.CreateDirectory(finalFolderPath);
            else
                dirInfo = new DirectoryInfo(finalFolderPath);

            this.GenerateMD(apiControllerList, finalFolderPath, this.XmlFullFilePath);

            return dirInfo;
        }
        

        private void GenerateMD(List<ApiControllerObj> apiControllerList, string savePath, string xmlPath)
        {
            XmlCommentHelper xmlCommentHelper = null;//isXmlExist = false;

            if (!string.IsNullOrEmpty(xmlPath) && File.Exists(xmlPath))
                xmlCommentHelper = new XmlCommentHelper(xmlPath);

            foreach (ApiControllerObj controllerItem in apiControllerList)
            {
                var folderName = GetValidFileName(controllerItem.ControllerName);

                string subControllerFolderPath = Path.Combine(savePath, folderName);

                if (!Directory.Exists(subControllerFolderPath))
                    Directory.CreateDirectory(subControllerFolderPath);

                foreach (ApiMethodObj methodItem in controllerItem.MethodArray)
                {
                    string methodTypes = GetMethodType(methodItem.SupportedHttpMethodArray);

                    bool hasPostApi = methodTypes.Contains("POST");

                    StringBuilder sbr_mdFileForMethod = BasicApiTemplate.GetTemplateStringBuilder(hasPostApi, false);

                    sbr_mdFileForMethod.Replace(TemplateConsts.API_NAME, methodItem.MethodName);

                    StringBuilder sbr_RequiredParams = new StringBuilder();
                    StringBuilder sbr_OptionalParams = new StringBuilder();

                    MethodXmlElement xmlMethodObj = null;

                    if (xmlCommentHelper != null)
                        xmlMethodObj = xmlCommentHelper.GetMemberDefinition(methodItem);

                    sbr_mdFileForMethod.Replace(TemplateConsts.ADDITIONAL_INFORMATION, xmlMethodObj?.Summary ?? "No information found.");

                    sbr_mdFileForMethod.Replace(TemplateConsts.URL, "/api/" + methodItem.MethodName.Replace("Controller", ""));

                    sbr_mdFileForMethod.Replace(TemplateConsts.METHOD_TYPE, methodTypes);

                    foreach (var paramItem in methodItem.ParameterArray)
                    {
                        string paramDesc = xmlMethodObj?.ParamList.FirstOrDefault(t => t.Name == paramItem.ParamName)?.Value ?? "";
                        
                        //TODO:  Handle IsFromBody
                        if (paramItem.IsFromBody)
                        {
                            sbr_mdFileForMethod.Replace(TemplateConsts.PARAM_FROM_BODY, paramDesc);
                        }
                        else
                        {
                            var refSbr = (paramItem.IsOptional) ? sbr_OptionalParams : sbr_RequiredParams;

                            refSbr.AppendFormat(PADDING_PARAM + "`{0}=[{1}]` : {2}\r\n", paramItem.ParamName, paramItem.ParamTypeName, paramDesc);
                            refSbr.AppendLine();
                        }
                    }

                    if (sbr_OptionalParams.Length == 0)
                        sbr_OptionalParams.AppendLine(PADDING_PARAM + "No Optional Params");

                    if (sbr_RequiredParams.Length == 0)
                        sbr_RequiredParams.AppendLine(PADDING_PARAM + "No Mandatory Params");

                    sbr_mdFileForMethod.Replace(TemplateConsts.PARAM_LIST_REQUIRED, sbr_RequiredParams.ToString());

                    sbr_mdFileForMethod.Replace(TemplateConsts.PARAM_LIST_OPTIONAL, sbr_OptionalParams.ToString());

                    string fileNameOnly = GetValidFileName(methodItem.MethodName);

                    this.CreateMD_File(fileNameOnly, subControllerFolderPath, sbr_mdFileForMethod);
                }
            }
        }

        private void CreateMD_File(string fileNameOnly, string subControllerFolderPath, StringBuilder sbr_mdFileForMethod)
        {
            string mdFileName = fileNameOnly + ".md";

            string fullMdFileAndPath = Path.Combine(subControllerFolderPath, mdFileName);
            int ctr = 1;

            while (File.Exists(fullMdFileAndPath))
            {
                string tempFileName = string.Format("{0}({1})", fileNameOnly, ctr++);
                fullMdFileAndPath = Path.Combine(subControllerFolderPath, tempFileName + ".md");
            }

            File.WriteAllText(fullMdFileAndPath, sbr_mdFileForMethod.ToString());
        }

        private string GetMethodType(HttpMethod[] supportedHttpMethods)
        {
            StringBuilder sbr = new StringBuilder();

            for (int i = 0; i < supportedHttpMethods.Length; i++)
            {
                HttpMethod httpMethodItem = supportedHttpMethods[i];
                sbr.AppendFormat("`{0}`", httpMethodItem.Method.ToUpper());
                if (i != supportedHttpMethods.Length - 1)
                {
                    sbr.Append(" | ");
                }
            }

            return sbr.ToString();
        }

        private string GetFolderPathWithCurrentTimeStamp()
        {
            return "MarkDown_" + DateTime.Now.ToString("yyyyMMddHHmmss");
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
