using IMMIWeb.Service.Service.Consultant;
using IMMIWeb.Service.Service.User;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace IMMIWeb.Service.Service.Setting
{
    public class OtherRepository : IOtherRepository
    {
        private readonly DbA976eeImmitestContext _dbContext;
    
        public OtherRepository(DbA976eeImmitestContext context)
        {
            _dbContext = context;
        }

        public int AddConsultantServiceForCountry(ConsultantServiceForCountry consultantServiceForCountry)
        {
            int returnId = 0;

            _dbContext.ConsultantServiceForCountries.Add(consultantServiceForCountry);
            _dbContext.SaveChanges();
            returnId = consultantServiceForCountry.Id;

            return returnId;
        }

        public bool AddConsultantTypeOfService(List<ConsultantTypeOfService> lstConsultantTypeOfService)
        {
            try
            {               
                _dbContext.ConsultantTypeOfServices.AddRange(lstConsultantTypeOfService);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }            
        }

        public int AddConsultantLanguage(ConsultantLanguage consultantLanguage)
        {
            int returnId = 0;

            _dbContext.ConsultantLanguages.Add(consultantLanguage);
            _dbContext.SaveChanges();
            returnId = consultantLanguage.Id;

            return returnId;
        }

        public Charge GetCharges()
        {
            return _dbContext.Charges.FirstOrDefault();
        }

        public bool AddConsultantLanguage(List<ConsultantLanguage> lstConsultantLanguage)
        {
            try
            {
                _dbContext.ConsultantLanguages.AddRange(lstConsultantLanguage);
                _dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool AddUserRetainToConsultant(int uId,int cId)
        {
            bool isValid = true;
            //int cnt = _dbContext.UserRetainToConsultants.Where(x => x.UserId == uId && x.ConsultantId == cId).Count();

            //if (cnt == 0)
            //{
            try
            {
                UserRetainToConsultant userRetainToConsultant = new UserRetainToConsultant();

                userRetainToConsultant.UserId = uId;
                userRetainToConsultant.ConsultantId = cId;
                userRetainToConsultant.IsAct = true;
                userRetainToConsultant.CreatedOn = DateTime.UtcNow;

                _dbContext.UserRetainToConsultants.Add(userRetainToConsultant);
                _dbContext.SaveChanges();
            }
            catch (Exception)
            {
                isValid = false;
                throw;
            }
            return isValid;

            //}
        }

        public string GetAdminInstruction()
        {
            return _dbContext.AdminInstructions.Where(b=> b.IsAct == true).Select(r=> r.Instruction).FirstOrDefault();
        }
    }
}
