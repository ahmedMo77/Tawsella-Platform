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

            entity.Property(c => c.NationalId).HasMaxLength(16);

            // Owned types
            entity.OwnsOne(c => c.CurrentLocation, location =>
            {
                location.Property(l => l.AddressName).HasMaxLength(200);
                location.Property(l => l.Latitude).HasColumnType("decimal(10,8)");
                location.Property(l => l.Longitude).HasColumnType("decimal(11,8)");
            });

            entity.OwnsOne(c => c.Vehicle, vehicle =>
            {
                vehicle.Property(v => v.PlateNumber).HasMaxLength(20);
                vehicle.Property(v => v.LicenseNumber).HasMaxLength(50);
            });

            // Relationships
            entity.HasOne(c => c.User)
                .WithOne()
                .HasForeignKey<Courier>(c => c.Id)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(c => c.Wallet)
                .WithOne(w => w.Courier)
                .HasForeignKey<Wallet>(w => w.CourierId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(c => c.Orders)
                        .WithOne(o => o.Courier)
                        .HasForeignKey(o => o.CourierId)
                        .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(c => c.ReviewsReceived)
                        .WithOne(r => r.Courier)
                        .HasForeignKey(r => r.CourierId)
                        .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(c => c.NationalId).IsUnique();
         
        }
    }
}
