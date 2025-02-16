using Bookly.Application.Abstractions;
using Bookly.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bookly.Infrastructure.Outbox
{
    public sealed class OutboxInterceptor : SaveChangesInterceptor
    {

        private static readonly JsonSerializerSettings SerializerSettings = new()
        {
            TypeNameHandling = TypeNameHandling.All,
        };

        private readonly IDateTimeProvider _dateTimeProvider;

        public OutboxInterceptor(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
        }


        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {

            if (eventData.Context is not null)
            {
                InsertOutboxMessages(eventData.Context);
            }
            return base.SavingChangesAsync(eventData, result, cancellationToken);

        }

        private void InsertOutboxMessages(DbContext context)
        {
            var outboxMessages = context.ChangeTracker
               .Entries<Entity>()
               .Select(entry => entry.Entity)
               .SelectMany(entity =>
               {
                   var domainEvents = entity.GetDomainEvents();
                   entity.ClearDomainEvents();
                   return domainEvents;
               })
               .Select(domainEvent => new OutboxMessage
               (
                   Guid.NewGuid(),
                   _dateTimeProvider.UtcNow,
                   domainEvent.GetType().Name,
                   JsonConvert.SerializeObject(domainEvent, SerializerSettings)
               ))
               .ToList();

            context.Set<OutboxMessage>().AddRange(outboxMessages);

        }
    }

}
