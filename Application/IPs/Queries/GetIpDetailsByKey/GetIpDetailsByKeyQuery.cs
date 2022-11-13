using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos;
using MediatR;

namespace Application.IPs.Queries.GetIpDetailsByKey
{
    public class GetIpDetailsByKeyQuery: IRequest<IpResponse>
    {
        public string Ip { get;}
        public GetIpDetailsByKeyQuery(string ip)
        {
            Ip = ip;
        }
    }
}
