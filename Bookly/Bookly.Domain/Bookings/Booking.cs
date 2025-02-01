using Bookly.Domain.Abstractions;
using Bookly.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookly.Domain.Bookings
{
    public sealed class Booking : Entity
    {
        private Booking(Guid id,
            Guid apartmentId,
            Guid userId,
            DateRange duration,
            Money priceForPeriod,
            Money cleaningFee,
            Money amenitiesCharge,
            Money totalPrice,
            BookingStatus status,
            DateTime createdOnUtc) : base(id)
        {
            ApartmentId = apartmentId;
            UserId = userId;
            Duration = duration;
            PriceForPeriod = priceForPeriod;
            CleaningFee = cleaningFee;
            AmenitiesCharge = amenitiesCharge;
            TotalPrice = totalPrice;
            Status = status;
            CreatedOnUtc = createdOnUtc;

        }


        public Guid ApartmentId { get; private set; }
        public Guid UserId { get; private set; }
        public DateRange Duration { get; private set; }
        public Money PriceForPeriod { get; private set; }
        public Money CleaningFee { get; private set; }
        public Money AmenitiesCharge { get; private set; }
        public Money TotalPrice { get; private set; }
        public BookingStatus Status { get; private set; }

        public DateTime CreatedOnUtc { get; set; }

        public static Booking Reserve(Guid apartmentId,
            Guid userId,
            DateRange duration,
            DateTime utcNow,
            PricingDetails pricingDetails
            )
        {
            var booking = new Booking(Guid.NewGuid(),
                apartmentId,
                userId,
                duration,
                pricingDetails.PriceForPeriod,
                pricingDetails.CleaningFee,
                pricingDetails.AmenitiesCharge,
                pricingDetails.TotalPrice,
                BookingStatus.Reserved,
                utcNow);

            booking.RaiseDomainEvent(new Events.BookingReservedDomainEvent(booking.Id));

            return booking;
        }
    }
}
