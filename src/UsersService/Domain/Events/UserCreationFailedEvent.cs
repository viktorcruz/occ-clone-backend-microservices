namespace UsersService.Domain.Events
{
    public class UserCreationFailedEvent
    {
        public string Reason { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public DateTime FailedAt { get; set; }

        public UserCreationFailedEvent(
            string reason,
            string userName, 
            string email,
            DateTime failedAt)
        {
            Reason = reason;
            UserName = userName;
            Email = email;
            FailedAt = failedAt;
        }
    }
}
