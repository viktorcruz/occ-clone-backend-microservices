namespace PublicationsService.Domain.Entity
{
    public class PublicationEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Company { get; set; }
        public string Location { get; set; }
        public DateTime PostedDate { get; set; }
        public string IdUser { get; set; }
    }
}
