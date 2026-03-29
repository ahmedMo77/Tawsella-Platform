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
    public class WithdrawalRequest
    {
        public string Id { get; set; }
        public decimal Amount { get; set; }
        public string BankDetails { get; set; }
        public string? ProcessedBy { get; set; }
        public string Notes { get; set; }
        public WithdrawalStatus Status { get; set; }
        public DateTime RequestedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }

        public string WalletId { get; set; }
        public Wallet Wallet { get; set; }
    }
}
