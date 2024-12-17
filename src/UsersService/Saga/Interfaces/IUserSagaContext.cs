namespace UsersService.Saga.Interfaces
{
    public interface IUserSagaContext
    {
        Guid IdSaga { get; }
        Dictionary<string, bool> Steps { get; }
        int IdUser { get; set; }
        int IdPublication { get; set; }
        bool IsJobSearchUpdated { get; set; }
        bool IsSagaComplete { get; set; }
        SagaState CurrentState { get; set; }
        List<SagaState> StateHistory { get; }
        void TransitionToState(SagaState newState);
        void MarkUserCreated(int userId);
        void MarkPublicationCreated(int publicationId);
        void MarkJobSearchUpdated();
        void CompleteSaga();
    }
}
