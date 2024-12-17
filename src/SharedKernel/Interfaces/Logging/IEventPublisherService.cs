namespace SharedKernel.Common.Interfaces.Logging
{
    public interface IEventPublisherService
    {
        // Método para CQRS
        Task PublishEventAsync(
            //string idCorrelation,
            string entityName,
            string operationType,
            bool success,
            string performedBy,
            string reason,
            object additionalData,
            string exchangeName,
            string routingKey
        );

        // Método para Saga
        Task PublicEventAsync<T>(string exchangeName, string routingKey, T eventMessage);
    }
}