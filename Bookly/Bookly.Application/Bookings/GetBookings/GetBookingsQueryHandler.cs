using Bookly.Application.Abstractions.Data;
using Bookly.Application.Abstractions.Messaging;
using Bookly.Domain.Abstractions;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookly.Application.Bookings.GetBookings
{
    internal sealed class GetBookingsQueryHandler : IQueryHandler<GetBookingQuery, BookingResponse>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFaCtory;

        public GetBookingsQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFaCtory = sqlConnectionFactory;
        }
        public async Task<Result<BookingResponse>> Handle(GetBookingQuery request, CancellationToken cancellationToken)
        {
            var connection = _sqlConnectionFaCtory.CreateConnection();

            var sql = """
                SELECT 
                id As Id,
                apartment_id As ApartmentId,
                user_id As UserId,
                status As Status,
                price_for_period As PriceForPeriod,
                price_for_period_currency As PriceForPeriodCurrency,
                cleaning_fee_amount As CleaningFeeAmount,
                cleaning_fee_currency As CleaningFeeCurrency,
                amenities_upcharge_amount As AmenitiesUpChargeAmount,
                amenities_upcharge_currency As AmenitiesUpChargeCurrency,
                total_price As TotalPriceAmount,
                total_price_currency As TotalPriceCurrency,
                duration_start As DurationStart,
                duration_end As DurationEnd,
                created_on_utc As CreatedOnUtc
                From Bookings
                WHERE id = @BookingId  
                """;

            var booking = await connection.QueryFirstOrDefaultAsync<BookingResponse>(
                sql,
                new
                {
                    request.BookingId
                });

            return booking;
        }
    }
}
