namespace SharedKernel.Events.Publication
{
    public class PublicationCreationFailedEvent
    {
        public int IdUser { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime TimeStamp { get; set; } = DateTime.Now;
    }
}
