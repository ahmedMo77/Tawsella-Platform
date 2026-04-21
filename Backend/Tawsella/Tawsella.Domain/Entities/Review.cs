using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tawsella.Domain.Entities
{
    public class Review : BaseEntity
    {
        public int Rating { get; set; }
        public string Comment { get; set; }

        public string OrderId { get; set; }
        public Order Order { get; set; }

        public string CustomerId { get; set; }  
        public Customer Customer { get; set; }

        public string CourierId { get; set; } 
        public Courier Courier { get; set; }
    }
}
