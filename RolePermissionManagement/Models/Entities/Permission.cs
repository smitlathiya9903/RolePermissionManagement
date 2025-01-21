using System.Data;

namespace RolePermissionManagement.Models.Entities
{
    public class Permission
    {
        public int Id { get; set; } // Primary Key
        public string Name { get; set; } // Permission Name
        public bool Active { get; set; } // Indicates if permission is active

        // Many-to-Many relationship with Role
        public ICollection<Role> Roles { get; set; } = new List<Role>();
    }

}
