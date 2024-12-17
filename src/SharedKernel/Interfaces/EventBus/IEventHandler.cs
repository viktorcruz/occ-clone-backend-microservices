namespace SharedKernel.Common.Interfaces.EventBus
{
    public interface IEventHandler<TEvent>
    {
        Task Handle(TEvent @event);
    }
}
