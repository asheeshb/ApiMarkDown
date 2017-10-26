using Bajra.ApiMdGenerator.XmlObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Bajra.Utils;

namespace Bajra.ApiMdGenerator
{
    public class XmlCommentHelper
    {
        //Reference source: https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/xmldoc/recommended-tags-for-documentation-comments

        private XmlDocument _FullDocument { get; set; }

        private Dictionary<string, XmlElement> MemberXmlElementLookup { get; set; } = new Dictionary<string, XmlElement>();

        public XmlCommentHelper(string xmlPath)
        {
            _FullDocument = new XmlDocument();
            _FullDocument.Load(xmlPath);

            ProcessAllMethodXml(_FullDocument);
        }

        private void ProcessAllMethodXml(XmlDocument xmlDocument)
        {
            XmlNodeList nodeList = xmlDocument.DocumentElement.SelectNodes("/doc/members/member");

            List<XmlElement> memberXmlList = nodeList.Cast<XmlElement>().ToList();

            foreach (XmlElement memberXml in memberXmlList)
            {
                try
                {
                    XmlAttribute nameAttribute = memberXml.Attributes.Cast<XmlAttribute>().First(a => a.Name == "name");

                    MemberXmlElementLookup.Add(nameAttribute.Value, memberXml);
                }
                catch (Exception ex)
                {
                    Console.Write(ex);
                }
            }
        }

        public MethodXmlElement GetMemberDefinition(ApiMethodObj methodObj)
        {
            string parametersSignature = "(";

            foreach (var p in methodObj.ParameterArray)
            {
                parametersSignature += p.ParamTypeName + ",";
            }

            if (methodObj.ParameterArray.Length > 0)
                parametersSignature = parametersSignature.Remove(parametersSignature.Length - 1, 1);

            parametersSignature += ")";

            if (methodObj.ParameterArray.Length == 0)
                parametersSignature = "";

            var keyToLookfor = string.Format("M:{0}{1}", methodObj.FullMethodName, parametersSignature);

            if (!MemberXmlElementLookup.ContainsKey(keyToLookfor))
                return null;

            XmlElement ele = MemberXmlElementLookup[keyToLookfor];

            MethodXmlElement memDef = new MethodXmlElement()
            {
                Name = methodObj.MethodName,
                SourceXmlElement = ele
            };

            foreach (var c in ele.ChildNodes.Cast<XmlElement>())
            {
                try
                {
                    if (c.Name == "summary")
                        memDef.Summary = this.GetXml_Summary(c);
                    else if (c.Name == "example")
                        this.HandleXml_Example(c, ref memDef);
                    else if (c.Name == "returns")
                        this.HandleXml_Returns(c, ref memDef);
                    else if (c.Name == "param")
                        memDef.ParamList.Add(this.GetXml_Param(c));
                }
                catch (Exception ex)
                {
                    Console.Write(ex);
                }
            }

            if (string.IsNullOrWhiteSpace(memDef.Summary))
                memDef.Summary = "No information found.";

            if (string.IsNullOrWhiteSpace(memDef.DatasParamFromBody))
                memDef.DatasParamFromBody = "N/A";

            return memDef;
        }

        #region Xml Element Handlers
        private string GetXml_Summary(XmlElement ele)
        {
            StringBuilder sbr = new StringBuilder();

            ProcessInnerElement(ele, ref sbr, 0);

            return sbr.ToString();
        }

        private void HandleXml_Example(XmlElement ele, ref MethodXmlElement memDef)
        {
            StringBuilder sbr = new StringBuilder();

            ProcessInnerElement(ele, ref sbr, 1);

            memDef.Example = sbr.ToString();
        }

        private void HandleXml_Returns(XmlElement ele, ref MethodXmlElement memDef)
        {
            StringBuilder sbrReturn = new StringBuilder();
            StringBuilder sbr_failed = new StringBuilder();
            StringBuilder sbr_success = new StringBuilder();

            List<XmlNode> childNodeList = ele.ChildNodes.Cast<XmlNode>().ToList();

            if (childNodeList.Any(t => t.NodeType == XmlNodeType.Element && t.Name == "para"
                    && t.InnerText.IsEqualToAny(StringComparison.InvariantCultureIgnoreCase, "fail", "failed", "fails", "success", "succeed")
                    )
             )
            {
                //Means there is a clear division of success and failed
                for (int i = 0; i < childNodeList.Count; i++)
                {
                    XmlNode xmlNode = childNodeList[i];

                    if (xmlNode.NodeType != XmlNodeType.Element && xmlNode.Name == "para")
                        ProcessElement(xmlNode, ref sbrReturn, 1);
                    else
                    {
                        if (xmlNode.InnerText.IsEqualToAny(StringComparison.InvariantCultureIgnoreCase, "fail", "failed", "fails"))
                            //keep reading subsequent nodes till not end OR success OR another Fail is encountered
                            ProcessElement_Till_SuccessOrFailOrEnd(childNodeList, ref i, ref sbr_failed, 1);
                        else if (xmlNode.InnerText.IsEqualToAny(StringComparison.InvariantCultureIgnoreCase, "success", "succeed"))
                            //keep reading subsequent nodes till not end OR success OR another Fail is encountered
                            ProcessElement_Till_SuccessOrFailOrEnd(childNodeList, ref i, ref sbr_success, 1);
                        else
                            ProcessInnerElement(xmlNode, ref sbrReturn, 1);
                    }

                }
            }
            else
            {
                //means consider as simple return statement
                ProcessInnerElement(ele, ref sbrReturn, 1);
            }


            memDef.Returns = sbrReturn.ToString();
            memDef.Returns_WithFail = sbr_failed.ToString();
            memDef.Returns_WithSuccess = sbr_success.ToString();
        }

        private ParamXmlElement GetXml_Param(XmlElement ele)
        {
            XmlAttribute nameAttribute = ele.Attributes.Cast<XmlAttribute>().First(a => a.Name == "name");

            //TODO: process the internal XML tags

            return new ParamXmlElement()
            {
                Name = nameAttribute.Value,
                Value = ele.InnerText
            };
        }

        private string GetPaddingTabString(int tabCount)
        {
            StringBuilder sbr = new StringBuilder();
            for (var i = 0; i < tabCount; i++)
            {
                sbr.Append("    ");
            }
            return sbr.ToString();
        }

        private void ProcessElement(XmlNode xmlNode, ref StringBuilder sbr, int currentPaddingTabCount)
        {
            switch (xmlNode.NodeType)
            {
                case XmlNodeType.Text:
                    sbr.Append(this.GetPaddingTabString(currentPaddingTabCount));
                    sbr.Append(xmlNode.InnerText.Trim());
                    break;

                case XmlNodeType.Element:

                    if (xmlNode.Name == "see")
                        sbr.AppendFormat(" **{0}** ", xmlNode.Attributes.Cast<XmlAttribute>().FirstOrDefault(a => a.Name == "cref")?.Value ?? "");
                    else if (xmlNode.Name == "code")
                    {
                        sbr.AppendLine();
                        sbr.AppendLine();
                        sbr.Append(this.GetPaddingTabString(currentPaddingTabCount));
                        sbr.Append("```csharp");
                        sbr.AppendLine();
                        sbr.Append(xmlNode.InnerText);
                        sbr.AppendLine();
                        sbr.Append(this.GetPaddingTabString(currentPaddingTabCount));
                        sbr.Append("```");
                        sbr.AppendLine();
                        sbr.AppendLine();
                    }
                    else if (xmlNode.Name == "para")
                    {
                        sbr.AppendLine();
                        sbr.AppendLine();
                        ProcessInnerElement(xmlNode, ref sbr, currentPaddingTabCount);
                        sbr.AppendLine();
                        sbr.AppendLine();
                    }
                    else
                    {
                        ProcessInnerElement(xmlNode, ref sbr, currentPaddingTabCount);
                    }

                    break;
            }
        }

        private void ProcessInnerElement(XmlNode ele, ref StringBuilder sbr, int currentPaddingTabCount)
        {
            foreach (XmlNode xmlNode in ele.ChildNodes)
            {
                ProcessElement(xmlNode, ref sbr, currentPaddingTabCount);
            }
        }

        private void ProcessElement_Till_SuccessOrFailOrEnd(IEnumerable<XmlNode> allChildElements, ref int curChildIndex, ref StringBuilder sbr, int currentPaddingTabCount)
        {
            var allChildElementsList = allChildElements.ToList();

            int i = curChildIndex + 1;

            for (; i < allChildElementsList.Count; i++)
            {
                XmlNode xmlNode = allChildElementsList[i];

                if (xmlNode.NodeType == XmlNodeType.Element && xmlNode.Name == "para"
                      && xmlNode.InnerText.IsEqualToAny(StringComparison.InvariantCultureIgnoreCase, "fail", "failed", "fails", "success", "succeed")
                )
                {
                    curChildIndex = i - 1;
                    return;
                }
                else
                    ProcessElement(xmlNode, ref sbr, currentPaddingTabCount);
            }

            curChildIndex = i;
        }
        #endregion Xml Element Handlers
    }
}
