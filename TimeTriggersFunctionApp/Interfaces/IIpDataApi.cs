using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TimeTriggersFunctionApp.Interfaces
{
    public interface IIpDataApi
    {
        public Task<IpCountry> GetIpDetails(string ip);
    }
}
