using Bookly.Application.Abstractions;
using Bookly.Domain.Abstractions;
using Bookly.Infrastructure.Outbox;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookly.Infrastructure
{
    public sealed class ApplicationDbContext : DbContext, IUnitOfWork
    {
        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        };

        private readonly IDateTimeProvider _dateTimeProvider;
        public ApplicationDbContext(DbContextOptions options, IDateTimeProvider dateTimeProvider)
            : base(options)
        {
            _dateTimeProvider = dateTimeProvider;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {

            AddDomainEventsAsOutboxMessages();

            var result = await base.SaveChangesAsync(cancellationToken);

            return result;
        }

        private void AddDomainEventsAsOutboxMessages()
        {
            var outboxMessages = ChangeTracker
                .Entries<Entity>()
                .Select(entry => entry.Entity)
                .SelectMany(entity =>
                {
                    var domainEvents = entity.GetDomainEvents();

                    entity.ClearDomainEvents();

                    return domainEvents;

                }).Select(domainEvent => new OutboxMessage
                (
                    Guid.NewGuid(),
                    _dateTimeProvider.UtcNow,
                    domainEvent.GetType().Name,
                    JsonConvert.SerializeObject(domainEvent, JsonSerializerSettings)
                  ))
                .ToList();

            AddRange(outboxMessages);
        }
    }
}
