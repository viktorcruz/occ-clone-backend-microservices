namespace SearchJobsServcie.Application.Dto
{
    public class SearchJobsDTO
    {
        public string Location { get; set; }
        public string Title { get; set; }
        public string Company { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
