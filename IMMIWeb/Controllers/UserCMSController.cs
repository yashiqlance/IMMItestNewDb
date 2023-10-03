using AutoMapper;
using IMMIWeb.Infrastructure;
using IMMIWeb.Service.Models;
using IMMIWeb.Service.Service.CMS;
using IMMIWeb.Service.Service.Communication;
using IMMIWeb.Service.Service.Setting;
using IMMIWeb.Service.Service.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IMMIWeb.Service.Service.General;
using System.Text.RegularExpressions;


namespace IMMIWeb.Controllers
{
    [Authorize(Roles = "User")]
    public class UserCMSController : Controller
    {

        #region Fields

        private readonly ICMSRepository _cmsRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<HomeController> _logger;
        private readonly IUserRepository _userRepository;
        private readonly ICountryRepository _countryRepository;
        private readonly IHelpRepository _helpRepository;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _hostingEnvironment;

        string OTPValidity = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("GeneralInfo")["OTPValidity"];
        int OTPAttemptVal = Convert.ToInt32(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("GeneralInfo")["OTPAttempt"]);

        #endregion

        #region Ctor

        public UserCMSController(
            Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnvironment,
            ILogger<HomeController> logger,
            ICMSRepository cmsRepository,
            IMapper mapper,
            IUserRepository userRepository,
            ICountryRepository countryRepository,
            IHelpRepository helpRepository)
        {
            _cmsRepository = cmsRepository;
            _mapper = mapper;
            _logger = logger;
            _userRepository = userRepository;
            _countryRepository = countryRepository;
            _hostingEnvironment = hostingEnvironment;
            _helpRepository = helpRepository;
        }

        #endregion

        #region Methods  


        public IActionResult Settings()
        {
            UserSettingViewModel userSettingViewModel = new UserSettingViewModel();

            var getMobileList = _countryRepository.CommonCountryMobileCodeList();
            userSettingViewModel.lstMobile = _mapper.Map<List<CommonListViewModel>>(getMobileList);

            IMMIWeb.Cm model = _cmsRepository.GetAll().Where(x => x.UserRole == 1 && x.Module == 1).FirstOrDefault();
            TempData["MyModel"] = model.Description;

            List<Help> helps = _helpRepository.GetAll().ToList();
            TempData["HelpData"] = helps;

            return View(userSettingViewModel);
        }
        [HttpPost]
        public IActionResult Settings(UserSettingViewModel userSettingViewModel)
        {
            UserSettingViewModel model = new UserSettingViewModel();
            var getCurrentUserMobile = SessionFactory.CurrentUserMobile.ToString();

            //if (!string.IsNullOrEmpty(userSettingViewModel.newMobile) && !string.IsNullOrEmpty(userSettingViewModel.newMobileCountryCode) && !string.IsNullOrEmpty(userSettingViewModel.currentMobile) && !string.IsNullOrEmpty(userSettingViewModel.currentMobileCountryCode))
            //{
            //    var getUser = _userRepository.Find(x =>
            //    x.Mobile == userSettingViewModel.currentMobile.Trim() &&
            //    x.MobileCountryCode == userSettingViewModel.currentMobileCountryCode.Trim()).FirstOrDefault();

            //    int checkUserNewMobileExist = _userRepository.Find(x => x.Mobile == userSettingViewModel.newMobile && x.MobileCountryCode == userSettingViewModel.newMobileCountryCode).Count();

            //    if (getUser != null)
            //    {
            //        bool isToday = Convert.ToDateTime(getUser.OtpDate).Day == DateTime.Today.Day;

            //        if (isToday == false || (isToday && getUser.LoginAttempt <= OTPAttemptVal))
            //        {
            //            if (getCurrentUserMobile == userSettingViewModel.currentMobile)
            //            {
            //                if (checkUserNewMobileExist == 0)
            //                {
            //                    HttpContext.Session.SetString("NewNumberOTPProcessFor", "ChangeNewNumber");
            //                    string OTP = Common.GenerateOTP();
            //                    string msgBody = "OTP for ADVENUSS is " + OTP + " Do not share it with anyone by any means.";

            //                    User userObj = new User();
            //                    userObj = getUser;
            //                    //userObj.Mobile = userSettingViewModel.newMobile;
            //                    userObj.Otp = OTP;
            //                    userObj.OtpDate = DateTime.UtcNow;
            //                    userObj.LoginAttempt = userObj.LoginAttempt + 1;
            //                    if (isToday == false)
            //                    {
            //                        userObj.LoginAttempt = 0;
            //                    }
            //                    _userRepository.Update(userObj);

            //                    HttpContext.Session.SetString("NewNumberChangeSetting", Convert.ToString(userSettingViewModel.newMobile));
            //                    HttpContext.Session.SetString("NewNumberCountryCode", Convert.ToString(userSettingViewModel.newMobileCountryCode));
            //                    HttpContext.Session.SetString("NewNumberGUID", getUser.Id.ToString());
            //                    HttpContext.Session.SetString("NewNumberOTPRequestTime", Convert.ToString(DateTime.UtcNow));

            //                    Send.SMS(userSettingViewModel.newMobile, msgBody);

            //                    TempData["AlertMessageSuccess"] = "OTP has been sent successfully.";

            //                    return PartialView("_OtpVerification");
            //                }
            //                else
            //                {
            //                    if (checkUserNewMobileExist > 0)
            //                        TempData["AlertMessageFailNewMobile"] = "This mobile already exists";
            //                }
            //            }
            //            else
            //            {
            //                if (checkUserNewMobileExist > 0)
            //                    TempData["AlertMessageFailCurrentMobile"] = "This is not Valid Mobile Number.";
            //            }
            //        }
            //        else
            //        {
            //            TempData["AlertMessageFailOtpAttempt"] = "Your today's attempt is over please try tomorrow";
            //        }
            //    }
            //    else
            //    {
            //        TempData["AlertMessageFailCurrentMobileValid"] = "Please enter valid mobile number";
            //    }

            //}

            if (!string.IsNullOrEmpty(userSettingViewModel.currentMobile) && !string.IsNullOrEmpty(userSettingViewModel.Email))
            {
                var getUser = _userRepository.Find(x => x.Mobile == userSettingViewModel.currentMobile.Trim() &&
                x.MobileCountryCode == userSettingViewModel.currentMobileCountryCode).FirstOrDefault();

                var getExistEmail = _userRepository.Find(x => x.Email == userSettingViewModel.Email
                && x.IsVerified == true && x.IsRegistered == true && x.EmailConfirmed == true).FirstOrDefault();

                if (getUser != null && getExistEmail != null)
                {
                    if (getUser.Id == getExistEmail.Id)
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

                            userSettingViewModel.GUID = Convert.ToString(getUser.Id);
                            userSettingViewModel.UserEmail = Convert.ToString(getUser.Email);


                            EmailTemplateMaker emailTemplateMaker = new EmailTemplateMaker(_hostingEnvironment);
                            string[] paramArray;
                            string imagepath = "";
                            paramArray = new string[2] { OTP, imagepath };
                            var emailConfirmationTemplate = emailTemplateMaker.GetTemplate("EmailOTPVerification", "VerificationCode.html", paramArray);

                            Common.SendMail("Advenuss Email OTP Link", emailConfirmationTemplate, userSettingViewModel.Email);
                            HttpContext.Session.SetString("OTPRequestTime", Convert.ToString(DateTime.UtcNow));

                            userSettingViewModel.OTPRequestTime = Convert.ToString(DateTime.UtcNow);

                            userSettingViewModel.otptimer = Convert.ToString(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("GeneralInfo")["OTPTimer"]);
                            //TempData["AlertMessageSuccess"] = "We just sent you OTP on your mail " + getUser.Email + " Please enter it now.";
                            //return RedirectToAction("OTPVerification", "UserAccount");

                            //OTPVerificationViewModel Model = new OTPVerificationViewModel();

                            //string GUID = Convert.ToString(HttpContext.Session.GetString("GUID"));

                            //if (GUID == "" || GUID == null)
                            //{
                            //    TempData["AlertMessageFail"] = "Invalid Process";
                            //    return RedirectToAction("Login");
                            //}
                            return Json(new { message = "Settings updated successfully", data = userSettingViewModel });

                            //return PartialView("_OtpVerification",Model);
                        }
                        else
                        {
                            userSettingViewModel.ErrorMsg = "Your profile is inactivated, please contact to the admin to activate it.";
                        }
                    }
                    else
                    {
                        userSettingViewModel.EmailErrorMsg = "Email not exist.";
                    }
                }
                else
                {
                    if (getUser == null)
                    {
                        userSettingViewModel.MobileErrorMsg = "Mobile number not exist.";
                    }

                    if (getExistEmail == null)
                    {
                        userSettingViewModel.EmailErrorMsg = "Email not exist.";
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(userSettingViewModel.currentMobile))
                    userSettingViewModel.ErrorMsg = "Mobile number is required.";
                else
                    userSettingViewModel.ErrorMsg = "Email is required.";
            }




            //return RedirectToAction("Settings", "UserCMS");
            return View(model);
        }


       

        [HttpPost]
        public ActionResult AddNewMobile(ChangeMobileViewModel changeMobileViewModel)
        {
            ChangeMobileViewModel Model = new ChangeMobileViewModel();

            string RandomNumber = HttpContext.Session.GetString("RandomNumber");
            string CurrentUserId = HttpContext.Session.GetString("CurrentUserId");

            var getCurrentUser = _userRepository.Find(x => x.Id == Convert.ToInt32(CurrentUserId)).FirstOrDefault();

            changeMobileViewModel.RandomNumber = RandomNumber;
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

                //TempData["AlertMessageSuccess"] = "We just sent you OTP on your mobile number " + mobileNum + " Please enter it now.";

                HttpContext.Session.SetString("UserEmail", "");
                HttpContext.Session.SetString("NewMobileCountryCode", changeMobileViewModel.NewMobileCountryCode);
                HttpContext.Session.SetString("NewMobile", changeMobileViewModel.NewMobile);
                HttpContext.Session.SetString("GUID", Convert.ToString(getCurrentUser.Id));
                HttpContext.Session.SetString("OTPRequestTime", Convert.ToString(DateTime.UtcNow));


               
                
                //OTPVerificationViewModel Models = new OTPVerificationViewModel();
                //string GUID = Convert.ToString(HttpContext.Session.GetString("GUID"));

                return Json(new { message = "Settings updated successfully", data = changeMobileViewModel });
               // return PartialView("_OtpVerification", Models);
                //return RedirectToAction("OTPVerification");
            }
            else
                Model.ErrorMsg = "New mobile number is already exist";


            var getMobileList = _countryRepository.CommonCountryMobileCodeList();
            Model.lstMobile = _mapper.Map<List<CommonListViewModel>>(getMobileList);
            Model.NewMobile = changeMobileViewModel.NewMobile;
            Model.lstMobileVal = changeMobileViewModel.NewMobileCountryCode;

            return View(Model);
        }

        [HttpPost]
        public IActionResult ContactUs(UserSettingViewModel userSettingViewModel)
        {
            string userEmail = userSettingViewModel.Email;
            string subject = userSettingViewModel.Subject;
            string description = userSettingViewModel.description;

            EmailTemplateMaker emailTemplateMaker = new EmailTemplateMaker(_hostingEnvironment);
            string[] paramArray;
            string userName = SessionFactory.CurrentUserName;
            paramArray = new string[4] { userName, description, userEmail, subject };

            var contactUsEmail = emailTemplateMaker.GetTemplate("sendContactUsMail", "ContactUs.htm", paramArray);
            Common.SendMail("Advenuss Contact Us", contactUsEmail, "yash.iqlance@gmail.com");

            //TempData["AlertMessageSuccess"] = "Your message has been sent to administration!";
            return RedirectToAction("Settings", "UserCMS");
        }

        [AllowAnonymous]
        public IActionResult AboutUs()
        {
            IMMIWeb.Cm Model = new Cm();
            Model = _cmsRepository.GetAll().Where(x => x.UserRole == 1 && x.Module == 1).FirstOrDefault();
            return View(Model);
        }

        [AllowAnonymous]
        public IActionResult PrivacyPolicy()
        {
            IMMIWeb.Cm Model = new Cm();
            Model = _cmsRepository.Find(x => x.UserRole == 1 && x.Module == 3).FirstOrDefault();
            return View(Model);
        }

        [AllowAnonymous]
        public IActionResult TermsAndCondition()
        {
            IMMIWeb.Cm Model = new Cm();
            Model = _cmsRepository.Find(x => x.UserRole == 1 && x.Module == 2).FirstOrDefault();
            return View(Model);
        }

        [AllowAnonymous]
        public IActionResult Agreement()
        {
            IMMIWeb.Cm Model = new Cm();
            Model = _cmsRepository.Find(x => x.UserRole == 1 && x.Module == 4).FirstOrDefault();
            return View(Model);
        }

       

        #endregion
    }
}
