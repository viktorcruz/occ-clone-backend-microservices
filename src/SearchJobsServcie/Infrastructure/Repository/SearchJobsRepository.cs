using Dapper;
using SearchJobsService.Application.Commands;
using SearchJobsService.Application.DTO;
using SearchJobsService.Application.DTO.Commands;
using SearchJobsService.Application.DTO.Queries;
using SearchJobsService.Infrastructure.Interface;
using SharedKernel.Common.Interfaces.Persistence;
using SharedKernel.Common.Responses;
using SharedKernel.Interfaces.Exceptions;
using System.Data.SqlClient;

namespace SearchJobsService.Infrastructure.Repository
{
    public class SearchJobsRepository : ISearchJobsRepository
    {
        #region Properties
        private readonly string OCC_Connection = "OCC_Connection";
        private readonly ISqlServerConnectionFactory _sqlServerConnection;
        private readonly IApplicationExceptionHandler _applicationExceptionHandler;
        #endregion

        #region Constructor
        public SearchJobsRepository(ISqlServerConnectionFactory sqlServerConnection, IApplicationExceptionHandler applicationExceptionHandler)
        {
            _sqlServerConnection = sqlServerConnection;
            _applicationExceptionHandler = applicationExceptionHandler;
        }
        #endregion

        #region Methods
        public async Task<DatabaseResult> ApplyAsync(ApplyCommand command)
        {
            using (var connection = _sqlServerConnection.GetConnection(OCC_Connection))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var query = "Usp_JobApplications_Add";
                        var parameters = new DynamicParameters();
                        parameters.Add("@IdPublication", command.IdPublication);
                        parameters.Add("@IdApplicant", command.IdApplicant);
                        parameters.Add("@ApplicantName", command.ApplicantName);
                        parameters.Add("@ApplicantResume", command.ApplicantResume);
                        parameters.Add("@CoverLetter", command.CoverLetter);
                        parameters.Add("@ApplicationDate", command.ApplicationDate);
                        parameters.Add("@Status", command.Status);
                        parameters.Add("@StatusMessage", command.StatusMessage);

                        var results = await connection.QuerySingleAsync<DatabaseResult>(query, param: parameters, transaction: transaction, commandType: System.Data.CommandType.StoredProcedure);

                        transaction.Commit();

                        if (results != null)
                        {
                            return results;
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        _applicationExceptionHandler.CaptureException<string>(ex, ApplicationLayer.Repository, ActionType.Insert);
                        return new DatabaseResult
                        {
                            ResultStatus = false,
                            ResultMessage = ex.Message,
                            OperationType = "CREATE",
                            AffectedRecordId = 0,
                            OperationDateTime = DateTime.Now,
                            ExceptionMessage = ex.Message,
                        };
                    }

                    return new DatabaseResult
                    {
                        ResultStatus = false,
                        ResultMessage = "User not found",
                        OperationType = "CREATE",
                        AffectedRecordId = 0,
                        OperationDateTime = DateTime.Now,
                        ExceptionMessage = null,
                    };
                }
            }
        }

        public async Task<DatabaseResult> WithdrawAsync(WithdrawApplicationRequestDTO withdrawDTO)
        {
            using (var connection = _sqlServerConnection.GetConnection(OCC_Connection))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var query = "Usp_JobApplications_Withdraw";
                        var parameters = new DynamicParameters();
                        parameters.Add("@IdApplicant", withdrawDTO.IdUser);
                        parameters.Add("@IdPublication", withdrawDTO.IdPublication);

                        var results = await connection.QueryAsync<DatabaseResult>(query, parameters, transaction, commandType: System.Data.CommandType.StoredProcedure);
                        var result = results.FirstOrDefault() ?? new DatabaseResult { ResultStatus = false };

                        transaction.Commit();

                        return result;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        _applicationExceptionHandler.CaptureException<string>(ex, ApplicationLayer.Repository, ActionType.Withdraw);
                        return new DatabaseResult { ResultStatus = false };
                    }
                }
            }
        }

        public async Task<RetrieveDatabaseResult<List<JobSearchResultDTO>>> SearchAsync(string keyword)
        {
            try
            {
                using (var connection = _sqlServerConnection.GetConnection(OCC_Connection))
                {
                    connection.Open();

                    var query = "Usp_JobApplications_Search";
                    var parameters = new DynamicParameters();
                    parameters.Add("@Keyword", keyword);

                    var results = await connection.QueryAsync<JobSearchResultDTO>(query, parameters);
                    var result = results.ToList();
                    
                    if (!result.Any())
                    {
                        throw new Exception("No jobs found");
                    }
                    
                    return new RetrieveDatabaseResult<List<JobSearchResultDTO>>
                    {
                        Details = results.ToList(),
                        ResultStatus = true,
                        ResultMessage = "Jobs retrieved successfully",
                        OperationType = "SEARCH",
                        AffectedRecordId = 0,
                        OperationDateTime = DateTime.Now,
                        ExceptionMessage = "No exceptions found"
                    };
                }
            }
            catch (Exception ex)
            {
                _applicationExceptionHandler.CaptureException<string>(ex, ApplicationLayer.Repository, ActionType.FetchAll);
                return new RetrieveDatabaseResult<List<JobSearchResultDTO>>
                {
                    Details = null,
                    ResultStatus = false,
                    ResultMessage = $"Error retrieving jobs: {ex.Message}",
                    OperationType = "GET ALL",
                    AffectedRecordId = 0,
                    OperationDateTime = DateTime.Now,
                    ExceptionMessage = ex.Message
                };
            }
        }

        public async Task<RetrieveDatabaseResult<List<UserApplicationsResponseDTO>>> ApplicationsAsync(int userId)
        {
            try
            {
                using (var connection = _sqlServerConnection.GetConnection(OCC_Connection))
                {
                    connection.Open();

                    var query = "Usp_JobApplications_GetAll";
                    var parameters = new DynamicParameters();
                    parameters.Add("@IdApplicant", userId);
                    var results = await connection.QueryAsync<UserApplicationsResponseDTO>(query, parameters, commandType: System.Data.CommandType.StoredProcedure);
                    var result = results.ToList();

                    if (!result.Any())
                    {
                        throw new Exception("No applications found for the given user.");
                    }

                    if (result.Any(item => item.IdPublication == 0))
                    {
                        throw new Exception("One or more applications have invalid publication IDs.");
                    }

                    return new RetrieveDatabaseResult<List<UserApplicationsResponseDTO>>
                    {
                        Details = result,
                        ResultStatus = true,
                        ResultMessage = "Applications retrieved successfully",
                        OperationType = "GET ALL",
                        OperationDateTime = DateTime.Now,
                        AffectedRecordId = 0,
                        ExceptionMessage = "No exceptions found",
                    };
                }
            }
            catch (Exception ex)
            {
                _applicationExceptionHandler.CaptureException<string>(ex, ApplicationLayer.Repository, ActionType.FetchAll);
                return new RetrieveDatabaseResult<List<UserApplicationsResponseDTO>>
                {
                    Details = null,
                    ResultStatus = false,
                    ResultMessage = $"Error retrieving applications: {ex.Message}",
                    OperationType = "GET ALL",
                    OperationDateTime = DateTime.Now,
                    AffectedRecordId = 0,
                    ExceptionMessage = ex.Message
                };
            }
        }

        public async Task<DatabaseResult> HasApplicationAsync(int userId, int applicationId)
        {
            try
            {
                using (var connection = _sqlServerConnection.GetConnection(OCC_Connection))
                {
                    connection.Open();

                    var query = "Usp_HasApplication_Get";
                    var parameters = new DynamicParameters();
                    parameters.Add("@IdApplicant", userId);
                    parameters.Add("@IdPublication", applicationId);

                    var result = await connection.QuerySingleOrDefaultAsync<DatabaseResult>(query, parameters, commandType: System.Data.CommandType.StoredProcedure);

                    if (result == null)
                    {
                        return new DatabaseResult
                        {
                            ResultStatus = false,
                            ResultMessage = "No application found for the given user and application ID."
                        };
                    }

                    return result;
                }
            }
            catch (SqlException sqlEx)
            {
                _applicationExceptionHandler.CaptureException<string>(sqlEx, ApplicationLayer.Repository, ActionType.FetchAll);
                return new DatabaseResult
                {
                    ResultStatus = false,
                    ResultMessage = $"Database error: {sqlEx.Message}"
                };
            }
            catch (Exception ex)
            {
                _applicationExceptionHandler.CaptureException<string>(ex, ApplicationLayer.Repository, ActionType.FetchAll);
                return new DatabaseResult
                {
                    ResultStatus = false,
                    ResultMessage = $"Unexpected error: {ex.Message}"
                };
            }
        }
        #endregion
    }
}
