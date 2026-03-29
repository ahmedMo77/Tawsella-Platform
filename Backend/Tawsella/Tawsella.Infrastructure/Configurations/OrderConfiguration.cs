using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Application.Entities;
using Tawsella.Application.Enums;

namespace Tawsella.Infrastructure.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> entity)
        {
            entity.ToTable("Orders");
            entity.HasKey(o => o.Id);

            entity.Property(o => o.OrderNumber)
                .IsRequired()
                .HasMaxLength(50);

            // Pickup details
            entity.Property(o => o.PickupAddress)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(o => o.PickupLatitude)
                .IsRequired()
                .HasColumnType("decimal(10,8)");

            entity.Property(o => o.PickupLongitude)
                .IsRequired()
                .HasColumnType("decimal(11,8)");

            entity.Property(o => o.PickupContactName)
                .HasMaxLength(200);

            entity.Property(o => o.PickupContactPhone)
                .HasMaxLength(20);

            // Dropoff details
            entity.Property(o => o.DropoffAddress)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(o => o.DropoffLatitude)
                .IsRequired()
                .HasColumnType("decimal(10,8)");

            entity.Property(o => o.DropoffLongitude)
                .IsRequired()
                .HasColumnType("decimal(11,8)");

            entity.Property(o => o.DropoffContactName)
                .HasMaxLength(200);

            entity.Property(o => o.DropoffContactPhone)
                .HasMaxLength(20);

            // Package details
            entity.Property(o => o.PackageSize)
                .HasMaxLength(100);

            entity.Property(o => o.PackageWeight)
                .HasColumnType("decimal(8,2)");

            entity.Property(o => o.PackageNotes)
                .HasMaxLength(1000);

            // Pricing
            entity.Property(o => o.EstimatedPrice)
                .IsRequired()
                .HasColumnType("decimal(10,2)");

            entity.Property(o => o.FinalPrice)
                .HasColumnType("decimal(10,2)");

            entity.Property(o => o.CourierEarnings)
                .HasColumnType("decimal(10,2)");

            entity.Property(o => o.PlatformCommission)
                .HasColumnType("decimal(10,2)");

            // Status
            entity.Property(o => o.Status)
                .IsRequired()
                .HasDefaultValue(OrderStatus.Pending);

            entity.Property(o => o.CancellationReason)
                .HasMaxLength(500);

            // Payment
            entity.Property(o => o.PaymentMethod)
                .IsRequired()
                .HasDefaultValue(PaymentMethod.Cash);

            entity.Property(o => o.PaymentStatus)
                .IsRequired()
                .HasDefaultValue(PaymentStatus.Pending);

            // Relationships
            entity.HasMany(o => o.StatusHistory)
                .WithOne(sh => sh.Order)
                .HasForeignKey(sh => sh.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(o => o.Review)
                .WithOne(r => r.Order)
                .HasForeignKey<Review>(r => r.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            entity.HasIndex(o => o.OrderNumber).IsUnique();
            entity.HasIndex(o => o.Status);
            entity.HasIndex(o => o.CreatedAt);
            entity.HasIndex(o => new { o.UserId });
            entity.HasIndex(o => new { o.UserId, o.Status });
            entity.HasIndex(o => new { o.CourierId, o.Status });
        }
    }
}
