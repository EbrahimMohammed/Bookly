﻿using Bookly.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookly.Domain.Apartments
{
    public sealed class Apartment : Entity
    {
        public Apartment(Guid id): base(id)
        {

        }

        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Country { get; private set; }
        public string State { get; private set; }
        public string ZipCode { get; private set; }
        public string City { get; private set; }
        public string Street { get; private set; }
        public string PriceAmount { get; private set; }
        public string PriceCurrency { get; private set; }
        public string CleaningFeeAmount { get; private set; }
        public string CleaningFeeCurrency { get; private set; }
        public DateTime? LastBookedOnUtc { get; private set; }

        public List<Amenity> Amenities { get; private set; } = new();

    }
}
