using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Application.Entities;

namespace Tawsella.Infrastructure.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> entity)
        {
            entity.ToTable("Customers");

            entity.HasKey(c => c.Id);

            entity.Property(c => c.DefaultPickupAddress).HasMaxLength(200);
            entity.Property(c => c.DefaultPickupLabel).HasMaxLength(50);
            entity.Property(c => c.DefaultPickupLatitude).HasColumnType("decimal(10,8)");
            entity.Property(c => c.DefaultPickupLongitude).HasColumnType("decimal(11,8)");


            //entity.Property(c => c.TotalOrdersCount)
            //    .HasDefaultValue(0);

            //entity.Property(c => c.CompletedOrdersCount)
            //    .HasDefaultValue(0);

            //entity.Property(c => c.CancelledOrdersCount)
            //    .HasDefaultValue(0);

            entity.HasOne(c => c.User)
                .WithOne()
                .HasForeignKey<Customer>(c => c.Id)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(c => c.Orders)
                .WithOne()
                .HasForeignKey(o => o.UserId);

            entity.HasMany(c => c.Reviews)
                .WithOne()
                .HasForeignKey(r => r.UserId);
        }
    }
}
