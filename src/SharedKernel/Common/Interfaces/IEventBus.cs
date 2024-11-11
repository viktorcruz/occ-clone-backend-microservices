namespace SharedKernel.Common.Interfaces
{
    public interface IEventBus
    {
        void Publish<T>(string exchange, string routingKey, T @event);
        void Subscribe<T>(string exchange, string routingKey, Func<T, Task> handler);
    }

    public interface IAsyncEventBus : IEventBus
    {
        Task PublishAsyn<T>(string exchange, string routingKey, T @event);
    }
}