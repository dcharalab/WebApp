using Application.Dtos;
using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.IPs.Queries.GetIpDetailsByKey
{
    public class GetIpDetailsByKeyQueryHandler: IRequestHandler<GetIpDetailsByKeyQuery, IpResponse?>
    {
        private readonly IIpRepository _ipRepository; //CachedIpRepository
        private readonly IHttpClientFactory _httpClientFactory;
        public GetIpDetailsByKeyQueryHandler(IIpRepository ipRepository, IHttpClientFactory httpClientFactory)
        {
            _ipRepository = ipRepository;
            _httpClientFactory = httpClientFactory;
        }
        public async Task<IpResponse?> Handle(GetIpDetailsByKeyQuery request, CancellationToken cancellationToken)
        {
            var countryDetails = await _ipRepository.GetByIpAsync(request.Ip);

            if (countryDetails == null)
            {
                //data not in cache or DB, call api
                var httpClient = _httpClientFactory.CreateClient("ip2c");
                var httpResponseMessage = await httpClient.GetAsync(request.Ip, cancellationToken);
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    var content = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);

                    string s = content;
                    string[] values = s.Split(';');
                    if (values[0] == "1")
                    {
                        countryDetails = new IpCountry(Guid.NewGuid(), values[3], values[1], values[2]);
                        await _ipRepository.AddCountryIpData(request.Ip, countryDetails, true);
                        return new IpResponse(countryDetails);
                    }
                }
            }

            return countryDetails != null? new IpResponse(countryDetails):null;
        }
    }
}
