using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace Pldt.Browser.Api.Infrastructure
{
    public interface IPayMayaGateway
    {
        string GenerateAuthValue(string key);
        string SendRequest(string key, string url, string jsonInput);
        string HashGenerator(string value);
    }

    public class PayMayaGateway : IPayMayaGateway
    {
        static IPayMayaGateway s_instance = new PayMayaGateway();

        private PayMayaGateway()
        {
            
        }

        public static IPayMayaGateway GetInstance()
        {
            return s_instance;
        }

        public string GenerateAuthValue(string key)
        {
            string basicAuth = string.Format(
                "Basic {0}",
                Convert.ToBase64String(Encoding.Default.GetBytes(key + ":")));

            return basicAuth;
        }

        public string HashGenerator(string value)
        {
            return Tuple.Create<string>(value).GetHashCode().ToString();
        }

        public string SendRequest(string key, string url, string jsonInput)
        {
            string jsonOutput = string.Empty;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            try
            {
                WebRequest request = WebRequest.Create(url);
                ASCIIEncoding encoding = new ASCIIEncoding();
                byte[] bodyByte = Encoding.UTF8.GetBytes(jsonInput);

                request.Method = "POST";
                request.ContentType = "application/json;";
                request.ContentLength = bodyByte.Length;
                request.Headers["Authorization"] = GenerateAuthValue(key);

                using (Stream dataStream = request.GetRequestStream())
                {
                    dataStream.Write(bodyByte, 0, bodyByte.Length);
                }

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    jsonOutput = reader.ReadToEnd();
                }
            }
            catch
            {
                throw;
            }

            return jsonOutput;
        }
    }
}