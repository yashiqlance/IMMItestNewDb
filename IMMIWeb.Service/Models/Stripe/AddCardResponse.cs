using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMMIWeb.Service.Models.Stripe
{
    public class AddCardResponse
    {
        public string ReturnCustomerId { get; set;}
        public string ReturnCardId { get; set; }
    }
}
