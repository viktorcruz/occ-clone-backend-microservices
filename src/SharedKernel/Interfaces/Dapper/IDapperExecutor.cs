using System.Data;

namespace SharedKernel.Interfaces.Dapper
{
    public interface IDapperExecutor
    {
        Task<int> ExecuteAsync(IDbConnection connection, string sql, object param = null, IDbTransaction transaction = null, CommandType? commandType = null);
    }
}
