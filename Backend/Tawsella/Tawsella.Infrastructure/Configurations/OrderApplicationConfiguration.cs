using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tawsella.Application.Entities;
using Tawsella.Application.Enums;

namespace Tawsella.Infrastructure.Configurations
{
    public class OrderApplicationConfiguration : IEntityTypeConfiguration<OrderApplication>
    {
        public void Configure(EntityTypeBuilder<OrderApplication> entity)
        {
            entity.ToTable("OrderApplications");
            entity.HasKey(a => a.Id);

            entity.Property(a => a.Status)
                .IsRequired()
                .HasDefaultValue(OrderApplicationStatus.Pending);

            entity.Property(a => a.RejectedReason).HasMaxLength(500);

            entity.HasOne(a => a.Order)
                .WithMany()
                .HasForeignKey(a => a.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(a => a.Courier)
                .WithMany()
                .HasForeignKey(a => a.CourierId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(a => new { a.OrderId, a.CourierId }).IsUnique();
            entity.HasIndex(a => new { a.CourierId, a.Status });
            entity.HasIndex(a => a.OrderId);
        }
    }
}
