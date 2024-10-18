using System.Net.Mail;
using UsersService.SharedKernel.Service.Interface;

namespace UsersService.SharedKernel.Service
{
    public class EmailService : IEmailService
    {
        #region Properties
        private readonly SmtpClient _smtpClient;
        #endregion

        #region Constructor
        public EmailService(SmtpClient smtpClient)
        {
            _smtpClient = smtpClient ?? throw new ArgumentNullException();
        }
        #endregion

        #region Methods
        public async Task SendEamilAsync(string email, string subject, string message)
        {
            var mailMessage = new MailMessage("noreply@yourdomain.com", email, subject, message);
            await _smtpClient.SendMailAsync(mailMessage);
        }
        #endregion
    }
}
