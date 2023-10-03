using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMMIWeb.Service.Models
{
    public class ChargeViewModel
    {
        public long Amount { get;set;}
        public string Currency { get; set; }
        public string ProductName { get; set; }
        public long Quantity { get; set; }
        public string Mode { get; set; }
        public string Desc { get; set; }

        public string CustomerId { get; set; }
        public string CardId { get; set; }
    }
}
