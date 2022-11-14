using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.IPs.Queries.GetReport
{
    public class ReportResponse
    {
        public string CountryName { get; set; }

        public int AddressesCount { get; set; }

        public DateTime LastAdderssUpdated { get; set; }

        public ReportResponse(Report report)
        {
            CountryName = report.CountryName;
            AddressesCount = report.AddressesCount;
            LastAdderssUpdated = report.LastAdderssUpdated;
        }
    }
}