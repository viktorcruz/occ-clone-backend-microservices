using AuthService.Domain.Entities;
using AuthService.Domain.Ports.Output.Repositories;
using AuthService.Factories.Interface;
using Dapper;
using SharedKernel.Common.Responses;
using SharedKernel.Interface;

namespace AuthService.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        #region Properties
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly string OCC_Connection = "OCC_Connection";
        private readonly IGlobalExceptionHandler _globalExceptionHandler;
        #endregion

        #region Constructor
        public UserRepository(IDbConnectionFactory dbConnectionFactory, IGlobalExceptionHandler globalExceptionHandler)
        {
            _connectionFactory = dbConnectionFactory;
            _globalExceptionHandler = globalExceptionHandler;
        }
        #endregion

        #region Methods
        public async Task<RetrieveDatabaseResult<UserByEmailEntity>> GetByEmailAsync(string email)
        {
            using (var connection = _connectionFactory.GetConnection(OCC_Connection))
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
                            }
                        };
                        return spResult;
                    }
                }
                catch (Exception ex)
                {
                    _globalExceptionHandler.HandleGenericException<string>(ex, "UserRepository");
                }
                finally
                {
                    if (connection.State == System.Data.ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
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
            using (var connection = _connectionFactory.GetConnection(OCC_Connection))
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
                    _globalExceptionHandler.HandleGenericException<string>(ex, "UserRepository");
                }
                finally
                {
                    if (connection.State == System.Data.ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
                return new RetrieveDatabaseResult<UserByEmailEntity>
                {
                    ResultStatus = false,
                    ResultMessage = null,
                    OperationType = null,
                    AffectedRecordId = 0,
                    Details = null
                };
            }
            #endregion
        }
    }
}
