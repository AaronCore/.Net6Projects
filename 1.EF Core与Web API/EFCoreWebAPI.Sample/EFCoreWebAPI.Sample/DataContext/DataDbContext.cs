using System;
using Microsoft.EntityFrameworkCore;
using EFCoreWebAPI.Sample.Entitys;

namespace EFCoreWebAPI.Sample.DataContext
{
    /// <summary>
    /// DbContext
    /// </summary>
    public class DataDbContext : DbContext
    {
        public DataDbContext(DbContextOptions<DataDbContext> options) : base(options)
        {

        }

        public DbSet<Class> Class { get; set; }
        public DbSet<Students> Students { get; set; }
        public DbSet<Archives> Archives { get; set; }
        public DbSet<Courses> Courses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Class>().HasComment("班级表").Property(p => p.Name).IsRequired().HasMaxLength(20);
            modelBuilder.Entity<Archives>().HasComment("档案表").Property(p => p.No).IsRequired().HasMaxLength(20);
            modelBuilder.Entity<Courses>().HasComment("课程表").Property(p => p.Name).HasMaxLength(20);
            //modelBuilder.Entity<Students>().HasComment("学生表");

            // 学生与档案1:1关系
            modelBuilder.Entity<Students>()
                .HasOne(x => x.Archive)
                .WithOne(x => x.Student)
                .HasForeignKey<Archives>(x => x.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            // 学生与班级1:N关系
            modelBuilder.Entity<Students>()
                .HasOne(x => x.Class)
                .WithMany(x => x.Students)
                .HasForeignKey(x => x.ClassId)
                .OnDelete(DeleteBehavior.Cascade);

            // 学生与课程N:N关系
            modelBuilder.Entity<StudentCourses>().HasKey(p => new { p.StudentId, p.CourseId });

            // 设置种子数据
            // 班级
            modelBuilder.Entity<Class>().HasData(
                new Class
                {
                    Id = Guid.Parse("4b501cb3-d168-4cc0-b375-48fb33f318a4"),
                    Name = "计算机一班"
                },
                new Class
                {
                    Id = Guid.Parse("a715b654-c721-fe03-428a-eecc71f91fd1"),
                    Name = "计算机二班"
                }
            );
            // 课程
            modelBuilder.Entity<Courses>().HasData(
                new Courses()
                {
                    Id = Guid.Parse("f078ef86-080b-2d04-5cf6-2681908d4cc9"),
                    Name = "C#高级编程"
                },
                new Courses()
                {
                    Id = Guid.Parse("a521b8b6-299a-284d-2cb6-07d8c558a244"),
                    Name = "Java高级编程"
                },
                new Courses()
                {
                    Id = Guid.Parse("ec094517-76b3-ab8a-f09b-e5e3c8f78516"),
                    Name = "MySQL操作手册"
                },
                new Courses()
                {
                    Id = Guid.Parse("8eee5b6d-bf73-6392-221c-14f20bb052c0"),
                    Name = "JavaScript入门到实践"
                },
                new Courses()
                {
                    Id = Guid.Parse("1f950006-aadd-23e8-de6b-88a95d8548b2"),
                    Name = "C++入门编程"
                }
            );
            // 学生
            modelBuilder.Entity<Students>().HasData(
                new Students()
                {
                    Id = Guid.Parse("2fa2c303-ca6e-8a08-8693-b8a7352d9c95"),
                    No = "0001",
                    Name = "jack",
                    Gender = Gender.男,
                    DateOfBirth = new DateTime(1993, 5, 1),
                    ClassId = Guid.Parse("4b501cb3-d168-4cc0-b375-48fb33f318a4"),
                    ArchiveId = Guid.Parse("4abb3198-87fd-a81f-41a1-40dfb58f47e7")
                },
                new Students()
                {
                    Id = Guid.Parse("590b2674-0d2d-9c2c-57f1-2813c04bdc69"),
                    No = "0002",
                    Name = "core",
                    Gender = Gender.女,
                    DateOfBirth = new DateTime(1995, 6, 1),
                    ClassId = Guid.Parse("a715b654-c721-fe03-428a-eecc71f91fd1"),
                    ArchiveId = Guid.Parse("5c496296-e9f1-80ee-1b94-5bbcbd706c49")
                }
            );
            // 学生课程
            modelBuilder.Entity<StudentCourses>().HasData(
                new StudentCourses()
                {
                    StudentId = Guid.Parse("2fa2c303-ca6e-8a08-8693-b8a7352d9c95"),
                    CourseId = Guid.Parse("f078ef86-080b-2d04-5cf6-2681908d4cc9"),
                },
                new StudentCourses()
                {
                    StudentId = Guid.Parse("2fa2c303-ca6e-8a08-8693-b8a7352d9c95"),
                    CourseId = Guid.Parse("a521b8b6-299a-284d-2cb6-07d8c558a244"),
                },
                new StudentCourses()
                {
                    StudentId = Guid.Parse("590b2674-0d2d-9c2c-57f1-2813c04bdc69"),
                    CourseId = Guid.Parse("a521b8b6-299a-284d-2cb6-07d8c558a244"),
                }
            );
            // 档案
            modelBuilder.Entity<Archives>().HasData(
                new Archives()
                {
                    Id = Guid.Parse("4abb3198-87fd-a81f-41a1-40dfb58f47e7"),
                    No = "A0001",
                    CreateTime = DateTime.Now,
                    StudentId = Guid.Parse("2fa2c303-ca6e-8a08-8693-b8a7352d9c95"),
                },
                new Archives()
                {
                    Id = Guid.Parse("5c496296-e9f1-80ee-1b94-5bbcbd706c49"),
                    No = "A0002",
                    CreateTime = DateTime.Now,
                    StudentId = Guid.Parse("590b2674-0d2d-9c2c-57f1-2813c04bdc69"),
                }
            );
        }
    }
}
