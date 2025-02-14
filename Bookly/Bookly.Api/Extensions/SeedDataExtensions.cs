using Bogus;
using Bookly.Application.Abstractions.Data;
using Bookly.Domain.Apartments;
using Dapper;
using Newtonsoft.Json;

namespace Bookly.Api.Extensions
{
    public static class SeedDataExtensions
    {
        public static void SeedApartmentsData(this IApplicationBuilder app)
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

        public static void SeedUsersData(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();

            var sqlConnectionFactory = scope.ServiceProvider.GetRequiredService<ISqlConnectionFactory>();
            using var connection = sqlConnectionFactory.CreateConnection();

            var faker = new Faker();

            // Track generated emails to ensure uniqueness
            var generatedEmails = new HashSet<string>();

            List<object> users = new();
            for (var i = 0; i < 100; i++)
            {
                string email;

                // Keep generating until a unique email is found
                do
                {
                    faker = new Faker();
                    email = faker.Person.Email;
                } while (!generatedEmails.Add(email));  // Will add email if it's unique, otherwise it will retry

                users.Add(new
                {
                    Id = Guid.NewGuid(),
                    FirstName = faker.Person.FirstName,
                    LastName = faker.Person.LastName,
                    Email = email,
                });
            }

            const string sql = """
            INSERT INTO dbo.users
            (Id, FirstName, LastName, Email)
            VALUES(@Id, @FirstName, @LastName, @Email);
            """;

            connection.Execute(sql, users);

        }
    }
}
