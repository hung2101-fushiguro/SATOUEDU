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
    [ApiController]//danh dau day la 1 api controller
    public class SubmissionsController : ControllerBase
    {
        private readonly AppDbContext _context;
        public SubmissionsController(AppDbContext context)
        {
            _context = context;
        }
        //hoc sinh nop bai tap
        [HttpPost("submit")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> SubmitAssignment([FromBody] SubmitAssignmentRequest request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            //kiem tra bai tap co ton tai khong
            var assignment = await _context.Assignments.FindAsync(request.AssignmentId);//tim bai tap theo id
            if (assignment == null) return NotFound("The exercise not exist");
            //kiem tra neu hoc sinh da nop bai tap chua
            var existingSubmission = await _context.Submissions.FirstOrDefaultAsync(s => s.AssignmentId == request.AssignmentId && s.StudentId == userId);
            if(existingSubmission != null)
            {
                //neu da nop roi thi cap nhat lai
                existingSubmission.Content = request.Content;
                existingSubmission.SubmittedDate = DateTime.Now;
                _context.Submissions.Update(existingSubmission);
                await _context.SaveChangesAsync();
                return Ok(new {message = "Update submission successfully!"});
            }
            //neu chua nop thi tao moi
            var newSubmission = new Submission
            {
                AssignmentId = request.AssignmentId,
                StudentId = userId,
                Content = request.Content,
                SubmittedDate = DateTime.Now
            };
            _context.Submissions.Add(newSubmission);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Submit assignment successfully!" });
        }
        [HttpPost("grade")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> GradeSubmission([FromBody] GradeSubmissionRequest request)
        {
            //kiem tra bai nop co ton tai khong
            var teacherId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var submission = await _context.Submissions
                .Include(s => s.Assignment)
                .ThenInclude(a => a.ClassRoom)
                .FirstOrDefaultAsync(s => s.Id == request.SubmissionId);
            if(submission == null) { return NotFound("The submission not exist"); }
            if(submission.Assignment.ClassRoom.TeacherId != teacherId)
            {
                return StatusCode(403,"You are not allowed to grade this submission");
            }
            //cap nhat diem va comment
            submission.Score = request.Score;
            submission.TeacherComment = request.Comment;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Grade submission successfully!" });
        }
    }
}
