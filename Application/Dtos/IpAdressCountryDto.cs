using Domain.Entities;
using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos
{
    public class IpAdressCountryDto
    {
        public string CountryName { get; set; }

        public string TwoLetterCode { get; set; }

        public string ThreeLetterCode { get; set; }

        public IpAdressCountryDto(IpCountry country)
        {
            CountryName = country.Name;
            TwoLetterCode = country.TwoLetterCode;
            ThreeLetterCode = country.ThreeLetterCode;  
        }
    }

}
