using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMMIWeb.Service.Models
{
    public class SlotDetail
    {
        public int Id { get; set; }
        public int Hour { get; set; }
        public int Minutes { get; set; }

        public DateTime Date { get; set; }
    }
}
