using Bookly.Domain.Apartments;
using Bookly.Domain.Bookings;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookly.Infrastructure.Repositories;

internal sealed class BookingRepository : Repository<Booking>, IBookingRepository
{
    private static readonly BookingStatus[] ActiveBookingStatus = new[]
    {
        BookingStatus.Reserved,
        BookingStatus.Confirmed,
        BookingStatus.Completed,
    };
    public BookingRepository(ApplicationDbContext dbContext)
        : base(dbContext)
    {

    }

    public async Task<bool> IsOverlappingAsync(Apartment apartment, 
        DateRange duration, 
        CancellationToken cancellationToken = default)
    {
       return await DbContext.Set<Booking>()
            .AnyAsync(booking => 
                booking.ApartmentId == apartment.Id &&
                booking.Duration.End >= duration.Start &&
                booking.Duration.Start <= duration.End &&
                ActiveBookingStatus.Contains(booking.Status),
                cancellationToken);
    }
}
