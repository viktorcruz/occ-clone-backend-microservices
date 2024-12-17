namespace SharedKernel.Events.User
{
    public class UserCreationFailedEvent
    {
        public int IdRole { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public object Reason { get; set; }
    }
}
