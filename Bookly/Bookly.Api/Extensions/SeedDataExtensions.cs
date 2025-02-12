using Bogus;
using Bookly.Application.Abstractions.Data;
using Bookly.Domain.Apartments;
using Dapper;
using Newtonsoft.Json;

namespace Bookly.Api.Extensions
{
    public static class SeedDataExtensions
    {
        public static void SeedData(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();

            var sqlConnectionFactory = scope.ServiceProvider.GetRequiredService<ISqlConnectionFactory>();
            using var connection = sqlConnectionFactory.CreateConnection();

            var faker = new Faker();

            List<object> apartments = new();
            for (var i = 0; i < 100; i++)
            {
                apartments.Add(new
                {
                    Id = Guid.NewGuid(),
                    Name = faker.Company.CompanyName(),
                    Description = "Amazing view",
                    Country = faker.Address.Country(),
                    State = faker.Address.State(),
                    ZipCode = faker.Address.ZipCode(),
                    City = faker.Address.City(),
                    Street = faker.Address.StreetAddress(),
                    PriceAmount = faker.Random.Decimal(50, 1000),
                    PriceCurrency = "USD",
                    CleaningFeeAmount = faker.Random.Decimal(25, 200),
                    CleaningFeeCurrency = "USD",
                    Amenities = JsonConvert.SerializeObject(new List<int> { (int)Amenity.Parking, (int)Amenity.MountainView }) // Serialize JSON
                });
            }

            const string sql = """
            INSERT INTO dbo.apartments
            (id, "name", description, Address_Country, Address_State, Address_ZipCode, Address_City, Address_Street, Price_Amount, Price_Currency, CleaningFee_Amount, CleaningFee_Currency, Amenities)
            VALUES(@Id, @Name, @Description, @Country, @State, @ZipCode, @City, @Street, @PriceAmount, @PriceCurrency, @CleaningFeeAmount, @CleaningFeeCurrency, @Amenities);
            """;

            connection.Execute(sql, apartments);
        }
    }
}
