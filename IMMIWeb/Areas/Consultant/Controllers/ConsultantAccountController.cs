using AutoMapper;
using IMMIWeb.Infrastructure;
using IMMIWeb.Service.Models;
using IMMIWeb.Service.Service.Communication;
using IMMIWeb.Service.Service.Setting;
using IMMIWeb.Service.Service.User;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.CodeModifier.CodeChange;
using RestSharp;
using System.Security.Claims;
using IMMIWeb.Service.Service.Consultant;
using IMMIWeb.Service.Service.General;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Amazon.S3.Model;
using Amazon.S3;
using Amazon;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using IMMIWeb;
using System.Net;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.IO;
using Microsoft.IdentityModel.Tokens;
using System.ServiceModel.Syndication;
using System.Xml;

namespace IMMIWeb.Areas.Consultant.Controllers
{
    [Area("Consultant")]
    public class ConsultantAccountController : Controller
    {

        #region Fields

        private readonly ITypeOfServiceRepository _typeOfServiceRepository;
        private readonly ICountryRepository _countryRepository;
        private readonly IConsultantRepository _consultantRepository;
        private readonly ILanguageRepository _languageRepository;
        private readonly IOtherRepository _otherRepository;
        private readonly IMapper _mapper;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _hostingEnvironment;

        string OTPValidity = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("GeneralInfo")["OTPValidity"];
        int OTPAttemptVal = Convert.ToInt32(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("GeneralInfo")["OTPAttempt"]);
        string stripeClientId = Convert.ToString(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("StripeSettings")["ClientId"]);
        string stripeSecretKey = Convert.ToString(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("StripeSettings")["SecretKey"]);
        string clientEmailAddress = Convert.ToString(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("GeneralInfo")["ClientEmailAddress"]);

        #endregion

        #region Ctor

        public ConsultantAccountController(
            ITypeOfServiceRepository typeOfServiceRepository,
            ICountryRepository countryRepository,
            IConsultantRepository consultantRepository,
            ILanguageRepository languageRepository,
            IOtherRepository otherRepository,
            IMapper mapper,
            Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnvironment)
        {
            _otherRepository = otherRepository;
            _typeOfServiceRepository = typeOfServiceRepository;
            _countryRepository = countryRepository;
            _consultantRepository = consultantRepository;
            _languageRepository = languageRepository;
            _mapper = mapper;
            _hostingEnvironment = hostingEnvironment;
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
                        foreach (var items in feed.Items)
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
        public IActionResult ConsultantSignUp()
        {
            ConsultantViewModel Model = new ConsultantViewModel();

            var getCountryList = _countryRepository.CommonCountryList();
            Model.lstCountry = _mapper.Map<List<CommonListViewModel>>(getCountryList);

            var getMobileList = _countryRepository.CommonCountryMobileCodeList();
            Model.lstMobile = _mapper.Map<List<CommonListViewModel>>(getMobileList);

            var getCountryListByTypeOfService = _countryRepository.TypeOfServiceCountryList();
            Model.lstCountryByTypeOfService = _mapper.Map<List<CommonListViewModel>>(getCountryListByTypeOfService);

            var getLanguageList = _languageRepository.CommonLanguageList();
            Model.lstLanguage = _mapper.Map<List<CommonListViewModel>>(getLanguageList);

            var getinstrucion = _otherRepository.GetAdminInstruction();
            Model.Instruction = getinstrucion;
            return View(Model);
        }
        [HttpPost]
        public async Task<IActionResult> ConsultantSignUp(ConsultantViewModel consultantViewModel)
        {
            ConsultantViewModel Model = new ConsultantViewModel();

            #region Add Consultant


            int checkConsultantMobileExist = _consultantRepository.Find(x => x.Mobile == consultantViewModel.Mobile).Count();
            int checkConsultantEmailExist = _consultantRepository.Find(x => x.Email == consultantViewModel.Email).Count();

            if ((checkConsultantMobileExist + checkConsultantEmailExist) == 0)
            {
                #region Add Consultant


                string OTP = Common.GenerateOTP();
                IMMIWeb.Consultant consultantObj = new IMMIWeb.Consultant();

                int countryId = consultantViewModel.Country;
                Country country = _countryRepository.GetById(countryId);

                string countryName = country.Name;

                string abbreviatedCountryName = countryName.Substring(0, Math.Min(3, countryName.Length)).ToUpper();

                string alphanumericString = GenerateRandomNumericString(7);

                string uniqueUserId = abbreviatedCountryName + alphanumericString;

                consultantObj.FirstName = consultantViewModel.FirstName;
                consultantObj.LastName = consultantViewModel.LastName;
                consultantObj.Email = consultantViewModel.Email;
                consultantObj.MobileCountryCode = consultantViewModel.MobileCountryCode;
                consultantObj.Mobile = consultantViewModel.Mobile;
                consultantObj.LicenceNumber = consultantViewModel.LicenceNumber;
                consultantObj.Country = consultantViewModel.Country;
                consultantObj.RequestRejectionCount = 0;
                consultantObj.CreatedOn = DateTime.UtcNow;
                consultantObj.UserTypeVal = (int)MasterEnum.EUserType.Consultant;
                consultantObj.LoginAttempt = 1;
                consultantObj.Otp = OTP;
                consultantObj.OtpDate = DateTime.UtcNow;
                consultantObj.IsAgreement = true;
                string profilePicURL = await UploadFileAWS(consultantViewModel.imageUploadAWS);
                consultantObj.ProfilePic = profilePicURL;
                consultantObj.UniqueId = uniqueUserId;

                int consultantId = _consultantRepository.AddConsultant(consultantObj);


                #region Service For Country

                ConsultantServiceForCountry consultantServiceForCountry = new ConsultantServiceForCountry();

                consultantServiceForCountry.ConsultantId = consultantId;
                consultantServiceForCountry.Country = consultantViewModel.ImmigrationCountry;
                consultantServiceForCountry.IsActive = true;

                int ConsultantServiceForCountryId = _otherRepository.AddConsultantServiceForCountry(consultantServiceForCountry);
                #endregion


                #region Type Of Service

                List<ConsultantTypeOfService> lstConsultantTypeOfService = new List<ConsultantTypeOfService>();
                foreach (var item in consultantViewModel.lstTypeOfServiceGet)
                {
                    ConsultantTypeOfService consultantTypeOfService = new ConsultantTypeOfService();

                    consultantTypeOfService.ConsultantId = consultantId;
                    consultantTypeOfService.TypeOfService = Convert.ToInt32(item);
                    consultantTypeOfService.IsActive = true;
                    consultantTypeOfService.CountryId = consultantViewModel.ImmigrationCountry;
                    lstConsultantTypeOfService.Add(consultantTypeOfService);
                }
                _otherRepository.AddConsultantTypeOfService(lstConsultantTypeOfService);

                #endregion


                if (consultantViewModel.CommLanguage != null && consultantViewModel.CommLanguage.Count() > 0)
                {
                    string[] comLang = consultantViewModel.CommLanguage.Split(',');
                    List<ConsultantLanguage> lstConsultantLanguage = new List<ConsultantLanguage>();
                    foreach (var item in comLang)
                    {
                        ConsultantLanguage consultantLanguage = new ConsultantLanguage();

                        consultantLanguage.ConsultantId = consultantId;
                        consultantLanguage.Language = Convert.ToInt32(item);
                        consultantLanguage.IsActive = true;
                        lstConsultantLanguage.Add(consultantLanguage);
                    }
                    _otherRepository.AddConsultantLanguage(lstConsultantLanguage);

                }

                #endregion

                #region Send Sms And Welcome Email

                HttpContext.Session.SetString("GUID", consultantId.ToString());
                HttpContext.Session.SetString("MobileCountryCode", consultantViewModel.MobileCountryCode);
                HttpContext.Session.SetString("Mobile", Convert.ToString(consultantViewModel.Mobile));
                HttpContext.Session.SetString("OTPRequestTime", Convert.ToString(DateTime.UtcNow));

                string msgBody = "OTP for ADVENUSS is " + OTP + " Do not share it with anyone by any means.";
                string receiverMobileNumber = consultantViewModel.MobileCountryCode + consultantViewModel.Mobile;
                Send.SMS(receiverMobileNumber, msgBody);

                EmailTemplateMaker emailTemplateMaker = new EmailTemplateMaker(_hostingEnvironment);
                string[] paramArray;
                string userName = consultantViewModel.FirstName + " " + consultantViewModel.LastName;
                paramArray = new string[1] { userName };
                var welcomeUserTemplate = emailTemplateMaker.GetTemplate("welcomeUser", "WelcomeEmail.html", paramArray);
                Common.SendMail("Welcome", welcomeUserTemplate, consultantViewModel.Email);

                TempData["AlertMessageSuccess"] = "We just sent you OTP on your mobile number " + receiverMobileNumber + " Please enter it now.";

                return RedirectToAction("ConsultantOTPVerification");

                #endregion                
            }
            else
            {
                if (checkConsultantMobileExist > 0)
                    Model.MobileErrMsg = "Mobile number already exist.";

                if (checkConsultantEmailExist > 0)
                    Model.EmailErrMsg = "Email address already exist.";

            }

            #endregion


            #region Consultant Bind

            Model.FirstName = consultantViewModel.FirstName;
            Model.LastName = consultantViewModel.LastName;
            Model.Email = consultantViewModel.Email;
            Model.Mobile = consultantViewModel.Mobile;
            Model.LicenceNumber = consultantViewModel.LicenceNumber;
            Model.Country = consultantViewModel.Country;
            Model.ImmigrationCountry = consultantViewModel.ImmigrationCountry;

            var getCountryList = _countryRepository.CommonCountryList();
            Model.lstCountry = _mapper.Map<List<CommonListViewModel>>(getCountryList);

            var getMobileList = _countryRepository.CommonCountryMobileCodeList();
            Model.lstMobile = _mapper.Map<List<CommonListViewModel>>(getMobileList);
            Model.lstMobileVal = consultantViewModel.MobileCountryCode;

            var getCountryListByTypeOfService = _countryRepository.TypeOfServiceCountryList();
            Model.lstCountryByTypeOfService = _mapper.Map<List<CommonListViewModel>>(getCountryListByTypeOfService);

            string strTypeOfService = String.Join(", ", consultantViewModel.lstTypeOfServiceGet);
            Model.TypeOfServiceReturnVal = strTypeOfService;

            var getLanguageList = _languageRepository.CommonLanguageList();
            Model.lstLanguage = _mapper.Map<List<CommonListViewModel>>(getLanguageList);


            Model.ReturnCommLanguage = string.Join(",", consultantViewModel.CommLanguage);

            #endregion

            return View(Model);
        }

        [HttpPost]
        public JsonResult GetServiceByCountry(int countryId)
        {

            var getTypeOfServiceList = _typeOfServiceRepository.CommonTypeOfServiceList().Where(x => x.CountryId == countryId).ToList();

            return Json(getTypeOfServiceList);
        }

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

        [HttpPost]
        public JsonResult ConsultantResendOtp(string GUID, string UserEmail, string NewMobileCountryCode, string NewMobile)
        {
            string returnMsg = "";
            string OTP = Common.GenerateOTP();

            if (!string.IsNullOrEmpty(GUID))
            {
                var getUser = _consultantRepository.Find(x => x.Id == Convert.ToInt32(GUID)).FirstOrDefault();

                if (getUser != null)
                {
                    
                    if (UserEmail != null)
                    {
                        // when user apply for lost mobile process 

                        IMMIWeb.Consultant userObj = new IMMIWeb.Consultant();
                        userObj = getUser;
                        userObj.EmailVerificationToken = OTP;
                        userObj.OtpDate = DateTime.UtcNow;
                        _consultantRepository.Update(userObj);

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
                        // consultant registration process

                        bool isToday = Convert.ToDateTime(getUser.OtpDate).Day == DateTime.UtcNow.Day;

                        if (isToday && getUser.LoginAttempt <= OTPAttemptVal)
                        {
                            string mobileNumber = string.Empty;

                            if (!string.IsNullOrEmpty(NewMobileCountryCode) && !string.IsNullOrEmpty(NewMobile))
                                mobileNumber = NewMobileCountryCode + NewMobile;
                            else
                                mobileNumber = getUser.MobileCountryCode + getUser.Mobile;

                            string msgBody = "OTP for ADVENUSS is " + OTP + " Do not share it with anyone by any means.";
                            IMMIWeb.Consultant userObj = getUser;
                            userObj.Otp = OTP;
                            userObj.OtpDate = DateTime.UtcNow;
                            int attemptVal = Convert.ToInt32(getUser.LoginAttempt) + 1;
                            userObj.LoginAttempt = attemptVal;

                            _consultantRepository.Update(userObj);

                            Send.SMS(mobileNumber.Replace(" ", ""), msgBody);
                            HttpContext.Session.SetString("OTPRequestTime", Convert.ToString(DateTime.UtcNow));

                            returnMsg = "True";
                            return Json(returnMsg);
                        }
                        HttpContext.Session.SetString("GUID", "");
                        returnMsg = "Today's attempt is over";
                        return Json(returnMsg);
                    }
                }
                else
                    returnMsg = "User not found";
                return Json(returnMsg);

            }
            returnMsg = "User not found";
            return Json(returnMsg);
        }

        public ActionResult ConsultantOTPVerification()
        {

            OTPVerificationViewModel Model = new OTPVerificationViewModel();

            string GUID = Convert.ToString(HttpContext.Session.GetString("GUID"));

            if (GUID == "" || GUID == null)
            {
                TempData["AlertMessageFail"] = "Invalid Process";
                return RedirectToAction("ConsultantLogin");
            }
            return View(Model);
        }

        [HttpPost]
        public JsonResult ConsultantOTPVerification(string OTP, string UserEmail, string NewMobileCountryCode, string NewMobile)
        {
            string returnmsg = string.Empty;

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
                    var getUser = _consultantRepository.Find(x => x.Id == Convert.ToInt32(sessionGUID)).FirstOrDefault();

                    if(getUser != null)
                    {
                        // Lost mobile 
                        if(!string.IsNullOrEmpty(UserEmail))
                        {
                            if(getUser.Email.Trim().ToLower() == UserEmail.Trim().ToLower() &&  getUser.EmailVerificationToken == OTP)
                            {
                                HttpContext.Session.SetString("CurrentUserId", sessionGUID);
                                Guid guid = Guid.NewGuid();
                                string guidString = guid.ToString("N").Substring(0, 16).Replace("-", "");
                                HttpContext.Session.SetString("RandomNumber", Convert.ToString(guidString));
                                //TempData["AlertMessageSuccess"] = "OTP verify successfull.";
                                return Json("ChangeMobileTrue");
                            }
                            else
                            {
                                return Json("OTP does not match.");
                            }
                        }

                        // Add new mobile otp 
                        if(!string.IsNullOrEmpty(NewMobileCountryCode) && !string.IsNullOrEmpty(NewMobile) && getUser.Otp == OTP)
                        {
                            IMMIWeb.Consultant user = new IMMIWeb.Consultant();
                            user = getUser;
                            user.MobileCountryCode = NewMobileCountryCode.Replace(" ", "");
                            user.Mobile = NewMobile;
                            _consultantRepository.Update(user);
                            HttpContext.Session.SetString("NewMobileCountryCode", "");
                            HttpContext.Session.SetString("NewMobile", "");
                            return Json("NewMobileAdded");
                        }

                        // General registration
                        if (UserEmail == null && NewMobile == null && getUser.Otp == OTP)
                        {
                            // send email if consultant email verification process is pending mostly during the registration
                            if (getUser.EmailConfirmed == null || getUser.EmailConfirmed == false)
                            {

                                HttpContext.Session.SetString("CurrentConsultantId", sessionGUID);
                                Guid guid = Guid.NewGuid();
                                string guidString = guid.ToString("N").Substring(0, 16).Replace("-", "");
                                guidString = guidString.Substring(guidString.Length - 10);
                                var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.PathBase}";
                                string url = baseUrl.ToString() + "/Consultant/ConsultantAccount/ConsultantEmailVerification?code=" + guidString;

                                IMMIWeb.Consultant consultantObj = new IMMIWeb.Consultant();

                                consultantObj = getUser;
                                consultantObj.IsVerified = true;
                                consultantObj.EmailVerificationToken = guidString;
                                _consultantRepository.Update(consultantObj);

                                #region Email Verification Link

                                EmailTemplateMaker emailTemplateMaker = new EmailTemplateMaker(_hostingEnvironment);
                                string[] paramArray;
                                string imagePath = string.Empty;
                                paramArray = new string[2] { imagePath, url };

                                var emailConfirmationTemplate = emailTemplateMaker.GetTemplate("sendEmailVerificationLink", "VerifyEmail.html", paramArray);
                                Common.SendMail("Advenuss Email Verification Link", emailConfirmationTemplate, getUser.Email);

                                TempData["AlertMessageSuccess"] = "We just sent you verification link on your mail " + getUser.Email + " Please verify.";
                                return Json("EmailNotTrue");

                                #endregion

                            }
                            else
                            {
                                // if otp process for login
                                #region Authentication

                                var identity = new ClaimsIdentity(new[]
                                {
                                            new Claim(ClaimTypes.MobilePhone, getUser.Mobile),
                                            new Claim(ClaimTypes.Role, MasterEnum.EUserType.Consultant.ToString())
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
                    }
                    else
                    {
                        return Json("User not found");
                    }
                }
                return Json("OTP verification process timeout");
            }
            return Json("Session has been timeout");
        }

        public ActionResult ConsultantLogin()
        {

            if (SessionFactory.CurrentUserId > 0)
                return RedirectToAction("Index", "Home");

            LoginViewModel loginViewModel = new LoginViewModel();

            var getMobileList = _countryRepository.CommonCountryMobileCodeList();
            loginViewModel.lstMobile = _mapper.Map<List<CommonListViewModel>>(getMobileList);

            return View(loginViewModel);
        }
        [HttpPost]
        public ActionResult ConsultantLogin(LoginViewModel loginViewModel)
        {

            HttpContext.Session.SetString("NewMobileCountryCode", "");
            HttpContext.Session.SetString("NewMobile", "");
            HttpContext.Session.SetString("UserEmail", "");

            LoginViewModel Model = new LoginViewModel();
            Model.MobileCountryCode = loginViewModel.MobileCountryCode;
            Model.Mobile = loginViewModel.Mobile;
            Model.lstMobileVal = loginViewModel.MobileCountryCode;

            var getMobileList = _countryRepository.CommonCountryMobileCodeList();
            Model.lstMobile = _mapper.Map<List<CommonListViewModel>>(getMobileList);

            if (!string.IsNullOrEmpty(loginViewModel.Mobile) && !string.IsNullOrEmpty(loginViewModel.MobileCountryCode))
            {
                var getUser = _consultantRepository.Find(x => x.Mobile == loginViewModel.Mobile.Trim() &&
                x.MobileCountryCode == loginViewModel.MobileCountryCode.Trim()).FirstOrDefault();

                if (getUser != null)
                {
                    bool isToday = Convert.ToDateTime(getUser.OtpDate).Day == DateTime.UtcNow.Day;

                    if (isToday == false || (isToday && getUser.LoginAttempt <= OTPAttemptVal))
                    {
                        // Consultant do not verify mobile
                        if (getUser.IsVerified == false || getUser.IsVerified == null)
                        {
                            HttpContext.Session.SetString("OTPProcessFor", "SignUp");
                            string OTP = Common.GenerateOTP();
                            string msgBody = "OTP for ADVENUSS is " + OTP + " Do not share it with anyone by any means.";

                            IMMIWeb.Consultant userObj = new IMMIWeb.Consultant();
                            userObj = getUser;
                            userObj.Otp = OTP;
                            userObj.OtpDate = DateTime.UtcNow;
                            userObj.LoginAttempt = userObj.LoginAttempt + 1;
                            if (isToday == false)
                            {
                                userObj.LoginAttempt = 0;
                            }
                            _consultantRepository.Update(userObj);

                            HttpContext.Session.SetString("GUID", getUser.Id.ToString());
                            HttpContext.Session.SetString("MobileCountryCode", getUser.MobileCountryCode);
                            HttpContext.Session.SetString("Mobile", Convert.ToString(getUser.Mobile));
                            HttpContext.Session.SetString("OTPRequestTime", Convert.ToString(DateTime.UtcNow));

                            string receiverMobileNumber = getUser.MobileCountryCode + getUser.Mobile;
                            Send.SMS(receiverMobileNumber, msgBody);

                            return RedirectToAction("ConsultantOTPVerification");
                        }

                        // Consultant do not verify email
                        if (getUser.IsVerified == true && (getUser.EmailConfirmed == false || getUser.EmailConfirmed == null))
                        {
                            Guid guid = Guid.NewGuid();
                            string guidString = guid.ToString("N").Substring(0, 16).Replace("-", "");
                            guidString = guidString.Substring(guidString.Length - 10);
                            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.PathBase}";
                            string url = baseUrl.ToString() + "/Consultant/ConsultantAccount/ConsultantEmailVerification?code=" + guidString;

                            IMMIWeb.Consultant userObj = new IMMIWeb.Consultant();
                            userObj = getUser;
                            userObj.EmailVerificationToken = guidString;
                            _consultantRepository.Update(userObj);

                            EmailTemplateMaker emailTemplateMaker = new EmailTemplateMaker(_hostingEnvironment);
                            string[] paramArray;
                            string imagePath = string.Empty;
                            paramArray = new string[2] { imagePath, url };

                            var emailConfirmationTemplate = emailTemplateMaker.GetTemplate("sendEmailVerificationLink", "VerifyEmail.html", paramArray);
                            Common.SendMail("Advenuss Email Verification Link", emailConfirmationTemplate, getUser.Email);

                            TempData["AlertMessageSuccess"] = "We just sent you verification link on your mail " + getUser.Email + " Please verify.";
                            //Model.Message = "Your today's attempt is over please try tomorrow";
                            return View(Model);
                        }

                        // Consultant not done the stripe account setup prcoess 
                        if (getUser.IsVerified == true && getUser.EmailConfirmed == true && (getUser.IsConsultantStripAccountVerified == false || getUser.IsConsultantStripAccountVerified == null))
                        {
                            var baseUrlNew = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.PathBase}";
                            string baseLink = baseUrlNew.ToString();
                            var redirectUri = baseLink + "/Consultant/ConsultantAccount/ConsultantStripeCallback";
                            string striplink = "https://connect.stripe.com/express/oauth/authorize?redirect_uri=" + redirectUri + "&client_id=" + stripeClientId + "&state=" + getUser.Id;
                            EmailTemplateMaker emailTemplateMaker = new EmailTemplateMaker(_hostingEnvironment);
                            string[] paramArray;
                            paramArray = new string[1] { striplink };

                            var emailStripeTemplate = emailTemplateMaker.GetTemplate("ConsultantStripe", "ConnectStripe.html", paramArray);
                            Common.SendMail("Stripe Email Verification", emailStripeTemplate, getUser.Email);

                            Model.Message = "Your stripe account has not been set up yet. We have sent the account set up link to your registered email.";
                        }

                        // Consultant not get admin approval
                        if (getUser.IsVerified == true && getUser.EmailConfirmed == true && getUser.IsConsultantStripAccountVerified == true && (getUser.IsAdminApproved == false || getUser.IsAdminApproved == null))
                        {
                            Model.Message = "Your account is not verified. Please contact us using this " + clientEmailAddress + " email to verify your account.";
                        }

                        // when leagal consultant try to login than he got otp 
                        if (getUser.IsVerified == true && getUser.EmailConfirmed == true && getUser.IsConsultantStripAccountVerified == true && getUser.IsAdminApproved == true)
                        {
                            if (getUser.IsSuspended == false)
                            {
                                string OTP = Common.GenerateOTP();
                                string msgBody = "OTP for ADVENUSS is " + OTP + " Do not share it with anyone by any means.";

                                IMMIWeb.Consultant userObj = new IMMIWeb.Consultant();
                                userObj = getUser;
                                userObj.Otp = OTP;
                                userObj.OtpDate = DateTime.UtcNow;
                                userObj.LoginAttempt = userObj.LoginAttempt + 1;
                                if (userObj.LoginAttempt == null)
                                {
                                    userObj.LoginAttempt = 1;
                                }

                                _consultantRepository.Update(userObj);

                                HttpContext.Session.SetString("MobileCountryCode", getUser.MobileCountryCode);
                                HttpContext.Session.SetString("Mobile", Convert.ToString(getUser.Mobile));
                                HttpContext.Session.SetString("OTPRequestTime", Convert.ToString(DateTime.UtcNow));

                                string receiverMobileNumber = getUser.MobileCountryCode + getUser.Mobile;
                                Send.SMS(receiverMobileNumber, msgBody);

                                TempData["AlertMessageSuccess"] = "OTP has been sent";
                                HttpContext.Session.SetString("GUID", getUser.Id.ToString());

                                return RedirectToAction("ConsultantOTPVerification", "ConsultantAccount");
                            }
                            else
                            {
                                if(getUser.SuspendedBy == "Systematic" || getUser.RequestRejectionCount >= Convert.ToInt32(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("GeneralInfo")["RejectionCountVal"]))
                                {
                                    
                                    TimeSpan timeSpan = Convert.ToDateTime(getUser.OtpDate) - DateTime.UtcNow;
                                    
                                    if (timeSpan.TotalDays > 7)
                                    {
                                        #region RESET TO UNSUSPENDED

                                        string OTP = Common.GenerateOTP();
                                        string msgBody = "OTP for ADVENUSS is " + OTP + " Do not share it with anyone by any means.";

                                        IMMIWeb.Consultant userObj = new IMMIWeb.Consultant();
                                        userObj = getUser;
                                        userObj.Otp = OTP;
                                        userObj.OtpDate = DateTime.UtcNow;
                                        userObj.LoginAttempt = 1;
                                        userObj.RequestRejectionCount  = 1;
                                        userObj.IsSuspended = false;
                                        userObj.SuspendedBy = string.Empty;
                                        if (userObj.LoginAttempt == null)
                                        {
                                            userObj.LoginAttempt = 1;
                                        }

                                        _consultantRepository.Update(userObj);

                                        HttpContext.Session.SetString("MobileCountryCode", getUser.MobileCountryCode);
                                        HttpContext.Session.SetString("Mobile", Convert.ToString(getUser.Mobile));
                                        HttpContext.Session.SetString("OTPRequestTime", Convert.ToString(DateTime.UtcNow));

                                        string receiverMobileNumber = getUser.MobileCountryCode + getUser.Mobile;
                                        Send.SMS(receiverMobileNumber, msgBody);

                                        TempData["AlertMessageSuccess"] = "OTP has been sent";
                                        HttpContext.Session.SetString("GUID", getUser.Id.ToString());

                                        return RedirectToAction("ConsultantOTPVerification", "ConsultantAccount");

                                        #endregion
                                    }
                                    else
                                    {
                                        Model.Message = "Your profile is inactivated, please contact to the admin to activate it.";
                                    }
                                }

                                Model.Message = "Your profile is inactivated, please contact to the admin to activate it.";
                            }
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

        public ActionResult ConsultantEmailVerification(string code)
        {
            EmailVerificationViewModel Model = new EmailVerificationViewModel();
               
            Model.MobileMsg = string.Empty;

            string mobileStausMsg = string.Empty;
            string emailStausMsg = string.Empty;

            var getCode = _consultantRepository.Find(x => x.EmailVerificationToken == code.Trim()).FirstOrDefault();

            if (getCode != null && (getCode.EmailConfirmed == false || getCode.EmailConfirmed == null))
            {
                IMMIWeb.Consultant userObj = new IMMIWeb.Consultant();
                userObj = getCode;
                userObj.EmailConfirmed = true;
                userObj.EmailVerificationToken = string.Empty;

                _consultantRepository.Update(userObj);

                Model.Status = "True";
                emailStausMsg = "Your email address confirmed successfully ";

                if (getCode.IsConsultantStripAccountVerified == null || getCode.IsConsultantStripAccountVerified == false)
                {
                    var baseUrlNew = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.PathBase}";
                    string baseLink = baseUrlNew.ToString();
                    var redirectUri = baseLink + "/Consultant/ConsultantAccount/ConsultantStripeCallback";
                    string striplink = "https://connect.stripe.com/express/oauth/authorize?redirect_uri=" + redirectUri + "&client_id=" + stripeClientId + "&state=" + getCode.Id;
                    EmailTemplateMaker emailTemplateMaker = new EmailTemplateMaker(_hostingEnvironment);
                    string[] paramArray;
                    paramArray = new string[1] { striplink };

                    var emailStripeTemplate = emailTemplateMaker.GetTemplate("ConsultantStripe", "ConnectStripe.html", paramArray);
                    Common.SendMail("Stripe", emailStripeTemplate, getCode.Email);

                    emailStausMsg = emailStausMsg + " Your stripe account has not been set up yet. We have sent the account set up link to your registered email.";
                }
            }
            else
            {
                Model.Status = "False";
                emailStausMsg = "Link expired";
            }

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.PathBase}";

            string url = baseUrl.ToString() + "/Consultant/ConsultantAccount/Login";

            Model.URL = url;
            Model.Msg = mobileStausMsg + emailStausMsg;

            return View(Model);
        }

        [AllowAnonymous]
        public ActionResult ConsultantStripeCallback(string code, int state)
        {
            TestViewModel Model = new TestViewModel();

            Model.status = false;
            string stripeSecretKey = Convert.ToString(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("StripeSettings")["SecretKey"]);
            dynamic data = StripeCurlCommand(code, stripeSecretKey);
            string stripeId = data.stripe_user_id;
            var getConsultatnData = _consultantRepository.Find(x => x.Id == state).FirstOrDefault();
            if (getConsultatnData != null)
            {
                getConsultatnData.IsConsultantStripAccountVerified = true;
                getConsultatnData.ConsultantStripeAccountId = stripeId;
                _consultantRepository.Update(getConsultatnData);
                Model.status = true;
            }
            return View(Model);
        }

        static dynamic StripeCurlCommand(string code, string stripeSecret)
        {
            string url = "https://connect.stripe.com/oauth/token";
            string postData = $"client_secret={stripeSecret}&code={code}&grant_type=authorization_code";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(postData);
            }

            string responseJson;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    responseJson = reader.ReadToEnd();
                }
            }

            dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(responseJson);
            return data;
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
            return RedirectToAction("ConsultantLogin", "ConsultantAccount", new { Area = "Consultant" });

        }


        public ActionResult ConsultantChangeMobile()
        {
            ChangeMobileViewModel Model = new ChangeMobileViewModel();
            var getMobileList = _countryRepository.CommonCountryMobileCodeList();
            Model.lstMobile = _mapper.Map<List<CommonListViewModel>>(getMobileList);
            return View(Model);
        }

        [HttpPost]
        public ActionResult ConsultantChangeMobile(ChangeMobileViewModel changeMobileViewModel, string emailString, string numberString, string codeString)
        {
            ChangeMobileViewModel Model = new ChangeMobileViewModel();

            var getMobileList = _countryRepository.CommonCountryMobileCodeList();
            Model.lstMobile = _mapper.Map<List<CommonListViewModel>>(getMobileList);

            Model.Mobile = changeMobileViewModel.Mobile;
            Model.Email = changeMobileViewModel.Email;
            Model.lstMobileVal = changeMobileViewModel.MobileCountryCode;

            if (!string.IsNullOrEmpty(changeMobileViewModel.Mobile) && !string.IsNullOrEmpty(changeMobileViewModel.Email))
            {
                var getUser = _consultantRepository.Find(x => x.Mobile == changeMobileViewModel.Mobile.Trim() &&
                x.MobileCountryCode == changeMobileViewModel.MobileCountryCode).FirstOrDefault();

                var getExistEmail = _consultantRepository.Find(x => x.Email == changeMobileViewModel.Email && x.IsActive == true 
                && x.IsVerified == true &&  x.EmailConfirmed == true   && x.IsConsultantStripAccountVerified == true && x.IsAdminApproved == true 
                &&   (x.IsSuspended == false || x.IsSuspended == null)).FirstOrDefault();

                if (getUser != null && getExistEmail != null)
                {
                    if (getUser.Id == getExistEmail.Id)
                    {
                        if (getUser.IsActive == true)
                        {
                            string OTP = Common.GenerateOTP();

                            IMMIWeb.Consultant userObj = new IMMIWeb.Consultant();
                            userObj = getUser;
                            userObj.EmailVerificationToken = OTP;
                            userObj.OtpDate = DateTime.UtcNow;
                            _consultantRepository.Update(userObj);

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
                            return RedirectToAction("ConsultantOTPVerification", "ConsultantAccount");
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


        public ActionResult ConsultantAddNewMobile()
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
        public ActionResult ConsultantAddNewMobile(ChangeMobileViewModel changeMobileViewModel)
        {
            ChangeMobileViewModel Model = new ChangeMobileViewModel();

            string RandomNumber = HttpContext.Session.GetString("RandomNumber");
            string CurrentUserId = HttpContext.Session.GetString("CurrentUserId");

            var getCurrentUser = _consultantRepository.Find(x => x.Id == Convert.ToInt32(CurrentUserId)).FirstOrDefault();

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

            var checkUserMobileExist = _consultantRepository.Find(x => x.Mobile == changeMobileViewModel.NewMobile
            && x.Email == getCurrentUser.Email).FirstOrDefault();

            if (checkUserMobileExist == null)
            {
                string OTP = Common.GenerateOTP();
                string mobileNum = changeMobileViewModel.NewMobileCountryCode + changeMobileViewModel.NewMobile;

                IMMIWeb.Consultant userObj = new IMMIWeb.Consultant();
                userObj = getCurrentUser;
                userObj.Otp = OTP;
                userObj.OtpDate = DateTime.UtcNow;
                _consultantRepository.Update(userObj);
                string msgBody = "OTP for ADVENUSS is " + OTP + " Do not share it with anyone by any means.";
                Send.SMS(mobileNum, msgBody);

                TempData["AlertMessageSuccess"] = "We just sent you OTP on your mobile number " + mobileNum + " Please enter it now.";

                HttpContext.Session.SetString("UserEmail", "");
                HttpContext.Session.SetString("NewMobileCountryCode", changeMobileViewModel.NewMobileCountryCode);
                HttpContext.Session.SetString("NewMobile", changeMobileViewModel.NewMobile);
                HttpContext.Session.SetString("GUID", Convert.ToString(getCurrentUser.Id));
                HttpContext.Session.SetString("OTPRequestTime", Convert.ToString(DateTime.UtcNow));
                return RedirectToAction("ConsultantOTPVerification");
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
    }
}
