using AuthService.Domain.Entities;
using AuthService.Domain.Ports.Output;
using Dapper;
using SharedKernel.Common.Interfaces;
using SharedKernel.Common.Responses;
using SharedKernel.Interface;
using System.Data;

namespace AuthService.Infrastructure.Repository
{
    public class UserRepository : IUserPort
    {
        #region Properties
        private readonly ISqlServerConnectionFactory _sqlServerConnection;
        private readonly string OCC_Connection = "OCC_Connection";
        private readonly IApplicationExceptionHandler _applicationExceptionHandler;
        #endregion

        #region Constructor
        public UserRepository(ISqlServerConnectionFactory sqlServerConnection, IApplicationExceptionHandler applicationExceptionHandler)
        {
            _sqlServerConnection = sqlServerConnection;
            _applicationExceptionHandler = applicationExceptionHandler;
        }
        #endregion

        #region Methods
        public async Task<RetrieveDatabaseResult<UserByEmailEntity>> GetByEmailAsync(string email)
        {
            using (var connection = _sqlServerConnection.GetConnection(OCC_Connection))
            {
                connection.Open();
                try
                {
                    var query = "Usp_UsersByCredentials_Get";
                    var parameters = new DynamicParameters();
                    parameters.Add("@Email", email);

                    var results = await connection.QueryAsync<dynamic>(query, parameters, commandType: System.Data.CommandType.StoredProcedure);

                    var response = results.FirstOrDefault();

                    if (response.ResultStatus)
                    {
                        var spResult = new RetrieveDatabaseResult<UserByEmailEntity>
                        {
                            ResultStatus = response.ResultStatus,
                            ResultMessage = response.ResultMessage,
                            OperationType = response.OperationType,
                            AffectedRecordId = response.AffectedRecordId,
                            Details = new UserByEmailEntity
                            {
                                IdUser = response.IdUser,
                                IdRole = response.IdRole,
                                Email = response.Email,
                                PasswordHash = response.PasswordHash,
                                IsActive = response.IsActive,
                                IsRegistrationConfirmed = response.IsRegistrationConfirmed,
                            }
                        };
                        return spResult;
                    }
                }
                catch (Exception ex)
                {
                    _applicationExceptionHandler.CaptureException<string>(ex, ApplicationLayer.Repository, ActionType.Query);
                }
                //finally
                //{
                //    if (connection.State == System.Data.ConnectionState.Open)
                //    {
                //        connection.Close();
                //    }
                //}
                return new RetrieveDatabaseResult<UserByEmailEntity>
                {
                    ResultStatus = false,
                    ResultMessage = null,
                    OperationType = null,
                    AffectedRecordId = 0,
                    Details = null
                };
            }
        }
        public async Task<RetrieveDatabaseResult<UserByEmailEntity>> GetUserByCredentialsAsync(string email)
        {
            using (var connection = _sqlServerConnection.GetConnection(OCC_Connection))
            {
                connection.Open();
                try
                {
                    var query = "Usp_UsersByCredentials_Get";
                    var parameters = new DynamicParameters();

                    parameters.Add("@Email", email);

                    var result = await connection.QueryAsync<dynamic>(query, parameters, commandType: System.Data.CommandType.StoredProcedure);

                    var response = result.FirstOrDefault();

                    if (response.ResultStatus)
                    {
                        var spResult = new RetrieveDatabaseResult<UserByEmailEntity>
                        {
                            ResultStatus = response.ResultStatus,
                            ResultMessage = response.ResultMessage,
                            OperationType = response.OperationType,
                            AffectedRecordId = response.AffectedRecordId,
                            Details = new UserByEmailEntity
                            {
                                IdUser = response.IdUser,
                                IdRole = response.IdRole,
                                Email = response.Email,
                                PasswordHash = response.PasswordHash,
                            }
                        };
                        return spResult;
                    }
                }
                catch (Exception ex)
                {
                    _applicationExceptionHandler.CaptureException<string>(ex, ApplicationLayer.Repository, ActionType.Query);
                }
                //finally
                //{
                //    if (connection.State == System.Data.ConnectionState.Open)
                //    {
                //        connection.Close();
                //    }
                //}
                return new RetrieveDatabaseResult<UserByEmailEntity>
                {
                    ResultStatus = false,
                    ResultMessage = null,
                    OperationType = null,
                    AffectedRecordId = 0,
                    Details = null
                };
            }
        }

        public async Task<DatabaseResult> ChangeUserStatusAsync(int userId, string email)
        {
            using (var connection = _sqlServerConnection.GetConnection(OCC_Connection))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var query = "Usp_Users_ConfirmRegister";
                        var parameters = new DynamicParameters();
                        parameters.Add("@IdUser", userId);
                        parameters.Add("@Email", email);

                        var results = await connection.QuerySingleAsync<DatabaseResult>(query, parameters, transaction, commandType: CommandType.StoredProcedure);

                        transaction.Commit();

                        if (results != null)
                        {
                            return results;
                        }
                        else
                        {
                            return await Task.FromResult<DatabaseResult>(new DatabaseResult
                            {
                                ResultStatus = false,
                                ResultMessage = "User not found",
                                OperationType = "Confirm register",
                                AffectedRecordId = 0,
                                OperationDateTime = DateTime.Now,
                                ExceptionMessage = "No exceptions found"
                            }); ;
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        _applicationExceptionHandler.CaptureException<string>(ex, ApplicationLayer.Repository, ActionType.Update);
                        return await Task.FromResult(new DatabaseResult
                        {
                            ResultStatus = false,
                            ResultMessage = ex.Message,
                            OperationType = "Confirm register",
                            AffectedRecordId = userId,
                            OperationDateTime = DateTime.Now,
                            ExceptionMessage = ex.Message
                        });
                    }
                }
            }
        }
        #endregion
    }
}
