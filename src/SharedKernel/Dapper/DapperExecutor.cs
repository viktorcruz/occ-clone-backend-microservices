using Dapper;
using SharedKernel.Interfaces.Dapper;
using System.Data;

namespace SharedKernel.Dapper
{
    public class DapperExecutor : IDapperExecutor
    {
        public Task<int> ExecuteAsync(IDbConnection connection, string sql, object param = null, IDbTransaction transaction = null, CommandType? commandType = null)
        {
            return connection.ExecuteAsync(sql, param, transaction, commandType: commandType);
        }
    }
}
