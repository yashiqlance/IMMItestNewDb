using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMMIWeb.Service.Models
{
    public class ConsultantPaymentHistoryViewModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePic { get; set; }
        public DateTime RetainDate { get; set; }
        public decimal RetainAmount { get; set; }
        public decimal Amount { get; set; }
        public string CancellationReason { get; set; }
        public decimal RejectionAmount { get; set; }

        public DateTime AppointmentDate { get;set; }
        public DateTime RejectionDate { get; set; }
    }
}
