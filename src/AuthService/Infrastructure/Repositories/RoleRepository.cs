using AuthService.Domain.Entities;
using AuthService.Domain.Ports.Output.Repositories;
using AuthService.Factories.Interface;
using Dapper;
using SharedKernel.Common.Responses;
using SharedKernel.Interface;

namespace AuthService.Infrastructure.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        #region Properties
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly string OCC_Connection = "OCC_Connection";
        private readonly IGlobalExceptionHandler _globalExceptionHandler;
        #endregion

        #region Constructor
        public RoleRepository(IDbConnectionFactory dbConnectionFactory, IGlobalExceptionHandler globalExceptionHandler)
        {
            _connectionFactory = dbConnectionFactory;
            _globalExceptionHandler = globalExceptionHandler;
        }
        #endregion

        #region Methods
        public async Task<RetrieveDatabaseResult<RoleEntity>> GetRoleByIdAsync(int roleId)
        {
            using (var connection = _connectionFactory.GetConnection(OCC_Connection))
            {
                connection.Open();
                try
                {
                    var query = "Usp_Roles_Get";
                    var parameters = new DynamicParameters();

                    parameters.Add("@IdRole", roleId);

                    var result = await connection.QueryAsync<dynamic>(query, parameters, commandType: System.Data.CommandType.StoredProcedure);

                    var response = result.FirstOrDefault();

                    if (response.ResultStatus)
                    {
                        var spResult = new RetrieveDatabaseResult<RoleEntity>
                        {
                            ResultStatus = response.ResultStatus,
                            ResultMessage = response.ResultMessage,
                            OperationType = response.OperationType,
                            AffectedRecordId = response.AffectedRecordId,
                            Details = new RoleEntity
                            {
                                IdRole = response.IdRole,
                                RoleName = response.RoleName,
                            }
                        };
                        return spResult;
                    }
                }
                catch (Exception ex)
                {
                    _globalExceptionHandler.HandleGenericException<string>(ex, "RoleRepository");
                }
                finally
                {
                    if (connection.State == System.Data.ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
                return new RetrieveDatabaseResult<RoleEntity>
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
