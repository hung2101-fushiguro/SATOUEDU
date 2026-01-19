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
    public class AssignmentsController : ControllerBase
    {
       private readonly AppDbContext _context;// Khai báo biến toàn cục để thao tác với database
       public AssignmentsController(AppDbContext context)
       {
           _context = context;
       }
        [HttpPost]
        [Authorize(Roles = "Teacher")]// Chỉ giáo viên mới được phép tạo bài tập
        public async Task<IActionResult> CreateAssignment([FromBody] CreateAssigmentRequest request)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString))
            {
                return Unauthorized("User information not found");
            }
            int teacherId = int.Parse(userIdString);
            //kiem tra xem lop co thuoc ve giao vien dang nhap khong
            var classRoom = await _context.ClassRooms.FindAsync(request.ClassId);
            if(classRoom == null)
            {
                return NotFound("Class not found");
            }
            if(classRoom.TeacherId != teacherId)
            {
                return StatusCode(403, "You are not allowed to create assignment for this class");
            }
            //tao bai tap moi
            var assignment = new Assignment
            {
                Title = request.Title,
                Description = request.Description,
                DueDate = request.DueDate,
                ClassRoomId = request.ClassId,
                CreatedDate = DateTime.Now
            };
            _context.Assignments.Add(assignment);
            await _context.SaveChangesAsync();
            return Ok(new {message = "Assignment create successfully!", assignmentId = assignment.Id});
        }
        [HttpGet("class/{classId}")]
        [Authorize]
        public async Task<IActionResult> GetAssignmentsByClass(int classId)
        {
            // 1. Lấy thông tin người đang đăng nhập
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value; // Lấy Role (Teacher hay Student)

            if (string.IsNullOrEmpty(userIdString)) return Unauthorized();
            int userId = int.Parse(userIdString);

            // 2. Kiểm tra lớp có tồn tại không
            var classRoom = await _context.ClassRooms.FindAsync(classId);
            if (classRoom == null) return NotFound("The class does not exist.");

       
            // Phải là giáo viên chủ nhiệm của lớp này mới được xem
            if (userRole == "Teacher" && classRoom.TeacherId != userId)
            {
                return StatusCode(403, "You are not the teacher for this class.");
            }

            // TRƯỜNG HỢP 2: Nếu là HỌC SINH
            // Phải có trong danh sách ghi danh (ClassEnrollment) mới được xem
            if (userRole == "Student")
            {
                bool isJoined = await _context.ClassEnrollments
                    .AnyAsync(ce => ce.ClassRoomId == classId && ce.StudentId == userId);

                if (!isJoined)
                {
                    return StatusCode(403, "You haven't joined this class, so you can't see the assignments!");
                }
            }
            // --- KẾT THÚC ĐOẠN CODE BẢO MẬT MỚI ---

            // 3. Lấy danh sách bài tập (Code cũ)
            var assignments = await _context.Assignments
                .Where(a => a.ClassRoomId == classId)
                .Select(a => new AssignmentDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Description = a.Description,
                    DueDate = a.DueDate,
                    CreatedDate = a.CreatedDate
                })
                .ToListAsync();

            return Ok(assignments);
        }
    }
}
