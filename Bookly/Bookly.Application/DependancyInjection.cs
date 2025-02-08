using Bookly.Application.Behaviours;
using Bookly.Domain.Bookings;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookly.Application
{
    public static class DependancyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(configuration =>
            {
                configuration.RegisterServicesFromAssembly(typeof(DependancyInjection).Assembly);
                configuration.AddOpenBehavior(typeof(LoggingBehaviour<,>));
                configuration.AddOpenBehavior(typeof(ValidationBehaviour<,>));
            });
            services.AddValidatorsFromAssembly(typeof(DependancyInjection).Assembly);
            services.AddTransient<PricingService>();

            return services;
        }
    }
}
