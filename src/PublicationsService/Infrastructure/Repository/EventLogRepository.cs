using Dapper;
using PublicationsService.Infrastructure.Interface;
using PublicationsService.Persistence.Interface;
using SharedKernel.Interface;

namespace PublicationsService.Infrastructure.Repository
{
    public class EventLogRepository : IEventLogRepository
    {
        #region Properties
        private readonly IDbConnectionFactory _dbConnectionFactory;
        private readonly string OCC_Conection = "OCC_Connection";
        private readonly IGlobalExceptionHandler _globalExceptionHandler;
        private readonly IDapperExecutor _dapperExecutor;
        #endregion

        #region Constructor
        public EventLogRepository(
            IDbConnectionFactory dbConnectionFactory,
            IGlobalExceptionHandler globalExceptionHandler,
            IDapperExecutor dapperExecutor
            )
        {
            _dbConnectionFactory = dbConnectionFactory;
            _globalExceptionHandler = globalExceptionHandler;
            _dapperExecutor = dapperExecutor;
        }
        #endregion

        #region Methods
        public async Task SaveEventLog(string query, DynamicParameters parameters)
        {
            using (var connection = _dbConnectionFactory.GetConnection(OCC_Conection))
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
                            commandType: System.Data.CommandType.StoredProcedure
                            );
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        _globalExceptionHandler.HandleGenericException<string>(ex, "EventLogsRepository");
                    }
                }
            }
        }
        #endregion
    }
}
