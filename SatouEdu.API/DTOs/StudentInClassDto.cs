namespace LMS.API.DTOs
{
    public class StudentInClassDto
    {
        public int StudentId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime EnrolledDate { get; set; }
    }
}
