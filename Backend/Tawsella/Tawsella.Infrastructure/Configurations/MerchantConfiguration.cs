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
    public class MerchantConfiguration : IEntityTypeConfiguration<Merchant>
    {
        public void Configure(EntityTypeBuilder<Merchant> entity)
        {
          
                entity.ToTable("Merchants");

                entity.HasKey(x => x.MerchantId);

                entity.Property(m => m.BusinessName)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(m => m.BusinessRegistrationNumber)
                    .HasMaxLength(100);

                entity.Property(m => m.BusinessAddress)
                    .HasMaxLength(500);

                entity.Property(m => m.IsApproved)
                    .HasDefaultValue(false);

            // Relationships

            entity.HasOne(c => c.User).WithOne()
           .HasForeignKey<Merchant>(m=>m.MerchantId)
           .OnDelete(DeleteBehavior.Cascade);


            entity.HasOne(m => m.Subscription)
                    .WithOne()
                    .HasForeignKey<Merchant>(m => m.SubscriptionId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasMany(m => m.Orders)
                    .WithOne(o => o.Merchant)
                    .HasForeignKey(o => o.MerchantId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(m => m.BusinessRegistrationNumber);
           

        }
    }
}
