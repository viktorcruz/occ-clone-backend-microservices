namespace AuthService.Infrastructure.Extensions
{
    public static class RoleTypeExtension
    {
        public static string GetRoleTypeName(int roleId)
        {
            string roleTypeName = string.Empty;
            switch(roleId)
            {
                case 1:
                    roleTypeName = UserRoles.Admin.ToRoleTypeName();
                    break;
                case 2:
                    roleTypeName = UserRoles.Recruiter.ToRoleTypeName();
                    break;
                case 3:
                    roleTypeName = UserRoles.Admin.ToRoleTypeName();
                    break;
            }
            return roleTypeName;
        }
        public static string ToRoleTypeName(this UserRoles userRoles)
        {
            return $"{userRoles.ToString().ToLower()}";
        }
    }
}
