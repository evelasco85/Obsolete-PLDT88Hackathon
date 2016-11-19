using Pldt.Browser.Api.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;

namespace Pldt.Browser.Api.Controllers
{
    public class LatencyController : ApiController
    {
        [HttpGet]
        public async Task<JsonResult<LatencyViewModel>> Get(string urlOrIp)
        {
            LatencyViewModel latency = new LatencyViewModel();

            latency.UrlOrIp = urlOrIp;
            string latencyApiTester = string.Format("http://api.hackertarget.com/nping/?q={0}", urlOrIp);
            WebRequest request = WebRequest.Create(latencyApiTester);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                string responseFromServer = reader.ReadToEnd();

                IList<string> lineItems = responseFromServer
                    .Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                    .ToList();

                latency.Statistics = lineItems
                    .Where(line =>
                        !string.IsNullOrEmpty(line) &&
                        line.ToLower().Contains("max rtt:")
                    )
                   .SelectMany(line => line.Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                   .Select(latencyEntry =>
                   {
                       string[] segments = latencyEntry.Split(":".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                       if ((segments != null) && (segments.Count() < 2))
                           return new StatisticField();

                       return new StatisticField
                       {
                           Name = segments[0],
                           Value = segments[1]
                       };
                   })
                   .ToList();
                
            }

            return Json(latency);
        }
    }
}
