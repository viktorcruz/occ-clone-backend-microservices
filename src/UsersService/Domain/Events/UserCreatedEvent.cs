namespace UsersService.Domain.Events
{
    public class UserCreatedEvent
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }

        public UserCreatedEvent(
            int userId,
            int roleId,
            string firstName,
            string lastName,
            string email,
            DateTime createdAt)
        {
            UserId = userId;
            RoleId = roleId;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            CreatedAt = createdAt;
        }

        public UserCreatedEvent()
        {
        }
    }
}
