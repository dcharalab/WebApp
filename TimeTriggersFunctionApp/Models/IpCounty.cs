using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTriggersFunctionApp.Models
{
    public class IpCounty
    {
        public string Ip { get; set; }
        public string CountryName { get; set; }

        public string TwoLetterCode { get; set; }

        public string ThreeLetterCode { get; set; }

        public IpCounty(string ip, string twoLetterCode, string threeLetterCode, string countryName)
        {
            Ip = ip;
            CountryName = countryName;
            TwoLetterCode = twoLetterCode;
            ThreeLetterCode = threeLetterCode;
        }
    }
}
