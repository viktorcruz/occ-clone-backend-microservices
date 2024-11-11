namespace AuthService.Application.DTO
{
    public class ConfirmRegisterResponseDTO
    {
        public int IdUser { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; } 
        public string ActiveStatusDescription { get; set; } 
        public bool IsRegistrationConfirmed { get; set; } 
        public string RegistrationConfirmationDescription { get; set; } 
    }
}
