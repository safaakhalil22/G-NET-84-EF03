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
    internal class StudentConfigurations : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.Property(S => S.FullName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(S => S.Email)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(S => S.DateOfBirth)
                .IsRequired();
        }
    }
}
