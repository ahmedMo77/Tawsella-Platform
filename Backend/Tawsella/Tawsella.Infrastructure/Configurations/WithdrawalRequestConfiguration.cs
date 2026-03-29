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
    public class WithdrawalRequestConfiguration : IEntityTypeConfiguration<WithdrawalRequest>
    {
        public void Configure(EntityTypeBuilder<WithdrawalRequest> entity)
        {
            entity.ToTable("WithdrawalRequests");
            entity.HasKey(wr => wr.Id);

            entity.Property(wr => wr.Amount)
                .IsRequired()
                .HasColumnType("decimal(10,2)");

            entity.Property(wr => wr.Status)
                .IsRequired()
                .HasDefaultValue(WithdrawalStatus.Pending);

            entity.Property(wr => wr.BankDetails)
                .HasMaxLength(500);

            entity.Property(wr => wr.Notes)
                .HasMaxLength(500);

            entity.HasIndex(wr => new { wr.WalletId, wr.Status });
            entity.HasIndex(wr => wr.RequestedAt);
        }
    }
}
