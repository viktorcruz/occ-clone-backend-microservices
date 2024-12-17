using SearchJobsService.Application.Commands;
using SearchJobsService.Application.DTO;
using SearchJobsService.Application.DTO.Commands;
using SearchJobsService.Application.DTO.Queries;
using SharedKernel.Common.Responses;

namespace SearchJobsService.Infrastructure.Interface
{
    public interface ISearchJobsRepository
    {
        Task<DatabaseResult> ApplyAsync(ApplyCommand command);
        Task<DatabaseResult> WithdrawAsync(WithdrawApplicationRequestDTO withdrawDTO);
        Task<RetrieveDatabaseResult<List<JobSearchResultDTO>>> SearchAsync(string keyword);
        Task<RetrieveDatabaseResult<List<UserApplicationsResponseDTO>>> ApplicationsAsync(int userId);
        Task<DatabaseResult> HasApplicationAsync(int userId, int applicationId);
    }
}
