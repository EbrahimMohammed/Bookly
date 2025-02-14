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
using Quartz;
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

            var connectionString =
                configuration.GetConnectionString("Database") ??
                throw new ArgumentNullException(nameof(configuration));

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
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
            services.Configure<OutboxOptions>(options =>
                  configuration.GetSection("Outbox").Bind(options));

            services.AddQuartz();

            services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

            services.ConfigureOptions<ProcessOutboxMessagesJobSetup>();
        }
    }
}
