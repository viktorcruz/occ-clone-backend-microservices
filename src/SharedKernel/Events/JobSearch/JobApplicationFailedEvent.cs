namespace SharedKernel.Events.JobSearch
{
    public class JobApplicationFailedEvent
    {
        public int IdJob { get; set; }
        public int IdApplicant { get; set; }
        public string Reason { get; set; }
        public DateTime FailedAt { get; set; }
    }
}
