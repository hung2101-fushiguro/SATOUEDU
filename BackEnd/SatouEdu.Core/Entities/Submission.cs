using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Entities
{
    public class Submission
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty; // Nội dung nộp (Text hoặc Link)
        public DateTime SubmittedDate { get; set; } = DateTime.Now;
        public double? Score { get; set; } // Điểm số (Có thể null nếu chưa chấm)
        public string TeacherComment { get; set; } = string.Empty; // Lời phê
        // Nộp cho bài tập nào?
        public int AssignmentId { get; set; }
        public Assignment Assignment { get; set; }
        // Ai nộp?
        public int StudentId { get; set; }
        public User Student { get; set; }
    }
}
