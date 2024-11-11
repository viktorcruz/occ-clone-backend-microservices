namespace UsersService.Events
{
    public class UserCreationSucceededEvent
    {
        public int IdUser { get; set; }
        //public string? FirstName { get; set; }
        //public string? LastName { get; set; }
        public object? AdditionalData { get; set; }
        public DateTime Timestamp { get; set; }


        public UserCreationSucceededEvent(int userId, object? additionalData)
        {
            IdUser = userId;
            //FirstName = firstName;
            //LastName = lastName;
            AdditionalData =
            Timestamp = DateTime.UtcNow;
            AdditionalData = additionalData;
        }
    }
}
