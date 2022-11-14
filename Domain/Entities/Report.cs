using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Primitives;

namespace Domain.Entities
{
    public class Report: Entity
    {
        public string CountryName { get; set; }

        public int AddressesCount { get; set; }

        public DateTime LastAdderssUpdated { get; set; }
    }
}