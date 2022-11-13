using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class IpRepository: IIpRepository
    {
        IpdbContext _context;
        public IpRepository(IpdbContext context)
        {
            _context = context;
        }

        public async Task<IpCountry?> GetByIpAsync(string ip)
        {
            var ipDetails = await _context.Ipaddresses.Include("Country").Where(x => x.Ip == ip).Select(y => y.Country).FirstOrDefaultAsync();
            
            return ipDetails != null? new IpCountry(ipDetails.Name, ipDetails.TwoLetterCode, ipDetails.ThreeLetterCode): null;
        }

    }
}
