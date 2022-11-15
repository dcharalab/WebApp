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
using TimeTriggersFunctionApp.Interfaces;

namespace TimeTriggersFunctionApp.HelperClasses
{
    public class IpDataApi : IIpDataApi
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public IpDataApi(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IpCountry> GetIpDetails(string ip)
        {
            var httpClient = _httpClientFactory.CreateClient("ip2c");
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