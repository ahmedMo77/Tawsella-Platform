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
    public class AdminConfiguration : IEntityTypeConfiguration<Admin>
    {
        public void Configure(EntityTypeBuilder<Admin> builder)
        {
            builder.ToTable("Admins");

           builder.HasKey(x => x.Id);

            builder.Property(a => a.IsSuperAdmin)
                .IsRequired();

            builder.HasOne(a=>a.User)
                .WithOne()
                .HasForeignKey<Admin>(a=>a.Id)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
