using SearchJobsService.Application.Commands;
using SearchJobsService.Application.Dto;
using SearchJobsService.Domain.Interface;
using SearchJobsService.Infrastructure.Interface;
using SharedKernel.Common.Responses;

namespace SearchJobsService.Domain.Core
{
    public class SearchJobsDomain : ISearchJobsDomain
    {
        #region Properties
        private readonly ISearchJobsRepository _searchJobsRepository;
        #endregion

        #region Constructor
        public SearchJobsDomain(ISearchJobsRepository searchJobsRepository)
        {
            _searchJobsRepository = searchJobsRepository;
        }
        #endregion

        #region Methods
        public async Task<DatabaseResult> ApplyAsync(ApplyCommand jobApplicationRequestDTO)
        {
            return await _searchJobsRepository.ApplyAsync(jobApplicationRequestDTO);
        }

        public async Task<DatabaseResult> WithdrawAsync(WithdrawApplicationRequestDTO withdrawApplicationRequestDTO)
        {
            return await _searchJobsRepository.WithdrawAsync(withdrawApplicationRequestDTO);
        }

        public async Task<RetrieveDatabaseResult<List<JobSearchResultDTO>>> SearchAsync(string keyword)
        {
            return await _searchJobsRepository.SearchAsync(keyword);
        }

        public async Task<RetrieveDatabaseResult<List<UserApplicationsResponseDTO>>> ApplicationsAsync(int userId)
        {
            return await _searchJobsRepository.ApplicationsAsync(userId);
        }
        #endregion
    }
}
