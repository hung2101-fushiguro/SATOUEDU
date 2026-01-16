using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Entities
{
    public class ClassRoom
    {
        public int Id { get; set; }// Primary key
        public string Name { get; set; } = string.Empty;// Class name
        public string Description { get; set; } = string.Empty;// Class description
        public string JoinCode { get; set; } = string.Empty;// Code for students to join the class
        public DateTime CreatedDate { get; set; }// Date the class was created
        public int TeacherId { get; set; }// GVCN - Foreign key to User
        public User Teacher { get; set; }
        public ICollection<ClassEnrollment> Enrollments { get; set; }// Students enrolled in the class
    }
}
