using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace Pldt.Browser.Api.Services
{
    public interface IPaymentService
    {
        string GetPaymentTokenId(string json);
    }

    public class PaymentService : IPaymentService
    {
        static IPaymentService s_instance = new PaymentService();

        private PaymentService()
        {

        }

        public static IPaymentService GetInstance()
        {
            return s_instance;
        }

        public string GetPaymentTokenId(string json)
        {
            string id = string.Empty;
            XmlDocument doc = JsonConvert.DeserializeXmlNode(json, "Root");

            id = XMLService.GetInstance().GetNodeValue(doc, XMLService.GetInstance().ConstructRelativeXPath("paymentTokenId"));

            return id;
        }
    }
}