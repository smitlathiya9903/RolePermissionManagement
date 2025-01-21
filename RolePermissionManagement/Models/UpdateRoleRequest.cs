namespace RolePermissionManagement.Models
{
    public class UpdateRoleRequest
    {
        public int Id { get; set; } // The ID of the Role to update (required)
        public string Name { get; set; } // The new name for the Role (required)
        public bool Active { get; set; } // Indicates if the role should be active
        public List<int> PermissionIds { get; set; } // List of updated Permission IDs associated with the Role
    }

}
