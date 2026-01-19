namespace LMS.API.DTOs
{
    public class CreateAssigmentRequest
    {
        public int ClassId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
    }
}
