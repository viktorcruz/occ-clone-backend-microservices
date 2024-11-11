using Dapper;

namespace AuthService.Domain.Ports.Output
{
    public interface IEventLogPort
    {
        Task SaveEventLog(string query, DynamicParameters parameters);
    }
}
