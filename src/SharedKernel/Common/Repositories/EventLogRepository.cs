using Dapper;
using SharedKernel.Common.Interfaces;
using SharedKernel.Interface;
using System.Data;

namespace SharedKernel.Common.Repositories
{
    public class EventLogRepository : IEventLogRepository
    {
        private readonly ISqlServerConnectionFactory _sqlServerConnectionFactory;
        private readonly string OCC_Connection = "OCC_Connection";
        private readonly IApplicationExceptionHandler _applicationExceptionHandler;
        private readonly IDapperExecutor _dapperExecutor;

        public EventLogRepository(
            ISqlServerConnectionFactory sqlServerConnectionFactory,
            IApplicationExceptionHandler applicationExceptionHandler,
            IDapperExecutor dapperExecutor
        )
        {
            _sqlServerConnectionFactory = sqlServerConnectionFactory;
            _applicationExceptionHandler = applicationExceptionHandler;
            _dapperExecutor = dapperExecutor;
        }

        public async Task SaveEventLog(string query, DynamicParameters parameters)
        {
            using (var connection = _sqlServerConnectionFactory.GetConnection(OCC_Connection))
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
