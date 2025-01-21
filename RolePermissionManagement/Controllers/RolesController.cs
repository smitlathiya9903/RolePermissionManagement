using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RolePermissionManagement.Data;
using RolePermissionManagement.Models;
using RolePermissionManagement.Models.Entities;

namespace RolePermissionManagement.Controllers
{
    [Route("api/Roles")]
    [ApiController]
    public class RolesController : Controller
    {
        private AppDbContext dbContext;

        public RolesController(AppDbContext dbContext) 
        {
            this.dbContext = dbContext;
        }

        [HttpGet("list")]
        public IActionResult GetRoles(
    [FromQuery] string? search,
    [FromQuery] int? permissionId,
    [FromQuery] bool? active,
    [FromQuery] int? pageNumber = 1)
        {
            // Validate and sanitize the pageNumber
            if (!pageNumber.HasValue || pageNumber < 1)
            {
                return BadRequest(new { Message = "Invalid page number. Page number must be greater than 0." });
            }

            const int pageSize = 10; // Fixed page size

            try
            {
                // Query roles with filters
                var rolesQuery = dbContext.Roles
                    .Include(r => r.Permissions)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(search))
                    rolesQuery = rolesQuery.Where(r => r.Name.Contains(search));

                if (permissionId.HasValue)
                    rolesQuery = rolesQuery.Where(r => r.Permissions.Any(p => p.Id == permissionId));

                if (active.HasValue)
                    rolesQuery = rolesQuery.Where(r => r.Active == active);

                // Total matches before pagination
                var totalMatches = rolesQuery.Count();

                // Ensure the requested page is valid
                if ((pageNumber - 1) * pageSize >= totalMatches)
                {
                    return BadRequest(new { Message = "Page number exceeds the total number of available pages." });
                }

                // Apply pagination
                var roles = rolesQuery
                    .Skip((pageNumber.Value - 1) * pageSize)
                    .Take(pageSize)
                    .Select(r => new
                    {
                        r.Name,
                        r.Active,
                        Permissions = r.Permissions.Select(p => p.Name).ToList()
                    })
                    .ToList();

                // Return response
                return Ok(new
                {
                    pageNumber,
                    totalMatches,
                    data = roles
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while processing your request.", Details = ex.Message });
            }
        }


        [HttpPost]

        public IActionResult AddRole([FromBody] AddRoleRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return BadRequest(new { Message = "Role name cannot be empty." });

            if (dbContext.Roles.Any(r => r.Name == request.Name))
                return BadRequest(new { Message = "Role name must be unique." });

            if (request.PermissionIds == null || !request.PermissionIds.Any())
                return BadRequest(new { Message = "Permission IDs cannot be empty." });

            var permissions = dbContext.Permissions.Where(p => request.PermissionIds.Contains(p.Id)).ToList();
            if (permissions.Count != request.PermissionIds.Count)
                return BadRequest(new { Message = "One or more Permission IDs are invalid." });

            var role = new Role
            {
                Name = request.Name,
                Active = request.Active,
                Permissions = permissions
            };

            dbContext.Roles.Add(role);
            dbContext.SaveChanges();

            return Ok(new { Message = "Added", Id = role.Id });
        }

        [HttpPut]
        public IActionResult UpdateRole([FromBody] UpdateRoleRequest request)
        {
            var role = dbContext.Roles.Include(r => r.Permissions).FirstOrDefault(r => r.Id == request.Id);

            if (role == null)
                return NotFound(new { Message = "Role not found." });

            if (string.IsNullOrWhiteSpace(request.Name))
                return BadRequest(new { Message = "Role name cannot be empty." });

            if (dbContext.Roles.Any(r => r.Name == request.Name && r.Id != request.Id))
                return BadRequest(new { Message = "Role name must be unique." });

            if (request.PermissionIds == null || !request.PermissionIds.Any())
                return BadRequest(new { Message = "Permission IDs cannot be empty." });

            var permissions = dbContext.Permissions.Where(p => request.PermissionIds.Contains(p.Id)).ToList();
            if (permissions.Count != request.PermissionIds.Count)
                return BadRequest(new { Message = "One or more Permission IDs are invalid." });

            role.Name = request.Name;
            role.Active = request.Active;
            role.Permissions = permissions;

            dbContext.SaveChanges();

            return Ok(new { Message = "Updated", Id = role.Id });
        }

        [HttpDelete]
        public IActionResult DeleteRole([FromBody] int roleId)
        {
            var role = dbContext.Roles.FirstOrDefault(r => r.Id == roleId);

            if (role == null)
                return NotFound(new { Message = "Role not found." });

            if (dbContext.Users.Any(u => u.RoleId == roleId))
                return BadRequest(new { Message = "Cannot delete role as it is assigned to one or more users." });

            dbContext.Roles.Remove(role);
            dbContext.SaveChanges();

            return Ok(new { Message = "Deleted", Id = role.Id });
        }

        [HttpGet]
        public IActionResult GetRole([FromQuery] int id)
        {
            var role = dbContext.Roles
                .Include(r => r.Permissions)
                .FirstOrDefault(r => r.Id == id);

            if (role == null)
                return NotFound(new { Message = "Role not found." });

            return Ok(new
            {
                role.Id,
                role.Name,
                role.Active,
                PermissionIds = role.Permissions.Select(p => p.Id).ToList()
            });
        }
    }
}
