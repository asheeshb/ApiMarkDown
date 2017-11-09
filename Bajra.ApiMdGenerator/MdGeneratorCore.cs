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
        private string OldMdPath { get; set; }

        private string ImagePath { get; set; }
        private string VersionInfo { get; set; }

        private string _mdFileCache = "";

        const string PADDING_PARAM = "    ";

        private AssemblyAnalyzer AssemblyAnalyzerRef { get; set; }

        public MdGeneratorCore(string dllFullFilePath, string xmlFullFilePath, string mdOutputPath, string oldMdLocationPath = null, string imagePath = "")
        {
            DllFullFilePath = dllFullFilePath;
            XmlFullFilePath = xmlFullFilePath;
            SavePath = mdOutputPath;
            OldMdPath = oldMdLocationPath;
            ImagePath = imagePath;

            AssemblyAnalyzerRef = new AssemblyAnalyzer();

            if (OldMdPath == null)
                OldMdPath = mdOutputPath;

            if (!Directory.Exists(OldMdPath))
                OldMdPath = null;

            try
            {
                this.VersionInfo = AssemblyAnalyzerRef.GetVersion(this.DllFullFilePath);
            }
            catch
            {

            }
        }

        public DirectoryInfo GenerateMDFilesForAssembly()
        {

            List<ApiControllerObj> apiControllerList = AssemblyAnalyzerRef.GetApiControllerListForAssembly(this.DllFullFilePath).ToList();

            string finalFolderPath = Path.Combine(this.SavePath, GetFolderPathWithCurrentTimeStamp());

            string possibleOldPath = TryGettingLastGeneratedFolderPath(this.OldMdPath);

            DirectoryInfo dirInfo = (!Directory.Exists(finalFolderPath)) ? Directory.CreateDirectory(finalFolderPath) : new DirectoryInfo(finalFolderPath);

            this.GenerateMD(apiControllerList, finalFolderPath, this.XmlFullFilePath, possibleOldPath);

            return dirInfo;
        }

        private void GenerateMD(List<ApiControllerObj> apiControllerList, string savePath, string xmlPath, string oldMdPathHint)
        {
            string dateTimeString = DateTime.Now.ToString("yyyy/MM/dd H:mm:ss");
            XmlCommentHelper xmlCommentHelper = null;//isXmlExist = false;

            if (!string.IsNullOrEmpty(xmlPath) && File.Exists(xmlPath))
                xmlCommentHelper = new XmlCommentHelper(xmlPath);

            foreach (ApiControllerObj controllerItem in apiControllerList)
            {
                var folderName = GetValidFileName(controllerItem.ControllerName);

                string subControllerFolderPath = Path.Combine(savePath, folderName);

                string subControllerFolderPath_Old = null;

                if (!Directory.Exists(subControllerFolderPath))
                    Directory.CreateDirectory(subControllerFolderPath);

                if (oldMdPathHint != null)
                {
                    subControllerFolderPath_Old = Path.Combine(oldMdPathHint, folderName);

                    if (!Directory.Exists(subControllerFolderPath_Old))
                        subControllerFolderPath_Old = null;
                }

                this.Process_ApiMethodObj(controllerItem, xmlCommentHelper, dateTimeString, subControllerFolderPath, subControllerFolderPath_Old);
            }

            string assemblyName = Path.GetFileName(DllFullFilePath);

            MdIndexGenerator gen = new MdIndexGenerator(savePath, apiControllerList, VersionInfo, assemblyName, this.ImagePath, true);
        }

        private void Process_ApiMethodObj(ApiControllerObj controllerItem, XmlCommentHelper xmlCommentHelper, string dateTimeString, string subControllerFolderPath, string subControllerFolderPath_Old)
        {
            foreach (ApiMethodObj methodItem in controllerItem.MethodArray)
            {
                string methodTypes = GetMethodType(methodItem.SupportedHttpMethodArray);

                bool hasPostApi = methodTypes.Contains("POST");

                StringBuilder sbr_mdFileForMethod = BasicApiTemplate.GetTemplateStringBuilder(hasPostApi, false, dateTimeString, this.VersionInfo, this.ImagePath);

                sbr_mdFileForMethod.Replace(TemplateConsts.PLACEHOLDER_API_NAME, methodItem.MethodName);
                sbr_mdFileForMethod.Replace(TemplateConsts.PLACEHOLDER_CONTROLLER_NAME, methodItem.ControllerName);

                MethodXmlElement xmlMethodObj = null;

                if (xmlCommentHelper != null)
                    xmlMethodObj = xmlCommentHelper.GetMemberDefinition(methodItem);

                if (xmlMethodObj == null || string.IsNullOrWhiteSpace(xmlMethodObj.Returns) || string.IsNullOrWhiteSpace(xmlMethodObj.Summary))
                    methodItem.IsCommentingMissing = true;

                sbr_mdFileForMethod.Replace(TemplateConsts.PLACEHOLDER_ADDITIONAL_INFO, xmlMethodObj?.Summary);

                sbr_mdFileForMethod.Replace(TemplateConsts.PLACEHOLDER_URL, "/api/" + methodItem.MethodName.Replace("Controller", ""));

                sbr_mdFileForMethod.Replace(TemplateConsts.PLACEHOLDER_METHOD_TYPE, methodTypes);

                this.SetTemplateData_MethodParameters(methodItem, xmlMethodObj, ref sbr_mdFileForMethod);

                this.SetTemplateData_Example(methodItem, xmlMethodObj, ref sbr_mdFileForMethod);

                this.SetTemplateData_ReturnType(methodItem, xmlMethodObj, ref sbr_mdFileForMethod);

                string fileNameOnly =this.GetValidFileName(methodItem.MethodName);

                methodItem.MDFileNameWithoutExtension = this.CreateMD_File_UsingOldNotes_IfApplicable(fileNameOnly, subControllerFolderPath, subControllerFolderPath_Old, sbr_mdFileForMethod);
            }
        }
        
        private string CreateMD_File_UsingOldNotes_IfApplicable(string fileNameOnly, string subControllerFolderPath, string subControllerFolderPath_Old, StringBuilder sbr_mdFileForMethod)
        {
            string mdFileName = fileNameOnly + ".md";
            string fullMdFileAndPath = Path.Combine(subControllerFolderPath, mdFileName);

            int ctr = 1;

            string tempFileName = fileNameOnly;

            while (File.Exists(fullMdFileAndPath))
            {
                tempFileName = string.Format("{0}({1})", fileNameOnly, ctr++);
                fullMdFileAndPath = Path.Combine(subControllerFolderPath, tempFileName + ".md");
            }

            string noteSection = string.Empty;
            string fullMdFileAndPath_old = null;

            if (subControllerFolderPath_Old != null)
            {
                fullMdFileAndPath_old = Path.Combine(subControllerFolderPath_Old, tempFileName + ".md");

                //extract the notes from file if old file exist
                noteSection = TryGettingNoteSectionFromOldMdFile(fullMdFileAndPath_old);
            }

            sbr_mdFileForMethod.Replace(TemplateConsts.PLACEHOLDER_NOTES, noteSection);

            File.WriteAllText(fullMdFileAndPath, sbr_mdFileForMethod.ToString());

            return tempFileName;
        }

        private string GetMethodType(List<HttpMethod> supportedHttpMethods)
        {
            StringBuilder sbr = new StringBuilder();

            for (int i = 0; i < supportedHttpMethods.Count; i++)
            {
                HttpMethod httpMethodItem = supportedHttpMethods[i];
                sbr.AppendFormat("`{0}`", httpMethodItem.Method.ToUpper());
                if (i != supportedHttpMethods.Count - 1)
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

        private static string TryGettingLastGeneratedFolderPath(string oldMdPathHint)
        {
            if (!string.IsNullOrEmpty(oldMdPathHint))
            {
                //List<Tuple<string, DateTime>> viableDirectoryNames = new List<Tuple<string, DateTime>>();

                Tuple<string, DateTime> latestFolder = null;

                foreach (var dirPath in Directory.EnumerateDirectories(oldMdPathHint))
                {
                    string directoryName = Path.GetFileName(dirPath);
                    if (directoryName.StartsWith("MarkDown_", StringComparison.InvariantCultureIgnoreCase))
                    {
                        DateTime dt = Directory.GetCreationTime(dirPath);

                        var curDirItem = Tuple.Create(dirPath, dt);

                        //viableDirectoryNames.Add(curDirItem);

                        if (latestFolder == null || curDirItem.Item2 > latestFolder.Item2)
                            latestFolder = curDirItem;
                    }
                }

                if (latestFolder != null)
                    return latestFolder.Item1;
            }

            return null;
        }

        private string TryGettingNoteSectionFromOldMdFile(string oldMdfullPathAndFileName)
        {
            string noteText = string.Empty;

            if (!File.Exists(oldMdfullPathAndFileName))
                return noteText;

            string oldFileInMem = File.ReadAllText(oldMdfullPathAndFileName);

            int i = oldFileInMem.IndexOf(TemplateConsts.NOTE_SIGNATURE);

            if (i > 0)//&& oldFileInMem.Length > (i + TemplateConsts.NOTE_SIGNATURE.Length + 1))
                noteText = oldFileInMem.Substring(i + TemplateConsts.NOTE_SIGNATURE.Length);

            return noteText;
        }

        private void SetTemplateData_MethodParameters(ApiMethodObj methodItem, MethodXmlElement xmlMethodObj, ref StringBuilder sbr_mdFileForMethod)
        {
            StringBuilder sbr_RequiredParams = new StringBuilder();
            StringBuilder sbr_OptionalParams = new StringBuilder();

            foreach (var paramItem in methodItem.ParameterArray)
            {
                string paramDesc = xmlMethodObj?.ParamList.FirstOrDefault(t => t.Name == paramItem.ParamName)?.Value ?? "";

                if (paramItem.IsFromBody)
                    sbr_mdFileForMethod.Replace(TemplateConsts.PLACEHOLDER_PARAM_FROM_BODY, string.Format(PADDING_PARAM + "`{0}=[{1}]` : {2}\r\n", paramItem.ParamName, paramItem.ParamTypeName, paramDesc));
                else
                {
                    var refSbr = (paramItem.IsOptional) ? sbr_OptionalParams : sbr_RequiredParams;

                    refSbr.AppendFormat(PADDING_PARAM + "`{0}=[{1}]` : {2}\r\n", paramItem.ParamName, paramItem.ParamTypeName, paramDesc);
                    refSbr.AppendLine();
                }
            }

            if (sbr_OptionalParams.Length == 0)
                sbr_OptionalParams.AppendLine(PADDING_PARAM + "N/A");

            if (sbr_RequiredParams.Length == 0)
                sbr_RequiredParams.AppendLine(PADDING_PARAM + "N/A");

            sbr_mdFileForMethod.Replace(TemplateConsts.PLACEHOLDER_PARAM_LIST_REQUIRED, sbr_RequiredParams.ToString());

            sbr_mdFileForMethod.Replace(TemplateConsts.PLACEHOLDER_PARAM_LIST_OPTIONAL, sbr_OptionalParams.ToString());



            if (xmlMethodObj != null && !string.IsNullOrEmpty(xmlMethodObj.DatasParamFromBody))
                sbr_mdFileForMethod.Replace(TemplateConsts.PLACEHOLDER_PARAM_FROM_BODY, xmlMethodObj.DatasParamFromBody);
            else
                sbr_mdFileForMethod.Replace(TemplateConsts.PLACEHOLDER_PARAM_FROM_BODY, TemplateConsts.TEXT_NONE);


        }

        private void SetTemplateData_ReturnType(ApiMethodObj methodItem, MethodXmlElement xmlMethodObj, ref StringBuilder sbr_mdFileForMethod)
        {
            string returnText = TemplateConsts.TEXT_NONE;
            if (xmlMethodObj != null && !string.IsNullOrEmpty(xmlMethodObj.Returns))
                returnText = xmlMethodObj.Returns;
            else if (methodItem.ReturnType != null)
                returnText = methodItem.ReturnType.Name;

            sbr_mdFileForMethod.Replace(TemplateConsts.PLACEHOLDER_RETURN_GENERIC_RESPONSE, returnText);


            if (xmlMethodObj != null && !string.IsNullOrEmpty(xmlMethodObj.Returns_WithSuccess))
                sbr_mdFileForMethod.Replace(TemplateConsts.PLACEHOLDER_SUCCESS_RESPONSE, xmlMethodObj.Returns_WithSuccess);
            else
                sbr_mdFileForMethod.Replace(TemplateConsts.PLACEHOLDER_SUCCESS_RESPONSE, TemplateConsts.TEXT_NONE);


            if (xmlMethodObj != null && !string.IsNullOrEmpty(xmlMethodObj.Returns_WithFail))
                sbr_mdFileForMethod.Replace(TemplateConsts.PLACEHOLDER_FAIL_RESPONSE, xmlMethodObj.Returns_WithFail);
            else
                sbr_mdFileForMethod.Replace(TemplateConsts.PLACEHOLDER_FAIL_RESPONSE, TemplateConsts.TEXT_NONE);

        }

        private void SetTemplateData_Example(ApiMethodObj methodItem, MethodXmlElement xmlMethodObj, ref StringBuilder sbr_mdFileForMethod)
        {
            if (xmlMethodObj != null && !string.IsNullOrEmpty(xmlMethodObj.Example))
                sbr_mdFileForMethod.Replace(TemplateConsts.PLACEHOLDER_EXAMPLE, xmlMethodObj.Example);
            else
                sbr_mdFileForMethod.Replace(TemplateConsts.PLACEHOLDER_EXAMPLE, TemplateConsts.TEXT_NONE);

        }
    }
}
