using Microsoft.Extensions.Options;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookly.Infrastructure.Outbox
{
    internal sealed class ProcessOutboxMessagesJobSetup : IConfigureOptions<QuartzOptions>
    {
        private readonly OutboxOptions _outboxOptions;

        public ProcessOutboxMessagesJobSetup(IOptions<OutboxOptions> outboxOptions)
        {
            _outboxOptions = outboxOptions.Value;
        }
        public void Configure(QuartzOptions options)
        {
            const string jobName = nameof(ProcessOutboxMessagesJobSetup);

            options.
                AddJob<ProcessOutboxMessages>(configure => configure.WithIdentity(jobName))
                .AddTrigger(configure =>
                 configure.ForJob(jobName)
                 .WithSimpleSchedule(scheduel =>
                    scheduel.WithIntervalInSeconds(_outboxOptions.IntervalInSeconds).RepeatForever()));

        }
    }
}
