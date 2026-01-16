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
    public class WalletConfiguration : IEntityTypeConfiguration<Wallet>
    {
        public void Configure(EntityTypeBuilder<Wallet> entity)
        {
            entity.ToTable("Wallets");
            entity.HasKey(w => w.Id);

            entity.Property(w => w.Balance)
                .IsRequired()
                .HasColumnType("decimal(10,2)")
                .HasDefaultValue(0);

            entity.Property(w => w.PendingBalance)
                .IsRequired()
                .HasColumnType("decimal(10,2)")
                .HasDefaultValue(0);

            entity.Property(w => w.TotalEarnings)
                .IsRequired()
                .HasColumnType("decimal(10,2)")
                .HasDefaultValue(0);

            // Relationships
            entity.HasMany(w => w.Transactions)
                .WithOne(wt => wt.Wallet)
                .HasForeignKey(wt => wt.WalletId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(w => w.WithdrawalRequests)
                .WithOne(wr => wr.Wallet)
                .HasForeignKey(wr => wr.WalletId)
                .OnDelete(DeleteBehavior.Cascade);

            // One wallet per courier
            entity.HasIndex(w => w.CourierId).IsUnique();
        }
    }
}
