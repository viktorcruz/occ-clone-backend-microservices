using AuthService.Domain.Entities;
using AuthService.Domain.Ports.Output;
using Dapper;
using SharedKernel.Common.Interfaces;
using SharedKernel.Common.Responses;
using SharedKernel.Interface;

namespace AuthService.Infrastructure.Repository
{
    public class RoleRepository : IRolePort
    {
        #region Properties
        private readonly ISqlServerConnectionFactory _sqlServerConnection;
        private readonly string OCC_Connection = "OCC_Connection";
        private readonly IGlobalExceptionHandler _globalExceptionHandler;
        #endregion

        #region Constructor
        public RoleRepository(ISqlServerConnectionFactory sqlServerConnection, IGlobalExceptionHandler globalExceptionHandler)
        {
            _sqlServerConnection = sqlServerConnection;
            _globalExceptionHandler = globalExceptionHandler;
        }
        #endregion

        #region Methods
        public async Task<RetrieveDatabaseResult<RoleEntity>> GetRoleByIdAsync(int roleId)
        {
            using (var connection = _sqlServerConnection.GetConnection(OCC_Connection))
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
