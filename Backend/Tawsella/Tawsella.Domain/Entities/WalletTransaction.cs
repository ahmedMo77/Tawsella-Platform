using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Domain.Enums;

namespace Tawsella.Domain.Entities
{
    public class WalletTransaction : BaseEntity
    {
        public string Id { get; set; }
        public decimal Amount { get; set; }
        public decimal BalanceAfter { get; set; }
        public string Description { get; set; }
        public TransactionType Type { get; set; }
        public DateTime CreatedAt { get; set; }


        public string? OrderId { get; set; }
        public Order Order { get; set; }

        public string WalletId { get; set; }
        public Wallet Wallet { get; set; }
    }
}
