namespace SharedKernel.Events.Auth
{
    public class RegisterErrorEvent
    {
        public int? IdUser { get; set; }
        public string? IdCorrelation { get; set; }
        public string ErrorMessage { get; set; }
        public string StackTrace { get; set; }
        public DateTime TimeStamp { get; set; } = DateTime.Now;

    }
}
