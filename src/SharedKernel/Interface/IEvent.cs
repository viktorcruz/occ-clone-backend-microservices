namespace SharedKernel.Interface
{
    public interface IEvent
    {
        Guid IdEvent { get; }
        DateTime OcurredOn { get; }
    }
}
