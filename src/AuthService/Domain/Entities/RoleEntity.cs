namespace AuthService.Domain.Entities
{
    public class RoleEntity
    {
        public int IdRole { get; set; }
        public string RoleName { get; set; } = string.Empty;

        public bool VerifyRole(int userId)
        {
            if(userId > 0)
            {  return true; }
            return false;
        }
    }
}
