namespace PublicationsService.Aplication.Dto
{
    public class PublicationRetrieveDTO
    {
        public int IdPublication { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Company { get; set; }
        public string Location { get; set; }
        public DateTime PostedDate { get; set; }
        public string IdUser { get; set; }
    }
}
