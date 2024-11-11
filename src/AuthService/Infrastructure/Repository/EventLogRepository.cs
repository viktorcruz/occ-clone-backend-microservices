using AuthService.Domain.Ports.Output;
using Dapper;
using SharedKernel.Common.Interfaces;
using SharedKernel.Interface;
using System.Data;

namespace AuthService.Infrastructure.Repository
{
    public class EventLogRepository : IEventLogPort
    {
        private readonly ISqlServerConnectionFactory _sqlServerConnection;
        private readonly string OCC_Connection = "OCC_Connection";
        private readonly IGlobalExceptionHandler _globalExceptionHandler;
        private readonly IDapperExecutor _dapperExecutor;

        public EventLogRepository(
            ISqlServerConnectionFactory sqlServerConnection,
            IGlobalExceptionHandler globalExceptionHandler,
            IDapperExecutor dapperExecutor
        )
        {
            _sqlServerConnection = sqlServerConnection;
            _globalExceptionHandler = globalExceptionHandler;
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
                        _globalExceptionHandler.HandleGenericException<string>(
                            ex,
                            "EventLogsRepository"
                        );
                    }
                }
            }
        }
    }
}