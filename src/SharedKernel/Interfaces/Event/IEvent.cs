namespace SharedKernel.Interfaces.Event
{
    public interface IEvent
    {
        Guid IdEvent { get; }
        DateTime OcurredOn { get; }
        Guid IdCorrelation { get; }
    }
}
