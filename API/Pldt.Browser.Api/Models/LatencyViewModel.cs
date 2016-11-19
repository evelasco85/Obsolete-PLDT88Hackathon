using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pldt.Browser.Api.Models
{
    public class StatisticField
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class LatencyViewModel
    {
        public string UrlOrIp { get; set; }
        public string Status { get; set; }
        public IList<StatisticField> Statistics { get; set; }

        public LatencyViewModel()
        {
            Statistics = new List<StatisticField>();
        }
    }
}