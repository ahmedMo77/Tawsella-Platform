using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tawsella.Domain.Entities
{
    public class Wallet
    {
        [Key]
        public Guid Id { get; set; }

        public Guid CourierId { get; set; }
        public Courier Courier { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Balance { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal PendingBalance { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalEarnings { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public ICollection<WalletTransaction> Transactions { get; set; }
        public ICollection<WithdrawalRequest> WithdrawalRequests { get; set; }
    }
}
