using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        services.AddStackExchangeRedisCache(options => {
            options.Configuration = Environment.GetEnvironmentVariable("RedisConnStr");
        });
    })
    .Build();

host.Run();
