namespace SharedKernel.Common.Interfaces
{
    public interface IEventPublisherService
    {
        // Método para CQRS
        Task PublishEventAsync(
            string entityName,
            string operationType,
            bool success,
            string performedBy,
            string? reason = null,
            object? additionalData = null,
            string exchangeName = "default_exchange",
            string routingKey = "default_key"
        );

        // Método para Saga
        Task PublicEventAsync<T>(string exchangeName, string routingKey, T eventMessage);
    }
}