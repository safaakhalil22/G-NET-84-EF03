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
    internal class InstructorConfigurations : IEntityTypeConfiguration<Instructor>
    {
        public void Configure(EntityTypeBuilder<Instructor> builder)
        {
            builder.Property(I => I.FullName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(I => I.Email)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(I => I.Bio)
                .HasMaxLength(500);

            builder.Property(I => I.JoinedAt)
                .IsRequired();

            builder.HasData(
                new Instructor { Id = 1, FullName = "Ahmed Hassan", Email = "ahmed.hassan@learnhub.com", Bio = "Senior .NET developer with 10 years of experience.", JoinedAt = new DateTime(2023, 1, 15) },
                new Instructor { Id = 2, FullName = "Sara Mostafa", Email = "sara.mostafa@learnhub.com", Bio = "Database architect and trainer.", JoinedAt = new DateTime(2023, 6, 1) },
                new Instructor { Id = 3, FullName = "Omar Khaled", Email = "omar.khaled@learnhub.com", JoinedAt = new DateTime(2024, 2, 20) }
            );
        }
    }
}
