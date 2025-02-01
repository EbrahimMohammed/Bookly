using Bookly.Domain.Apartments;
using Bookly.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookly.Domain.Bookings
{
    public class PricingService
    {
        public PricingDetails CalculatePrice(Apartment apartment, DateRange duration)
        {
            var currency = apartment.Price.Currency;

            var priceForPeriod = new Money(apartment.Price.Amount * duration.LengtInDays,
                currency);

            decimal percentageUpCharge = 0;

            foreach (var amenity in apartment.Amenities)
            {
                percentageUpCharge += amenity switch
                {

                    Amenity.GardenView or Amenity.MountainView => 0.05m,
                    Amenity.AirConditioning => 0.01m,
                    Amenity.Parking => 0.01m,
                    _ => 0
                };

            }

            var amenitiesUpCharge = Money.Zero(currency);   

            if(percentageUpCharge > 0)
            {
                amenitiesUpCharge = new Money(priceForPeriod.Amount * percentageUpCharge, currency);
            }

            var totalPrice = Money.Zero(currency);

            totalPrice += priceForPeriod;

            if(!apartment.CleaningFee.IsZero())
            {
                totalPrice += apartment.CleaningFee;
            }

            totalPrice += amenitiesUpCharge;

            return new PricingDetails(priceForPeriod, apartment.CleaningFee, amenitiesUpCharge, totalPrice);
        }
    }
}

