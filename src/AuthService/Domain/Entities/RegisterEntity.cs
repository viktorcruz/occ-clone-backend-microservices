using AuthService.Infrastructure.Security;
using Microsoft.AspNetCore.Identity;

namespace AuthService.Domain.Entities
{
    public class RegisterEntity
    {
        public int IdRole { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            return PasswordHasher.VerifyPassword(password, hashedPassword);
        }
    }
}
