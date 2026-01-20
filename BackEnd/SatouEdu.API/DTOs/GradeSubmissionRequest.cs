namespace LMS.API.DTOs
{
    public class GradeSubmissionRequest
    {
        public int SubmissionId { get; set; }
        public double Score { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
