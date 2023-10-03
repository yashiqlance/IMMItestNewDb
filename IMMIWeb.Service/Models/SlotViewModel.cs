using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMMIWeb.Service.Models
{
    public class SlotViewModel
    {
        //        public string StartDate { get; set; }
        public string SlotDate { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public bool IsAct { get; set; }
    }
}
