using Application.Dtos;
using Application.IPs.Queries.GetIpDetailsByKey;
using Application.IPs.Queries.GetReport;
using Domain.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Mvc;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IpWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IpController : ControllerBase
    {
        private readonly IMediator _mediatr;

        public IpController(IMediator mediatr)
        {
            _mediatr = mediatr;
        }

        // GET api/<IpController>/5
        [HttpGet("{ip}")]
        public async Task<ActionResult<IpResponse>> Get(string ip)
        {
            var query = new GetIpDetailsByKeyQuery(ip);
            var result = await _mediatr.Send(query);
            return result != null? (ActionResult) Ok(result): NotFound();
        }

        [HttpGet("GetReport")]
        public async Task<ActionResult<List<ReportResponse>>> GetReport([FromQuery] List<string> codes)
        {
            var query = new GetReportQuery(codes);
            var result = await _mediatr.Send(query);
            return result != null? (ActionResult) Ok(result): NotFound();
        }
    }
}
