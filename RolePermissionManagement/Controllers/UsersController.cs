using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RolePermissionManagement.Data;
using RolePermissionManagement.Models;
using RolePermissionManagement.Models.Entities;

namespace RolePermissionManagement.Controllers
{
    [Route("api/Users")]
    [ApiController]
    public class UsersController : Controller
    {
        private AppDbContext dbContext;

        public UsersController(AppDbContext dbContext) 
        {
            this.dbContext = dbContext;
        }

        [HttpGet("list")]
        public IActionResult GetUsers([FromQuery] string? search, [FromQuery] int? roleId, [FromQuery] bool? active, [FromQuery] int? pageNumber)
        {
            var usersQuery = dbContext.Users.Include(u => u.Role).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                usersQuery = usersQuery.Where(u => u.Name.Contains(search));

            if (roleId.HasValue)
                usersQuery = usersQuery.Where(u => u.RoleId == roleId);

            if (active.HasValue)
                usersQuery = usersQuery.Where(u => u.Active == active);

            const int pageSize = 10;
            var matchesCount = usersQuery.Count();

            if (pageNumber.HasValue)
                usersQuery = usersQuery.Skip((pageNumber.Value - 1) * pageSize).Take(pageSize);

            var users = usersQuery.Select(u => new
            {
                u.Name,
                Role = u.Role.Name,
                u.Active
            }).ToList();

            return Ok(new { pageNumber = pageNumber ?? 1, matchesCount, data = users });
        }

        [HttpPost]
        public IActionResult AddUser([FromBody] AddUserRequest request)
        {
            // Validate the user name
            if (string.IsNullOrWhiteSpace(request.Name))
                return BadRequest(new { Message = "User name cannot be empty." });

            if (dbContext.Users.Any(u => u.Name == request.Name))
                return BadRequest(new { Message = "User name must be unique." });

            // Validate the role ID
            var role = dbContext.Roles.FirstOrDefault(r => r.Id == request.RoleId);
            if (role == null)
                return BadRequest(new { Message = "Invalid Role ID." });

            // Create and add the new user
            var user = new User
            {
                Name = request.Name,
                Active = request.Active,
                RoleId = request.RoleId
            };

            dbContext.Users.Add(user);
            dbContext.SaveChanges();

            return Ok(new { Message = "Added", Id = user.Id });
        }

        [HttpPut]
        public IActionResult UpdateUser([FromBody] UpdateUserRequest request)
        {
            var user = dbContext.Users.FirstOrDefault(u => u.Id == request.Id);

            if (user == null)
                return NotFound(new { Message = "User not found." });

            if (string.IsNullOrWhiteSpace(request.Name))
                return BadRequest(new { Message = "User name cannot be empty." });

            if (dbContext.Users.Any(u => u.Name == request.Name && u.Id != request.Id))
                return BadRequest(new { Message = "User name must be unique." });

            if (dbContext.Users.Any(u => u.Name == request.Name && u.Id != request.Id))
                return BadRequest(new { Message = "Role ID cannot be empty." });

            var role = dbContext.Roles.FirstOrDefault(r => r.Id == request.RoleId);
            if (role == null)
                return BadRequest(new { Message = "Invalid Role ID." });

            user.Name = request.Name;
            user.Active = request.Active;
            user.RoleId = request.RoleId;

            dbContext.SaveChanges();

            return Ok(new { Message = "Updated", Id = user.Id });
        }

    }
}
