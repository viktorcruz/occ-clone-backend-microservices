﻿using Dapper;
using System.Data;
using UsersService.Application.Dto;
using UsersService.Infrastructure.Interface;
using UsersService.Persistence.Interface;
using UsersService.SharedKernel.Common.Response;
using UsersService.SharedKernel.Interface;

namespace UsersService.Infrastructure.Repository
{
    public class UserRepository : IUserRepository
    {
        #region Properties
        private readonly string OCC_Connection = "OCC_Connection";
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly IExceptionManagement _exceptionManagement;
        #endregion

        #region Constructor
        public UserRepository(IDbConnectionFactory connectionFactory, IExceptionManagement exceptionManagement)
        {
            _connectionFactory = connectionFactory;
            _exceptionManagement = exceptionManagement;
        }
        #endregion

        #region Methods
        public async Task<SpResult?> CreateUserAsync(AddUserDTO userDTO)
        {
            using (var connection = _connectionFactory.GetConnection(OCC_Connection))
            {
                using (var transaction = connection.BeginTransaction())
                {
                    connection.Open();
                    try
                    {
                        var query = "Usp_Users_Add";
                        var parameters = new DynamicParameters();
                        parameters.Add("@IdRole", userDTO.IdRole);
                        parameters.Add("@FirstName", userDTO.FirstName);
                        parameters.Add("@LastName", userDTO.LastName);
                        parameters.Add("@Email", userDTO.Email);

                        var results = await connection.QuerySingleAsync<SpResult>(
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
                        _exceptionManagement.HandleGenericException<string>(ex, "UserRepository.CreateUserAsync");
                    }
                    return null;
                }
            }
        }


        public async Task<SpRetrieveResult<UserRetrieveDTO>> GetUserByIdAsync(int userId)
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
                        var spResult = new SpRetrieveResult<UserRetrieveDTO>
                        {
                            ResultStatus = result.ResultStatus,
                            ResultMessage = result.ResultMessage,
                            OperationType = result.OperationType,
                            AffectedRecordId = result.AffectedRecordId,
                            OperationDateTime = result.OperationDateTime,
                            ExceptionMessage = result.ExceptionMessage,
                            Data = new UserRetrieveDTO
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
                    _exceptionManagement.HandleGenericException<string>(ex, "UserRepository.GetUserByIdAsync");
                }
                return new SpRetrieveResult<UserRetrieveDTO>();
            }
        }

        public async Task<SpRetrieveResult<List<UserRetrieveDTO>>> GetAllUsersAsync()
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
                        return new SpRetrieveResult<List<UserRetrieveDTO>>
                        {
                            Data = userList,
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
                        return new SpRetrieveResult<List<UserRetrieveDTO>>
                        {
                            Data = null,
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
                    _exceptionManagement.HandleGenericException<string>(ex, "UserRepository.GetAllUsersAsync");
                    return new SpRetrieveResult<List<UserRetrieveDTO>>
                    {
                        Data = null,
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

        public Task<SpResult> UpdateUserAsync(UserRetrieveDTO userDTO)
        {
            using (var connection = _connectionFactory.GetConnection(OCC_Connection))
            {
                using (var transaction = connection.BeginTransaction())
                {
                    connection.Open(); 
                    try
                    {
                        var query = "Ups_Users_Update";
                        var parameters = new DynamicParameters();
                        parameters.Add("@IdUser", userDTO.IdUser);
                        parameters.Add("@IdRole", userDTO.IdRole);
                        parameters.Add("@FirtsName", userDTO.FirstName);
                        parameters.Add("@LastName", userDTO.LastName);
                        parameters.Add("@Email", userDTO.Email);

                        var results = connection.QuerySingleAsync<SpResult>(query, parameters, transaction, commandType: CommandType.StoredProcedure);

                        transaction.Commit();

                        if (results != null)
                        {
                            return results;
                        }
                        else
                        {
                            return Task.FromResult(new SpResult
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
                        _exceptionManagement.HandleGenericException<string>(ex, "UserRepository.UpdateUserAync");
                        return Task.FromResult(new SpResult
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

        public async Task<SpResult?> DeleteUserAsync(int userId)
        {
            using (var connection = _connectionFactory.GetConnection(OCC_Connection))
            {
                using (var transaction = connection.BeginTransaction())
                {
                    connection.Open();
                    try
                    {
                        var query = "Usp_Users_Delete";
                        var parameters = new DynamicParameters();
                        parameters.Add("idUser", userId);

                        var result = await connection.QuerySingleAsync<SpResult>(
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
                        _exceptionManagement.HandleGenericException<string>(ex, "UserRepository.DeleteUserAsync");
                        return new SpResult();
                    }
                }
            }
        }
        #endregion
    }
}