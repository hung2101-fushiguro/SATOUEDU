using LMS.API.DTOs;
using LMS.Core.Entities;
using LMS.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace LMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
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
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            //tim user theo email
            var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == request.Email);
            //check neu khong thay hoac mat khau khong dung
            //BCrypt.Verify se tu dong so sanh password goc vs password da ma hoa
            if(user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return Unauthorized("Invalid email or password.");
            }
            //neu dung het thi tao jwt
            var token = GenerateJwtToken(user);
            return Ok(new { token });
        }
        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");//lay cau hinh trong appsettings.json
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);//lay key trong appsettings.json
            //trong token chua thong tin gi ? goi la claims
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),//luu id user
                    new Claim(ClaimTypes.Email, user.Email),//luu email user
                    new Claim(ClaimTypes.Role, user.Role.Name)//luu role user
                }),
                Expires = DateTime.UtcNow.AddHours(2),//thoi gian het token la 2 gio
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),//toan bo token se duoc ky bang key va thuat toan HmacSha256
                Issuer = jwtSettings["Issuer"],//lay issuer trong appsettings.json
                Audience = jwtSettings["Audience"]//lay audience trong appsettings.json
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);//tao token
            return tokenHandler.WriteToken(token);//tra ve token dang string
        }
    }
}