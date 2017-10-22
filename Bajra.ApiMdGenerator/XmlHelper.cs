using Bajra.ApiMdGenerator.XmlObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

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
                        memDef.Example = this.GetXml_Example(c);
                    else if (c.Name == "returns")
                        memDef.Returns = this.GetXml_Returns(c);
                    else if (c.Name == "param")
                    {
                        memDef.ParamList.Add(this.GetXml_Param(c));
                    }
                }
                catch (Exception ex)
                {
                    Console.Write(ex);
                }
            }

            if (string.IsNullOrWhiteSpace(memDef.Summary))
                memDef.Summary = "No information found.";

            return memDef;
        }

        #region Xml Element Handlers
        private string GetXml_Summary(XmlElement ele)
        {
            //TODO: process the internal XML tags
            return ele.InnerText.Trim();
        }

        private string GetXml_Example(XmlElement ele)
        {
            //TODO: process the internal XML tags
            return ele.InnerText;
        }

        private string GetXml_Returns(XmlElement ele)
        {
            //TODO: process the internal XML tags
            return ele.InnerText;
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
        #endregion Xml Element Handlers
    }
}
