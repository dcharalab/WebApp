using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Abstractions
{
    public interface IIpRepository
    {
        public Task<IpCountry?> GetByIpAsync(string ip);
    }
}
