namespace SharedKernel.Interface
{
    public interface IEventBus
    {
        void Publish<T>(string exchange, string routingKey, T @event);
        void Subscribe<T>(string exchange, string routingKey, Func<T, Task> handler);
    }
}
