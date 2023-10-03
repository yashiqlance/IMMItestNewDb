using CorePush.Google;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using IMMIWeb.Service.Models;
using Nancy.Json;
//using Nancy.Json;
//using IMMIAPI.ViewModel;

namespace IMMIWeb.Service.Service
{
    public class Utility
    {
        public static bool SendNotification(NotificationModel notificationModel)
        {
            ResponseModel response = new ResponseModel();
            bool returnStatus = false;
            FcmSettings settings = new FcmSettings()
            {
                SenderId = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("FcmNotification")["SenderId"],
                ServerKey = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("FcmNotification")["ServerKey"]
            };
            if (notificationModel.IsAndroiodDevice)
            {
                /* FCM Sender (Android Device) */
                HttpClient httpClient = new HttpClient();

                string authorizationKey = string.Format("keyy={0}", settings.ServerKey);
                string deviceToken = notificationModel.DeviceId;

                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authorizationKey);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

               GoogleNotification.DataPayload dataPayload = new GoogleNotification.DataPayload();
                dataPayload.Title = notificationModel.Title;
                dataPayload.Body = notificationModel.Body;

                GoogleNotification notification = new GoogleNotification();
                notification.Data = dataPayload;
                notification.Notification = dataPayload;

                var fcm = new FcmSender(settings, httpClient);
                var fcmSendResponse = fcm.SendAsync(deviceToken, notification);

                returnStatus = fcmSendResponse.IsCompletedSuccessfully;

            }
            else
            {
                //Create the web request with fire base API  
                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "post";
                //serverKey - Key from Firebase cloud messaging server  
                tRequest.Headers.Add(string.Format("Authorization: key={0}", settings.ServerKey));
                //Sender Id - From firebase project setting  
                tRequest.Headers.Add(string.Format("Sender: id={0}", settings.SenderId));
                tRequest.ContentType = "application/json";
                var payload = new
                {
                    to = notificationModel.DeviceId,
                    priority = "high",
                    //content_available = true,
                    notification = new
                    {
                        body = notificationModel.Body,
                        title = notificationModel.Title,

                    },
                };
                var serializer = new JavaScriptSerializer();
                var jsonBody = JsonConvert.SerializeObject(payload);
                Byte[] byteArray = Encoding.UTF8.GetBytes(jsonBody);
                tRequest.ContentLength = byteArray.Length;
                using (Stream dataStream = tRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    using (WebResponse tResponse = tRequest.GetResponse())
                    {
                        if (((HttpWebResponse)tResponse).StatusCode == HttpStatusCode.OK)
                        {
                            returnStatus = true;
                        }
                        else
                        {
                            returnStatus = false;
                        }
                    }
                }
            }

            return returnStatus;
        }
    }
}
