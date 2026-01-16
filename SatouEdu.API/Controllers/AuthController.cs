using LMS.API.DTOs;
using LMS.Core.Entities;
using LMS.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            // 1. Kiểm tra Email đã tồn tại chưa
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return BadRequest("Email already used!");
            }

            // 2. Kiểm tra Role có tồn tại không (Chỉ cho phép Teacher hoặc Student)
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == request.RoleName);
            if (role == null)
            {
                return BadRequest("Role not suitable! [Teacher] or [Student]");
            }

            // 3. Mã hóa mật khẩu (Hashing) - QUAN TRỌNG NHẤT
            // BCrypt sẽ biến "123456" thành "$2a$11$sU..."
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // 4. Tạo User mới
            var newUser = new User
            {
                FullName = request.FullName,
                Email = request.Email,
                PasswordHash = passwordHash, // Lưu cái đã mã hóa, KHÔNG lưu password gốc
                RoleId = role.Id
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Register successfully!", userId = newUser.Id });
        }
    }
}