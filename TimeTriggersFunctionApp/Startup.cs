using Domain.Abstractions;
using Infrastructure.Repositories.Cached;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.Extensions.Http;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTriggersFunctionApp.Interfaces;
using TimeTriggersFunctionApp.HelperClasses;

[assembly: FunctionsStartup(typeof(TimeTriggersFunctionApp.Startup))]
namespace TimeTriggersFunctionApp
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddScoped<IIpRepository, CachedIpRepository>();
            builder.Services.AddScoped<IIpDataApi, IpDataApi>();

            builder.Services.AddStackExchangeRedisCache(options => {
                options.Configuration = Environment.GetEnvironmentVariable("RedisConnStr");
            });

            builder.Services.AddHttpClient("ip2c", httpClient =>
            {
                httpClient.BaseAddress = new Uri(Environment.GetEnvironmentVariable("ip2cURI"));
            });
        }
    }
}
