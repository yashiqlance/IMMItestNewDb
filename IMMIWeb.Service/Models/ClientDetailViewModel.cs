using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMMIWeb.Service.Models
{
    public class ClientDetailViewModel
    {
        public ConsultantRetainClientListViewModel ClientDetails { get; set; }
        public List<UserDocument> UserDocuments { get; set; }
    }
}
