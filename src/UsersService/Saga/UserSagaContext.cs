using UsersService.Saga.Interfaces;

namespace UsersService.Saga
{
    public class UserSagaContext : IUserSagaContext
    {
        public Guid IdSaga { get; private set; }
        public Dictionary<string, bool> Steps { get; private set; }

        public int IdUser { get; set; }
        public int IdPublication { get; set; }
        public bool IsJobSearchUpdated { get; set; }
        public bool IsSagaComplete { get; set; }
        public SagaState CurrentState { get; set; } = SagaState.NotStarted;

        public List<SagaState> StateHistory { get; private set; } = new List<SagaState>();

        public UserSagaContext()
        {
            IdSaga = Guid.NewGuid();
            Steps = new Dictionary<string, bool>();
        }

        public void TransitionToState(SagaState newState)
        {
            if (newState != CurrentState)
            {
                StateHistory.Add(newState);
                CurrentState = newState;
                Console.WriteLine($"Saga transitioned to state: {CurrentState}");
            }
            else
            {
                Console.WriteLine($"Attempted to transition to the same state: {CurrentState}");
            }
        }

        public void MarkUserCreated(int userId)
        {
            IdUser = userId;
            TransitionToState(SagaState.UserCreated);
        }

        public void MarkPublicationCreated(int publicationId)
        {
            IdPublication = publicationId;
            TransitionToState(SagaState.PublicationCreated);
        }

        public void MarkJobSearchUpdated()
        {
            IsJobSearchUpdated = true;
            TransitionToState(SagaState.JobSearchUpdated);
        }

        public void CompleteSaga()
        {
            IsSagaComplete = true;
            TransitionToState(SagaState.SagaComplete);
        }

    }
}
