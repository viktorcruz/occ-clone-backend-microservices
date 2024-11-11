namespace UsersService.Services.Interfaces
{
    public interface ISearchJobsService
    {
        Task PublishUpdateJobSearchevent(int userId, int publicationId);
        Task PublishRevertJobSearchUpdateEvent(int userId);
    }
}
