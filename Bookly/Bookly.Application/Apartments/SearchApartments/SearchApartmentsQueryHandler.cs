using Bookly.Application.Abstractions.Data;
using Bookly.Application.Abstractions.Messaging;
using Bookly.Domain.Abstractions;
using Bookly.Domain.Bookings;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Bookly.Application.Apartments.SearchApartments
{
    public sealed class SearchApartmentsQueryHandler :
        IQueryHandler<SearchApartmentQuery, IReadOnlyList<ApartmentResponse>>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;
        private static readonly int[] ActiveBookingStatuses =
    {
        (int)BookingStatus.Reserved,
        (int)BookingStatus.Confirmed,
        (int)BookingStatus.Completed
    };
        public SearchApartmentsQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }
        public async Task<Result<IReadOnlyList<ApartmentResponse>>> Handle(SearchApartmentQuery request, CancellationToken cancellationToken)
        {
            if (request.StartDate > request.EndDate)
            {
                return new List<ApartmentResponse>();
            }

            using var connection = _sqlConnectionFactory.CreateConnection();

            var sql = """
                     SELECT
                    a.id AS Id,
                    a.name AS Name,
                    a.description AS Description,
                    a.price_amount AS Price,
                    a.price_currency AS Currency,
                    a.address_country AS Country,
                    a.address_state AS State,
                    a.address_zip_code AS ZipCode,
                    a.address_city AS City,
                    a.address_street AS Street
                FROM apartments AS a
                WHERE NOT EXISTS
                (
                    SELECT 1
                    FROM bookings AS b
                    WHERE
                        b.apartment_id = a.id AND
                        b.duration_start <= @EndDate AND
                        b.duration_end >= @StartDate AND
                        b.status = ANY(@ActiveBookingStatuses)
                )
                """;

            var apartments = await connection
        .QueryAsync<ApartmentResponse, AddressResponse, ApartmentResponse>(
            sql,
            (apartment, address) =>
            {
                apartment.Address = address;

                return apartment;
            },
            new
            {
                request.StartDate,
                request.EndDate,
                ActiveBookingStatuses
            },
            splitOn: "Country");

            return apartments.ToList();
        }
    }
}
