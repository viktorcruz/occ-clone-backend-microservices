using Dapper;

namespace SharedKernel.Common.Interfaces.Logging
{
    public interface IEventLogStorage
    {
        Task SaveEventLog(string query, DynamicParameters parameters);
    }
}
