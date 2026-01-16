using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Entities
{
    public class ClassEnrollment
    {
        public int StudentId { get; set; } // Foreign key to User
        public User Student { get; set; } // Navigation property
        public int ClassRoomId { get; set; } // Foreign key to ClassRoom
        public ClassRoom ClassRoom { get; set; } // Navigation property
        public DateTime EnrolledDate { get; set; } = DateTime.Now; // Date of enrollment
    }
}
