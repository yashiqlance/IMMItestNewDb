using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMMIWeb.Service.Models
{
    public class BookConsultantViewModel
    {
        public GetConsultantDetail GetConsultantDetail { get; set; }
        public ConsultantSlot ConsultantSlot { get; set; }
        public UserCardsDetail UserCard { get; set; }
        public IEnumerable<string> lstConsultantDate { get; set; }
    }
}
