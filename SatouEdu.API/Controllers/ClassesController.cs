using LMS.API.DTOs;
using LMS.Core.Entities;
using LMS.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassesController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ClassesController(AppDbContext context)
        {
            _context = context;
        }
        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> CreateClass([FromBody] CreateClassRequest request)
        {
            // Lấy thông tin giáo viên từ token
            //khi dang nhap id user dc luu trong claim bay gio ta lay no ra
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;//lay id tu token
            if (string.IsNullOrEmpty(userIdString))
            {
                return Unauthorized("User information not found");
            }
            int teacherId = int.Parse(userIdString);
            string joinCode = "SATOU-" + Guid.NewGuid().ToString().Substring(0, 4).ToUpper();//tao joincode ngau nhien gom SATOU + 4 ky tu ngau nhien
            // Tạo lớp học mới
            var newClass = new ClassRoom
            {
                Name = request.Name,
                Description = request.Description,
                JoinCode = joinCode,
                TeacherId = teacherId,//gan giao vien cho lop hoc
                CreatedDate = DateTime.Now
            };
            // Lưu lớp học vào cơ sở dữ liệu
            _context.ClassRooms.Add(newClass);
            await _context.SaveChangesAsync();
            return Ok(new
            {
                message = "Class created successfully",
                classId = newClass.Id,
                joinCode = newClass.JoinCode
            });
        }
    }
}
