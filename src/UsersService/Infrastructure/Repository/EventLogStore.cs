using System.Data;
using Dapper;
using SharedKernel.Common.Interfaces.Logging;
using SharedKernel.Common.Interfaces.Persistence;
using SharedKernel.Interfaces.Exceptions;
using SharedKernel.Interfaces.Dapper;

namespace UsersService.Infrastructure.Repository
{
    public class EventLogStore : IEventLogStorage
    {
        private readonly ISqlServerConnectionFactory _sqlServerConnection;
        private readonly string OCC_Connection = "OCC_Connection";
        private readonly IApplicationExceptionHandler _applicationExceptionHandler;
        private readonly IDapperExecutor _dapperExecutor;

        public EventLogStore(
            ISqlServerConnectionFactory sqlServerConnection,
            IApplicationExceptionHandler applicationExceptionHandler,
            IDapperExecutor dapperExecutor
        )
        {
            _sqlServerConnection = sqlServerConnection;
            _applicationExceptionHandler = applicationExceptionHandler;
            _dapperExecutor = dapperExecutor;
        }

        public async Task SaveEventLog(string query, DynamicParameters parameters)
        {
            using (var connection = _sqlServerConnection.GetConnection(OCC_Connection))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var results = await _dapperExecutor.ExecuteAsync(
                            connection,
                            query,
                            parameters,
                            transaction: transaction,
                            commandType: CommandType.StoredProcedure
                        );
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        _applicationExceptionHandler.CaptureException<string>(ex, ApplicationLayer.Repository, ActionType.Execute);
                    }
                }
            }
        }
    }
}
