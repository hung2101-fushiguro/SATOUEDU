using LMS.API.DTOs;
using LMS.Core.Entities;
using LMS.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly AppDbContext _context;
        public RolesController(AppDbContext context)
        {
                       _context = context;
        }
        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request)
        {
            var exists = await _context.Roles.AnyAsync(r => r.Name == request.RoleName);
            if (exists)
            {
                return BadRequest("Role already exists.");
            }
            var newRole = new Role
            {
                Name = request.RoleName
            };// Tạo đối tượng Role mới
            _context.Roles.Add(newRole);// Thêm vào DbSet
            await _context.SaveChangesAsync();// Lưu vào CSDL
            return Ok(new {Message = "Role created successfully.", RoleId = newRole.Id });
        }
        [HttpGet]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _context.Roles.ToListAsync();
            return Ok(roles);
        }
    }
}
