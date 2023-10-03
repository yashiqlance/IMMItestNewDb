using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMMIWeb.Service.Models
{
    public class ConsultantReviewViewModel
    {
        public int Id { get; set; }
        public int? ConsultantId { get; set; }
        public int? UserId { get; set; }
        public int? Rating { get; set; }
        public string? Review { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsActive { get; set; }
    }
}
