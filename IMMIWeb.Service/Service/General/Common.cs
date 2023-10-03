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
using Microsoft.EntityFrameworkCore;
using IMMIWeb.Service.Repo;
using IMMIWeb.Service.Models;
using SendGrid.Helpers.Mail;
using SendGrid;

namespace IMMIWeb.Service.Service.General
{
    public static class Common
    {        
        public static string GenerateOTP()
        {
            Random random = new Random();
            int randomNumber = random.Next(10000, 99999);
            return randomNumber.ToString().Substring(0, 4);
        }
        public static IMMIWeb.User VerifyOTP(string guid)
        {
            DbA976eeImmitestContext _dbContext = new DbA976eeImmitestContext();

            var getOTP = _dbContext.Users.Where(x => x.Id == Convert.ToInt32(guid)).FirstOrDefault();
            return getOTP;
        }       
        public static NotificationModel SetNotificationModel(string Title, string Description, string? DeviceId, string? DeviceType)
        {
            NotificationModel obj = new NotificationModel();
            obj.Title = Title;
            obj.Body = Description;
            obj.DeviceId = DeviceId;
            obj.IsAndroiodDevice = IsAndroidDevice(DeviceType);

            return obj;
        }
        public static Boolean IsAndroidDevice(string DeviceType)
        {
            return DeviceType == "Android" ? true : false;
        }
        public static bool SendMail(string subject, string body, string toEmail)
        {
            string SendMailProcessStatus = 
                Convert.ToString(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("GeneralInfo")["SendMailProcessStatus"]);
            var isSend = true;

            try
            {
                if(SendMailProcessStatus== "true")
                {
                    var hosts = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("SMTPSettings")["hOST"];
                    var fromEmail = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("SMTPSettings")["fromEmail"];
                    var apiKey = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("SMTPSettings")["SendGridKey"];
                    var client = new SendGridClient(apiKey);
                    var from_email = new EmailAddress(fromEmail, "Advenuss");
                    var msg = MailHelper.CreateSingleEmail(from_email, new EmailAddress(toEmail), subject, "", body);
                    var response = client.SendEmailAsync(msg).ConfigureAwait(false);
                }               
            }
            catch (Exception x)
            {
                string msg = x.Message;
                isSend = false;
            }
            return isSend;

        }
        public static IMMIWeb.User UserSendSMSAttempt(string id)
        {
            DbA976eeImmitestContext _dbContext = new DbA976eeImmitestContext();
            var getUser = _dbContext.Users.Where(x => x.Id == Convert.ToInt32(id)).FirstOrDefault();

            return getUser;
            
        }

        public static string GenerateUniqueStamp()
        {
            // Implement your unique stamp generation logic here
            // This could involve a combination of timestamps, random strings, or other unique identifiers
            return Guid.NewGuid().ToString();
        }

        public static TimeZoneInfo GetLocalTimeZone()
        {
            return TimeZoneInfo.Local;
        }

        public static decimal ExchangeRate(int CountryId)
        {
            DbA976eeImmitestContext _dbContext = new DbA976eeImmitestContext();
            return (decimal)_dbContext.Countries.Where(m => m.Id == CountryId).Select(b => b.ExchangeRate ?? 0).FirstOrDefault();
        }

        public static DateTime GetTimeZone(DateTime date,string timezone)
        {
            var convertfromutctolocal = TimeZoneInfo.ConvertTimeFromUtc(date, TimeZoneInfo.Local);
            var LocalTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(convertfromutctolocal, TimeZoneInfo.Local.Id, timezone);
            return LocalTime;
        }

        //public static int GetTimeZone(int hours, int ConsultantId = 0)
        //{
        //    DbA976eeImmitestContext _dbContext = new DbA976eeImmitestContext();

        //    var localtimezone = TimeZoneInfo.Local;
        //    var constimezone = _dbContext.ConsultantSlots.Where(n => n.ConsultantId == ConsultantId && n.TimeZone != null && n.TimeZone != string.Empty).FirstOrDefault();

        //    TimeZoneInfo sourceTimeZone = TimeZoneInfo.FindSystemTimeZoneById(constimezone.TimeZone);

        //    // Define the target time zone (Indian Standard Time)
        //    TimeZoneInfo targetTimeZone = TimeZoneInfo.FindSystemTimeZoneById(localtimezone.StandardName);

        //    // Specify the time in the source time zone
        //    DateTime sourceTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hours, 0, 0);

        //    // Use LINQ to perform the conversion
        //    var convertedTime = new[] { sourceTime }
        //        .Select(dt => TimeZoneInfo.ConvertTime(dt, sourceTimeZone, targetTimeZone))
        //        .FirstOrDefault();
        //    int hr = convertedTime.Hour;
        //    return hr;
        //}

        public static List<DateTime> GetDateTimeZone(int ConsultantId)
        {
            DbA976eeImmitestContext _dbContext = new DbA976eeImmitestContext();
            List<DateTime> convertedTime = new List<DateTime>();
            var localtimezone = TimeZoneInfo.Local;
            var constimezone = _dbContext.ConsultantSlots.Where(n => n.ConsultantId == 78 && n.TimeZone != null && n.TimeZone != string.Empty).ToList();

           

            // Define the target time zone (Indian Standard Time)
            TimeZoneInfo targetTimeZone = TimeZoneInfo.FindSystemTimeZoneById(localtimezone.StandardName);

            foreach ( var i in constimezone)
            {
                TimeZoneInfo sourceTimeZone = TimeZoneInfo.FindSystemTimeZoneById(i.TimeZone);
                // Specify the time in the source time zone
                DateTime sourceTime = new DateTime(i.Date.Year, i.Date.Month, i.Date.Day, i.StartHour, 0, 0);

                // Use LINQ to perform the conversion
                var converted = new[] { sourceTime }
                    .Select(dt => TimeZoneInfo.ConvertTime(dt, sourceTimeZone, targetTimeZone))
                    .FirstOrDefault();
                //int hr = convertedTime.Hour;
                convertedTime.Add(converted);
            }

           return convertedTime;
        }

        
        
    }
}
