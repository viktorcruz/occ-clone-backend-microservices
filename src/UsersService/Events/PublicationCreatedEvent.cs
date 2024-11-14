namespace UsersService.Events
{
    public class PublicationCreatedEvent
    {
        public int UserId { get; set; }
        public int PublicationId { get; set; }
    }
}
