using Bookly.Application.Abstractions.Data;
using Bookly.Application.Abstractions.Messaging;
using Bookly.Domain.Abstractions;
using Bookly.Domain.Bookings;
using Dapper;
using Newtonsoft.Json; // Ensure Newtonsoft.Json is used for serialization
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bookly.Application.Apartments.SearchApartments
{
    public sealed class SearchApartmentsQueryHandler :
        IQueryHandler<SearchApartmentQuery, IReadOnlyList<ApartmentResponse>>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        private static readonly string ActiveBookingStatuses =
            string.Join(",", new int[]
            {
                (int)BookingStatus.Reserved,
                (int)BookingStatus.Confirmed,
                (int)BookingStatus.Completed
            });

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
                        a.Id AS Id,
                        a.Name AS Name,
                        a.Description AS Description,
                        a.Price_Amount AS Price,
                        a.Price_Currency AS Currency,
                        a.Address_Country AS Country,
                        a.Address_State AS State,
                        a.Address_ZipCode AS ZipCode,
                        a.Address_City AS City,
                        a.Address_Street AS Street
                    FROM apartments AS a
                    WHERE NOT EXISTS
                    (
                        SELECT 1
                        FROM bookings AS b
                        WHERE
                            b.ApartmentId = a.id AND
                            b.Duration_Start <= @EndDate AND
                            b.Duration_End >= @StartDate AND
                            b.Status IN (SELECT value FROM STRING_SPLIT(@ActiveBookingStatuses, ','))
                    )
                    """;

            var apartments = await connection.QueryAsync<ApartmentResponse, AddressResponse, ApartmentResponse>(
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
