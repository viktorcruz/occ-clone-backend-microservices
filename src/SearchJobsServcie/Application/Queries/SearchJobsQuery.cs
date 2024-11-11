using MediatR;
using SearchJobsService.Application.Dto;
using SharedKernel.Common.Responses;
using SharedKernel.Interface;

namespace SearchJobsService.Application.Queries
{
    public class SearchJobsQuery : IRequest<IEndpointResponse<RetrieveDatabaseResult<List<JobSearchResultDTO>>>>
    {
        #region Properties
        public string? Keyword { get; set; }
        #endregion

        #region Constructor
        #endregion

        #region Methods
        public SearchJobsQuery(string? keyword)
        {
            Keyword = keyword;
        }
        #endregion
    }
}
