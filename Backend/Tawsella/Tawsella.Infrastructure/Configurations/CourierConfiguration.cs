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
    public class CourierConfiguration : IEntityTypeConfiguration<Courier>
    {
        public void Configure(EntityTypeBuilder<Courier> entity)
        {
            entity.ToTable("Couriers");

            entity.HasKey(c => c.Id);

            entity.Property(c => c.NationalId)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(c => c.VehicleType)
                        .HasMaxLength(50);

            entity.Property(c => c.VehiclePlateNumber)
                        .HasMaxLength(50);

            entity.Property(c => c.LicenseNumber)
                        .HasMaxLength(50);

            entity.Property(c => c.IsApproved)
                        .HasDefaultValue(false);

            entity.Property(c => c.IsOnline)
                        .HasDefaultValue(false);

            entity.Property(c => c.IsAvailable)
                        .HasDefaultValue(false);

            entity.Property(c => c.CurrentLatitude)
                        .HasColumnType("decimal(10,8)");

            entity.Property(c => c.CurrentLongitude)
                        .HasColumnType("decimal(11,8)");

            // Relationships
            entity.HasOne(c => c.User).WithOne()
              .HasForeignKey<Courier>(c => c.CourierId)
              .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(c => c.Wallet)
                        .WithOne(w => w.Courier)
                        .HasForeignKey<Wallet>(w => w.CourierId)
                        .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(c => c.Subscription)
                        .WithOne()
                        .HasForeignKey<Courier>(c => c.SubscriptionId)
                        .OnDelete(DeleteBehavior.SetNull);

            entity.HasMany(c => c.Orders)
                        .WithOne(o => o.Courier)
                        .HasForeignKey(o => o.CourierId)
                        .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(c => c.ReviewsReceived)
                        .WithOne(r => r.Courier)
                        .HasForeignKey(r => r.CourierId)
                        .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(c => c.NationalId).IsUnique();
            entity.HasIndex(c => new
            {
                c.IsOnline,
                c.IsAvailable,
                c.CurrentLatitude,
                c.CurrentLongitude
            });
        }

           


    }
    
}
