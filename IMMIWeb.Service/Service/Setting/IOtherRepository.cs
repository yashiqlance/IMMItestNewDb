using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMMIWeb.Service.Service.Setting
{
    public interface IOtherRepository
    {
        IMMIWeb.Charge GetCharges();

        public int AddConsultantServiceForCountry(ConsultantServiceForCountry consultantServiceForCountry);
        public bool AddConsultantTypeOfService(List<ConsultantTypeOfService> lstConsultantTypeOfService);
        public bool AddConsultantLanguage(List<ConsultantLanguage> lstConsultantLanguage);

        public bool AddUserRetainToConsultant(int uId, int cId);

        string GetAdminInstruction();

    }
}
