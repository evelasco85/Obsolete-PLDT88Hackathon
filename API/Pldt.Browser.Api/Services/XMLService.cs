using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace Pldt.Browser.Api.Services
{
    public interface IXMLService
    {
        XmlNode GetNode(XmlDocument document, string xpath);
        string GetNodeValue(XmlNode node);
        string GetNodeValue(XmlDocument document, string xpath);
        string ConstructRelativeXPath(string field);
    }

    public class XMLService : IXMLService
    {
        static IXMLService s_instance = new XMLService();

        private XMLService()
        {
        }

        public static IXMLService GetInstance()
        {
            return s_instance;
        }

        public string GetNodeValue(XmlNode node)
        {
            return (node == null) ? string.Empty : node.InnerText;
        }

        public string GetNodeValue(XmlDocument document, string xpath)
        {
            return GetNodeValue(GetNode(document, xpath));
        }

        public XmlNode GetNode(XmlDocument document, string xpath)
        {
            if ((document == null) || (string.IsNullOrEmpty(xpath)))
                return null;

            return document.SelectSingleNode(xpath);
        }

        public string ConstructRelativeXPath(string field)
        {
            if (string.IsNullOrEmpty(field))
                return string.Empty;

            return string.Format("{0}/{1}", "Root", field);
        }
    }
}