namespace SharedKernel.Events.JobSearch
{
    public class JobAppliedEvent
    {
        public int IdJob { get; set; }
        public int IdUser { get; set; }
        public DateTime AppliedAt { get; set; }
    }
}
