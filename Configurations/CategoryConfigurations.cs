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
    internal class CategoryConfigurations : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.Property(C => C.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(C => C.Description)
                .HasMaxLength(300);

            builder.HasOne(C => C.ParentCategory)
                .WithMany(C => C.SubCategories)
                .HasForeignKey(C => C.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasData(
                new Category { Id = 1, Name = "Programming", Description = "Software development courses" },
                new Category { Id = 2, Name = "Backend", Description = "Server side development", ParentCategoryId = 1 },
                new Category { Id = 3, Name = "ASP.NET", ParentCategoryId = 2 },
                new Category { Id = 4, Name = "Databases", Description = "Data storage and querying", ParentCategoryId = 2 },
                new Category { Id = 5, Name = "Design", Description = "Visual and interface design" },
                new Category { Id = 6, Name = "UI Design", ParentCategoryId = 5 },
                new Category { Id = 7, Name = "Mobile Development", ParentCategoryId = 1 }
            );
        }
    }
}
