using Application.Dtos;
using Application.IPs.Queries.GetIpDetailsByKey;
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
        public async Task<ActionResult<IpAdressCountryDto>> Get(string ip)
        {
            var query = new GetIpDetailsByKeyQuery(ip);
            var result = await _mediatr.Send(query);
            return result != null? (ActionResult) Ok(result): NotFound();
        }

    }
}
