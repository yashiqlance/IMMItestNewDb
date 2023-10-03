using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMMIWeb.Service.Models
{
    public class RetrieveAgreementsModel
    {       
        public string Description { get; set; } = null!;
        public int Module { get; set; }
        public int UserRole { get; set; }


    }

    public class GetAgreementsModel
    {
        public int UserRole { get; set; }

        public string PrivacyPolicy { get; set; }

        public string TermsandConditions { get; set; }
        public string Agreements { get; set; }
        public string AboutUs { get; set; }    

    }
}
