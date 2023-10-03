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
using System.Drawing;
using Stripe;
using IMMIWeb.Service.Service.Communication;
using System.Security.Cryptography;

namespace IMMIWeb.Service.Service.Retains
{
    public class RetainRepository : GenericRepository<IMMIWeb.Retain>, IRetainRepository
    {
        private IConfiguration Configuration;

        public RetainRepository(DbA976eeImmitestContext context, IConfiguration _configuration) : base(context)
        {
            Configuration = _configuration;
        }
        
        public List<RetainConsultantDetails> RetainConsultantDetails(int ConsultantId)
        {
            List<RetainConsultantDetails> result = new List<RetainConsultantDetails>();

            using (IDbConnection connection = new SqlConnection(this.Configuration.GetConnectionString("dbConn")))
            {
                var parameters = new
                {
                    ConsultantId = ConsultantId,
                };

                result = connection.Query<RetainConsultantDetails>("RetainConsultantDetails", parameters, commandType: CommandType.StoredProcedure)
                   .Select(x => new RetainConsultantDetails
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
                       RetainAmount = x.RetainAmount                   
                   }).ToList();

            }

            return result;

        }
        public int AddRetainConsultant(Retain param)
        {
            int returnId = 0;
            
            try
            {               
                _dbContext.Retains.Add(param);
                _dbContext.SaveChanges();
                returnId = param.Id;
            }
            catch (Exception e)
            {
                throw;
            }
            return returnId;
        }
        public List<GetDocumentandPaymentDetails> GetDocumentandPaymentDetails(int UserId, int ConsultantId)
        {
            List<GetDocumentandPaymentDetails> result = new List<GetDocumentandPaymentDetails>();

            using (IDbConnection connection = new SqlConnection(this.Configuration.GetConnectionString("dbConn")))
            {
                var parameters = new
                {
                    ConsultantId = ConsultantId,
                    UserId = UserId
                };

                result = connection.Query<GetDocumentandPaymentDetails>("GetDocumentandPaymentDetails", parameters, commandType: CommandType.StoredProcedure)
                   .Select(x => new GetDocumentandPaymentDetails
                   {

                       ConsultantId = x.ConsultantId,
                       ConsultantFirstName = x.ConsultantFirstName,
                       ConsultantLastName = x.ConsultantLastName,
                       ConsultantEmail = x.ConsultantEmail,
                       ConsultantLicenceNumber = x.ConsultantLicenceNumber,
                       ConsultantMobile = x.ConsultantMobile,
                       ConsultantMobileCountryCode = x.ConsultantMobileCountryCode,
                       ConsultantDeviceToken = x.ConsultantDeviceToken,
                       ConsultantDeviceType = x.ConsultantDeviceType,
                       UserFirstName = x.UserFirstName,
                       UserLastName = x.UserLastName,
                       UserEmail = x.UserEmail,
                       UserMobile = x.UserMobile,
                       UserMobileCountryCode = x.UserMobileCountryCode,
                       UserDeviceToken = x.UserDeviceToken,
                       UserDeviceType = x.UserDeviceType,
                       UserId = x.UserId,
                       Filename = x.Filename,
                       UserDocumentId = x.UserDocumentId,
                       Size = x.Size,
                       Extensions = x.Extensions,
                       CommunicationLanguage = x.CommunicationLanguage,
                       TypeofServiceName = x.TypeofServiceName,
                       ImmigrationCountry = x.ImmigrationCountry,
                       LanguageName = x.LanguageName,
                       CountryName = x.CountryName,
                       ServiceName = x.ServiceName,
                       averageRating = x.averageRating,
                       RetainAmount = x.RetainAmount,
                       PaymentTitle = x.PaymentTitle


                   }).ToList();

            }

            return result;

        }
        public int PayRetentionAmount(payretentionamountparam param)
        {
            int returnId = 0;
            var par = new Retain();
            var records = new RetainPayment();
            var option = new ChargeCreateOptions();
            var stripecharge = new Stripe.Charge();
            var emitables = new Emitable();

            StripeConfiguration.ApiKey = Configuration["Integration:key"];
            try
            {
                var stripeDetails = _dbContext.AppointmentPayments.Where(d => d.UserId == param.UserId && d.ConsultantId == param.ConsultantId && d.IsAct == true).ToList();

                if (stripeDetails.Count > 0)
                {
                    if (param.PaymentModeName == 1)
                    {
                        try
                        {
                            option.Amount = (long?)(param.RetainAmount * 100);
                            option.Currency = param.Currency;
                            option.Customer = stripeDetails[0].StripeCustomerId;
                            option.Source = stripeDetails[0].StripeCardId;

                            var services = new ChargeService();
                            stripecharge = services.Create(option);

                            if (stripecharge.Status == "succeeded")
                            {
                                records.RetainId = param.RetainId;
                                records.RetainAmount = param.RetainAmount;
                                records.PaymentModeName = param.PaymentModeName;
                                records.TransactionId = stripecharge.Id;
                                records.ForNumberOfEmi = 0;
                                records.RetainStripeAccountId = stripeDetails[0].StripeCustomerId;
                                records.CreatedOn = Convert.ToDateTime(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"));
                                _dbContext.Add(records);
                                _dbContext.SaveChanges();

                            }
                        }
                        catch(Exception x)
                        {
                            throw x;
                        }
                    }
                    else
                    {
                        try
                        {
                            decimal percentage = 20;
                            decimal DownPay = param.RetainAmount * (percentage / 100) * 100;


                            option.Amount = (long?)DownPay;
                            option.Currency = param.Currency;
                            option.Customer = stripeDetails[0].StripeCustomerId;
                            option.Source = stripeDetails[0].StripeCardId;

                            var services = new ChargeService();
                            stripecharge = services.Create(option);

                            if (stripecharge.Status == "succeeded")
                            {
                                records.RetainId = param.RetainId;
                                records.RetainAmount = param.RetainAmount;
                                records.PaymentModeName = param.PaymentModeName;
                                records.TransactionId = stripecharge.Id;
                                records.ForNumberOfEmi = 3;
                                records.RetainStripeAccountId = stripeDetails[0].StripeCustomerId;
                                records.CreatedOn = Convert.ToDateTime(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"));

                                _dbContext.Add(records);
                                _dbContext.SaveChanges();


                                var EPamout = (long)(option.Amount) / 100;

                                emitables.RetainId = param.RetainId;
                                emitables.StripeCustomerId = stripeDetails[0].StripeCustomerId;
                                emitables.StripeCardId = stripeDetails[0].StripeCardId;
                                emitables.ForNumberOfEmi = 3;
                                emitables.TotalAmount = (long)param.RetainAmount;
                                emitables.PaidAmount = EPamout;
                                emitables.Emicount = 0;
                                emitables.IsActive = true;
                                emitables.Emiamount = (long)param.RetainAmount - EPamout / 3;
                                emitables.CreatedOn = Convert.ToDateTime(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"));

                                _dbContext.Add(records);
                                _dbContext.SaveChanges();
                            
                            }
                        }
                        catch(Exception x)
                        {
                            throw x;
                        }
                    }

                    CommonInsertNotificationandSendNotificationparam paramnot = new CommonInsertNotificationandSendNotificationparam();

                    paramnot.Header = "Appointment Retain";
                    paramnot.Body = "Retaining Consultant By User";
                    paramnot.Title = "Advenuss";
                    paramnot.Description = "Retaining Consultant By User";
                    paramnot.UserId = param.UserId;
                    paramnot.ConsultantId = param.ConsultantId;
                    paramnot.NotificationTypeName = 8;

                    Send.CommonInsertNotificationandSendNotification(paramnot);





                }

                
            }
            catch (Exception e)
            {

                throw;
            }
            return returnId;
        }
        public void AddUserDocuments(List<UserDocument> lstUserAddDocument,int uId,int cId)
        {
            
            try
            {
                _dbContext.UserDocuments.AddRange(lstUserAddDocument);
                _dbContext.SaveChanges();

                //CommonInsertNotificationandSendNotificationparam paramnot = new CommonInsertNotificationandSendNotificationparam();

                //paramnot.Header = "Add/Edit Documents";
                //paramnot.Body = "User Add/Edit Documents";
                //paramnot.Title = "Advenuss";
                //paramnot.Description = "User Add/Edit Documents";
                //paramnot.UserId = uId;
                //paramnot.ConsultantId = cId;
                //paramnot.NotificationTypeName = 10;

                //Send.CommonInsertNotificationandSendNotification(paramnot);

            }
            catch (Exception e)
            {
                throw;
            }            
        }

        public bool RemoveUserDocument(string FileName, string FileExtension, int UserId)
        {
            bool success = false;
            try
            {
                var doclist = (from p in _dbContext.UserDocuments
                               where p.UserId == UserId && p.Filename == FileName && p.Extensions == FileExtension
                               select p).ToList();
                if (doclist.Count > 0)
                {
                    _dbContext.UserDocuments.RemoveRange(doclist);
                    _dbContext.SaveChanges();
                    success = true;
                }
                else
                {
                    success = false;
                }
            }
            catch (Exception e)
            {
                success = false;
            }

            return success;
            
        }
        public int AddRetainConsultantPayment(RetainPayment param)
        {
            int returnId = 0;

            try
            {
                _dbContext.RetainPayments.Add(param);
                _dbContext.SaveChanges();
                returnId = param.Id;
            }
            catch (Exception e)
            {
                throw;
            }
            return returnId;
        }
        public void AddEmiTable(List<Emitable> lstEmitable)
        {
            try
            {
                _dbContext.Emitables.AddRange(lstEmitable);
                _dbContext.SaveChanges();

                //CommonInsertNotificationandSendNotificationparam paramnot = new CommonInsertNotificationandSendNotificationparam();

                //paramnot.Header = "Add/Edit Emi";
                //paramnot.Body = "User Add/Edit Emi";
                //paramnot.Title = "Advenuss";
                //paramnot.Description = "Emi Add/Edit Documents";
                //paramnot.UserId = uId;
                //paramnot.ConsultantId = cId;
                //paramnot.NotificationTypeName = 10;

                //Send.CommonInsertNotificationandSendNotification(paramnot);

            }
            catch (Exception e)
            {
                throw;
            }
        }

        public List<GetRetainConsultnatListForUserViewModel> GetRetainConsultnatListForUser(int UserId)
        {
            List<GetRetainConsultnatListForUserViewModel> result = new List<GetRetainConsultnatListForUserViewModel>();

            using (IDbConnection connection = new SqlConnection(this.Configuration.GetConnectionString("dbConn")))
            {
                var parameters = new
                {
                    UserId = UserId
                };

                result = connection.Query<GetRetainConsultnatListForUserViewModel>("GetRetainConsultnatListForUser", parameters, commandType: CommandType.StoredProcedure)
                   .Select(x => new GetRetainConsultnatListForUserViewModel
                   {

                       ConsultantId = x.ConsultantId,
                       RetainID = x.RetainID,
                       ConsultantName = x.ConsultantName,
                       ProfilePic = x.ProfilePic,
                       CometChatConsultantUID = x.CometChatConsultantUID
                   }).ToList();

            }

            return result;
        }

        public List<GetRetainConsultnatListForUserViewModel> GetRetainUserListForConsultnat(int ConsultantId)
        {
            List<GetRetainConsultnatListForUserViewModel> result = new List<GetRetainConsultnatListForUserViewModel>();

            using (IDbConnection connection = new SqlConnection(this.Configuration.GetConnectionString("dbConn")))
            {
                var parameters = new
                {
                    ConsultantId = ConsultantId
                };

                result = connection.Query<GetRetainConsultnatListForUserViewModel>("GetRetainUserListForConsultnat", parameters, commandType: CommandType.StoredProcedure)
                   .Select(x => new GetRetainConsultnatListForUserViewModel
                   {
                       UserId = x.UserId,
                       RetainID = x.RetainID,
                       UserName = x.UserName,
                       ProfilePic = x.ProfilePic,
                       CometChatUserUID = x.CometChatUserUID
                   }).ToList();
            }
            return result;
        }
    }
}
