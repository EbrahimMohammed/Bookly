using Bookly.Domain.Abstractions;
using Bookly.Domain.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookly.Domain.Apartments
{
    public sealed class Apartment : Entity
    {
        public Apartment(Guid id,
            Name name,
            Description description,
            Address address,
            Money price,
            Money cleaningFee,
                List<Amenity> amenities

            ) : base(id)
        {
            Name = name;
            Description = description;
            Address = address;
            Price = price;
            CleaningFee = cleaningFee;
            Amenities = amenities;
        }

        private Apartment() { }

        public Name Name { get; private set; }
        public Description Description { get; private set; }

        public Address Address { get; private set; }
        public Money Price { get; private set; }
        public Money CleaningFee { get; private set; }
        public DateTime? LastBookedOnUtc { get; internal set; }

        private string _amenitiesJson = "[]"; // Backing field for EF Core

        public List<Amenity> Amenities
        {
            get => JsonConvert.DeserializeObject<List<Amenity>>(_amenitiesJson) ?? new List<Amenity>();
            private set => _amenitiesJson = JsonConvert.SerializeObject(value);
        }
    }
}
