using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMMIWeb.Service.Models
{
    public class ConsultantProfileViewModel
    {
        public int ConsultantId { get; set; }
        public string Name { get; set; } = null!;
        public IEnumerable<CommonListViewModel> ConsultantTypeOfServices { get; set; }
        public IEnumerable<CommonListViewModel> ConsultantCountryForService { get; set; }
        public IEnumerable<CommonListViewModel> ConsultantCommunicationLanguage { get; set; }
    }
}
