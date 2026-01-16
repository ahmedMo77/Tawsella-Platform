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
    public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
    {
        public void Configure(EntityTypeBuilder<Subscription> entity)
        {
            entity.ToTable("Subscriptions");
            entity.HasKey(s => s.Id);

            entity.Property(s => s.StartDate)
                .IsRequired();

            entity.Property(s => s.EndDate)
                .IsRequired();

            entity.Property(s => s.Status)
                .IsRequired()
                .HasDefaultValue(SubscriptionStatus.Active);

            entity.Property(s => s.AutoRenew)
                .HasDefaultValue(true);

            // Ensure only one of these can be set
            entity.HasCheckConstraint(
                "CK_Subscription_OnlyOneSubscriber",
                "([MerchantId] IS NOT NULL AND [CourierId] IS NULL) OR " +
                "([MerchantId] IS NULL AND [CourierId] IS NOT NULL) OR " +
                "([MerchantId] IS NULL AND [CourierId] IS NULL)"
            );

            entity.HasIndex(s => new { s.Status, s.EndDate });
        }
    }
}
