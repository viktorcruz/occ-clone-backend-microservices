using Dapper;
using SharedKernel.Common.Responses;
using SharedKernel.Interface;
using System.Data;
using UsersService.Application.Dto;
using UsersService.Infrastructure.Interface;
using UsersService.Persistence.Interface;

namespace UsersService.Infrastructure.Repository
{
    public class UserRepository : IUserRepository
    {
        #region Properties
        private readonly string OCC_Connection = "OCC_Connection";
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly IGlobalExceptionHandler _globalExceptionHandler;
        #endregion

        #region Constructor
        public UserRepository(IDbConnectionFactory connectionFactory, IGlobalExceptionHandler globalExceptionHandler)
        {
            _connectionFactory = connectionFactory;
            _globalExceptionHandler = globalExceptionHandler;
        }
        #endregion

        #region Methods
        public async Task<DatabaseResult> CreateUserAsync(AddUserDTO userDTO)
        {
            using (var connection = _connectionFactory.GetConnection(OCC_Connection))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var query = "Usp_Users_Add";
                        var parameters = new DynamicParameters();
                        parameters.Add("@IdRole", userDTO.IdRole);
                        parameters.Add("@FirstName", userDTO.FirstName);
                        parameters.Add("@LastName", userDTO.LastName);
                        parameters.Add("@Email", userDTO.Email);

                        var results = await connection.QuerySingleAsync<DatabaseResult>(
                                query,
                                parameters,
                                transaction: transaction,
                                commandType: CommandType.StoredProcedure
                        );

                        transaction.Commit();
                        if (results != null)
                        {
                            return results;
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        _globalExceptionHandler.HandleGenericException<string>(ex, "UserRepository.CreateUserAsync");
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


        public async Task<RetrieveDatabaseResult<UserRetrieveDTO>> GetUserByIdAsync(int userId)
        {
            using (var connection = _connectionFactory.GetConnection(OCC_Connection))
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
                    _globalExceptionHandler.HandleGenericException<string>(ex, "UserRepository.GetUserByIdAsync");
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
                return new RetrieveDatabaseResult<UserRetrieveDTO>();
            }
        }

        public async Task<RetrieveDatabaseResult<List<UserRetrieveDTO>>> GetAllUsersAsync()
        {
            using (var connection = _connectionFactory.GetConnection(OCC_Connection))
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
                            ResultMessage = "Users retrieved seccessfully",
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
                            OperationType = "GET ALL",
                            AffectedRecordId = 0,
                            OperationDateTime = DateTime.Now,
                            ExceptionMessage = null
                        };
                    }
                }
                catch (Exception ex)
                {
                    _globalExceptionHandler.HandleGenericException<string>(ex, "UserRepository.GetAllUsersAsync");
                    return new RetrieveDatabaseResult<List<UserRetrieveDTO>>
                    {
                        Details = null,
                        ResultStatus = false,
                        ResultMessage = $"Error retrieving users: {ex.Message}",
                        OperationType = "GET ALL",
                        AffectedRecordId = 0,
                        OperationDateTime = DateTime.Now,
                        ExceptionMessage = ex.Message
                    };
                };
            }
        }

        public async Task<RetrieveDatabaseResult<List<UserRetrieveDTO>>> SearchUsersAsync(string firstName, string lastName, string email)
        {
            using (var connection = _connectionFactory.GetConnection(OCC_Connection))
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

                    var results = await connection.QueryAsync<UserRetrieveDTO>(sql, parameters);

                    return new RetrieveDatabaseResult<List<UserRetrieveDTO>>
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
                    _globalExceptionHandler.HandleGenericException<string>(ex, "SearchUsersAsync");
                    return new RetrieveDatabaseResult<List<UserRetrieveDTO>>
                    {
                        Details = null,
                        ResultStatus = false,
                        ResultMessage = $"Error retrieving users: {ex.Message}",
                        OperationType = "GET ALL",
                        AffectedRecordId = 0,
                        OperationDateTime = DateTime.Now,
                        ExceptionMessage = ex.Message
                    };
                }
            }
        }

        public async Task<DatabaseResult> UpdateUserAsync(UserRetrieveDTO userDTO)
        {
            using (var connection = _connectionFactory.GetConnection(OCC_Connection))
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
                        _globalExceptionHandler.HandleGenericException<string>(ex, "UserRepository.UpdateUserAync");
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
                    finally
                    {
                        if (connection.State == ConnectionState.Open)
                        {
                            connection.Close();
                        }
                    }
                }
            }
        }

        public async Task<DatabaseResult> UpdateUserProfileAsync(UserProfileDTO userProfile)
        {
            using (var connection = _connectionFactory.GetConnection(OCC_Connection))
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
                        _globalExceptionHandler.HandleGenericException<string>(ex, "UserRepository.UpdateUserProfileAsync");
                        return await Task.FromResult(new DatabaseResult
                        {
                            ResultStatus = false,
                            ResultMessage = $"Error updating profile: {ex.Message}",
                            OperationType = "UPDATE",
                            AffectedRecordId = 0,
                            OperationDateTime = DateTime.Now,
                            ExceptionMessage = ex.Message
                        }); ;
                    }
                    finally
                    {
                        if (connection.State == ConnectionState.Open)
                        {
                            connection.Close();
                        }
                    }
                }
            }
        }

        public async Task<DatabaseResult?> DeleteUserAsync(int userId)
        {
            using (var connection = _connectionFactory.GetConnection(OCC_Connection))
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
                        _globalExceptionHandler.HandleGenericException<string>(ex, "UserRepository.DeleteUserAsync");
                        return new DatabaseResult();
                    }
                }
            }
        }


        #endregion
    }
}
