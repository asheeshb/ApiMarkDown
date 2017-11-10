using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace Bajra.ApiMdGenerator.XmlObjects
{
    public class MethodXmlElement
    {
        public string Name { get; set; } = "";

        public string Summary { get; set; } = "";

        public string Example { get; set; } = "";

        public string Returns { get; set; } = null;

        public List<ParamXmlElement> ParamList { get; set; } = new List<ParamXmlElement>();

        public XmlElement SourceXmlElement { get; set; }

        public string Returns_WithSuccess { get; set; } = null;
        public string Returns_WithFail { get; set; } = null;

        public string DataParamFromBody { get; set; } = null;
    }

}
