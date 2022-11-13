using Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class IpCountry: Entity
    {
        public string Name { get; set; } 

        public string TwoLetterCode { get; set; }

        public string ThreeLetterCode { get; set; }


        public IpCountry(string name, string twoLetterCode, string threeLetterCode)
        {
            Name = name;
            TwoLetterCode = twoLetterCode;
            ThreeLetterCode = threeLetterCode;
        }
    }
}
