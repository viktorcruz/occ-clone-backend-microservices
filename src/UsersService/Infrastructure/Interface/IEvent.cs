namespace UsersService.Infrastructure.Interface
{
    public interface IEvent
    {
        Guid IdEvent { get; }
        DateTime OcurredOn { get; }
    }
}
