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
    public class SubscriptionPlanConfiguration : IEntityTypeConfiguration<SubscriptionPlan>
    {
        public void Configure(EntityTypeBuilder<SubscriptionPlan> entity)
        {
            entity.ToTable("SubscriptionPlans");
            entity.HasKey(sp => sp.Id);

            entity.Property(sp => sp.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(sp => sp.Description)
                .HasMaxLength(500);

            entity.Property(sp => sp.Type)
                .IsRequired();

            entity.Property(sp => sp.Price)
                .IsRequired()
                .HasColumnType("decimal(10,2)");

            entity.Property(sp => sp.BillingPeriod)
                .IsRequired();

            entity.Property(sp => sp.IsActive)
                .HasDefaultValue(true);

            entity.Property(sp => sp.Features)
                .HasMaxLength(1000);

            // Relationships
            entity.HasMany(sp => sp.Subscriptions)
                .WithOne(s => s.SubscriptionPlan)
                .HasForeignKey(s => s.SubscriptionPlanId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
