using LMS.API.DTOs;
using LMS.Core.Entities;
using LMS.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        [HttpPost("join")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> JoinClass([FromBody] JoinClassRequest request)
        {
            // Lấy thông tin học sinh từ token
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;//lay id tu token
            if (string.IsNullOrEmpty(userIdString))
                return Unauthorized();//neu khong co thong tin user tra ve loi 401
            int studentId = int.Parse(userIdString);//chuyen id tu string sang int
            // Tìm lớp học theo mã tham gia
            var classRoom = await _context.ClassRooms.FirstOrDefaultAsync(c => c.JoinCode == request.JoinCode);
            if (classRoom == null)
            {
                return NotFound("The join code not exist or not true!");
            }
            // Kiểm tra nếu học sinh đã tham gia lớp học
            bool alreadyEnrolled = await _context.ClassEnrollments.AnyAsync(ce => ce.StudentId == studentId && ce.ClassRoomId == classRoom.Id);
            if (alreadyEnrolled)
            {
                return BadRequest("You have already joined this class");
            }
            // neu chua thi tao moi ghi danh
            var enrollment = new ClassEnrollment
            {
                StudentId = studentId,
                ClassRoomId = classRoom.Id,
                EnrolledDate = DateTime.Now
            };
            _context.ClassEnrollments.Add(enrollment);
            await _context.SaveChangesAsync();
            return Ok(new { message = $"Welcome to class {classRoom.Name}!" });
        }
        [HttpGet("{id}/students")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> GetStudentsInClass(int id)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString)) return Unauthorized();
            int currentTeacherId = int.Parse(userIdString);//lay id giao vien hien tai
            var classRoom = await _context.ClassRooms.FindAsync(id);//tim lop hoc theo id
            if(classRoom == null)
            {
                return NotFound("Class not found");
            }
            //kiem tra giao vien co phai la giao vien cua lop hoc nay khong
            if(classRoom.TeacherId != currentTeacherId)
            {
                return StatusCode(403, "You are not the teacher of this class");
            }
            //lay danh sach hoc sinh trong lop hoc
            var students = await _context.ClassEnrollments
                .Where(ce => ce.ClassRoomId == id)//loc nhung ban ghi co lop hoc id trung voi id truyen vao
                .Select(ce => new StudentInClassDto
                {
                    StudentId = ce.Student.Id,
                    FullName = ce.Student.FullName,
                    Email = ce.Student.Email,
                    EnrolledDate = ce.EnrolledDate
                }).ToListAsync();
            return Ok(students);//tra ve danh sach hoc sinh
        }
    }
}
