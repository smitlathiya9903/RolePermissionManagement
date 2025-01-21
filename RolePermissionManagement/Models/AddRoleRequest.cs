namespace RolePermissionManagement.Models
{
    public class AddRoleRequest
    {
        public string Name { get; set; } // Name of the Role
        public bool Active { get; set; } // Indicates if the role is active
        public List<int> PermissionIds { get; set; } // List of Permission IDs to associate with the Role
    }
}