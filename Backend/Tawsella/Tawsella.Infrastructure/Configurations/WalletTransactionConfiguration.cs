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
    public class WalletTransactionConfiguration : IEntityTypeConfiguration<WalletTransaction>
    {
        public void Configure(EntityTypeBuilder<WalletTransaction> entity)
        {
            entity.ToTable("WalletTransactions");
            entity.HasKey(wt => wt.Id);

            entity.Property(wt => wt.Type)
                .IsRequired();

            entity.Property(wt => wt.Amount)
                .IsRequired()
                .HasColumnType("decimal(10,2)");

            entity.Property(wt => wt.BalanceAfter)
                .IsRequired()
                .HasColumnType("decimal(10,2)");

            entity.Property(wt => wt.Description)
                .HasMaxLength(500);

            entity.HasIndex(wt => new { wt.WalletId, wt.CreatedAt });
            entity.HasIndex(wt => wt.OrderId);
        }
    }
}
