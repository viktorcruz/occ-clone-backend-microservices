using AuthService.Domain.Entities;

namespace AuthService.Infrastructure.Extensions
{
    public static class AuditDataExtension
    {
        public static object CreatePasswordAuditData(UserByEmailEntity user, string performedBy)
        {
            return new
            {
                IdUser = user.IdUser,
                Email = user.Email,
                PerformedBy = performedBy,
                Message = "Successfully updated"
            };
        }
    }
}