using Bookly.Application.Abstractions;
using Bookly.Application.Abstractions.Data;
using Bookly.Domain.Abstractions;
using Dapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookly.Infrastructure.Outbox
{
    internal sealed class ProcessOutboxMessagesJob : IPorcessOutboxMessagesJob
    {
        private static readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        };

        private readonly ISqlConnectionFactory _sqlConnectionFactory;
        private readonly IPublisher _publisher;
        private readonly OutboxOptions _outboxOptions;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ILogger<ProcessOutboxMessagesJob> _logger;


        public ProcessOutboxMessagesJob(ISqlConnectionFactory sqlConnectionFactory,
            IPublisher publisher,
            IOptions<OutboxOptions> options,
            ILogger<ProcessOutboxMessagesJob> logger,
            IDateTimeProvider dateTimeProvider)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
            _publisher = publisher;
            _outboxOptions = options.Value;
            _logger = logger;
            _dateTimeProvider = dateTimeProvider;
        }


        public async Task ProcessAsync()
        {
            _logger.LogInformation("Processing outbox messages");

            using var connection = _sqlConnectionFactory.CreateConnection();
            using var transaction = connection.BeginTransaction();

            var outboxMessages = await GetOutboxMessagesAsync(connection, transaction);

            foreach (var outboxMessage in outboxMessages)
            {
                Exception? exception = null;
                try
                {

                    var domainEvent = JsonConvert.DeserializeObject<IDomainEvent>(
                        outboxMessage.Content,
                        _jsonSerializerSettings);

                    await _publisher.Publish(domainEvent);

                }
                catch (Exception caughtException)
                {
                    _logger.LogError(caughtException,
                        "Error processing outbox message {OutboxMessageId}",
                        outboxMessage.Id);

                    exception = caughtException;
                }

                await UpdateOutboxMessageAsync(connection, transaction, outboxMessage, exception);
            }

            transaction.Commit();

            _logger.LogInformation("Outbox messages processed");
        }
        private async Task<IReadOnlyList<OutBoxMessageResponse>> GetOutboxMessagesAsync(
            IDbConnection connection,
            IDbTransaction transaction)
        {

            var sql = $"""
                       SELECT TOP ({_outboxOptions.BatchSize}) Id, Content
                       FROM OutboxMessages WITH (UPDLOCK, ROWLOCK, READPAST)
                       WHERE ProcessedOnUtc IS NULL
                       ORDER BY OccuredOnUtc
                       """;

            var outboxMessages = await connection.QueryAsync<OutBoxMessageResponse>(
                sql,
                transaction: transaction);

            return outboxMessages.ToList();

        }


        private async Task UpdateOutboxMessageAsync(
            IDbConnection connection,
            IDbTransaction transaction,
            OutBoxMessageResponse outboxMessage,
            Exception? exception)
        {
            var sql = $"""
                       UPDATE OutboxMessages
                       SET ProcessedOnUtc = @ProcessedOnUtc,
                           Error = @Error
                       WHERE Id = @Id
                       """;

            await connection.ExecuteAsync(
                sql,
                new
                {
                    outboxMessage.Id,
                    ProcessedOnUtc = _dateTimeProvider.UtcNow,
                    Error = exception?.ToString()
                },
                transaction: transaction);
        }


        internal sealed record OutBoxMessageResponse(Guid Id, string Content);

    }
}
