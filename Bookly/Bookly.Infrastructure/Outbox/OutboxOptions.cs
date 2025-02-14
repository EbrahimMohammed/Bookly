using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookly.Infrastructure.Outbox
{
    internal class OutboxOptions
    {
        public int IntervalInSeconds { get; init; }

        public int BatchSize { get; init; }
    }
}
