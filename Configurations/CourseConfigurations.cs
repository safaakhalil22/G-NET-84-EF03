using G_NET_84_EF03.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace G_NET_84_EF03.Configurations
{
    internal class CourseConfigurations : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            builder.Property(C => C.Title)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(C => C.Description)
                .HasMaxLength(1000);

            builder.Property(C => C.Price)
                .HasPrecision(10, 2);

            builder.Property(C => C.Level)
                .IsRequired();

            builder.Property(C => C.IsPublished)
                .IsRequired();

            builder.HasOne(C => C.Instructor)
                .WithMany(I => I.Courses)
                .HasForeignKey(C => C.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(C => C.Category)
                .WithMany(Cat => Cat.Courses)
                .HasForeignKey(C => C.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasData(
                new Course { Id = 1, Title = "C# Fundamentals", Description = "Learn the basics of C# from scratch.", Price = 299.99m, Level = CourseLevel.Beginner, IsPublished = true, PublishedAt = new DateTime(2023, 3, 1), InstructorId = 1, CategoryId = 1 },
                new Course { Id = 2, Title = "ASP.NET Core Web API", Description = "Build RESTful APIs with ASP.NET Core.", Price = 599.00m, Level = CourseLevel.Intermediate, IsPublished = true, PublishedAt = new DateTime(2023, 5, 10), InstructorId = 1, CategoryId = 3 },
                new Course { Id = 3, Title = "Advanced EF Core", Description = "Performance, tracking and advanced mappings.", Price = 799.50m, Level = CourseLevel.Advanced, IsPublished = true, PublishedAt = new DateTime(2023, 9, 18), InstructorId = 2, CategoryId = 4 },
                new Course { Id = 4, Title = "SQL Server Essentials", Price = 349.00m, Level = CourseLevel.Beginner, IsPublished = true, PublishedAt = new DateTime(2024, 1, 7), InstructorId = 2, CategoryId = 4 },
                new Course { Id = 5, Title = "UI Design Principles", Description = "Layout, color and typography for interfaces.", Price = 450.00m, Level = CourseLevel.Intermediate, IsPublished = false, InstructorId = 3, CategoryId = 6 },
                new Course { Id = 6, Title = "Microservices Architecture", Price = 950.00m, Level = CourseLevel.Advanced, IsPublished = false, InstructorId = 3, CategoryId = 2 }
            );
        }
    }
}
