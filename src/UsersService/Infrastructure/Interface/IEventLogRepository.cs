namespace UsersService.Infrastructure.Interface
{
    public interface IEventLogRepository
    {
        Task SaveEventLog(string eventName, string eventData, string exchange, string routingKey);
    }
}
