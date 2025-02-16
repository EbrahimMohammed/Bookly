namespace Bookly.Infrastructure.Outbox
{
    internal interface IPorcessOutboxMessagesJob
    {
        public Task ProcessAsync();
    }
}