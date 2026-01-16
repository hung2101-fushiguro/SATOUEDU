using LMS.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Entities
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // Ví dụ: Admin, Teacher

        // 1 role has many users
        public ICollection<User> Users { get; set; }
    }
}
