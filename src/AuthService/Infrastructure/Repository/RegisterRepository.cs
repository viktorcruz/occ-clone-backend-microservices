using AuthService.Domain.Entities;
using AuthService.Domain.Ports.Output;
using Dapper;
using SharedKernel.Common.Interfaces;
using SharedKernel.Common.Responses;
using SharedKernel.Interface;
using System.Data;

namespace AuthService.Infrastructure.Repository
{
    public class RegisterRepository : IRegisterUserPort
    {
        #region Properties
        private readonly ISqlServerConnectionFactory _sqlServerConnection;
        private readonly string OCC_Connection = "OCC_Connection";
        private readonly IGlobalExceptionHandler _globalExceptionHandler;
        #endregion

        #region Constructor
        public RegisterRepository(ISqlServerConnectionFactory sqlServerConnection, IGlobalExceptionHandler globalExceptionHandler)
        {
            _sqlServerConnection = sqlServerConnection;
            _globalExceptionHandler = globalExceptionHandler;
        }
        #endregion

        #region Methods
        public async Task<DatabaseResult> AddAsync(RegisterEntity registerEntity)
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
                        parameters.Add("@IdRole", registerEntity.IdRole);
                        parameters.Add("@FirstName", registerEntity.FirstName);
                        parameters.Add("@LastName", registerEntity.LastName);
                        parameters.Add("@Email", registerEntity.Email);
                        parameters.Add("@PasswordHash", registerEntity.PasswordHash);

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
                        _globalExceptionHandler.HandleGenericException<string>(ex, "RegisterRepository.AddAsync");
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
                    finally
                    {
                        if (connection.State == System.Data.ConnectionState.Open)
                        {
                            connection.Close();
                        }
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
