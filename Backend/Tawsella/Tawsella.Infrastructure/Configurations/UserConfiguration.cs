using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Domain.Entities;

namespace Tawsella.Infrastructure.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.ToTable("Users");
            builder.HasKey(u => u.Id);

            builder.Property(u => u.FullName)
                .IsRequired()
                .HasMaxLength(100);

         
            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(u => u.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(u => u.PasswordHash)
                .IsRequired();

            builder.Property(u => u.SecurityStamp)
                .HasMaxLength(256);
            builder.HasOne<Admin>()
                .WithOne(a => a.User)
                .HasForeignKey<Admin>(a => a.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<Customer>()
                .WithOne(c => c.User)
                .HasForeignKey<Customer>(c => c.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<Courier>()
                .WithOne(c => c.User)
                .HasForeignKey<Courier>(c => c.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(u => u.Email).IsUnique();

            builder.HasIndex(u => u.PhoneNumber).IsUnique();
        }

    }
}
