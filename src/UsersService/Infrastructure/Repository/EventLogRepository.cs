using System.Data;
using UsersService.Infrastructure.Interface;
using UsersService.Persistence.Interface;
using Dapper;
using SharedKernel.Interface;
using System.Diagnostics;

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
            IDapperExecutor dapperExecutor)
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
                        _globalExceptionHandler.HandleGenericException<string>(ex, "EventLogsRepository");
                    }
                }
            }
        }

        //public async Task AddEventLog(
        //    string eventName,
        //    string eventData,
        //    string exchange,
        //    string routingKey)
        //{
        //    var query = "Usp_EventLog_Add";
        //    var parameters = new DynamicParameters();
        //    parameters.Add("@EventName", eventName);
        //    parameters.Add("@EventData", eventData);
        //    parameters.Add("@Exchange", exchange);
        //    parameters.Add("@RoutingKey", routingKey);

        //    await SaveEventLog(query, parameters);
        //}

        //public async Task UpdateEventLog(
        //    int eventId,
        //    string eventName,
        //    string eventData,
        //    string routingKey
        //    )
        //{
        //    var query = "Usp_EventLog_Add";
        //    var parameters = new DynamicParameters();
        //    parameters.Add("@EventId", eventId);
        //    parameters.Add("@EventName", eventName);
        //    parameters.Add("@EventData", eventData);
        //    parameters.Add("@RoutingKey", routingKey);
        //    await SaveEventLog(query, parameters);
        //}

        //public async Task<EventLog> GetEventLogById(int eventId)
        //{
        //    using (var connection = _connectionFactory.GetConnection(OCC_Connection))
        //    {
        //        var query = "Usp_EventLoG_Add";
        //        var parameters = new DynamicParameters();
        //        parameters.Add("@EventId", eventId);
        //        connection.Open();
        //        return await connection.
        //            QueryFirstOrDefaultAsync<EventLog>
        //            (query,
        //             param: parameters,
        //             commandType: CommandType.StoredProcedure);

        //    }
        //}

        //public async Task<IEnumerable<EventLog>> GetAllEventLogs()
        //{
        //    using (var connection = _connectionFactory.GetConnection(OCC_Connection))
        //    {
        //        var query = "";
        //        connection.Open();
        //        return await connection.QueryAsync<EventLog>(
        //            query,
        //            commandType: CommandType.StoredProcedure
        //            );
        //    }
        //}

        //public async Task UpdatedEventLogFailed(int eventId, string errorMessage)
        //{
        //    var parameters = new DynamicParameters();
        //    parameters.Add("@EventId", eventId);
        //    parameters.Add("@ErrorMessage", errorMessage);

        //    await SaveEventLog("", parameters);
        //}
    }
}
