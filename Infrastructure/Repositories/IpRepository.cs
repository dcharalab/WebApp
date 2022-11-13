using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class IpRepository: IIpRepository
    {
        private readonly IpdbContext _context;
        public IpRepository(IpdbContext context)
        {
            _context = context;
        }

        public async Task AddCountryIpData(string ip, IpCountry ipCountry, bool reachDB)
        {
            var country = _context.Countries.Where(c => c.ThreeLetterCode == ipCountry.ThreeLetterCode).FirstOrDefault();
            var newIp = new Ipaddress();
            newIp.Ip = ip;

            if (country != null)
            {
                newIp.Country = country;
            }
            else
            {
                var newCountry = new Country()
                {
                    Name = ipCountry.Name,
                    TwoLetterCode = ipCountry.TwoLetterCode,
                    ThreeLetterCode = ipCountry.ThreeLetterCode
                };

                _context.Add<Country>(newCountry);
                _context.SaveChanges();
                newIp.CountryId = newCountry.Id;
            }

            _context.Add<Ipaddress>(newIp);
            await _context.SaveChangesAsync();
        }

        public async Task<IpCountry?> GetByIpAsync(string ip)
        {
            var ipDetails = await _context.Ipaddresses.Include("Country").Where(x => x.Ip == ip).Select(y => y.Country).FirstOrDefaultAsync();
            return ipDetails != null? new IpCountry(Guid.NewGuid(), ipDetails.Name, ipDetails.TwoLetterCode, ipDetails.ThreeLetterCode): null;
        }

    }
}
