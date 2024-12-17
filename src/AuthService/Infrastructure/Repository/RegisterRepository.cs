using AuthService.Domain.Ports.Output;
using Dapper;
using SharedKernel.Common.Interfaces.Persistence;
using SharedKernel.Common.Responses;
using SharedKernel.Events.User;
using SharedKernel.Interfaces.Exceptions;
using System.Data;

namespace AuthService.Infrastructure.Repository
{
    public class RegisterRepository : IRegisterUserPort
    {
        #region Properties
        private readonly ISqlServerConnectionFactory _sqlServerConnection;
        private readonly string OCC_Connection = "OCC_Connection";
        private readonly IApplicationExceptionHandler _applicationExceptionHandler;
        #endregion

        #region Constructor
        public RegisterRepository(ISqlServerConnectionFactory sqlServerConnection, IApplicationExceptionHandler applicationExceptionHandler)
        {
            _sqlServerConnection = sqlServerConnection;
            _applicationExceptionHandler = applicationExceptionHandler;
        }
        #endregion

        #region Methods
        public async Task<DatabaseResult> AddAsync(UserCreatedEvent createdEvent)
        {
            using (var connection = _sqlServerConnection.GetConnection(OCC_Connection))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var query = "Usp_Users_Add";
                        var parameters = new DynamicParameters();
                        parameters.Add("@IdRole", createdEvent.IdRole);
                        parameters.Add("@FirstName", createdEvent.FirstName);
                        parameters.Add("@LastName", createdEvent.LastName);
                        parameters.Add("@Email", createdEvent.Email);
                        parameters.Add("@PasswordHash", createdEvent.PasswordHash);

                        var results = await connection.QuerySingleAsync<DatabaseResult>(
                                query,
                                parameters,
                                transaction: transaction,
                                commandType: CommandType.StoredProcedure
                        );

                        transaction.Commit();
                        if (results.ResultStatus && results != null)
                        {
                            return results;
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        _applicationExceptionHandler.CaptureException<string>(ex, ApplicationLayer.Repository, ActionType.Insert);
                       
                        return new DatabaseResult
                        {
                            ResultStatus = false,
                            ResultMessage = ex.Message,
                            OperationType = "CREATE",
                            AffectedRecordId = 0,
                            OperationDateTime = DateTime.Now,
                            ExceptionMessage = ex.Message
                        };
                    }

                    return new DatabaseResult
                    {
                        ResultStatus = false,
                        ResultMessage = "",
                        OperationType = "CREATE",
                        AffectedRecordId = 0,
                        OperationDateTime = DateTime.Now,
                        ExceptionMessage = ""
                    };
                }
            }
        }
        #endregion
    }
}
