using Dapper;

namespace UsersService.Infrastructure.Interface
{
    public interface IEventLogRepository
    {
        Task SaveEventLog(string query, DynamicParameters parameters);
    }
}
