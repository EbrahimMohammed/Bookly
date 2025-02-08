using Bookly.Domain.Apartments;
using Bookly.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookly.Infrastructure.Configurations
{
    internal sealed class ApartmentConfiguration : IEntityTypeConfiguration<Apartment>
    {
        public void Configure(EntityTypeBuilder<Apartment> builder)
        {
            builder.ToTable("Apartments");
            builder.HasKey(a => a.Id);
            builder.OwnsOne(a => a.Address);

            builder.Property(a => a.Name)
                .HasMaxLength(200)
                .HasConversion(name => name.Value, value => new Name(value));

            builder.Property(a => a.Description)
                .HasMaxLength(2000)
                .HasConversion(description => description.Value, value => new Description(value));

            builder.OwnsOne(a => a.Price, priceBuilder =>
               {
                   priceBuilder.Property(money => money.Currency)
                       .HasConversion(
                           currency => currency.Code, 
                           code => Currency.FromCode(code) 
                       );
               });
            
            builder.OwnsOne(a => a.CleaningFee, priceBuilder =>
               {
                   priceBuilder.Property(money => money.Currency)
                       .HasConversion(
                           currency => currency.Code, 
                           code => Currency.FromCode(code) 
                       );
               });

        }
    }
}
