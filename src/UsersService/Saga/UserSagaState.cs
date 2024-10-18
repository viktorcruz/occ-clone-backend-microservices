namespace UsersService.Saga
{
    public class UserSagaState
    {
        public int IdUser { get; set; }
        public int IdPublication { get; set; }
        public bool IsJobSearchUpdated { get; set; }
        public bool IsSagaComplete { get; set; }
        public string CurrentState { get; set; }

        public UserSagaState()
        {
            IdUser = 0;
            IdPublication = 0;
            IsJobSearchUpdated = false;
            IsSagaComplete = false;
            CurrentState = String.Empty;
        }

        public void UserCreated(int userId)
        {
            IdUser = userId;
        }

        public void PublicationCreated(int publicationId)
        {
            IdPublication = publicationId;
        }

        public void JobSearchUpdated()
        {
            IsJobSearchUpdated = true;
        }

        public void CompleteSata()
        {
            IsSagaComplete = true;
        }
    }
}
