using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LMS.Core.Entities;

namespace LMS.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        // Constructor nhận cấu hình từ bên ngoài (API)
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Khai báo: Tôi muốn tạo 4 bảng này trong SQL Server
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ClassRoom> ClassRooms { get; set; }
        public DbSet<ClassEnrollment> ClassEnrollments { get; set; }

        // Cấu hình nâng cao (Fluent API)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Cấu hình bảng Role: Tên quyền (Admin, Teacher) không được trùng nhau
            modelBuilder.Entity<Role>()
                .HasIndex(r => r.Name)
                .IsUnique();

            // 2. Cấu hình khóa chính cho bảng trung gian (Học sinh - Lớp học)
            // Khóa chính là sự kết hợp của StudentId và ClassRoomId
            modelBuilder.Entity<ClassEnrollment>()
                .HasKey(ce => new { ce.StudentId, ce.ClassRoomId });

            // 3. Thiết lập quan hệ:
            // Một lần ghi danh (Enrollment) thuộc về một Lớp học
            modelBuilder.Entity<ClassEnrollment>()
                .HasOne(ce => ce.ClassRoom)// 1 lần ghi danh có 1 lớp học
                .WithMany(c => c.Enrollments)// 1 lớp học có nhiều lần ghi danh
                .HasForeignKey(ce => ce.ClassRoomId)
                .OnDelete(DeleteBehavior.Cascade); // Khi lớp học bị xóa, các ghi danh liên quan cũng bị xóa

            // Một lần ghi danh thuộc về một Học sinh
            modelBuilder.Entity<ClassEnrollment>()
                .HasOne(ce => ce.Student)
                .WithMany().HasForeignKey(ce => ce.StudentId)
                .OnDelete(DeleteBehavior.Restrict);// Khi học sinh bị xóa, không xóa các ghi danh liên quan
        }
    }
}
