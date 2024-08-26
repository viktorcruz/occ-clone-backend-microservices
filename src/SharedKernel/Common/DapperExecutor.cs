using Dapper;
using SharedKernel.Interface;
using System.Data;

namespace SharedKernel.Common
{
    public class DapperExecutor : IDapperExecutor
    {
        public Task<int> ExecuteAsync(IDbConnection connection, string sql, object param = null, IDbTransaction transaction = null, CommandType? commandType = null)
        {
            return connection.ExecuteAsync(sql, param, transaction, commandType: commandType);
        }
    }
}
