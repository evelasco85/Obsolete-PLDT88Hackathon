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
    }
}