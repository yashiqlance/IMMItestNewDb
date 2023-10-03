using Microsoft.Extensions.Configuration;
using RestSharp;

namespace IMMIWeb.Service.Service.CometChat
{
    public static class Service
    {
        static string cometChatAppId = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("CometChatSettings")["AppId"];
        static string cometChatAppToken = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("CometChatSettings")["AppToken"];

        public static string CreateUser(string caseVal,string EmailArg, string MobileNumberArg, string Name, string ProfilPic, string Uid)
        {
            string GenerateUniqueUserIdVal = GenerateUniqueUserId("user", Uid);

            var client = new RestClient("https://" + cometChatAppId + ".api-us.cometchat.io/v3/users");
            var request = new RestRequest("", Method.Post);
            request.AddHeader("accept", "application/json");
            request.AddHeader("content-type", "application/json");
            request.AddHeader("apikey", cometChatAppToken);
            request.AddParameter("application/json", "{\"metadata\":{\"@private\":{\"email\":\"" + EmailArg + "\",\"contactNumber\":\"" + MobileNumberArg + "\"}},\"uid\":\"" + GenerateUniqueUserIdVal + "\",\"name\":\"" + Name + "\",\"withAuthToken\":true}", ParameterType.RequestBody);
            var aData = client.Execute(request);

            return GenerateUniqueUserIdVal;
        }
        static string GenerateUniqueUserId(string caseVal,string uid)
        {
            DateTime now = DateTime.Now;

            if(caseVal=="user")
            {
                uid = $"user_{now:ddMMyyHHmmss}";
            }
            else
            {
                uid = $"consultant_{now:ddMMyyHHmmss}";
            }
            
            return uid.ToUpper();
        }


    }
}
