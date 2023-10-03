using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMMIWeb.Service.Models
{
    public class ConsultantRetainViewModel
    {
        public List<AppointmentPendingViewModel> lstModelAppoitment { get; set; }

        public List<AppointmentHistoryViewModel> lstHistoryAppoitment { get; set; }

        public List<AppointmentRetainViewModel> lstRetainAppoitment { get; set; }

        public RetainDetailsByUserViewModel lstRetainDetailsAppoitment { get; set; }

        public IEnumerable<GetConsultantReviewViewModel> lstGetConsultantReviewViewModel { get; set; }

        public List<UserDocument> lstUserDocuments { get; set; }
    }
}
