using Dapper;

namespace SharedKernel.Common.Interfaces
{
    public interface IEventLogRepository
    {
        Task SaveEventLog(string query, DynamicParameters parameters);
    }
}
