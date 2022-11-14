using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Abstractions
{
    public interface IRawSqlIpRepository
    {
        public Task<List<Report>> GetReport(List<string>? codes);
    }
}