using Bookly.Infrastructure.Outbox;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookly.Infrastructure.Extensions
{
    public static class BackgroundJobExtensions
    {
        public static IApplicationBuilder UseBackgroundJobs(this WebApplication app)
        {
            app.Services.GetRequiredService<IRecurringJobManager>
                ().AddOrUpdate<IPorcessOutboxMessagesJob>("outbox-processor",
                job => job.ProcessAsync(),
                app.Configuration["BackgroundJobs:Outbox:Schedule"]);

            return app;

        }
    }
}
