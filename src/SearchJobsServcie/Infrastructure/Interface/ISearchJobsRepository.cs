using SearchJobsService.Application.Commands;
using SearchJobsService.Application.Dto;
using SharedKernel.Common.Responses;

namespace SearchJobsService.Infrastructure.Interface
{
    public interface ISearchJobsRepository
    {
        Task<DatabaseResult> ApplyAsync(ApplyCommand command);
        Task<DatabaseResult> WithdrawAsync(WithdrawApplicationRequestDTO withdrawDTO);
        Task<RetrieveDatabaseResult<List<JobSearchResultDTO>>> SearchAsync(string keyword);
        Task<RetrieveDatabaseResult<List<UserApplicationsResponseDTO>>> ApplicationsAsync(int userId);
    }
}
