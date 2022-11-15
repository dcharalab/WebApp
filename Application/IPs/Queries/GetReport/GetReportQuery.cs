using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace Application.IPs.Queries.GetReport
{
    public class GetReportQuery: IRequest<List<ReportResponse>>
    {
        public List<string>? Codes  { get;}
        public GetReportQuery(List<string>? codes)
        {
            Codes = codes;
        }
    }
}