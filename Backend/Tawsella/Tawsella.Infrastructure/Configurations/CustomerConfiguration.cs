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
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> entity)
        {
            entity.ToTable("Customers");

            entity.HasKey(c => c.Id);

            entity.Property(c => c.DefaultPickupAddress)
                .HasMaxLength(200);

            entity.Property(c => c.DefaultPickupLatitude)
                .HasColumnType("decimal(10,8)");

            entity.Property(c => c.DefaultPickupLongitude)
                .HasColumnType("decimal(11,8)");

            entity.Property(c => c.DefaultDropoffAddress)
                .HasMaxLength(200);

            entity.Property(c => c.DefaultDropoffLatitude)
                .HasColumnType("decimal(10,8)");

            entity.Property(c => c.DefaultDropoffLongitude)
                .HasColumnType("decimal(11,8)");

            entity.Property(c => c.TotalOrdersCount)
                .HasDefaultValue(0);

            entity.Property(c => c.CompletedOrdersCount)
                .HasDefaultValue(0);

            entity.Property(c => c.CancelledOrdersCount)
                .HasDefaultValue(0);

            // Relationships
            entity.HasMany(c => c.Orders)
                .WithOne(o => o.Customer)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(c => c.Reviews)
                .WithOne(r => r.Customer)
                .HasForeignKey(r => r.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(c => c.User).WithOne()
                .HasForeignKey<Customer>(c => c.Id)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
