namespace LMS.API.DTOs
{
    public class AssignmentDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }// han nop bai tap
        public DateTime CreatedDate { get; set; }
    }
}
