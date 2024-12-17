using PublicationsService.Saga.Interfaces;

namespace PublicationsService.Saga
{
    public class PublicationSagaContext : IPublicationSagaContext
    {
        public Guid IdSaga => throw new NotImplementedException();

        public Dictionary<string, bool> Steps => throw new NotImplementedException();

        public int IdUser { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int IdPublication { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool IsJobSearchUpdated { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool IsSagaComplete { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public SagaState CurrentState { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public List<SagaState> StateHistory => throw new NotImplementedException();

        public void CompleteSaga()
        {
            throw new NotImplementedException();
        }

        public void MarkJobSearchUpdated()
        {
            throw new NotImplementedException();
        }

        public void MarkPublicationCreated(int publicationId)
        {
            throw new NotImplementedException();
        }

        public void MarkUserCreated(int userId)
        {
            throw new NotImplementedException();
        }

        public void TransitionToState(SagaState newState)
        {
            throw new NotImplementedException();
        }
    }
}
