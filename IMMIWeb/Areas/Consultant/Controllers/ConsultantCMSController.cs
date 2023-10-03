using AutoMapper;
using IMMIWeb.Infrastructure;
using IMMIWeb.Service.Models;
using IMMIWeb.Service.Service.CMS;
using IMMIWeb.Service.Service.Communication;
using IMMIWeb.Service.Service.Consultant;
using IMMIWeb.Service.Service.General;
using IMMIWeb.Service.Service.Setting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Twilio.Rest.Autopilot.V1.Assistant;

namespace IMMIWeb.Areas.Consultant.Controllers
{
    [Area("Consultant")]
    [Authorize(Roles = "Consultant")]
    public class ConsultantCMSController : Controller
    {
        #region Fields

        private readonly ICMSRepository _cMSRepository;
        private readonly IMapper _mapper;
        private readonly IHelpRepository _helpRepository;
        private readonly ICountryRepository _countryRepository;
        private readonly IConsultantRepository _consultantRepository;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _hostingEnvironment;

        string OTPValidity = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("GeneralInfo")["OTPValidity"];
        int OTPAttemptVal = Convert.ToInt32(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("GeneralInfo")["OTPAttempt"]);

        #endregion

        #region Ctor

        public ConsultantCMSController(ICMSRepository cMSRepository,
            IConsultantRepository consultantRepository,
            IMapper mapper,
            IHelpRepository helpRepository,
            Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnvironment,
            ICountryRepository countryRepository)
        {
            _cMSRepository = cMSRepository;
            _mapper = mapper;
            _helpRepository = helpRepository;
            _hostingEnvironment = hostingEnvironment;
            _countryRepository = countryRepository;
            _consultantRepository = consultantRepository;
        }

        #endregion

        #region Methods

        public IActionResult CMSConfigurationConsultant()
        {
            ConsultantSettingViewModel consultantSettingViewModel = new ConsultantSettingViewModel();

            var getMobileList = _countryRepository.CommonCountryMobileCodeList();
            consultantSettingViewModel.lstMobile = _mapper.Map<List<CommonListViewModel>>(getMobileList);

            var CMSdata = _cMSRepository.GetAll().Where(x => x.UserRole == 2 && x.Module == 1).FirstOrDefault();
            //var ViewModel = _mapper.Map<IEnumerable<CMSViewModel>>(CMSdata);
            
            TempData["CMyModel"] = CMSdata.Description;

            List<Help> chelps = _helpRepository.GetAll().ToList();
            TempData["CHelpData"] = chelps;

            return View(consultantSettingViewModel);
        }
        [HttpPost]
        public IActionResult CMSConfigurationConsultant(ConsultantSettingViewModel consultantSettingViewModel)
        {
            ConsultantSettingViewModel model = new ConsultantSettingViewModel();
            var getCurrentUserMobile = SessionFactory.CurrentUserMobile.ToString();
            //if (!string.IsNullOrEmpty(consultantSettingViewModel.newMobile) && !string.IsNullOrEmpty(consultantSettingViewModel.newMobileCountryCode) && !string.IsNullOrEmpty(consultantSettingViewModel.currentMobile) && !string.IsNullOrEmpty(consultantSettingViewModel.currentMobileCountryCode))
            //{
            //    var getUser = _consultantRepository.Find(x =>
            //    x.Mobile == consultantSettingViewModel.currentMobile.Trim() &&
            //    x.MobileCountryCode == consultantSettingViewModel.currentMobileCountryCode.Trim()).FirstOrDefault();

            //    int checkUserNewMobileExist = _consultantRepository.Find(x => x.Mobile == consultantSettingViewModel.newMobile && x.MobileCountryCode == consultantSettingViewModel.newMobileCountryCode).Count();

            //    if (getUser != null)
            //    {
            //        bool isToday = Convert.ToDateTime(getUser.OtpDate).Day == DateTime.Today.Day;
            //        if (isToday == false || (isToday && getUser.LoginAttempt <= OTPAttemptVal))
            //        {
            //            if (getCurrentUserMobile == consultantSettingViewModel.currentMobile)
            //            {
            //                if (checkUserNewMobileExist == 0)
            //                {
            //                    HttpContext.Session.SetString("NewNumberOTPProcessFor", "ChangeNewNumber");
            //                    string OTP = Common.GenerateOTP();
            //                    string msgBody = "OTP for ADVENUSS is " + OTP + " Do not share it with anyone by any means.";

            //                    IMMIWeb.Consultant consultantObj = new IMMIWeb.Consultant();
            //                    consultantObj = getUser;
            //                    consultantObj.Otp = OTP;
            //                    consultantObj.OtpDate = DateTime.UtcNow;
            //                    consultantObj.LoginAttempt = consultantObj.LoginAttempt + 1;
            //                    if (isToday == false)
            //                    {
            //                        consultantObj.LoginAttempt = 0;
            //                    }
            //                    _consultantRepository.Update(consultantObj);

            //                    HttpContext.Session.SetString("ConsultantNewNumber", Convert.ToString(consultantSettingViewModel.newMobile));
            //                    HttpContext.Session.SetString("ConsultantNewMobileCode", Convert.ToString(consultantSettingViewModel.newMobileCountryCode));
            //                    HttpContext.Session.SetString("ConsultantGUID", getUser.Id.ToString());
            //                    HttpContext.Session.SetString("ConsultantOtpRequestTime", Convert.ToString(DateTime.UtcNow));

            //                    Send.SMS(consultantSettingViewModel.newMobile, msgBody);

            //                    TempData["AlertMessageSuccess"] = "OTP has been sent successfully.";
            //                    return RedirectToAction("ConsultantChangeMobileOtpVerfiy");

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


            if (!string.IsNullOrEmpty(consultantSettingViewModel.currentMobile) && !string.IsNullOrEmpty(consultantSettingViewModel.Email))
            {
                var getUser = _consultantRepository.Find(x => x.Mobile == consultantSettingViewModel.currentMobile.Trim() &&
                x.MobileCountryCode == consultantSettingViewModel.currentMobileCountryCode).FirstOrDefault();

                var getExistEmail = _consultantRepository.Find(x => x.Email == consultantSettingViewModel.Email
                && x.IsVerified == true && x.IsSuspended != true && x.EmailConfirmed == true).FirstOrDefault();

                if (getUser != null && getExistEmail != null)
                {
                    if (getUser.Id == getExistEmail.Id)
                    {
                        if (getUser.IsActive == true)
                        {
                            string OTP = Common.GenerateOTP();

                            IMMIWeb.Consultant consultantObj = new IMMIWeb.Consultant();
                            consultantObj = getUser;
                            consultantObj.EmailVerificationToken = OTP;
                            consultantObj.OtpDate = DateTime.UtcNow;
                            _consultantRepository.Update(consultantObj);

                            HttpContext.Session.SetString("GUID", Convert.ToString(getUser.Id));
                            HttpContext.Session.SetString("UserEmail", Convert.ToString(getUser.Email));

                            consultantSettingViewModel.GUID = Convert.ToString(getUser.Id);
                            consultantSettingViewModel.UserEmail = Convert.ToString(getUser.Email);

                            EmailTemplateMaker emailTemplateMaker = new EmailTemplateMaker(_hostingEnvironment);
                            string[] paramArray;
                            string imagepath = "";
                            paramArray = new string[2] { OTP, imagepath };
                            var emailConfirmationTemplate = emailTemplateMaker.GetTemplate("EmailOTPVerification", "VerificationCode.html", paramArray);

                            Common.SendMail("Advenuss Email OTP Link", emailConfirmationTemplate, consultantSettingViewModel.Email);
                            HttpContext.Session.SetString("OTPRequestTime", Convert.ToString(DateTime.UtcNow));

                            consultantSettingViewModel.OTPRequestTime = Convert.ToString(DateTime.UtcNow);

                            consultantSettingViewModel.ConsultantOTPVerification = "1";

                            consultantSettingViewModel.otptimer = Convert.ToString(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("GeneralInfo")["OTPTimer"]);

                            //TempData["AlertMessageSuccess"] = "We just sent you OTP on your mail " + getUser.Email + " Please enter it now.";
                            //return RedirectToAction("OTPVerification", "UserAccount");

                            //OTPVerificationViewModel Model = new OTPVerificationViewModel();

                            //string GUID = Convert.ToString(HttpContext.Session.GetString("GUID"));

                            //if (GUID == "" || GUID == null)
                            //{
                            //    TempData["AlertMessageFail"] = "Invalid Process";
                            //    return RedirectToAction("Login");
                            //}
                            return Json(new { message = "Settings updated successfully", data = consultantSettingViewModel });
                            //return PartialView("_OtpVerification", Model);
                        }
                        else
                        {
                            consultantSettingViewModel.ErrorMsg = "Your profile is inactivated, please contact to the admin to activate it.";
                        }
                    }
                    else
                    {
                        consultantSettingViewModel.EmailErrorMsg = "Email not exist.";
                    }
                }
                else
                {
                    if (getUser == null)
                    {
                        consultantSettingViewModel.MobileErrorMsg = "Mobile number not exist.";
                    }

                    if (getExistEmail == null)
                    {
                        consultantSettingViewModel.EmailErrorMsg = "Email not exist.";
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(consultantSettingViewModel.currentMobile))
                    consultantSettingViewModel.ErrorMsg = "Mobile number is required.";
                else
                    consultantSettingViewModel.ErrorMsg = "Email is required.";
            }


            return RedirectToAction("CMSConfigurationConsultant", "ConsultantCMS");
        }
        [HttpPost]
        public IActionResult ConsultantContactUs(ConsultantSettingViewModel consultantSettingViewModel)
        {
            string userEmail = consultantSettingViewModel.Email;
            string subject = consultantSettingViewModel.Subject;
            string description = consultantSettingViewModel.description;

            EmailTemplateMaker emailTemplateMaker = new EmailTemplateMaker(_hostingEnvironment);
            string[] paramArray;
            string userName = SessionFactory.CurrentUserName;
            paramArray = new string[4] { userName, description, userEmail, subject };

            var contactUsEmail = emailTemplateMaker.GetTemplate("sendContactUsMail", "ContactUs.htm", paramArray);
            Common.SendMail("Advenuss Consultant Contact Us", contactUsEmail, "yash.iqlance@gmail.com");

            TempData["AlertMessageSuccess"] = "Your message has been sent to administration!";
            return RedirectToAction("CMSConfigurationConsultant", "ConsultantCMS", new { area = "Consultant" });
        }

        [AllowAnonymous]
        public IActionResult PrivacyPolicy()
        {
            IMMIWeb.Cm Model = new Cm();
            Model = _cMSRepository.Find(x => x.UserRole == 2 && x.Module == 3).FirstOrDefault();
            return View(Model);
        }

        [AllowAnonymous]
        public IActionResult TermsAndCondition()
        {
            IMMIWeb.Cm Model = new Cm();
            Model = _cMSRepository.Find(x => x.UserRole == 2 && x.Module == 2).FirstOrDefault();
            return View(Model);
        }

        [AllowAnonymous]
        public IActionResult Agreement()
        {
            IMMIWeb.Cm Model = new Cm();
            Model = _cMSRepository.Find(x => x.UserRole == 2 && x.Module == 4).FirstOrDefault();
            return View(Model);
        }


        

        [HttpPost]
        public ActionResult AddNewMobile(ChangeMobileViewModel changeMobileViewModel)
        {
            ChangeMobileViewModel Model = new ChangeMobileViewModel();

            string RandomNumber = HttpContext.Session.GetString("RandomNumber");
            string CurrentUserId = HttpContext.Session.GetString("CurrentUserId");

            var getCurrentUser = _consultantRepository.Find(x => x.Id == Convert.ToInt32(CurrentUserId)).FirstOrDefault();

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

            var checkUserMobileExist = _consultantRepository.Find(x => x.Mobile == changeMobileViewModel.NewMobile
            && x.Email == getCurrentUser.Email).FirstOrDefault();

            if (checkUserMobileExist == null)
            {
                string OTP = Common.GenerateOTP();
                string mobileNum = changeMobileViewModel.NewMobileCountryCode + changeMobileViewModel.NewMobile;

                IMMIWeb.Consultant consultantObj = new IMMIWeb.Consultant();
                consultantObj = getCurrentUser;
                consultantObj.Otp = OTP;
                consultantObj.OtpDate = DateTime.UtcNow;
                _consultantRepository.Update(consultantObj);
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


                //return PartialView("_OtpVerification", Models);
                //return RedirectToAction("OTPVerification");
                return Json(new { message = "Settings updated successfully", data = changeMobileViewModel });
            }
            else
                Model.ErrorMsg = "New mobile number is already exist";


            var getMobileList = _countryRepository.CommonCountryMobileCodeList();
            Model.lstMobile = _mapper.Map<List<CommonListViewModel>>(getMobileList);
            Model.NewMobile = changeMobileViewModel.NewMobile;
            Model.lstMobileVal = changeMobileViewModel.NewMobileCountryCode;

            //HttpContext.Session.SetString("ConsultantOTPVerification", "1");

            return View(Model);
        }

        #endregion
    }
}
