namespace UsersService.Events
{
    public class UserCreationFailedEvent
    {
        public int IdUser { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime Timestamp { get; set; }


        public UserCreationFailedEvent(int userId, string errorMessage)
        {
            IdUser = userId;
            ErrorMessage = errorMessage;
            Timestamp = DateTime.Now;
        }
    }
}
