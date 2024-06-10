namespace AuthService.Entities
{
    public class Role : BaseEntity
    {
        public string RoleName { get; set; } = string.Empty;

        public string RoleKey { get; set; } = string.Empty;
    }
}
