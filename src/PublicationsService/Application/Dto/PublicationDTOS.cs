namespace PublicationsService.Aplication.Dto
{
    public class PublicationDTOS
    {
        public int IdUser { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Company { get; set; }
        public string Location { get; set; }
        public DateTime PostedDate { get; set; }
    }
}
