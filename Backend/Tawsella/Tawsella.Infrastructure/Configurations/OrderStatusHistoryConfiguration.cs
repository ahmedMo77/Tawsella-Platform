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
    public class OrderStatusHistoryConfiguration : IEntityTypeConfiguration<OrderStatusHistory>
    {
        public void Configure(EntityTypeBuilder<OrderStatusHistory> entity)
        {
            entity.ToTable("OrderStatusHistories");
            entity.HasKey(osh => osh.Id);

            entity.Property(osh => osh.Status)
                .IsRequired();

            entity.Property(osh => osh.Notes)
                .HasMaxLength(500);

            entity.HasIndex(osh => new { osh.OrderId, osh.CreatedAt });
        }
    }
}
