using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Domain.Entities;
using Tawsella.Domain.Enums;

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

            // Pickup (OrderContact owns a Location)
            entity.OwnsOne(o => o.Pickup, pickup =>
            {
                pickup.Property(p => p.Name).HasMaxLength(200);
                pickup.Property(p => p.Phone).HasMaxLength(20);

                pickup.OwnsOne(p => p.Location, location =>
                {
                    location.Property(l => l.AddressName).IsRequired().HasMaxLength(500);
                    location.Property(l => l.Latitude).IsRequired().HasColumnType("decimal(10,8)");
                    location.Property(l => l.Longitude).IsRequired().HasColumnType("decimal(11,8)");
                });
            });

            // Dropoff (OrderContact owns a Location)
            entity.OwnsOne(o => o.Dropoff, dropoff =>
            {
                dropoff.Property(p => p.Name).HasMaxLength(200);
                dropoff.Property(p => p.Phone).HasMaxLength(20);

                dropoff.OwnsOne(p => p.Location, location =>
                {
                    location.Property(l => l.AddressName).IsRequired().HasMaxLength(500);
                    location.Property(l => l.Latitude).IsRequired().HasColumnType("decimal(10,8)");
                    location.Property(l => l.Longitude).IsRequired().HasColumnType("decimal(11,8)");
                });
            });

            // Package details
            entity.OwnsOne(o => o.Package, package =>
            {
                package.Property(p => p.Weight).HasColumnType("decimal(8,2)");
                package.Property(p => p.Notes).HasMaxLength(1000);
            });

            // Finances
            entity.OwnsOne(o => o.Money, money =>
            {
                money.Property(m => m.EstimatedPrice).IsRequired().HasColumnType("decimal(10,2)");
                money.Property(m => m.FinalPrice).HasColumnType("decimal(10,2)");
                money.Property(m => m.CourierEarnings).HasColumnType("decimal(10,2)");
                money.Property(m => m.PlatformCommission).HasColumnType("decimal(10,2)");
            });

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
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            entity.HasIndex(o => o.OrderNumber).IsUnique();
            entity.HasIndex(o => o.Status);
            entity.HasIndex(o => o.CreatedAt);
            entity.HasIndex(o => new { o.CustomerId });
            entity.HasIndex(o => new { o.CustomerId, o.Status });
            entity.HasIndex(o => new { o.CourierId, o.Status });
        }
    }
}
