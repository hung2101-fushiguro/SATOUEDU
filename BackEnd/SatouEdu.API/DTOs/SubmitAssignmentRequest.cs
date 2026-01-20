namespace LMS.API.DTOs
{
    public class SubmitAssignmentRequest
    {
        public int AssignmentId { get; set; }
        public string Content { get; set; } = string.Empty; //link nộp bài hoặc nội dung bài làm
    }
}
