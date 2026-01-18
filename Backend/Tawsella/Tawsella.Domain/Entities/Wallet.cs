using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tawsella.Domain.Entities
{
    public class Wallet : BaseEntity
    {
        public decimal Balance { get; set; }
        public decimal PendingBalance { get; set; }
        public decimal TotalEarnings { get; set; }
        public string CourierId { get; set; }

        // Navigation
        public Courier Courier { get; set; }
        public ICollection<WalletTransaction> Transactions { get; set; }
        public ICollection<WithdrawalRequest> WithdrawalRequests { get; set; }
    }
}
