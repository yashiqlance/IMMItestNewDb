using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using Twilio.Types;
using Twilio.Rest.Api.V2010;
using static System.Net.WebRequestMethods;
using Twilio.Rest.Api.V2010.Account;
using System.Net.Mail;
using SendGrid;
using SendGrid.Helpers.Mail;
using IMMIWeb.Service.Models;
using Microsoft.EntityFrameworkCore;
using IMMIWeb.Service.Service.General;

namespace IMMIWeb.Service.Service.Communication
{
    public static class Send
    {
        public static bool SMS(string ReceiverMobileNumber, string Message)
        {
            string SendSmsProcessStatus =
                            Convert.ToString(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("GeneralInfo")["SendSmsProcessStatus"]);

            string Id = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("Twilio")["AccountSID"];
            string Token = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("Twilio")["AuthToken"];
            string DefaultMobileNumber = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("Twilio")["DefaultMobile"];

            TwilioClient.Init(Id, Token);

            try
            {
                if (SendSmsProcessStatus == "true")
                {
                    var message = MessageResource.Create(
                                body: Message,
                                from: new Twilio.Types.PhoneNumber(DefaultMobileNumber.Trim()),
                                to: new Twilio.Types.PhoneNumber(ReceiverMobileNumber.Trim())
                            );
                }
                return true;
            }
            catch (Exception ex)
            {
                string errorEx = ex.ToString();
                return false;
                throw;
            }
        }
        public static void SendMail(string subject, string body, string toEmail)
        {
            bool Exists = false;
            var hosts = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("SMTPSettings")["hOST"];
            var fromEmail = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("SMTPSettings")["fromEmail"];


            var apiKey = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("SMTPSettings")["SendGridKey"];
            var client = new SendGridClient(apiKey);
            var from_email = new EmailAddress(fromEmail, "Advenuss");
            var msg = MailHelper.CreateSingleEmail(from_email, new EmailAddress(toEmail), subject, "", body);
            Task<Response> response = Task.Run(async () =>
            {
                return await client.SendEmailAsync(msg);
            });

            //Task.Run(async () =>
            //{
            //    Response resvalue = await response;

            //    if (resvalue.StatusCode == System.Net.HttpStatusCode.Accepted)
            //    {
            //        Exists = true;
            //    }
            //    else
            //    {
            //        Exists = false;
            //    }
            //});
            //return Exists; 
            // return response.IsCompletedSuccessfully == System.Net.HttpStatusCode.Accepted;
            //return isSend;
        }
        public static void CommonInsertNotificationandSendNotification(CommonInsertNotificationandSendNotificationparam compar)
        {
            var paramnot = new InsertNotificationUserparam();

            //using (var _context = new DbA976eeImmitestContext())
            //{

            //    _context.NotificationMasters.Add(paramnot);
            //    _context.SaveChanges();
            //}
            paramnot.SenderId = compar.UserId;
            paramnot.ReceiverId = compar.ConsultantId;
            paramnot.Header = compar.Header;            
            paramnot.Body = compar.Body;
            paramnot.CreatedOn = Convert.ToDateTime(DateTime.UtcNow);
            paramnot.SenderUserType = 2;
            paramnot.ReceiverUserType = 3;            
            paramnot.NotificationTypeName = compar.NotificationTypeName;
            paramnot.IsAct = true;

            InsertNotificationUser(paramnot);

            CommonSendNotificationtoConsultantparam paramconsnot = new CommonSendNotificationtoConsultantparam();            

            paramconsnot.Title = compar.Title;
            paramconsnot.Description = compar.Description;
            paramconsnot.UserId = compar.UserId;
            paramconsnot.ConsultantId = compar.ConsultantId;

            CommonSendNotificationtoConsultant(paramconsnot);

        }
        public static void CommonSendNotificationtoConsultant(CommonSendNotificationtoConsultantparam param)
        {
            List<NotificationModel> objList = new List<NotificationModel>();
            var context = new DbA976eeImmitestContext();
            if (param.UserId != 0 && param.ConsultantId != 0)
            {
                var Listofconsultant = context.Consultants.Where(d => d.Id == param.ConsultantId && d.IsAdminApproved == true && d.IsSuspended != true && d.IsAvailable == true).Select(r => new
                {
                    r.DeviceToken,
                    r.DeviceType,
                }).ToList();

                if (Listofconsultant.Count > 0)
                {
                    NotificationModel obj = Common.SetNotificationModel(param.Title, param.Description, Listofconsultant[0].DeviceToken, Listofconsultant[0].DeviceType);
                    Utility.SendNotification(obj);
                }
            }
        }

        public static void InsertNotificationUser(InsertNotificationUserparam param)
        {
            var context = new DbA976eeImmitestContext();

            var records = new NotificationMaster
            {
                SenderId = param.SenderId,
                ReceiverId = param.ReceiverId,
                Header = param.Header,
                Body = param.Body,
                CreatedOn = Convert.ToDateTime(DateTime.UtcNow),
                SenderUserType = 2,
                ReceiverUserType = 3,
                NotificationTypeName = param.NotificationTypeName,
                IsAct = true,
            };
            context.Add(records);
            context.SaveChanges();
        }                                          
        public static async Task<string> CometChatCreateNewUserAsync(CreateCometChatUserparam param)
        {
            string result = string.Empty;
            if (param.UserId != 0 && param.Email != "")
            {
                string apiKey = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("CometChatAPIKey")["key"] ?? string.Empty;
                string appId = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("CometChatAPIID")["key"] ?? string.Empty;
                string apiEndpoint = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("CometChatAPIEndpoint")["key"].Replace("appid", appId);
                result = await CometChatApi.CreateUserAsync(param, apiKey, apiEndpoint);
            }
            return result;
        }
        public static async Task<string> CometChatDeleteUserAsync(DeleteCometChatUserparam param)
        {
            string result = string.Empty;
            if (param.UserId != 0)
            {
                string apiKey = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("CometChatAPIKey")["key"] ?? string.Empty;
                string appId = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("CometChatAPIID")["key"] ?? string.Empty;
                string apiEndpoint = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("CometChatAPIEndpoint")["key"].Replace("appid", appId);

                result = await CometChatApi.DeleteUserAsync(param, apiKey, apiEndpoint);
            }
            return result;
        }
        public static async Task<bool> CometChatGetUserAsync(DeleteCometChatConsultantparam param)
        {
            //JsonResult result;
            bool Exist = false;
            if (param.ConsultantId != 0)
            {
                string apiKey = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("CometChatAPIKey")["key"] ?? string.Empty;
                string appId = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("CometChatAPIID")["key"] ?? string.Empty;
                string apiEndpoint = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("CometChatAPIEndpoint")["key"].Replace("appid", appId);
                Exist = await CometChatApi.GetUserAsync(param, apiKey, apiEndpoint);
            }
            else
            {
                Exist = false;
            }
            return Exist;
        }
        public static async Task<string> CometChatDeleteConsultantAsync(DeleteCometChatConsultantparam param)
        {
            string result = string.Empty;
            if (!string.IsNullOrEmpty(param.CometChatConsultantUid))
            {
                string apiKey = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("CometChatAPIKey")["key"] ?? string.Empty;
                string appId = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("CometChatAPIID")["key"] ?? string.Empty;
                string apiEndpoint = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("CometChatAPIEndpoint")["key"].Replace("appid", appId);

                result = await CometChatApi.DeleteConsultantAsync(param, apiKey, apiEndpoint);
            }
            return result;
        }
        public static class CometChatApi
        {
            public static async Task<string> CreateUserAsync(CreateCometChatUserparam param, string apiKey, string apiEndpoint)
            {
                //JsonResult result;
                var responseContent = string.Empty;

                // string apiKey =  _configuration["CometChatAPIKey:key"];
                // string appId =  _configuration["CometChatAPIID:key"];
                // string apiEndpoint = _configuration["CometChatAPIEndpoint:key"].Replace("appid", appId);


                using (System.Net.Http.HttpClient client = new System.Net.Http.HttpClient())
                {
                    client.DefaultRequestHeaders.Add("apikey", apiKey);

                    var requestContent = new StringContent(
                        $"{{ \"name\": \"{param.Name}\", \"email\": \"{param.Email}\",\"uid\": \"{param.UserId}\" }}",
                        Encoding.UTF8,
                        "application/json"
                    );

                    var response = await client.PostAsync($"{apiEndpoint}/users", requestContent);
                    responseContent = await response.Content.ReadAsStringAsync();

                    return responseContent;
                }

            }
            public static async Task<string> DeleteUserAsync(DeleteCometChatUserparam param, string apiKey, string apiEndpoint)
            {
                var responseContent = string.Empty;


                using (System.Net.Http.HttpClient client = new System.Net.Http.HttpClient())
                {
                    client.DefaultRequestHeaders.Add("apikey", apiKey);

                    var requestContent = new StringContent(
                        "{\"permanent\":true}",
                        Encoding.UTF8,
                        "application/json"
                    );

                    var request = new HttpRequestMessage(System.Net.Http.HttpMethod.Delete, $"{apiEndpoint}/users/{param.UserId}");
                    request.Content = requestContent;

                    var response = await client.SendAsync(request);
                    responseContent = await response.Content.ReadAsStringAsync();


                }
                return responseContent;
            }

            public static async Task<bool> GetUserAsync(DeleteCometChatConsultantparam param, string apiKey, string apiEndpoint)
            {
                bool Exist = false;


                using (System.Net.Http.HttpClient client = new System.Net.Http.HttpClient())
                {
                    client.DefaultRequestHeaders.Add("apikey", apiKey);

                    //var requestContent = new StringContent(
                    //    "{\"permanent\":true}",
                    //    Encoding.UTF8,
                    //    "application/json"
                    //);

                    var request = new HttpRequestMessage(System.Net.Http.HttpMethod.Get, $"{apiEndpoint}/users/{param.ConsultantId}");
                    //request.Content = requestContent;


                    var response = await client.SendAsync(request);
                    var responseContent = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        Exist = true; // User exists
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        Exist = false; // User does not exist
                    }
                }
                return Exist;
            }

            public static async Task<string> DeleteConsultantAsync(DeleteCometChatConsultantparam param, string apiKey, string apiEndpoint)
            {
                var responseContent = string.Empty;


                using (System.Net.Http.HttpClient client = new System.Net.Http.HttpClient())
                {
                    client.DefaultRequestHeaders.Add("apikey", apiKey);

                    var requestContent = new StringContent(
                        "{\"permanent\":true}",
                        Encoding.UTF8,
                        "application/json"
                    );

                    var request = new HttpRequestMessage(System.Net.Http.HttpMethod.Delete, $"{apiEndpoint}/users/{param.CometChatConsultantUid}");
                    request.Content = requestContent;

                    var response = await client.SendAsync(request);
                    responseContent = await response.Content.ReadAsStringAsync();


                }
                return responseContent;
            }

        }
    }
}
