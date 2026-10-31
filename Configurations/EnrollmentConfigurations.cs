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
    internal class EnrollmentConfigurations : IEntityTypeConfiguration<Enrollment>
    {
        public void Configure(EntityTypeBuilder<Enrollment> builder)
        {
            builder.HasKey(E => new { E.StudentId, E.CourseId });

            builder.Property(E => E.EnrolledAt)
                .IsRequired();

            builder.Property(E => E.Progress)
                .IsRequired();

            builder.Property(E => E.Status)
                .IsRequired();

            builder.Property(E => E.FinalGrade)
                .HasPrecision(5, 2);

            builder.HasOne(E => E.Student)
                .WithMany(S => S.Enrollments)
                .HasForeignKey(E => E.StudentId);

            builder.HasOne(E => E.Course)
                .WithMany(C => C.Enrollments)
                .HasForeignKey(E => E.CourseId);
        }
    }
}
