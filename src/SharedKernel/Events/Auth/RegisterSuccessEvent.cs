namespace SharedKernel.Events.Auth
{
    public class RegisterSuccessEvent
    {
        public int IdUser { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime ExpirationDate { get; set; }
        public int Status { get; set; }
        public decimal Salary { get; set; }
        public string Location { get; set; }
        public string Company { get; set; }
    }
}
