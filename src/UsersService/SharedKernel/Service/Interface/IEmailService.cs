namespace UsersService.SharedKernel.Service.Interface
{
    public interface IEmailService
    {
        Task SendEamilAsync(string email, string subject, string message);
    }
}
