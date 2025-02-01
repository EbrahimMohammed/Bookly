using Bookly.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookly.Domain.Bookings.Events
{
    public sealed record BookingReservedDomainEvent(Guid id) : IDomainEvent;
}
