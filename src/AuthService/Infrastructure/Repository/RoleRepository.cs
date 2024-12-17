using AuthService.Domain.Entities;
using AuthService.Domain.Ports.Output;
using Dapper;
using SharedKernel.Common.Interfaces.Persistence;
using SharedKernel.Common.Responses;
using SharedKernel.Interfaces.Exceptions    ;

namespace AuthService.Infrastructure.Repository
{
    public class RoleRepository : IRolePort
    {
        #region Properties
        private readonly ISqlServerConnectionFactory _sqlServerConnection;
        private readonly string OCC_Connection = "OCC_Connection";
        private readonly IApplicationExceptionHandler _applicationExceptionHandler;
        #endregion

        #region Constructor
        public RoleRepository(ISqlServerConnectionFactory sqlServerConnection, IApplicationExceptionHandler applicationExceptionHandler)
        {
            _sqlServerConnection = sqlServerConnection;
            _applicationExceptionHandler = applicationExceptionHandler;
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
                    _applicationExceptionHandler.CaptureException<string>(ex, ApplicationLayer.Repository, ActionType.Query);
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
