using Bookly.Application.Abstractions;
using Bookly.Application.Abstractions.Data;
using Bookly.Application.Abstractions.Email;
using Bookly.Domain.Abstractions;
using Bookly.Domain.Apartments;
using Bookly.Domain.Bookings;
using Bookly.Domain.Users;
using Bookly.Infrastructure.Clock;
using Bookly.Infrastructure.Data;
using Bookly.Infrastructure.Email;
using Bookly.Infrastructure.Outbox;
using Bookly.Infrastructure.Repositories;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Hangfire;
namespace Bookly.Infrastructure
{
    public static class DependancyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddTransient<IDateTimeProvider, DateTimeProvider>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddSingleton<OutboxInterceptor>();
            services.Configure<OutboxOptions>(configuration.GetSection("Outbox"));

            var connectionString =
                configuration.GetConnectionString("Database") ??
                throw new ArgumentNullException(nameof(configuration));

            services.AddDbContext<ApplicationDbContext>((provider, options) =>
            {
                options.UseSqlServer(connectionString);

                var interceptor = provider.GetRequiredService<OutboxInterceptor>();
                options.AddInterceptors(interceptor);
            });

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IApartmentRepository, ApartmentRepository>();
            services.AddScoped<IBookingRepository, BookingRepository>();
            services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());
            services.AddSingleton<ISqlConnectionFactory>(new SqlConnectionFactory(connectionString));
            SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());
            AddBackgroundJobs(services, configuration);
            return services;
        }


        private static void AddBackgroundJobs(IServiceCollection services, IConfiguration configuration)
        {
            services.AddHangfire(config =>
               config.UseSqlServerStorage(
                  configuration.GetConnectionString("Database")
               ));

            services.AddHangfireServer(options => options.SchedulePollingInterval = TimeSpan.FromSeconds(1));

            services.AddScoped<IPorcessOutboxMessagesJob, ProcessOutboxMessagesJob>();


        }
    }
}
