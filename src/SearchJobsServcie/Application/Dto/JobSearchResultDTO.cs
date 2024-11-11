namespace SearchJobsService.Application.Dto
{
    public class JobSearchResultDTO
    {
        public int IdPublication { get; set; }
        public int IdJob { get; set; }
        public string JobTypeName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Company { get; set; }
        public string Location { get; set; }
        public decimal Salary { get; set; }
        public DateTime ExpirationDate { get; set; }
        public DateTime PostedDate { get; set; }
    }
}
