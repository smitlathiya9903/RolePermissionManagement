namespace RolePermissionManagement.Models
{
    public class AddUserRequest
    {
        public string Name { get; set; }
        public bool Active { get; set; }
        public int RoleId { get; set; }
    }
}
