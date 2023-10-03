using IMMIWeb.Service.Models;
using IMMIWeb.Service.Repo;
using IMMIWeb.Service.Service.General;
using IMMIWeb.Service.Service.User;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Configuration;
using SendGrid.Helpers.Mail;
using Twilio.Rest.Trunking.V1;

namespace IMMIWeb.Service.Service.Favourite
{
    public class FavouriteRepository : GenericRepository<IMMIWeb.FavouriteConsultant>, IFavouriteRepository
    {
        private IConfiguration Configuration;

        public FavouriteRepository(DbA976eeImmitestContext context, IConfiguration _configuration) : base(context)
        {
            Configuration = _configuration;
        }

        
        public List<GetAvailableConsultantData> GetUserFavouriteConsultant(int UserId)
        {
            List<GetAvailableConsultantData> result = new List<GetAvailableConsultantData>();

            using (IDbConnection connection = new SqlConnection(this.Configuration.GetConnectionString("dbConn")))
            {
                var parameters = new
                {
                    UserId = UserId,
                };

                result = connection.Query<GetAvailableConsultantData>("GetFavouriteConsultant", parameters, commandType: CommandType.StoredProcedure)
                   .Select(x => new GetAvailableConsultantData
                   {
                       Id = x.Id,
                       FirstName = x.FirstName,
                       LastName = x.LastName,
                       Email = x.Email,
                       LicenceNumber = x.LicenceNumber,
                       Mobile = x.Mobile,
                       MobileCountryCode = x.MobileCountryCode,
                       DeviceToken = x.DeviceToken,
                       DeviceType = x.DeviceType,
                       LanguageName = x.LanguageName,
                       CountryName = x.CountryName,
                       ServiceName = x.ServiceName,
                       averageRating = x.averageRating,
                       reviewCount = x.reviewCount,                     
                       IsFavConsultantornot = x.IsFavConsultantornot                     
                   }).ToList();

            }

            return result;

        }
        public int AddFavouriteConsultant(int ConsultantId, int UserId)
        {
            int returnId = 0;
            var param = new FavouriteConsultant();
            try
            {
                param.ConsultantId = ConsultantId;
                param.UserId = UserId;
                param.IsActive = true;
                param.CreatedOn = Convert.ToDateTime(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"));
                _dbContext.FavouriteConsultants.Add(param);
                _dbContext.SaveChanges();
                returnId = param.Id;
            }
            catch (Exception e)
            {

                throw;
            }
            return returnId;
        }

        public int RemoveFavouriteConsultant(int ConsultantId, int UserId)
        {
            int returnId = 0;
            var param = new FavouriteConsultant();
            try
            {
                var RemoveFavList = (from p in _dbContext.FavouriteConsultants
                                     where p.UserId == UserId
                                     select p).ToList();
                if (RemoveFavList.Count > 0)
                {
                    _dbContext.FavouriteConsultants.RemoveRange(RemoveFavList);
                    _dbContext.SaveChanges();
                    returnId = 1;
                }
                else
                {
                   returnId =0;
                }                             
            }
            catch (Exception e)
            {

                throw;
            }
            return returnId;
        }

    }
}
