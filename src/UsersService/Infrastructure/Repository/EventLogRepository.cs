using System.Data;
using UsersService.Infrastructure.Interface;
using UsersService.Persistence.Interface;
using Dapper;
using SharedKernel.Interface;

namespace UsersService.Infrastructure.Repository
{
    public class EventLogRepository : IEventLogRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly string OCC_Connection = "OCC_Connection";
        private readonly IGlobalExceptionHandler _globalExceptionHandler;
        private readonly IDapperExecutor _dapperExecutor;

        public EventLogRepository(
            IDbConnectionFactory connectionFactory,
            IGlobalExceptionHandler globalExceptionHandler,
            IDapperExecutor dapperExecutor
        )
        {
            _connectionFactory = connectionFactory;
            _globalExceptionHandler = globalExceptionHandler;
            _dapperExecutor = dapperExecutor;
        }

        public async Task SaveEventLog(string query, DynamicParameters parameters)
        {
            using (var connection = _connectionFactory.GetConnection(OCC_Connection))
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
