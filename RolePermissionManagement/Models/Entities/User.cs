namespace RolePermissionManagement.Models.Entities
{
    public class User
    {
        public int Id { get; set; } // Primary Key
        public string Name { get; set; } // User Name
        public bool Active { get; set; } // Indicates if the user is active

        public int RoleId { get; set; } // Foreign Key to Role
        public Role Role { get; set; } // Navigation Property
    }

}
