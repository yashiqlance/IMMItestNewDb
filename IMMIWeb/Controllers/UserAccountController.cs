using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using AutoMapper;
using IMMIWeb.Infrastructure;
using IMMIWeb.Service.Models;
using IMMIWeb.Service.Service.Communication;
using IMMIWeb.Service.Service.Consultant;
using IMMIWeb.Service.Service.General;
using IMMIWeb.Service.Service.Setting;
using IMMIWeb.Service.Service.StripePay;
using IMMIWeb.Service.Service.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static System.Net.WebRequestMethods;
using System.Reflection;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Http;
using Stripe;
using Org.BouncyCastle.Crypto.Tls;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Components;
//using System.Threading;
using System.Timers;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.Text;
using System.Security.Cryptography;
using System;
using Microsoft.EntityFrameworkCore.Diagnostics;
using IMMIWeb.Service.Service.CMS;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.ServiceModel.Syndication;
using System.Xml;


namespace IMMIWeb.Controllers
{
    public class UserAccountController : Controller
    {

        #region Fields

        private readonly ITypeOfServiceRepository _typeOfServiceRepository;
        private readonly ICountryRepository _countryRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILanguageRepository _languageRepository;
        private readonly ICMSRepository _cmsRepository;
        private readonly IMapper _mapper;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _hostingEnvironment;
        private readonly IConsultantRepository _consultantRepository;

        string OTPValidity = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("GeneralInfo")["OTPValidity"];
        int OTPAttemptVal = Convert.ToInt32(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("GeneralInfo")["OTPAttempt"]);

        #endregion

        #region Ctor

        public UserAccountController(
            Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnvironment,
            ITypeOfServiceRepository typeOfServiceRepository,
            ICountryRepository countryRepository,
            IUserRepository userRepository,
            IConsultantRepository consultantRepository,
            ILanguageRepository languageRepository,
            ICMSRepository cmsRepository,
            IMapper mapper,
            IStripeRepository stripeService)
        {
            _typeOfServiceRepository = typeOfServiceRepository;
            _countryRepository = countryRepository;
            _userRepository = userRepository;
            _languageRepository = languageRepository;
            _cmsRepository = cmsRepository;
            _mapper = mapper;
            _hostingEnvironment = hostingEnvironment;
            _consultantRepository = consultantRepository;
        }

        #endregion

        #region Method
        private string GenerateRandomNumericString(int length)
        {
            const string numericCharacters = "0123456789";
            var random = new Random();
            var result = new string(
                Enumerable.Repeat(numericCharacters, length)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());

            return result;
        }
        public ActionResult GetRssFeed()
        {
            string rssFeedUrl = "https://api.io.canada.ca/io-server/gc/news/en/v2?dept=departmentofcitizenshipandimmigration&sort=publishedDate&orderBy=desc&publishedDate%3E=2021-07-23&pick=50&format=atom&atomtitle=Immigration,%20Refugees%20and%20Citizenship%20Canada";

            try
            {                
                var request = System.Net.WebRequest.Create(rssFeedUrl);                
                using (var response = request.GetResponse())
                {                    
                    using (var reader = XmlReader.Create(response.GetResponseStream()))
                    {                        
                        var feed = SyndicationFeed.Load(reader);
                        var feedItems = feed.Items;
                        return View(feedItems);
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur during the request or parsing.
                ViewBag.ErrorMessage = ex.Message;
                return View();
            }
        }


        public int GetRSSNotificationCount()
        {

            int result = 0;
            string rssFeedUrl = "https://api.io.canada.ca/io-server/gc/news/en/v2?dept=departmentofcitizenshipandimmigration&sort=publishedDate&orderBy=desc&publishedDate%3E=2021-07-23&pick=50&format=atom&atomtitle=Immigration,%20Refugees%20and%20Citizenship%20Canada";

            try
            {
                var request = System.Net.WebRequest.Create(rssFeedUrl);
                using (var response = request.GetResponse())
                {
                    using (var reader = XmlReader.Create(response.GetResponseStream()))
                    {
                        var feed = SyndicationFeed.Load(reader);
                        foreach(var items  in feed.Items)
                        {
                            if (items.Categories[0].Name == "news releases")
                            {
                                result++;
                            }
                        }
                       
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur during the request or parsing.
                ViewBag.ErrorMessage = ex.Message;
                result = 0;
            }




            return result;


        }

        public ActionResult GuestSignUp()
        {
            UserViewModel userViewModel = new UserViewModel();

            var getCountryListByTypeOfService = _countryRepository.TypeOfServiceCountryList();
            userViewModel.lstCountryByTypeOfService = _mapper.Map<List<CommonListViewModel>>(getCountryListByTypeOfService);

            var getTypeOfServiceList = _typeOfServiceRepository.CommonTypeOfServiceList();
            userViewModel.lstTypeOfService = _mapper.Map<List<CommonListViewModel>>(getTypeOfServiceList);

            var getMobileList = _countryRepository.CommonCountryMobileCodeList();
            userViewModel.lstMobile = _mapper.Map<List<CommonListViewModel>>(getMobileList);

            return View(userViewModel);
        }
        [HttpPost]
        public ActionResult GuestSignUp(UserViewModel userViewModel)
        {
            UserViewModel Model = new UserViewModel();

            HttpContext.Session.SetString("NewMobileCountryCode", "");
            HttpContext.Session.SetString("NewMobile", "");
            HttpContext.Session.SetString("UserEmail", "");

            if (!string.IsNullOrEmpty(userViewModel.Mobile) && userViewModel.TypeOfServiceName >= 1 && userViewModel.ImmigrationCountry >= 1)
            {

                HttpContext.Session.SetString("MobileCountryCode", userViewModel.MobileCountryCode);
                HttpContext.Session.SetString("Mobile", Convert.ToString(userViewModel.Mobile));

                string mobileNum = userViewModel.MobileCountryCode.Replace('+', ' ').Trim() + userViewModel.Mobile;

                var getUser = _userRepository.Find(x => x.Mobile == userViewModel.Mobile).FirstOrDefault();

                string OTP = Common.GenerateOTP();
                string msgBody = "OTP for ADVENUSS is " + OTP + " Do not share it with anyone by any means.";
                User userObj = new User();

                if (getUser == null)
                {
                    #region Add Guest User

                    userObj.LoginAttempt = 1;
                    userObj.FirstName = "Hello";
                    userObj.LastName = "Guest";
                    userObj.MobileCountryCode = userViewModel.MobileCountryCode;
                    userObj.Mobile = userViewModel.Mobile;
                    userObj.TypeOfServiceName = userViewModel.TypeOfServiceName;
                    userObj.ImmigrationCountry = userViewModel.ImmigrationCountry;
                    userObj.DeviceType = "web";
                    userObj.Otp = OTP;
                    userObj.OtpDate = DateTime.UtcNow;
                    userObj.CreatedOn = DateTime.UtcNow;
                    userObj.IsActive = true;
                    userObj.IsVerified = true;
                    userObj.UserTypeVal = (int)MasterEnum.EUserType.User;
                    userObj.IsGuest = true;
                    int returnId = _userRepository.AddUser(userObj);

                    string receiverMobileNumber = userViewModel.MobileCountryCode + userViewModel.Mobile;
                    Send.SMS(receiverMobileNumber, msgBody);

                    HttpContext.Session.SetString("OTPRequestTime", Convert.ToString(DateTime.UtcNow));
                    HttpContext.Session.SetString("GUID", Convert.ToString(returnId));

                    return RedirectToAction("OTPVerification", "UserAccount");

                    #endregion
                }
                else
                {
                    if (getUser.IsGuest == true)
                    {
                        #region Update Guest User

                        bool isToday = Convert.ToDateTime(getUser.OtpDate).Day == DateTime.UtcNow.Day;

                        if (isToday == false || (isToday && getUser.LoginAttempt <= OTPAttemptVal))
                        {

                            userObj = getUser;
                            userObj.TypeOfServiceName = userViewModel.TypeOfServiceName;
                            userObj.ImmigrationCountry = userViewModel.ImmigrationCountry;
                            userObj.LoginAttempt = 1;
                            userObj.Otp = OTP;
                            userObj.OtpDate = DateTime.UtcNow;
                            _userRepository.Update(userObj);

                            string receiverMobileNumber = userViewModel.MobileCountryCode + userViewModel.Mobile;
                            Send.SMS(receiverMobileNumber, msgBody);

                            HttpContext.Session.SetString("OTPRequestTime", Convert.ToString(DateTime.UtcNow));
                            HttpContext.Session.SetString("GUID", Convert.ToString(getUser.Id));
                            return RedirectToAction("OTPVerification", "UserAccount");
                        }
                        else
                        {
                            Model.Message = "You have exceeded the maximum number of OTP request attempts for today.\r\n";
                        }

                        #endregion
                    }
                    else
                    {
                        Model.Message = "Guest login not valid for you please try to do login";
                    }
                }
            }
            else
            {
                Model.Message = "All fields are required.";
            }

            #region Model Binding


            var getCountryListByTypeOfService = _countryRepository.TypeOfServiceCountryList();
            Model.lstCountryByTypeOfService = _mapper.Map<List<CommonListViewModel>>(getCountryListByTypeOfService);

            var getTypeOfServiceList = _typeOfServiceRepository.CommonTypeOfServiceList();
            Model.lstTypeOfService = _mapper.Map<List<CommonListViewModel>>(getTypeOfServiceList);

            var getMobileList = _countryRepository.CommonCountryMobileCodeList();
            Model.lstMobile = _mapper.Map<List<CommonListViewModel>>(getMobileList);

            Model.MobileCountryCode = userViewModel.MobileCountryCode;
            Model.Mobile = userViewModel.Mobile;
            Model.ImmigrationCountry = userViewModel.ImmigrationCountry;
            Model.TypeOfServiceName = userViewModel.TypeOfServiceName;
            Model.lstTypeOfServiceVal = userViewModel.TypeOfServiceName;

            Model.ReturnCase = "OTPVerify";

            #endregion

            return View(Model);
        }
        public IActionResult UserSignUp()
        {
            UserViewModel userViewModel = new UserViewModel();

            var getCountryListByTypeOfService = _countryRepository.TypeOfServiceCountryList();
            userViewModel.lstCountryByTypeOfService = _mapper.Map<List<CommonListViewModel>>(getCountryListByTypeOfService);

            var getCountryList = _countryRepository.CommonCountryList();
            userViewModel.lstCountry = _mapper.Map<List<CommonListViewModel>>(getCountryList);

            var getLanguageList = _languageRepository.CommonLanguageList();
            userViewModel.lstLanguage = _mapper.Map<List<CommonListViewModel>>(getLanguageList);

            var getTypeOfServiceList = _typeOfServiceRepository.CommonTypeOfServiceList();
            userViewModel.lstTypeOfService = _mapper.Map<List<CommonListViewModel>>(getTypeOfServiceList);

            var getMobileList = _countryRepository.CommonCountryMobileCodeList();
            userViewModel.lstMobile = _mapper.Map<List<CommonListViewModel>>(getMobileList);

            return View(userViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> UserSignUp(UserViewModel userViewModel)
        {
            HttpContext.Session.SetString("NewMobileCountryCode", "");
            HttpContext.Session.SetString("NewMobile", "");
            HttpContext.Session.SetString("UserEmail", "");


            UserViewModel Model = new UserViewModel();

            int checkUserMobileExist = _userRepository.Find(x => x.Mobile == userViewModel.Mobile).Count();
            int checkUserEmailExist = _userRepository.Find(x => x.Email == userViewModel.Email).Count();

            // if google or apple login try to signup
            if (checkUserEmailExist > 0 && checkUserMobileExist == 0)
            {
                var getData = _userRepository.Find(x => x.Email == userViewModel.Email).FirstOrDefault();

                if (!string.IsNullOrEmpty(getData.UserGoogleReturnId) || !string.IsNullOrEmpty(getData.UserAppleReturnId))
                {
                    Model.Message = "Try to do Login with your social account";
                    return View(Model);
                }
            }

            // if guest user try to signup
            if (checkUserMobileExist > 0 && checkUserEmailExist == 0)
            {
                var getData = _userRepository.Find(x => x.Mobile == userViewModel.Mobile).FirstOrDefault();

                if (getData.IsGuest != null && getData.IsGuest == true)
                {

                    #region Update Guest User     

                    User userObj = new User();
                    userObj = getData;
                    userObj.FirstName = userViewModel.FirstName;
                    userObj.LastName = userViewModel.LastName;
                    userObj.Email = userViewModel.Email;
                    userObj.Country = userViewModel.Country;
                    userObj.CommunicationLanguage = userViewModel.CommunicationLanguage;
                    userObj.TypeOfServiceName = userViewModel.TypeOfServiceName;
                    userObj.ImmigrationCountry = userViewModel.ImmigrationCountry;
                    userObj.DeviceType = "web";
                    userObj.UserTypeVal = (int)MasterEnum.EUserType.User;
                    string profilePicURL = await UploadFileAWS(userViewModel.imageUploadAWS);
                    userObj.ProfilePic = profilePicURL;
                    userObj.IsVerified = true;
                    userObj.IsGuest = false;
                    Guid guid = Guid.NewGuid();
                    string guidString = guid.ToString("N").Substring(0, 16).Replace("-", "");
                    userObj.EmailVerificationToken = guidString;

                    _userRepository.Update(userObj);

                    #endregion

                    #region Send Verification Code Mail

                    var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.PathBase}";
                    string url = baseUrl.ToString() + "/UserAccount/EmailVerification?code=" + guidString;

                    EmailTemplateMaker emailTemplateMaker = new EmailTemplateMaker(_hostingEnvironment);
                    string[] paramArray;
                    paramArray = new string[2] { getData.FirstName, url };

                    var emailConfirmationTemplate = emailTemplateMaker.GetTemplate("sendEmailVerificationLink", "UserEmailConfirmation.htm", paramArray);

                    Common.SendMail("Advenuss Email Verification Link", emailConfirmationTemplate, getData.Email);

                    #endregion

                    #region Welcome Email

                    EmailTemplateMaker emailTemplateMakerWelcome = new EmailTemplateMaker(_hostingEnvironment);
                    string[] paramArrayWelcome;
                    string userName = userViewModel.FirstName + " " + userViewModel.LastName;
                    paramArrayWelcome = new string[1] { userName };
                    var emailTemplateMakerData = emailTemplateMaker.GetTemplate("welcomeUser", "WelcomeEmail.html", paramArrayWelcome);

                    Common.SendMail("Welcome", emailTemplateMakerData, getData.Email);

                    #endregion

                    TempData["AlertMessageSuccess"] = "We just sent you verification link on your mail " + getData.Email + " Please verify.";

                    return RedirectToAction("Login", "UserAccount");
                }
            }

            // if new user try to signup
            if ((checkUserMobileExist + checkUserEmailExist) == 0)
            {

                string receiverMobileNumber = userViewModel.MobileCountryCode + userViewModel.Mobile;
                string OTP = Common.GenerateOTP();

                int countryId = userViewModel.Country;
                Country country = _countryRepository.GetById(countryId);

                string countryName = country.Name;

                string abbreviatedCountryName = countryName.Substring(0, Math.Min(3, countryName.Length)).ToUpper();

                string alphanumericString = GenerateRandomNumericString(7);

                string uniqueUserId = abbreviatedCountryName + alphanumericString;


                User userObj = new User();
                userObj.FirstName = userViewModel.FirstName;
                userObj.LastName = userViewModel.LastName;
                userObj.Email = userViewModel.Email;
                userObj.MobileCountryCode = userViewModel.MobileCountryCode;
                userObj.Mobile = userViewModel.Mobile;
                userObj.Country = userViewModel.Country;
                userObj.CommunicationLanguage = userViewModel.CommunicationLanguage;
                userObj.TypeOfServiceName = userViewModel.TypeOfServiceName;
                userObj.ImmigrationCountry = userViewModel.ImmigrationCountry;
                string profilePicURL = await UploadFileAWS(userViewModel.imageUploadAWS);
                userObj.ProfilePic = profilePicURL;
                userObj.DeviceType = "web";
                userObj.Otp = OTP;
                userObj.UserTypeVal = (int)MasterEnum.EUserType.User;
                userObj.LoginAttempt = 1;
                userObj.OtpDate = DateTime.UtcNow;
                userObj.CreatedOn = DateTime.UtcNow;
                userObj.UniqueId = uniqueUserId;


                int returnId = _userRepository.AddUser(userObj);

                HttpContext.Session.SetString("GUID", returnId.ToString());
                HttpContext.Session.SetString("MobileCountryCode", userViewModel.MobileCountryCode);
                HttpContext.Session.SetString("Mobile", Convert.ToString(userViewModel.Mobile));
                HttpContext.Session.SetString("OTPRequestTime", Convert.ToString(DateTime.UtcNow));
                HttpContext.Session.SetString("CurrentUserCountryName", Convert.ToString(userViewModel.Country));

                string msgBody = "OTP for ADVENUSS is " + OTP + " Do not share it with anyone by any means.";
                Send.SMS(receiverMobileNumber, msgBody);


                #region Welcome Email

                EmailTemplateMaker emailTemplateMaker = new EmailTemplateMaker(_hostingEnvironment);
                string[] paramArray;
                string userName = userViewModel.FirstName + " " + userViewModel.LastName;
                paramArray = new string[1] { userName };
                var welcomeUserTemplate = emailTemplateMaker.GetTemplate("welcomeUser", "WelcomeEmail.html", paramArray);
                Common.SendMail("Welcome", welcomeUserTemplate, userViewModel.Email);

                #endregion

                TempData["AlertMessageSuccess"] = "We just sent you OTP on your " + receiverMobileNumber + " mobile number Please enter it now.";
                return RedirectToAction("OTPVerification");
            }
            else
            {
                if (checkUserMobileExist > 0)
                    Model.MobileErrMsg = "Mobile number already exist.";
                if (checkUserEmailExist > 0)
                    Model.EmailErrMsg = "Email already exist.";

            }

            Model.FirstName = userViewModel.FirstName;
            Model.LastName = userViewModel.LastName;
            Model.Email = userViewModel.Email;
            Model.Mobile = userViewModel.Mobile;

            var getCountryListByTypeOfService = _countryRepository.TypeOfServiceCountryList();
            Model.lstCountryByTypeOfService = _mapper.Map<List<CommonListViewModel>>(getCountryListByTypeOfService);
            Model.lstCountryByTypeOfServiceVal = userViewModel.ImmigrationCountry;

            var getCountryList = _countryRepository.CommonCountryList();
            Model.lstCountry = _mapper.Map<List<CommonListViewModel>>(getCountryList);
            Model.lstCountryVal = userViewModel.Country;

            var getLanguageList = _languageRepository.CommonLanguageList();
            Model.lstLanguage = _mapper.Map<List<CommonListViewModel>>(getLanguageList);
            Model.lstLanguageVal = userViewModel.CommunicationLanguage;

            var getTypeOfServiceList = _typeOfServiceRepository.CommonTypeOfServiceList();
            Model.lstTypeOfService = _mapper.Map<List<CommonListViewModel>>(getTypeOfServiceList);
            Model.lstTypeOfServiceVal = userViewModel.TypeOfServiceName;

            var getMobileList = _countryRepository.CommonCountryMobileCodeList();
            Model.lstMobile = _mapper.Map<List<CommonListViewModel>>(getMobileList);
            Model.lstMobileVal = userViewModel.MobileCountryCode;

            return View(Model);
        }

        [HttpPost]
        public JsonResult GetServiceByCountry(int countryId)
        {

            var getTypeOfServiceList = _typeOfServiceRepository.CommonTypeOfServiceList().Where(x => x.CountryId == countryId).ToList();

            return Json(getTypeOfServiceList);
        }
        public ActionResult OTPVerification()
        {

            OTPVerificationViewModel Model = new OTPVerificationViewModel();

            string GUID = Convert.ToString(HttpContext.Session.GetString("GUID"));

            if (GUID == "" || GUID == null)
            {
                TempData["AlertMessageFail"] = "Invalid Process";
                return RedirectToAction("Login");
            }
            return View(Model);

            //if (isUserAllowed()==true)                                       
            //else            
            //    return RedirectToAction("Login");
        }
        [HttpPost]
        public JsonResult OTPVerification(string OTP, string UserEmail, string NewMobileCountryCode, string NewMobile)
        {

            OTPVerificationViewModel Model = new OTPVerificationViewModel();

            string OTPRequestTime = HttpContext.Session.GetString("OTPRequestTime");
            string sessionGUID = Convert.ToString(HttpContext.Session.GetString("GUID"));

            if (!string.IsNullOrEmpty(OTPRequestTime) || !string.IsNullOrEmpty(sessionGUID))
            {
                DateTime dtRequestTime = Convert.ToDateTime(OTPRequestTime);
                DateTime dtCurrentDateTime = DateTime.UtcNow;

                TimeSpan timeSpan = dtCurrentDateTime - Convert.ToDateTime(dtRequestTime);
                int duration = timeSpan.Minutes * 60 + timeSpan.Seconds;

                if (duration < Convert.ToInt32(OTPValidity))
                {
                    var getUser = Common.VerifyOTP(sessionGUID);


                    if (getUser != null && !string.IsNullOrEmpty(UserEmail))
                    {
                        if (getUser.EmailVerificationToken == OTP)
                        {
                            HttpContext.Session.SetString("CurrentUserId", sessionGUID);
                            Guid guid = Guid.NewGuid();
                            string guidString = guid.ToString("N").Substring(0, 16).Replace("-", "");
                            HttpContext.Session.SetString("RandomNumber", Convert.ToString(guidString));
                            //TempData["AlertMessageSuccess"] = "OTP verify successfull.";
                            return Json("ChangeMobileTrue");
                        }
                        return Json("OTP does not match.");
                    }
                    else
                    {
                        if (getUser != null && getUser.Otp == OTP)
                        {
                            //string folderpath = @"h:\root\home\iqlance-001\www\immiweb-dev\logs\";
                            //string logFilePath = Path.Combine(folderpath, "log.txt");

                            //using (StreamWriter writer = new StreamWriter(logFilePath))
                            //{
                            //    writer.WriteLine($"{DateTime.Now}: {DateTime.UtcNow.Day}");
                            //}

                            bool isToday = Convert.ToDateTime(getUser.OtpDate).Day == DateTime.UtcNow.Day;

                            if (isToday == true)
                            {
                                //if (getUser.LoginAttempt <= OTPAttemptVal)
                                //{
                                // for the guest user and normal user
                                if ((getUser.IsGuest == true) || (getUser.IsVerified == true && getUser.EmailConfirmed == true && getUser.IsActive == true && getUser.IsRegistered == true))
                                {
                                    // if otp process for add new mobile
                                    if (!string.IsNullOrEmpty(NewMobileCountryCode) && !string.IsNullOrEmpty(NewMobile))
                                    {
                                        //NewMobileCountryCode = "+" + NewMobileCountryCode;

                                        User user = new User();
                                        user = getUser;
                                        user.MobileCountryCode = NewMobileCountryCode.Replace(" ", "");
                                        user.Mobile = NewMobile;
                                        _userRepository.Update(user);

                                        //TempData["AlertMessageSuccess"] = "Mobile number updated successfully";

                                        HttpContext.Session.SetString("NewMobileCountryCode", "");
                                        HttpContext.Session.SetString("NewMobile", "");

                                        return Json("NewMobileAdded");
                                    }
                                    else
                                    {
                                        // if otp process for login
                                        #region Authentication

                                        var identity = new ClaimsIdentity(new[]
                                        {
                                            new Claim(ClaimTypes.MobilePhone, getUser.Mobile),
                                            new Claim(ClaimTypes.Role, MasterEnum.EUserType.User.ToString())
                                            }, CookieAuthenticationDefaults.AuthenticationScheme);

                                        var principal = new ClaimsPrincipal(identity);
                                        HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                                        SessionFactory.CurrentUserId = getUser.Id;
                                        SessionFactory.CurrentUserName = getUser.FirstName + " " + getUser.LastName;
                                        SessionFactory.CurrentUserCountryId = Convert.ToInt32(getUser.Country);
                                        SessionFactory.CurrentUserMobile = getUser.Mobile;
                                        SessionFactory.CurrentUserMobileCountryCode = Convert.ToInt32(getUser.MobileCountryCode);

                                        if (!string.IsNullOrEmpty(getUser.Email))
                                            SessionFactory.CurrentUserEmail = getUser.Email;
                                        if (!string.IsNullOrEmpty(getUser.ProfilePic))
                                            SessionFactory.CurrentUserProfilePic = getUser.ProfilePic;

                                        //TempData["AlertMessageSuccess"] = "Login successfully";

                                        return Json("True");

                                        #endregion
                                    }
                                }
                                else
                                {
                                    // if user do not did email verification process
                                    if ((getUser.IsGuest == false || getUser.IsGuest == null) && (getUser.EmailConfirmed == false || getUser.EmailConfirmed == null))
                                    {
                                        Guid guid = Guid.NewGuid();
                                        string guidString = guid.ToString("N").Substring(0, 16).Replace("-", "");
                                        var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.PathBase}";
                                        string url = baseUrl.ToString() + "/UserAccount/EmailVerification?code=" + guidString;

                                        User userObj = new User();
                                        userObj = getUser;
                                        userObj.IsVerified = true;
                                        userObj.EmailVerificationToken = guidString;
                                        _userRepository.Update(userObj);

                                        #region Email Verification Link

                                        EmailTemplateMaker emailTemplateMaker = new EmailTemplateMaker(_hostingEnvironment);
                                        string[] paramArray;
                                        string imagePath = string.Empty;
                                        paramArray = new string[2] { imagePath, url };

                                        var emailConfirmationTemplate = emailTemplateMaker.GetTemplate("sendEmailVerificationLink", "VerifyEmail.html", paramArray);
                                        Common.SendMail("Advenuss Email Verification Link", emailConfirmationTemplate, getUser.Email);

                                        TempData["AlertMessageSuccess"] = "Your email verification is pending. We send a mail on your registered email address. Please verify your email to login";
                                        return Json("EmailNotTrue");

                                        #endregion
                                    }
                                    else
                                    {
                                        return Json("User does not exist.");
                                    }
                                }
                                //}
                                //return Json("Your today's attempt is over");
                            }
                            return Json("Records not found");
                        }
                        return Json("OTP does not match.");
                    }
                }
                return Json("OTP is expired.");
            }
            return Json("OTP session expired.");
        }
        [HttpPost]
        public JsonResult ResendOtp(string GUID, string UserEmail, string NewMobileCountryCode, string NewMobile)
        {
            string returnMsg = "";
            string OTP = Common.GenerateOTP();

            if (!string.IsNullOrEmpty(GUID))
            {
                var getUser = _userRepository.Find(x => x.Id == Convert.ToInt32(GUID)).FirstOrDefault();

                if (getUser != null)
                {
                    if (UserEmail != null)
                    {
                        User userObj = new User();
                        userObj = getUser;
                        userObj.EmailVerificationToken = OTP;
                        userObj.OtpDate = DateTime.UtcNow;
                        _userRepository.Update(userObj);

                        HttpContext.Session.SetString("OTPRequestTime", Convert.ToString(DateTime.UtcNow));
                        HttpContext.Session.SetString("GUID", Convert.ToString(getUser.Id));
                        HttpContext.Session.SetString("UserEmail", Convert.ToString(getUser.Email));

                        EmailTemplateMaker emailTemplateMaker = new EmailTemplateMaker(_hostingEnvironment);
                        string[] paramArray;
                        string imagepath = "";
                        paramArray = new string[2] { OTP, imagepath };
                        var emailConfirmationTemplate = emailTemplateMaker.GetTemplate("EmailOTPVerification", "VerificationCode.html", paramArray);

                        Common.SendMail("Advenuss Email OTP Link", emailConfirmationTemplate, getUser.Email);

                        returnMsg = "True";
                    }
                    else
                    {
                        bool isToday = Convert.ToDateTime(getUser.OtpDate).Day == DateTime.UtcNow.Day;

                        if (isToday && getUser.LoginAttempt <= OTPAttemptVal)
                        {
                            string mobileNumber = string.Empty;

                            if (!string.IsNullOrEmpty(NewMobileCountryCode) && !string.IsNullOrEmpty(NewMobile))
                                mobileNumber = NewMobileCountryCode + NewMobile;
                            else
                                mobileNumber = getUser.MobileCountryCode + getUser.Mobile;

                            string msgBody = "OTP for ADVENUSS is " + OTP + " Do not share it with anyone by any means.";
                            User userObj = getUser;
                            userObj.Otp = OTP;
                            userObj.OtpDate = DateTime.UtcNow;
                            userObj.LoginAttempt = userObj.LoginAttempt + 1;

                            _userRepository.Update(userObj);

                            Send.SMS(mobileNumber.Replace(" ", ""), msgBody);
                            HttpContext.Session.SetString("OTPRequestTime", Convert.ToString(DateTime.UtcNow));

                            returnMsg = "True";
                            return Json(returnMsg);
                        }
                        returnMsg = "You have exceeded the maximum number of OTP request attempts for today.";
                        return Json(returnMsg);
                    }
                }
                else
                    returnMsg = "User does not exist.";
                return Json(returnMsg);
            }
            returnMsg = "User does not exist.";
            return Json(returnMsg);


        }
        public bool isUserAllowed()
        {
            bool isAllowed = false;

            string sessionGUID = Convert.ToString(HttpContext.Session.GetString("GUID"));

            if (!string.IsNullOrEmpty(sessionGUID))
            {
                var getUser = Common.UserSendSMSAttempt(sessionGUID);

                if (getUser != null)
                {
                    bool isToday = Convert.ToDateTime(getUser.OtpDate).Day == DateTime.UtcNow.Day;

                    if (isToday == true && getUser.LoginAttempt <= OTPAttemptVal)
                    {
                        isAllowed = true;
                    }
                    else
                    {
                        TempData["AlertMessageFail"] = "You have exceeded the maximum number of OTP request attempts for today.";
                        isAllowed = false;
                    }
                }
                else
                {
                    TempData["AlertMessageFail"] = "Invalid Process";
                    isAllowed = false;
                }
            }
            else
            {
                TempData["AlertMessageFail"] = "Invalid Process";
                isAllowed = false;
            }
            return isAllowed;
        }
        public ActionResult EmailVerification(string code)
        {
            EmailVerificationViewModel Model = new EmailVerificationViewModel();
            Model.MobileMsg = string.Empty;

            string mobileStausMsg = string.Empty;
            string emailStausMsg = string.Empty;

            var getCode = _userRepository.Find(x => x.EmailVerificationToken == code.Trim()).FirstOrDefault();

            if (getCode != null && (getCode.EmailConfirmed == false || getCode.EmailConfirmed == null))
            {
                User userObj = new User();
                userObj = getCode;
                userObj.EmailConfirmed = true;
                userObj.EmailVerificationToken = string.Empty;
                if (getCode.IsVerified == true)
                {
                    userObj.IsRegistered = true;
                    userObj.IsActive = true;
                }

                if(getCode.IsGuest==true)
                {
                    getCode.IsGuest = false;
                }

                _userRepository.Update(userObj);

                Model.Status = "True";
                emailStausMsg = "Your email verification is successfull.";
            }
            else
            {
                Model.Status = "False";
                emailStausMsg = "Your email verification link is expired.";
            }

            Model.Msg = mobileStausMsg + emailStausMsg;
            return View(Model);
        }
        public ActionResult NewEmailVerification(string code, string newEmail)
        {
            EmailVerificationViewModel Model = new EmailVerificationViewModel();
            Model.MobileMsg = string.Empty;

            string mobileStausMsg = string.Empty;
            string emailStausMsg = string.Empty;

            var getCode = _userRepository.Find(x => x.EmailVerificationToken == code.Trim()).FirstOrDefault();

            if (getCode != null && !string.IsNullOrEmpty(newEmail))
            {
                User userObj = new User();
                userObj = getCode;                
                userObj.EmailVerificationToken = string.Empty;
                userObj.Email = newEmail;
                _userRepository.Update(userObj);

                Model.Status = "True";
                emailStausMsg = "Your new email verification is successfull.";
            }
            else
            {
                Model.Status = "False";
                emailStausMsg = "Your new email verification link is expired.";
            }

            Model.Msg = mobileStausMsg + emailStausMsg;

            return View("~/Views/UserAccount/EmailVerification.cshtml", Model);

        }

        [HttpPost]
        public JsonResult GetExistEmail(string emailString)
        {
            var getExistEmail = _userRepository.Find(x => x.Email == emailString).Count();
            return Json(getExistEmail);
        }
        [HttpPost]
        public JsonResult GetNotExistEmail(string emailString)
        {
            var getNotExistEmail = _userRepository.Find(x => x.Email == emailString).Any();
            return Json(!getNotExistEmail);
        }
        [HttpPost]
        public JsonResult GetExistMobile(string numberString, string codeString)
        {
            var getExistMobile = _userRepository.Find(x => x.Mobile == numberString).Count();
            return Json(getExistMobile);
        }
        [HttpPost]
        public JsonResult GetNotExistMobile(string numberString, string codeString)
        {
            var getNotExistMobile = _userRepository.Find(x => x.Mobile == numberString).Any();
            return Json(!getNotExistMobile);
        }
        public ActionResult Login()
        {
            HttpContext.Session.SetString("GUID", "");

            if (SessionFactory.CurrentUserCountryId > 0)
                return RedirectToAction("Index", "Home");

            LoginViewModel loginViewModel = new LoginViewModel();

            var getMobileList = _countryRepository.CommonCountryMobileCodeList();
            loginViewModel.lstMobile = _mapper.Map<List<CommonListViewModel>>(getMobileList);

            return View(loginViewModel);
        }
        [HttpPost]
        public ActionResult Login(LoginViewModel loginViewModel)
        {

            HttpContext.Session.SetString("NewMobileCountryCode", "");
            HttpContext.Session.SetString("NewMobile", "");
            HttpContext.Session.SetString("UserEmail", "");


            LoginViewModel Model = new LoginViewModel();
            Model.MobileCountryCode = loginViewModel.MobileCountryCode;
            Model.Mobile = loginViewModel.Mobile;


            var getMobileList = _countryRepository.CommonCountryMobileCodeList();
            Model.lstMobile = _mapper.Map<List<CommonListViewModel>>(getMobileList);

            if (!string.IsNullOrEmpty(loginViewModel.Mobile) && !string.IsNullOrEmpty(loginViewModel.MobileCountryCode))
            {
                var getUser = _userRepository.Find(x => x.Mobile == loginViewModel.Mobile.Trim() &&
                x.MobileCountryCode == loginViewModel.MobileCountryCode.Trim()).FirstOrDefault();

                if (getUser != null)
                {
                    bool isToday = Convert.ToDateTime(getUser.OtpDate).Day == DateTime.UtcNow.Day;

                    if (isToday == false || (isToday && getUser.LoginAttempt <= OTPAttemptVal))
                    {
                        // user do not verify mobile
                        if (getUser.IsVerified == false || getUser.IsVerified == null)
                        {
                            HttpContext.Session.SetString("OTPProcessFor", "SignUp");
                            string OTP = Common.GenerateOTP();
                            string msgBody = "OTP for ADVENUSS is " + OTP + " Do not share it with anyone by any means.";

                            User userObj = new User();
                            userObj = getUser;
                            userObj.Otp = OTP;
                            userObj.OtpDate = DateTime.UtcNow;
                            userObj.LoginAttempt = userObj.LoginAttempt + 1;
                            if (isToday == false)
                            {
                                userObj.LoginAttempt = 0;
                            }
                            _userRepository.Update(userObj);

                            HttpContext.Session.SetString("GUID", getUser.Id.ToString());
                            HttpContext.Session.SetString("MobileCountryCode", getUser.MobileCountryCode);
                            HttpContext.Session.SetString("Mobile", Convert.ToString(getUser.Mobile));
                            HttpContext.Session.SetString("OTPRequestTime", Convert.ToString(DateTime.UtcNow));

                            string receiverMobileNumber = getUser.MobileCountryCode + getUser.Mobile;
                            Send.SMS(receiverMobileNumber, msgBody);

                            return RedirectToAction("OTPVerification", "UserAccount");
                        }

                        // user do not verify email
                        if ((getUser.IsGuest == null || getUser.IsGuest == false) && (getUser.EmailConfirmed == false || getUser.EmailConfirmed == null))
                        {
                            HttpContext.Session.SetString("OTPProcessFor", "SignUp");

                            Guid guid = Guid.NewGuid();
                            string guidString = guid.ToString("N").Substring(0, 16).Replace("-", "");
                            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.PathBase}";
                            string url = baseUrl.ToString() + "/UserAccount/EmailVerification?code=" + guidString;

                            User userObj = new User();
                            userObj = getUser;
                            userObj.EmailVerificationToken = guidString;
                            _userRepository.Update(userObj);

                            EmailTemplateMaker emailTemplateMaker = new EmailTemplateMaker(_hostingEnvironment);
                            string[] paramArray;
                            string imagePath = string.Empty;
                            paramArray = new string[2] { imagePath, url };

                            var emailConfirmationTemplate = emailTemplateMaker.GetTemplate("sendEmailVerificationLink", "VerifyEmail.html", paramArray);
                            Common.SendMail("Advenuss Email Verification Link", emailConfirmationTemplate, getUser.Email);

                            TempData["AlertMessageSuccess"] = "We just sent you verfication link on your mail " + getUser.Email + " Please verify.";
                            return View(Model);
                        }

                        // user verify both
                        if (getUser.IsGuest == true || (getUser.IsActive == true && getUser.EmailConfirmed == true && getUser.IsVerified == true && getUser.IsRegistered == true))
                        {
                            string OTP = Common.GenerateOTP();
                            string msgBody = "OTP for ADVENUSS is " + OTP + " Do not share it with anyone by any means.";

                            User userObj = new User();
                            userObj = getUser;
                            userObj.Otp = OTP;
                            userObj.OtpDate = DateTime.UtcNow;
                            userObj.LoginAttempt = userObj.LoginAttempt + 1;
                            if (userObj.LoginAttempt == null)
                            {
                                userObj.LoginAttempt = 1;
                            }
                            if (isToday == false && (getUser.IsGuest == false || getUser.IsGuest == null))
                            {
                                userObj.LoginAttempt = 0;
                            }
                            _userRepository.Update(userObj);

                            HttpContext.Session.SetString("GUID", getUser.Id.ToString());
                            HttpContext.Session.SetString("MobileCountryCode", getUser.MobileCountryCode);
                            HttpContext.Session.SetString("Mobile", Convert.ToString(getUser.Mobile));
                            HttpContext.Session.SetString("OTPRequestTime", Convert.ToString(DateTime.UtcNow));

                            string receiverMobileNumber = getUser.MobileCountryCode + getUser.Mobile;
                            Send.SMS(receiverMobileNumber, msgBody);

                            TempData["AlertMessageSuccess"] = "We just sent you OTP on your mobile number " + receiverMobileNumber + " Please enter it now.";

                            //Open the Comment Code For Notification


                            var list = _userRepository.GetNotificationMasters(userObj.Id);
                            if (list.Count == 0)
                            {

                                var UserId = userObj.Id;
                                var SocialCountryForService = userObj.ImmigrationCountry ?? 0;
                                var SocialTypeOfService = userObj.TypeOfServiceName ?? 0;
                                SendNotificationtoAllConsultant(UserId, SocialTypeOfService, SocialCountryForService);
                            }

                            return RedirectToAction("OTPVerification", "UserAccount");
                        }
                        else
                        {
                            Model.Message = "Your profile is inactivated, please contact to the admin to activate it.";
                        }
                    }
                    else
                    {
                        Model.Message = "You have exceeded the maximum number of OTP request attempts for today.";
                    }
                }
                else
                {
                    Model.Message = "Mobile number does not exist.";
                }
            }
            return View(Model);
        }
        public IActionResult LogOut()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);


            SessionFactory.CurrentUserId = 0;
            SessionFactory.CurrentUserName = string.Empty;
            SessionFactory.CurrentUserEmail = string.Empty;
            SessionFactory.CurrentUserCountryId = 0;
            SessionFactory.CurrentUserMobile = string.Empty;
            SessionFactory.CurrentUserProfilePic = string.Empty;
            SessionFactory.CurrentUserMobileCountryCode = 0;

            //TempData["AlertMessageSuccess"] = "Logout successfully";
            return RedirectToAction("Login", "UserAccount");

        }

        #region Change Mobile

        public ActionResult ChangeMobile()
        {
            ChangeMobileViewModel Model = new ChangeMobileViewModel();
            var getMobileList = _countryRepository.CommonCountryMobileCodeList();
            Model.lstMobile = _mapper.Map<List<CommonListViewModel>>(getMobileList);
            return View(Model);
        }

        [HttpPost]
        public ActionResult ChangeMobile(ChangeMobileViewModel changeMobileViewModel, string emailString, string numberString, string codeString)
        {
            ChangeMobileViewModel Model = new ChangeMobileViewModel();

            var getMobileList = _countryRepository.CommonCountryMobileCodeList();
            Model.lstMobile = _mapper.Map<List<CommonListViewModel>>(getMobileList);

            Model.Mobile = changeMobileViewModel.Mobile;
            Model.Email = changeMobileViewModel.Email;
            Model.lstMobileVal = changeMobileViewModel.MobileCountryCode;

            if (!string.IsNullOrEmpty(changeMobileViewModel.Mobile) && !string.IsNullOrEmpty(changeMobileViewModel.Email))
            {
                var getUser = _userRepository.Find(x => x.Mobile == changeMobileViewModel.Mobile.Trim() &&
                x.MobileCountryCode == changeMobileViewModel.MobileCountryCode).FirstOrDefault();

                var getExistEmail = _userRepository.Find(x => x.Email == changeMobileViewModel.Email
                && x.IsVerified == true && x.IsRegistered == true && x.EmailConfirmed == true).FirstOrDefault();

                if (getUser != null && getExistEmail != null   )
                {
                    if(getUser.Id == getExistEmail.Id)
                    {
                        if (getUser.IsActive == true)
                        {
                            string OTP = Common.GenerateOTP();

                            User userObj = new User();
                            userObj = getUser;
                            userObj.EmailVerificationToken = OTP;
                            userObj.OtpDate = DateTime.UtcNow;
                            _userRepository.Update(userObj);

                            HttpContext.Session.SetString("GUID", Convert.ToString(getUser.Id));
                            HttpContext.Session.SetString("UserEmail", Convert.ToString(getUser.Email));


                            EmailTemplateMaker emailTemplateMaker = new EmailTemplateMaker(_hostingEnvironment);
                            string[] paramArray;
                            string imagepath = "";
                            paramArray = new string[2] { OTP, imagepath };
                            var emailConfirmationTemplate = emailTemplateMaker.GetTemplate("EmailOTPVerification", "VerificationCode.html", paramArray);

                            Common.SendMail("Advenuss Email OTP Link", emailConfirmationTemplate, changeMobileViewModel.Email);
                            HttpContext.Session.SetString("OTPRequestTime", Convert.ToString(DateTime.UtcNow));

                            TempData["AlertMessageSuccess"] = "We just sent you OTP on your mail " + getUser.Email + " Please enter it now.";
                            return RedirectToAction("OTPVerification", "UserAccount");
                        }
                        else
                        {
                            Model.ErrorMsg = "Your profile is inactivated, please contact to the admin to activate it.";
                        }
                    }
                    else
                    {                        
                            Model.EmailErrorMsg = "Email not exist.";                        
                    }                   
                }
                else
                {
                    if (getUser == null)
                    {
                        Model.MobileErrorMsg = "Mobile number not exist.";
                    }

                    if (getExistEmail == null)
                    {
                        Model.EmailErrorMsg = "Email not exist.";
                    }                    
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(changeMobileViewModel.Mobile))
                    Model.ErrorMsg = "Mobile number is required.";
                else
                    Model.ErrorMsg = "Email is required.";
            }
            return View(Model);
        }

        public ActionResult AddNewMobile()
        {
            string RandomNumber = HttpContext.Session.GetString("RandomNumber");
            string CurrentUserId = HttpContext.Session.GetString("CurrentUserId");


            if (string.IsNullOrEmpty(RandomNumber) || string.IsNullOrEmpty(CurrentUserId))
            {
                TempData["AlertMessageFail"] = "Unauthorized access";
                return RedirectToAction("Login", "UserAccount");
            }

            ChangeMobileViewModel Model = new ChangeMobileViewModel();

            Model.RandomNumber = RandomNumber;
            var getMobileList = _countryRepository.CommonCountryMobileCodeList();
            Model.lstMobile = _mapper.Map<List<CommonListViewModel>>(getMobileList);


            return View(Model);
        }

        [HttpPost]
        public ActionResult AddNewMobile(ChangeMobileViewModel changeMobileViewModel)
        {
            ChangeMobileViewModel Model = new ChangeMobileViewModel();

            string RandomNumber = HttpContext.Session.GetString("RandomNumber");
            string CurrentUserId = HttpContext.Session.GetString("CurrentUserId");

            var getCurrentUser = _userRepository.Find(x => x.Id == Convert.ToInt32(CurrentUserId)).FirstOrDefault();

            if (string.IsNullOrEmpty(RandomNumber))
            {
                TempData["AlertMessageFail"] = "Unauthorized access";
                return RedirectToAction("Login", "UserAccount");
            }
            if (getCurrentUser == null && !string.IsNullOrEmpty(getCurrentUser.Email))
            {
                TempData["AlertMessageFail"] = "Unauthorized access";
                return RedirectToAction("Login", "UserAccount");
            }

            if (RandomNumber != changeMobileViewModel.RandomNumber)
            {
                TempData["AlertMessageFail"] = "Unauthorized access";
                return RedirectToAction("Login", "UserAccount");
            }

            if (string.IsNullOrEmpty(changeMobileViewModel.NewMobile))
            {
                Model.ErrorMsg = "Mobile number is required.";
                return View(Model);
            }

            Model.RandomNumber = changeMobileViewModel.RandomNumber;

            var checkUserMobileExist = _userRepository.Find(x => x.Mobile == changeMobileViewModel.NewMobile
            && x.Email == getCurrentUser.Email).FirstOrDefault();

            if (checkUserMobileExist == null)
            {
                string OTP = Common.GenerateOTP();
                string mobileNum = changeMobileViewModel.NewMobileCountryCode + changeMobileViewModel.NewMobile;

                User userObj = new User();
                userObj = getCurrentUser;
                userObj.Otp = OTP;
                userObj.OtpDate = DateTime.UtcNow;
                _userRepository.Update(userObj);
                string msgBody = "OTP for ADVENUSS is " + OTP + " Do not share it with anyone by any means.";
                Send.SMS(mobileNum, msgBody);

                TempData["AlertMessageSuccess"] = "We just sent you OTP on your mobile number " + mobileNum + " Please enter it now.";

                HttpContext.Session.SetString("UserEmail", "");
                HttpContext.Session.SetString("NewMobileCountryCode", changeMobileViewModel.NewMobileCountryCode);
                HttpContext.Session.SetString("NewMobile", changeMobileViewModel.NewMobile);
                HttpContext.Session.SetString("GUID", Convert.ToString(getCurrentUser.Id));
                HttpContext.Session.SetString("OTPRequestTime", Convert.ToString(DateTime.UtcNow));
                return RedirectToAction("OTPVerification");
            }
            else
                Model.ErrorMsg = "New mobile number is already exist";


            var getMobileList = _countryRepository.CommonCountryMobileCodeList();
            Model.lstMobile = _mapper.Map<List<CommonListViewModel>>(getMobileList);
            Model.NewMobile = changeMobileViewModel.NewMobile;
            Model.lstMobileVal = changeMobileViewModel.NewMobileCountryCode;

            return View(Model);
        }


        #endregion

        [HttpPost]
        public async Task<string> UploadFileAWS(IFormFile file)
        {
            string newUrl = string.Empty;

            if (file == null || file.Length == 0)
            {
                return newUrl;
            }
            string myBucketName = "guardian-temp"; // your S3 bucket name goes here
            //string s3DirectoryName = "";
            string s3FileName = file.FileName;

            var AWSAccessKey = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AWSAccessKey")["Key"];
            var AWSSecretKey = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AWSSecretKey")["Key"];
            IAmazonS3 client = new AmazonS3Client(AWSAccessKey, AWSSecretKey, RegionEndpoint.USEast2);
            var request = new PutObjectRequest()
            {
                BucketName = myBucketName,
                Key = s3FileName,
                InputStream = file.OpenReadStream()
            };

            request.Metadata.Add("Content-Type", file.ContentType);
            PutObjectResponse returnvalue = await client.PutObjectAsync(request);

            if (returnvalue.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                var urlRequest = new GetPreSignedUrlRequest()
                {
                    BucketName = myBucketName,
                    Key = s3FileName,
                    Protocol = Protocol.HTTPS,
                    Expires = DateTime.UtcNow.AddMinutes(20)
                };
                var PresignedUrl = client.GetPreSignedURL(urlRequest);
                newUrl = PresignedUrl;

                int index = PresignedUrl.LastIndexOf(file.FileName); // Find the last index of ".jpg"

                if (index >= 0)
                {
                    newUrl = PresignedUrl.Substring(0, index) + file.FileName;
                }
            }
            return newUrl;
        }
        [HttpGet]
        public IActionResult GoogleSignIn()
        {
            var redirectUrl = Url.Action("GoogleSignInCallback", "UserAccount", null, Request.Scheme);
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };

            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }
        [HttpGet]
        public async Task<IActionResult> GoogleSignInCallback()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync();

            if (authenticateResult.Succeeded)
            {
                var claimsPrincipal = authenticateResult.Principal;

                // Retrieve email ID
                var emailId = claimsPrincipal.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");


                int checkUserEmailExist = _userRepository.Find(x => x.Email == emailId && x.Mobile != null
                ).Count();

                if (checkUserEmailExist > 0)
                {
                    TempData["AlertMessageFail"] = "Email already exist.";
                    return RedirectToAction("Login", "UserAccount");
                }


                // Retrieve Google UID
                var googleUid = claimsPrincipal.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");


                // Retrieve firstName
                var firstname = claimsPrincipal.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname");

                // Retrieve lastName
                var lastname = claimsPrincipal.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname");


                var accessToken = authenticateResult.Properties.GetTokenValue("access_token");
                var profileImageUrl = await GetUserProfileImageAsync(accessToken);

                int returnId = 0;

                // Use the retrieved values as needed
                Console.WriteLine($"Google UID: {googleUid}");
                Console.WriteLine($"Email ID: {emailId}");
                Console.WriteLine($"FirstName: {firstname}");
                Console.WriteLine($"LastName: {lastname}");


                var findGoogleUser = _userRepository.Find(x => x.UserGoogleReturnId == googleUid).FirstOrDefault();
                if (findGoogleUser == null)
                {
                    User userObj = new User();
                    userObj.FirstName = firstname;
                    userObj.LastName = lastname;
                    userObj.Email = emailId;
                    userObj.UserSignUpType = "google";
                    userObj.SocialUid = googleUid;
                    userObj.DeviceType = "Web";
                    userObj.UserGoogleReturnId = googleUid;
                    userObj.CreatedOn = DateTime.UtcNow;
                    userObj.IsActive = true;
                    userObj.UserTypeVal = 2;
                    userObj.ProfilePic = profileImageUrl;
                    returnId = _userRepository.AddUser(userObj);
                }

                var getUserData = _userRepository.Find(x => x.UserGoogleReturnId == googleUid).FirstOrDefault();

                #region Authentication

                var identity = new ClaimsIdentity(new[]{
                                //new Claim(ClaimTypes.Name, name),
                                new Claim(ClaimTypes.Sid, googleUid),
                                new Claim(ClaimTypes.Role, MasterEnum.EUserType.User.ToString())
                                }, CookieAuthenticationDefaults.AuthenticationScheme);

                var principal = new ClaimsPrincipal(identity);
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                SessionFactory.CurrentUserId = getUserData.Id;
                SessionFactory.CurrentUserName = getUserData.FirstName + getUserData.LastName;
                //SessionFactory.CurrentUserEmail = emailId;

                //TempData["AlertMessageSuccess"] = "You are login successfully";

                //Open comment code
                var list = _userRepository.GetNotificationMasters(SessionFactory.CurrentUserId);
                if (list.Count == 0)
                {

                    //var UserId = SessionFactory.CurrentUserId;
                    var SocialCountryForService = getUserData.ImmigrationCountry ?? 0;
                    var SocialTypeOfService = getUserData.TypeOfServiceName ?? 0;
                    SendNotificationtoAllConsultant(SessionFactory.CurrentUserId, SocialTypeOfService, SocialCountryForService);
                }
                return RedirectToAction("Index", "Home");

                #endregion

            }
            else
            {
                TempData["AlertMessageFail"] = "Invalid Process";
                return RedirectToAction("Index", "Home");
            }
        }

        private async Task<string> GetUserProfileImageAsync(string accessToken)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                // Use the correct Google People API endpoint to retrieve user information
                var response = await httpClient.GetAsync("https://www.googleapis.com/auth/admin.directory.user");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    // Log or print the content to better understand what is received from the API
                    Console.WriteLine("API Response Content: " + content);

                    // Check if the response is a valid JSON object
                    if (IsJsonObject(content))
                    {
                        try
                        {
                            dynamic userInfo = JObject.Parse(content);
                            // Retrieve the profile image URL from the user information
                            string pictureUrl = userInfo.photos?.FirstOrDefault()?.url;
                            return pictureUrl;
                        }
                        catch (JsonReaderException ex)
                        {
                            Console.WriteLine("Error parsing JSON: " + ex.Message);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid JSON response.");
                    }
                }
                else
                {
                    Console.WriteLine("Error Response Status Code: " + response.StatusCode);
                }
            }

            return null; // If the request fails or the image URL is not available
        }

        private bool IsJsonObject(string content)
        {
            // Check if the content represents a valid JSON object
            return (content.TrimStart().StartsWith("{") && content.TrimEnd().EndsWith("}"));
        }

        [NonAction]
        public void SendNotificationtoAllConsultant(int UserId, int SocialTypeOfService, int SocialCountryForService)
        {
            DbA976eeImmitestContext _dbContext = new DbA976eeImmitestContext();
            List<NotificationModel> objList = new List<NotificationModel>();

            if (UserId != 0 && SocialTypeOfService != 0 && SocialCountryForService != 0)
            {
                var ConsultantIDs = (from d in _dbContext.Consultants
                                     join s in _dbContext.ConsultantTypeOfServices on d.Id equals s.ConsultantId
                                     where s.CountryId == SocialCountryForService && s.TypeOfService == SocialTypeOfService

                                     select new
                                     {
                                         d.Id,
                                         d.DeviceToken,
                                         d.DeviceType


                                     }).ToList();



                if (ConsultantIDs.Count > 0)
                {

                    foreach (var item in ConsultantIDs)
                    {
                        InsertNotificationUserparam paramnot = new InsertNotificationUserparam();
                        paramnot.SenderId = UserId;
                        paramnot.ReceiverId = item.Id;
                        paramnot.Header = "New Login";

                        paramnot.Body = "Their is a new user loggedin in our system that match your services.";
                        paramnot.CreatedOn = Convert.ToDateTime(DateTime.UtcNow);
                        paramnot.SenderUserType = 2;
                        paramnot.ReceiverUserType = 3;
                        paramnot.NotificationTypeName = 4;
                        paramnot.IsAct = true;
                        Send.InsertNotificationUser(paramnot);

                        //var records = _mapper.Map<NotificationMaster>(paramnot);
                        //_dbContext.Add(records);
                        //_dbContext.SaveChanges();

                        NotificationModel obj = Common.SetNotificationModel("New Login", "Their is a new user loggedin in our system that match your services.", item.DeviceToken, item.DeviceType);
                        objList.Add(obj);
                        obj = null;
                    }

                    foreach (var item in objList)
                    {
                        Service.Service.Utility.SendNotification(item);
                    }
                }
            }
        }

        #region Social Login

        //[HttpGet]


        //public IActionResult AppleSignIn()
        //{
        //    TempData["AlertMessageSuccess"] = "You are login successfully";
        //    var redirectUrl = Url.Action("AppleSignInCallback", "UserAccount", null, Request.Scheme);
        //    System.IO.File.AppendAllText("redirectUrl.txt", "redirectUrl " + redirectUrl);
        //    var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
        //    System.IO.File.AppendAllText("properties.txt", "properties " + properties);
        //    request.Metadata.Add("Content-Type", file.ContentType);
        //    PutObjectResponse returnvalue = await client.PutObjectAsync(request);

        //    if (returnvalue.HttpStatusCode == System.Net.HttpStatusCode.OK)
        //    {
        //        var urlRequest = new GetPreSignedUrlRequest()
        //        {
        //            BucketName = myBucketName,
        //            Key = s3FileName,
        //            Protocol = Protocol.HTTPS,
        //            Expires = DateTime.UtcNow.AddMinutes(20)
        //        };
        //        var PresignedUrl = client.GetPreSignedURL(urlRequest);
        //        newUrl = PresignedUrl;

        //        int index = PresignedUrl.LastIndexOf(file.FileName); // Find the last index of ".jpg"

        //        if (index >= 0)
        //        {
        //            newUrl = PresignedUrl.Substring(0, index) + file.FileName;
        //        }
        //    }
        //    return newUrl;
        //    return Challenge(properties, OpenIdConnectDefaults.AuthenticationScheme);
        //}

        //[HttpGet]
        //public async Task<IActionResult> AppleSignInCallback()
        //{
        //    TempData["AlertMessageSuccess"] = "You are login successfully";
        //    var result = await HttpContext.AuthenticateAsync();
        //    System.IO.File.AppendAllText("AppleSignInCallback.txt", "AppleSignInCallback");
        //    if (result.Succeeded)
        //    {
        //        System.IO.File.AppendAllText("Succeeded.txt", "Succeeded");

        //        var claimsPrincipal = result.Principal;

        //        var appleUid = claimsPrincipal.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");

        //        var emailId = claimsPrincipal.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");

        //        var name = claimsPrincipal.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");
        //        int returnId = 0;

        //        var findAppleUser = _userRepository.Find(x => x.UserAppleReturnId == appleUid).FirstOrDefault();
        //        if (findAppleUser == null)
        //        {
        //            User userObj = new User();
        //            userObj.FirstName = name;
        //            userObj.Email = emailId;
        //            userObj.SocialUid = appleUid;
        //            userObj.DeviceType = "Web";
        //            userObj.UserGoogleReturnId = appleUid;
        //            userObj.CreatedOn = DateTime.UtcNow;
        //            userObj.IsActive = true;
        //            userObj.UserTypeVal = 2;
        //            returnId = _userRepository.AddUser(userObj);
        //        }

        //        var getUserData = _userRepository.Find(x => x.UserGoogleReturnId == appleUid).FirstOrDefault();

        //        #region Authentication

        //        var identity = new ClaimsIdentity(new[]{
        //                        new Claim(ClaimTypes.Sid, appleUid),
        //                        new Claim(ClaimTypes.Role, MasterEnum.EUserType.User.ToString())
        //                        }, CookieAuthenticationDefaults.AuthenticationScheme);

        //        var principal = new ClaimsPrincipal(identity);
        //        HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        //        SessionFactory.CurrentUserId = getUserData.Id;
        //        SessionFactory.CurrentUserName = getUserData.FirstName;

        //        #endregion

        //        TempData["AlertMessageSuccess"] = "You are login successfully";
        //        return RedirectToAction("Index", "Home");
        //    }

        //    return RedirectToAction("Index", "Home");
        //}

        #endregion

        #endregion
    }
}
