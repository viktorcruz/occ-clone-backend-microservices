namespace PublicationsService.Aplication.Dto
{
    public class PublicationDTO
    {
        public int IdUser { get; set; }
        public int IdRole { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime ExpirationDate { get; set; }
        public int Status { get; set; }
        public decimal Salary { get; set; }
        public string Location { get; set; }
        public string Company { get; set; }
        public int IdJobType { get; set; }
    }
}

