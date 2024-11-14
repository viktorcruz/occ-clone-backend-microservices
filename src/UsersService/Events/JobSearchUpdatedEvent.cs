namespace UsersService.Events
{
    public class JobSearchUpdatedEvent
    {
        public int UserId { get; set; }
        public int PublicationId { get; set; }
    }
}
