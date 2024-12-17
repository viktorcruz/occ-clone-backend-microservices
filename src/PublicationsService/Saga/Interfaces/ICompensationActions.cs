namespace PublicationsService.Saga.Interfaces
{
    public interface ICompensationActions
    {
        Task CompensateCreateUserAsync(IPublicationSagaContext sagaState);
    }
}
