using PublicationsService.Saga.Interfaces;

namespace PublicationsService.Saga
{
    public class CompensationActions : ICompensationActions
    {
        public Task CompensateCreateUserAsync(IPublicationSagaContext sagaState)
        {
            throw new NotImplementedException();
        }
    }
}
