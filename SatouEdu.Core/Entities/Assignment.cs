using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Entities
{
    public class Assignment
    {
        public int Id { get; set; }// khoa chinh
        public string Title { get; set; } = string.Empty;// tieu de bai tap
        public string Description { get; set; } = string.Empty;// mo ta bai tap
        public DateTime CreatedDate { get; set; } = DateTime.Now;// ngay tao bai tap
        public DateTime? DueDate { get; set; }// han nop bai tap
        public int ClassRoomId { get; set; }// bai tap thuoc lop hoc nao
        public ClassRoom ClassRoom { get; set; }// thuoc tinh dieu huong den lop hoc
    }
}
