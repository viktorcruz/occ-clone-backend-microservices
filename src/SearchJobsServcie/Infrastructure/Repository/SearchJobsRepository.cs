using Dapper;
using SearchJobsService.Application.Commands;
using SearchJobsService.Application.Dto;
using SearchJobsService.Infrastructure.Interface;
using SharedKernel.Common.Interfaces;
using SharedKernel.Common.Responses;
using SharedKernel.Interface;

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
                        parameters.Add("@Status", command.GetRecruitmentStatus(Domain.Enum.RecruitmentStatus.Applied));

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
                    finally
                    {
                        if (connection.State == System.Data.ConnectionState.Open)
                        {
                            connection.Close();
                        }
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

                        //if (result.ResultStatus)
                        //{
                        return result;
                        //}
                        //return null;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        _applicationExceptionHandler.CaptureException<string>(ex, ApplicationLayer.Repository, ActionType.Withdraw);
                        return new DatabaseResult { ResultStatus = false };
                    }
                    //finally
                    //{
                    //    if (connection.State == System.Data.ConnectionState.Open)
                    //    {
                    //        connection.Close();
                    //    }
                    //}
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
                    if (result.Any())
                    {
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
                    else
                    {
                        return new RetrieveDatabaseResult<List<JobSearchResultDTO>>
                        {
                            Details = null,
                            ResultStatus = false,
                            ResultMessage = "No jobs found",
                            OperationType = "SEARCH",
                            AffectedRecordId = 0,
                            OperationDateTime = DateTime.Now,
                            ExceptionMessage = null
                        };
                    }
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
                    foreach (var item in result)
                    {
                        if (item.IdPublication == 0)
                        {
                            return null;
                        }
                    }
                    if (results.Any())
                    {
                        return new RetrieveDatabaseResult<List<UserApplicationsResponseDTO>>
                        {
                            Details = result,
                            ResultStatus = true,
                            ResultMessage = "Applicatoins retireved successfully",
                            OperationType = "GET ALL",
                            OperationDateTime = DateTime.Now,
                            AffectedRecordId = 0,
                            ExceptionMessage = "No exceptions found",
                        };
                    }
                    else
                    {
                        return new RetrieveDatabaseResult<List<UserApplicationsResponseDTO>>
                        {
                            Details = null,
                            ResultStatus = false,
                            ResultMessage = "No applications found",
                            OperationType = "GET ALL",
                            OperationDateTime = DateTime.Now,
                            AffectedRecordId = 0,
                            ExceptionMessage = null,
                        };
                    }
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
        #endregion
    }
}
