namespace PublicationsService.Aplication.Dto
{
    public class PublicationDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Company { get; set; }
        public string Location { get; set; }
        public DateTime PostedDate { get; set; }
        public int IdUser { get; set; }
    }
}
