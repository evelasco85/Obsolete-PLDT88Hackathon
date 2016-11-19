using Pldt.Browser.Api.Database;
using Pldt.Browser.Api.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace Pldt.Browser.Api.Controllers
{
    public class PayMayaController : ApiController
    {

        string _url = "https://pg-sandbox.paymaya.com/payments/v1/customers";

        IEFRepository _repository;

        public PayMayaController(IEFRepository repository)
        {
            _repository = repository;
        }


        [HttpPost]
        public string CreateCustomer(
            string firstName, string middleName, string lastName,
            string birthday, string sex,
            string line1, string line2,
            string city, string state, string zipCode, string countryCode,
            string phone, string email
            )
        {
            var customerData = new
            {
                firstName,
                middleName,
                lastName,
                sex,
                contact = new
                {
                    phone,
                    email
                },
                billingAddress = new
                {
                    line1,
                    line2,
                    city,
                    state,
                    zipCode,
                    countryCode
                },
                metadata = new { }
            };

            if (_repository.Entities.CustomerRecords.Any(customer => customer.email == email))
                return _repository.Entities.CustomerRecords.Where(customer => customer.email == email).FirstOrDefault().id;

                string jsonInput = new JavaScriptSerializer().Serialize(customerData);
            string jsonOutput = string.Empty;
            string basicAuth = string.Format(
                "Basic {0}",
                Convert.ToBase64String(Encoding.Default.GetBytes("sk-oF33wv5pXp7gvIpHLvCjoNKW2BL48ZxPY6H2Lc8v5mD" + ":")));

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            try
            {
                WebRequest request = WebRequest.Create(_url);
                ASCIIEncoding encoding = new ASCIIEncoding();
                byte[] bodyByte = Encoding.UTF8.GetBytes(jsonInput);

                request.Method = "POST";
                request.ContentType = "application/json;";
                request.ContentLength = bodyByte.Length;
                request.Headers["Authorization"] = basicAuth;

                using (Stream dataStream = request.GetRequestStream())
                {
                    dataStream.Write(bodyByte, 0, bodyByte.Length);
                }

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    jsonOutput = reader.ReadToEnd();
                }

                string id = CustomerService.GetInstance().GetCustomerId(jsonOutput);

                _repository.Entities.CustomerRecords.Add(new CustomerRecord
                {
                    id = id,
                    jsonData = jsonOutput,
                    email = email
                });

                _repository.Entities.SaveChanges();
            }
            catch
            {
                throw;
            }

            return jsonOutput;
        }
    }
}
