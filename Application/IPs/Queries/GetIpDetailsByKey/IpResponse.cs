using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IPs.Queries.GetIpDetailsByKey
{
    public class IpResponse
    {
        public string CountryName { get; set; }

        public string TwoLetterCode { get; set; }

        public string ThreeLetterCode { get; set; }

        public IpResponse(IpCountry country)
        {
            CountryName = country.Name;
            TwoLetterCode = country.TwoLetterCode;
            ThreeLetterCode = country.ThreeLetterCode;
        }
    }
}
