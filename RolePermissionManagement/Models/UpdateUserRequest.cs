namespace RolePermissionManagement.Models
{
    public class UpdateUserRequest
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool Active { get; set; }

        public int RoleId { get; set; }


    }
}
