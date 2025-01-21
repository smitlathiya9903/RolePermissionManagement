using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RolePermissionManagement.Data;

namespace RolePermissionManagement.Controllers
{
    [Route("api/Permission")]
    [ApiController]
    public class PermissionController : Controller
    {
        private AppDbContext dbContext;

        public PermissionController(AppDbContext dbContext) 
        { 
            this.dbContext = dbContext;
        }

        [HttpGet("list")]
        public IActionResult GetPermissions()
        {
            var permissions = dbContext.Permissions
                .Select(p => new { p.Name, p.Active })
                .ToList();

            return Ok(new { data = permissions });
        }
    }
}
