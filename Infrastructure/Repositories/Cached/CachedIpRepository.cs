using Domain.Abstractions;
using Domain.Entities;
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
        public CachedIpRepository(IIpRepository ipRepository)
        {
            _ipRepository= ipRepository;
        }
        public Task<IpCountry?> GetByIpAsync(string ip)
        {
            throw new NotImplementedException();
        }
    }
}
