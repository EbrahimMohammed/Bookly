using Bookly.Application.Abstractions.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookly.Application.Bookings.GetBookings
{
    internal sealed record GetBookingQuery(Guid BookingId) : IQuery<BookingResponse>;
}
