using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMMIWeb.Service.Models
{
    public class ConsultantDetailViewModel
    {
        public int SlotCount { get; set; }
        public int ConsultantId { get; set; }
        public GetConsultantDetail GetConsultantDetail { get; set; }
        public IEnumerable<GetConsultantReviewViewModel> lstGetConsultantReviewViewModel { get; set; }
        public List<GetConsultantByTosClangCountryViewModel> lstModelConsultant { get; set; }

    }
}
