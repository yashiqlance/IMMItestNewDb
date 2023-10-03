using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMMIWeb.Service.Models
{
    public class BookConsultant
    {
        public int Id { get; set; }
        
        public IEnumerable<string> lstConsultantDate { get; set; }

        public int ConsultantId { get; set; }
        public string ConsultantName { get; set; }
        public string ConsultantAmount { get; set; }
        
    }
}
