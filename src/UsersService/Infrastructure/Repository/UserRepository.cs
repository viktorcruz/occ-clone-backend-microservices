using Dapper;
using SharedKernel.Common.Interfaces.Persistence;
using SharedKernel.Common.Responses;
using SharedKernel.Interfaces.Exceptions;
using System.Data;
using UsersService.Application.DTO;
using UsersService.Domain.Entity;
using UsersService.Infrastructure.Interface;

namespace UsersService.Infrastructure.Repository
{
    public class UserRepository : IUserRepository
    {
        #region Properties
        private readonly string OCC_Connection = "OCC_Connection";
        private readonly ISqlServerConnectionFactory _sqlServerConnection;
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
        public async Task<RetrieveDatabaseResult<UserRetrieveDTO>> GetUserByIdAsync(int userId)
        {
            using (var connection = _sqlServerConnection.GetConnection(OCC_Connection))
            {
                connection.Open();
                try
                {
                    var query = "Usp_Users_Get";
                    var parameters = new DynamicParameters();
                    parameters.Add("idUser", userId);

                    var results = await connection.QueryAsync<dynamic>(
                        query,
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );

                    var result = results.FirstOrDefault();

                    if (result != null)
                    {
                        var spResult = new RetrieveDatabaseResult<UserRetrieveDTO>
                        {
                            ResultStatus = result.ResultStatus,
                            ResultMessage = result.ResultMessage,
                            OperationType = result.OperationType,
                            AffectedRecordId = result.AffectedRecordId,
                            OperationDateTime = result.OperationDateTime,
                            ExceptionMessage = result.ExceptionMessage,
                            Details = new UserRetrieveDTO
                            {
                                IdUser = result.IdUser,
                                IdRole = result.IdRole,
                                FirstName = result.FirstName,
                                LastName = result.LastName,
                                Email = result.Email,
                            }
                        };

                        return spResult;
                    }
                }
                catch (Exception ex)
                {
                    _applicationExceptionHandler.CaptureException<string>(ex, ApplicationLayer.Repository, ActionType.Get);
                }

                return new RetrieveDatabaseResult<UserRetrieveDTO>();
            }
        }

        public async Task<RetrieveDatabaseResult<List<UserRetrieveDTO>>> GetAllUsersAsync()
        {
            using (var connection = _sqlServerConnection.GetConnection(OCC_Connection))
            {
                connection.Open();
                try
                {
                    var query = "Usp_Users_GetAll";

                    var results = await connection.QueryAsync<UserRetrieveDTO>(query, commandType: CommandType.StoredProcedure);

                    var userList = results.ToList();

                    if (userList.Any())
                    {
                        return new RetrieveDatabaseResult<List<UserRetrieveDTO>>
                        {
                            Details = userList,
                            ResultStatus = true,
                            ResultMessage = "Users retrieved successfully",
                            OperationType = "GET ALL",
                            AffectedRecordId = 0,
                            OperationDateTime = DateTime.Now,
                            ExceptionMessage = "No exceptions found"
                        };
                    }
                    else
                    {
                        return new RetrieveDatabaseResult<List<UserRetrieveDTO>>
                        {
                            Details = null,
                            ResultStatus = false,
                            ResultMessage = "No users found",
                            OperationType = "GETALL",
                            AffectedRecordId = 0,
                            OperationDateTime = DateTime.Now,
                            ExceptionMessage = null
                        };
                    }
                }
                catch (Exception ex)
                {
                    _applicationExceptionHandler.CaptureException<string>(ex, ApplicationLayer.Repository, ActionType.FetchAll);
                    return new RetrieveDatabaseResult<List<UserRetrieveDTO>>
                    {
                        Details = null,
                        ResultStatus = false,
                        ResultMessage = $"Error retrieving users: {ex.Message}",
                        OperationType = "GETALL",
                        AffectedRecordId = 0,
                        OperationDateTime = DateTime.Now,
                        ExceptionMessage = ex.Message
                    };
                };
            }
        }

        public async Task<RetrieveDatabaseResult<List<SearchUsersDTO>>> SearchUsersAsync(string firstName, string lastName, string email)
        {
            using (var connection = _sqlServerConnection.GetConnection(OCC_Connection))
            {
                connection.Open();

                try
                {
                    var sql = "SELECT FirstName, LastName, Email, IsActive, IsRegistrationConfirmed FROM Users WHERE 1=1";
                    var parameters = new DynamicParameters();

                    if (!string.IsNullOrEmpty(firstName))
                    {
                        sql += " AND FirstName = @FirstName";
                        parameters.Add("@FirstName", firstName);
                    }

                    if (!string.IsNullOrEmpty(lastName))
                    {
                        sql += " AND LastName = @LastName";
                        parameters.Add("@LastName", lastName);
                    }

                    if (!string.IsNullOrEmpty(email))
                    {
                        sql += " AND Email = @Email";
                        parameters.Add("@Email", email);
                    }

                    var results = await connection.QueryAsync<SearchUsersDTO>(sql, parameters);

                    return new RetrieveDatabaseResult<List<SearchUsersDTO>>
                    {
                        Details = results.ToList(),
                        ResultStatus = true,
                        ResultMessage = "Users retrieved successfully",
                        OperationType = "SEARCH",
                        AffectedRecordId = 0,
                        OperationDateTime = DateTime.Now,
                        ExceptionMessage = ""
                    };
                }
                catch (Exception ex)
                {
                    _applicationExceptionHandler.CaptureException<string>(ex, ApplicationLayer.Repository, ActionType.FetchAll);
                    return new RetrieveDatabaseResult<List<SearchUsersDTO>>
                    {
                        Details = null,
                        ResultStatus = false,
                        ResultMessage = $"Error retrieving users: {ex.Message}",
                        OperationType = "GETALL",
                        AffectedRecordId = 0,
                        OperationDateTime = DateTime.Now,
                        ExceptionMessage = ex.Message
                    };
                }
            }
        }

        public async Task<DatabaseResult> UpdateUserAsync(UserRetrieveDTO userDTO)
        {
            using (var connection = _sqlServerConnection.GetConnection(OCC_Connection))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var query = "Ups_Users_Update";
                        var parameters = new DynamicParameters();
                        parameters.Add("@IdUser", userDTO.IdUser);
                        parameters.Add("@IdRole", userDTO.IdRole);
                        parameters.Add("@FirstName", userDTO.FirstName);
                        parameters.Add("@LastName", userDTO.LastName);
                        parameters.Add("@Email", userDTO.Email);
                        parameters.Add("@IsRegistrationConfirmed", userDTO.IsRegistrationConfirmed);
                        parameters.Add("@RegistrationConfirmedAt", userDTO.RegistrationConfirmedAt);
                        parameters.Add("@IsActive", userDTO.IsActive);

                        var results = await connection.QuerySingleAsync<DatabaseResult>(query, parameters, transaction, commandType: CommandType.StoredProcedure);

                        transaction.Commit();

                        if (results != null)
                        {
                            return results;
                        }
                        else
                        {
                            return await Task.FromResult(new DatabaseResult
                            {
                                ResultStatus = false,
                                ResultMessage = "User not found",
                                OperationType = "UPDATE",
                                AffectedRecordId = 0,
                                OperationDateTime = DateTime.Now,
                                ExceptionMessage = "No exceptions found"
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        _applicationExceptionHandler.CaptureException<string>(ex, ApplicationLayer.Repository, ActionType.Update);
                        return await Task.FromResult(new DatabaseResult
                        {
                            ResultStatus = false,
                            ResultMessage = $"Error updating user: {ex.Message}",
                            OperationType = "UPDATE",
                            AffectedRecordId = 0,
                            OperationDateTime = DateTime.Now,
                            ExceptionMessage = ex.Message
                        });
                    }
                }
            }
        }

        public async Task<DatabaseResult> UpdateUserProfileAsync(UserProfileDTO userProfile)
        {
            using (var connection = _sqlServerConnection.GetConnection(OCC_Connection))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var query = "Ups_UsersProfile_Update";
                        var parameters = new DynamicParameters();
                        parameters.Add("@IdUser", userProfile.IdUser);
                        parameters.Add("@FirstName", userProfile.FirstName);
                        parameters.Add("@LastName", userProfile.LastName);
                        parameters.Add("@Email", userProfile.Email);

                        var results = await connection.QuerySingleAsync<DatabaseResult>(query, parameters, transaction, commandType: CommandType.StoredProcedure);

                        transaction.Commit();

                        if (results != null)
                        {
                            return results;
                        }
                        else
                        {
                            return await Task.FromResult(new DatabaseResult
                            {
                                ResultStatus = false,
                                ResultMessage = "User not found",
                                OperationType = "UPDATE",
                                AffectedRecordId = 0,
                                OperationDateTime = DateTime.Now,
                                ExceptionMessage = "No exceptions found"
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        _applicationExceptionHandler.CaptureException<string>(ex, ApplicationLayer.Repository, ActionType.Update);
                        return await Task.FromResult(new DatabaseResult
                        {
                            ResultStatus = false,
                            ResultMessage = $"Error updating profile: {ex.Message}",
                            OperationType = "UPDATE",
                            AffectedRecordId = 0,
                            OperationDateTime = DateTime.Now,
                            ExceptionMessage = ex.Message
                        });
                    }
                }
            }
        }

        public async Task<DatabaseResult> ChangeUserPasswordAsync(int userId, string password, string email)
        {
            using (var connection = _sqlServerConnection.GetConnection(OCC_Connection))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var query = "Ups_UserPassword_Update";
                        var parameters = new DynamicParameters();
                        parameters.Add("@IdUser", userId);
                        parameters.Add("@Email", email);
                        parameters.Add("@PasswordHash", password);

                        var results = await connection.QuerySingleAsync<DatabaseResult>(query, parameters, transaction, commandType: CommandType.StoredProcedure);

                        transaction.Commit();

                        if (results != null || Convert.ToBoolean(results?.ResultStatus))
                        {
                            return results;
                        }
                        else
                        {
                            return await Task.FromResult(
                                new DatabaseResult
                                {
                                    ResultStatus = false,
                                    ResultMessage = "User not found",
                                    AffectedRecordId = 0,
                                    OperationDateTime = DateTime.Now,
                                    ExceptionMessage = "No exceptions found"
                                });
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
                            OperationType = "UPDATE",
                            AffectedRecordId = 0,
                            OperationDateTime = DateTime.Now,
                            ExceptionMessage = ex.Message
                        });
                    }
                }
            }
        }

        public async Task<DatabaseResult?> DeleteUserAsync(int userId)
        {
            using (var connection = _sqlServerConnection.GetConnection(OCC_Connection))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var query = "Usp_Users_Delete";
                        var parameters = new DynamicParameters();
                        parameters.Add("idUser", userId);

                        var result = await connection.QuerySingleAsync<DatabaseResult>(
                            query,
                            parameters,
                            transaction,
                            commandType: CommandType.StoredProcedure
                        );

                        transaction.Commit();

                        if (result.ResultStatus)
                        {
                            return result;
                        }
                        return null;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        _applicationExceptionHandler.CaptureException<string>(ex, ApplicationLayer.Repository, ActionType.Delete);
                        return new DatabaseResult();
                    }
                }
            }
        }

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

                    var results = await connection.QueryAsync<dynamic>(query, parameters, commandType: CommandType.StoredProcedure);
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
                    _applicationExceptionHandler.CaptureException<string>(ex, ApplicationLayer.Repository, ActionType.Get);
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


        #endregion
    }
}
