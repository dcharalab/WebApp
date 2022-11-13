using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Helpers;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Cached
{
    public class CachedIpRepository : IIpRepository
    {
        private readonly IIpRepository _ipRepository; //IpRepository
        private readonly IDistributedCache _cache;
        public CachedIpRepository(IIpRepository ipRepository, IDistributedCache cache)
        {
            _ipRepository= ipRepository;
            _cache = cache;
        }
        public async Task<IpCountry?> GetByIpAsync(string ip)
        {
            //List<User>? users;
            string recordKey = ip;

            var ipDetails = await _cache.GetRecordAsync<IpCountry>(recordKey); // Get data from cache

            if (ipDetails is null) // Data not available in the Cache
            {
                ipDetails = await _ipRepository.GetByIpAsync(ip); // Read data from ipRepository
                if (ipDetails != null)
                {
                    await AddCountryIpData(ip, ipDetails, false);
                }
            }         

            return ipDetails;
        }

        public async Task AddCountryIpData(string ip, IpCountry ipCountry, bool reachDB)
        {
            await _cache.SetRecordAsync(ip, ipCountry); // Set cache
            if (reachDB)
            {
                await _ipRepository.AddCountryIpData(ip, ipCountry, reachDB);
            }
        }
    }
}
