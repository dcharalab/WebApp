using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Abstractions;
using MediatR;

namespace Application.IPs.Queries.GetReport
{
    public class GetReportQueryHandler : IRequestHandler<GetReportQuery, List<ReportResponse>>
    {
        private readonly IRawSqlIpRepository _rawSqlIpRepository;
        public GetReportQueryHandler(IRawSqlIpRepository rawSqlRepository)
        {
            _rawSqlIpRepository = rawSqlRepository;
        }
        public async Task<List<ReportResponse>> Handle(GetReportQuery request, CancellationToken cancellationToken)
        {
            var result =  await _rawSqlIpRepository.GetReport(request.Codes);
            var reportList = new List<ReportResponse>();

            foreach (var item in result)
            {
                reportList.Add(new ReportResponse(item));
            }

            return reportList;
        }
    }
}