using Dapper;
using RolesServices.Aplication.Dto;
using RolesServices.Infrastructure.Interface;
using RolesServices.Persistence.Interface;
using SharedKernel.Common.Responses;
using SharedKernel.Interface;

namespace RolesServices.Infrastructure.Repository
{
    public class RoleRepository : IRoleRespository
    {
        #region Properties
        private readonly string OCC_Connection = "OCC_Connection";
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly IGlobalExceptionHandler _globalExceptionHandler;
        #endregion

        #region Constructor
        public RoleRepository(IDbConnectionFactory connectionFactory, IGlobalExceptionHandler globalExceptionHandler)
        {
            _connectionFactory = connectionFactory;
            _globalExceptionHandler = globalExceptionHandler;
        }
        #endregion

        #region Methods
        public async Task<DatabaseResult?> CreateRoleAsync(RoleDTO roleDTO)
        {
            using (var connection = _connectionFactory.GetConnection(OCC_Connection))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var query = "Usp_Roles_Add";
                        var parameters = new DynamicParameters();
                        parameters.Add("@RoleName", roleDTO.RoleName);

                        var results = await connection.QuerySingleAsync<DatabaseResult>(
                            query,
                            parameters,
                            transaction: transaction,
                            commandType: System.Data.CommandType.StoredProcedure
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
                        _globalExceptionHandler.HandleGenericException<string>(ex, "RoleRepository.CreateRoleAsync");
                    }
                    return null;
                }
            }
        }

        public async Task<RetrieveDatabaseResult<RoleDTO>> GetRoleByIdAsync(int roleId)
        {
            using (var connection = _connectionFactory.GetConnection(OCC_Connection))
            {
                connection.Open();
                try
                {
                    var query = "Usp_Roles_Get";
                    var parameters = new DynamicParameters();
                    parameters.Add("@idRole", roleId);

                    var results = await connection.QueryAsync<dynamic>(
                        query,
                        parameters,
                        commandType: System.Data.CommandType.StoredProcedure
                        );

                    var result = results.FirstOrDefault();

                    if (result != null)
                    {
                        var spResult = new RetrieveDatabaseResult<RoleDTO>
                        {
                            ResultStatus = result.ResultStatus,
                            ResultMessage = result.ResultMessage,
                            OperationType = result.OperationType,
                            AffectedRecordId = result.AffectedRecordId,
                            ExceptionMessage = result.ExceptionMessage,
                            Details = new RoleDTO
                            {
                                IdRole = result.IdRole,
                                RoleName = result.RoleName,
                            }
                        };
                        return spResult;
                    }
                }
                catch (Exception ex)
                {
                    _globalExceptionHandler.HandleGenericException<string>(ex, "RoleRepository.GetRoleByIdAsync");
                }
                return new RetrieveDatabaseResult<RoleDTO>();
            }
        }

        public async Task<RetrieveDatabaseResult<List<RoleDTO>>> GetAllRolesAsync()
        {
            try
            {
                using (var connection = _connectionFactory.GetConnection(OCC_Connection))
                {
                    connection.Open();

                    var query = "Usp_Roles_GetAll";

                    var results = await connection.QueryAsync<RoleDTO>(query, commandType: System.Data.CommandType.StoredProcedure);

                    var roleList = results.ToList();

                    if (roleList.Any())
                    {
                        return new RetrieveDatabaseResult<List<RoleDTO>>
                        {
                            Details = roleList,
                            ResultStatus = true,
                            ResultMessage = "Roles retrieved successfully",
                            AffectedRecordId = 0,
                            OperationType = "GET ALL",
                            OperationDateTime = DateTime.Now,
                            ExceptionMessage = "No exception found"
                        };
                    }
                    else
                    {
                        return new RetrieveDatabaseResult<List<RoleDTO>>
                        {
                            Details = new List<RoleDTO>(),
                            ResultStatus = false,
                            ResultMessage = "No roles found",
                            OperationType = "GET ALL",
                            AffectedRecordId = 0,
                            OperationDateTime = DateTime.Now,
                            ExceptionMessage = null
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                _globalExceptionHandler.HandleGenericException<string>(ex, "RoleRepository.GetAllRolesAsync");
                return new RetrieveDatabaseResult<List<RoleDTO>>
                {
                    Details = null,
                    ResultStatus = false,
                    ResultMessage = $"Error retrieving roles: {ex.Message}",
                    OperationType = "GET ALL",
                    AffectedRecordId = 0,
                    OperationDateTime = DateTime.Now,
                    ExceptionMessage = ex.Message
                };
            }
        }


        public async Task<DatabaseResult> UpdateRoleAsync(RoleDTO roleDTO)
        {
            using (var connection = _connectionFactory.GetConnection(OCC_Connection))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var query = "Ups_Roles_Update";
                        var parameters = new DynamicParameters();
                        parameters.Add("@idRole", roleDTO.IdRole);
                        parameters.Add("@RoleName", roleDTO.RoleName);

                        var results = await connection.QuerySingleAsync<DatabaseResult>(query, parameters, transaction: transaction, commandType: System.Data.CommandType.StoredProcedure);

                        transaction.Commit();

                        if (results != null)
                        {
                            return results;
                        }
                        else
                        {
                            return new DatabaseResult
                            {
                                ResultStatus = false,
                                ResultMessage = "Role not found",
                                AffectedRecordId = 0,
                                OperationType = "UPDATE",
                                OperationDateTime = DateTime.Now,
                                ExceptionMessage = "No exception found"
                            };
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        _globalExceptionHandler.HandleGenericException<string>(ex, "RoleRepository.UpdateRoleAsync");
                        return new DatabaseResult
                        {
                            ResultStatus = false,
                            ResultMessage = $"Error updating role: {ex.Message}",
                            AffectedRecordId = 0,
                            OperationType = "UPDATE",
                            OperationDateTime = DateTime.Now,
                            ExceptionMessage = ex.Message
                        };
                    }
                }
            }
        }

        public async Task<DatabaseResult> DeleteRoleAsync(int roleId)
        {
            using (var connection = _connectionFactory.GetConnection(OCC_Connection))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var query = "Usp_Roles_Delete";
                        var parameters = new DynamicParameters();
                        parameters.Add("idRole", roleId);

                        var results = await connection.QuerySingleAsync<DatabaseResult>(query, parameters, transaction: transaction, commandType: System.Data.CommandType.StoredProcedure);

                        transaction.Commit();

                        if (results.ResultStatus)
                        {
                            return results;
                        }
                        return null;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        _globalExceptionHandler.HandleGenericException<string>(ex, "RoleRepository.DeleteRoleAsync");
                        return new DatabaseResult();
                    }
                }
            }
        }
        #endregion
    }
}
