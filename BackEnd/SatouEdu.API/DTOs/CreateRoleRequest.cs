namespace LMS.API.DTOs
{
    public class CreateRoleRequest
    {
        public string RoleName { get; set; } = string.Empty;// Tên vai trò cần tạo, ví dụ: "Admin", "Teacher"
    }
}
