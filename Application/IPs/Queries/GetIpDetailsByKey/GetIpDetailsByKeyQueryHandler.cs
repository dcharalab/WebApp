using Application.Dtos;
using Domain.Abstractions;
using Infrastructure.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IPs.Queries.GetIpDetailsByKey
{
    public class GetIpDetailsByKeyQueryHandler: IRequestHandler<GetIpDetailsByKeyQuery, IpResponse?>
    {
        private readonly IIpRepository _ipRepository; //CachedIpRepository
        public GetIpDetailsByKeyQueryHandler(IIpRepository ipRepository)
        {
            _ipRepository = ipRepository;
        }
        public async Task<IpResponse?> Handle(GetIpDetailsByKeyQuery request, CancellationToken cancellationToken)
        {
            var countryDetails = await _ipRepository.GetByIpAsync(request.Ip);

            return countryDetails != null? new IpResponse(countryDetails):null;
        }
    }
}
