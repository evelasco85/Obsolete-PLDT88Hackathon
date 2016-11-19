using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace Pldt.Browser.Api.Services
{
    public interface ICustomerService
    {
        string GetCustomerId(string json);
        string GetCustomerEmail(string json);
    }

    public class CustomerService : ICustomerService
    {
        static ICustomerService s_instance = new CustomerService();

        private CustomerService()
        {

        }

        public static ICustomerService GetInstance()
        {
            return s_instance;
        }

        public string GetCustomerId (string json)
        {
            string id = string.Empty;
            XmlDocument doc = JsonConvert.DeserializeXmlNode(json, "Root");

            id = XMLService.GetInstance().GetNodeValue(doc, XMLService.GetInstance().ConstructRelativeXPath("id"));

            return id;
        }
        public string GetCustomerEmail(string json)
        {
            string id = string.Empty;
            XmlDocument doc = JsonConvert.DeserializeXmlNode(json, "Root");

            id = XMLService.GetInstance().GetNodeValue(doc, XMLService.GetInstance().ConstructRelativeXPath("id"));

            return id;
        }        
    }
}