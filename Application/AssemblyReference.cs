using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Application
{
    public static class AssemblyReference
    {

        public static IServiceCollection AddApplicationModule(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddMediatR(typeof(AssemblyReference).Assembly);

            return serviceCollection;
        }
    }
}
