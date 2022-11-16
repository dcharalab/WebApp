using Azure.Core;
using Domain.Entities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TimeTriggersFunctionApp.HelperClasses
{
    public class IpDataApi
    {
        public IpDataApi()
        {
        }

        public async Task<IpCountry> GetIpDetails(string ip)
        {
            using var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(Environment.GetEnvironmentVariable("ip2cURI"));
            var httpResponseMessage = await httpClient.GetAsync(ip);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var content = await httpResponseMessage.Content.ReadAsStringAsync();

                string s = content;
                string[] values = s.Split(';');
                if (values[0] == "1")
                {
                    var countryDetails = new IpCountry(Guid.NewGuid(), values[3], values[1], values[2]);
                    return countryDetails;
                }
            }
            return null;
        }
       
    }
}