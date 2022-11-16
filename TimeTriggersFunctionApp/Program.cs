using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTriggersFunctionApp
{
    public class Program
    {
        public static void Main()
        {
            var host = new HostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddStackExchangeRedisCache(options => {
                         options.Configuration = Environment.GetEnvironmentVariable("RedisConnStr");
                     });
                }).Build();
            host.Run();
        }
    }
}
