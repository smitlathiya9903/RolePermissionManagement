namespace RolePermissionManagement.Models.Entities
{
    public class Role
    {
        public int Id { get; set; } // Primary Key
        public string Name { get; set; } // Role Name
        public bool Active { get; set; } // Indicates if the role is active

        // Many-to-Many relationship with Permission
        public ICollection<Permission> Permissions { get; set; } = new List<Permission>();

        // One-to-Many relationship with User
        public ICollection<User> Users { get; set; } = new List<User>();
    }

}
