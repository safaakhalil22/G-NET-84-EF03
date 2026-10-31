using G_NET_84_EF03.Configurations;
using G_NET_84_EF03.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace G_NET_84_EF03
{
    internal class APPDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=.;DataBase=OnlineLearningDb;Trusted_Connection=true;TrustServerCertificate=true");
        }
        #region DbSets
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        #endregion
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration<Instructor>(new InstructorConfigurations());
            modelBuilder.ApplyConfiguration<Category>(new CategoryConfigurations());
            modelBuilder.ApplyConfiguration<Course>(new CourseConfigurations());
            modelBuilder.ApplyConfiguration<Student>(new StudentConfigurations());
            modelBuilder.ApplyConfiguration<Enrollment>(new EnrollmentConfigurations());
        }
    }
}
