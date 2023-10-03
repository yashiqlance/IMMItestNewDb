using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IMMIWeb.Infrastructure;
using IMMIWeb.Service.Models;
using IMMIWeb.Service.Service.Consultant;
using IMMIWeb.Service.Service.Appointment;
using IMMIWeb.Service.Service.General;
using System.Security.Cryptography.Xml;
using Stripe;
using IMMIWeb.Service.Service.StripePay;
using IMMIWeb.Service.Service.Communication;

namespace IMMIWeb.Areas.Consultant.Controllers
{
    [Area("Consultant")]
    [Authorize(Roles = "Consultant")]
    public class ConsultantAppointmentController : Controller
    {
        #region Fields
        private readonly IConsultantRepository _consultantRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IAppointmentPaymentRepository _appointmentPaymentRepository;
        private readonly IStripeRepository _stripeRepository;
        #endregion

        #region Ctor
        public ConsultantAppointmentController(IConsultantRepository consultantRepository,
            IAppointmentRepository appointmentRepository,
            IAppointmentPaymentRepository appointmentPaymentRepository,
            IStripeRepository stripeRepository)
        {
            _consultantRepository = consultantRepository;
            _appointmentRepository = appointmentRepository;
            _appointmentPaymentRepository = appointmentPaymentRepository;
            _stripeRepository = stripeRepository;
        }
        #endregion

        public IActionResult AppointmentPendingList(int Consultantid, int StatusType)
        {
            ConsultantAppointmentByStatusViewModel model = new ConsultantAppointmentByStatusViewModel();

            var getPendingAppointment = _consultantRepository.ConsultantAppointmentByStatus(SessionFactory.CurrentUserId, 1);
            model.lstconsultantAppointmentByStatus = getPendingAppointment.ToList();

            return View(model);
        }

        public JsonResult ConsultantAcceptRequest(int AppId)
        {
            string returnMsg = string.Empty;

            if (AppId > 0)
            {
                Appointment appointment = _appointmentRepository.Find(x => x.Id == AppId).FirstOrDefault();
                appointment.UserRequestTypeName = (int)MasterEnum.EUserRequestType.Approved;
                appointment.AppointmentStatusName = (int)MasterEnum.EAppointmentStatus.Consultant_Appointment_Accepted;
                _appointmentRepository.Update(appointment);

                returnMsg = "acceptTrue";

                //Open the Comment Code For Notification
                CommonInsertNotificationandSendNotificationparam paramnot = new CommonInsertNotificationandSendNotificationparam();

                paramnot.Header = "Appointment Accepted";
                paramnot.Body = "Appointment Accepted for New User Login";
                paramnot.Title = "Advenuss";
                paramnot.Description = "Appointment Accepted for New User Login";
                paramnot.UserId = appointment.UserId;
                paramnot.ConsultantId = SessionFactory.CurrentUserId;
                paramnot.NotificationTypeName = 13;

                Send.CommonInsertNotificationandSendNotification(paramnot);
            }
            else
            {
                returnMsg = "Invalid appointment.";
            }
            return Json(returnMsg);
        }

        public JsonResult ConsultantRejectRequest(int appId, string reason)
        {
            string returnMsg = string.Empty;

            if (appId > 0 || !string.IsNullOrEmpty(reason))
            {
                var getAppointmentPayment = _appointmentPaymentRepository.Find(x => x.AppointmentId == appId).FirstOrDefault();
                var getAppointment = _appointmentRepository.Find(x => x.Id == appId).FirstOrDefault();

                string status = _stripeRepository.RefundPayment(Convert.ToInt64(getAppointmentPayment.Amount), getAppointmentPayment.TransactionId);

                if (status.ToLower().ToString() == "succeeded")
                {
                    var getSlot = _consultantRepository.BookConsultantSlot(Convert.ToInt32(getAppointment.ConsultantSlotId), false);
                    Appointment appointmentObj = new Appointment();
                    appointmentObj = getAppointment;

                    appointmentObj.AppointmentStatusName = (int)MasterEnum.EAppointmentStatus.Consultant_Appointment_Rejected;
                    appointmentObj.UserRequestTypeName = (int)MasterEnum.EUserRequestType.Denied;
                    appointmentObj.CancelledByUserTypeName = Convert.ToInt32((int)MasterEnum.EUserType.Consultant);
                    appointmentObj.CancelledById = SessionFactory.CurrentUserId;
                    appointmentObj.CancellationDate = Convert.ToDateTime(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"));

                    decimal penaltyCharges = ((decimal)getAppointmentPayment.Amount * (20 / 100) * 100);
                    //appointmentObj.RejectionAmount = penaltyCharges;
                    appointmentObj.CancellationReason = reason;
                    _appointmentRepository.Update(appointmentObj);
                    returnMsg = "rejectTrue";

                    //Open the Comment Code For Notification
                    CommonInsertNotificationandSendNotificationparam paramnot = new CommonInsertNotificationandSendNotificationparam();

                    paramnot.Header = "Appointment Rejected";
                    paramnot.Body = "Appointment Rejected for New User Login";
                    paramnot.Title = "Advenuss";
                    paramnot.Description = "Appointment Rejected for New User Login";
                    paramnot.UserId = appointmentObj.UserId;
                    paramnot.ConsultantId = SessionFactory.CurrentUserId;
                    paramnot.NotificationTypeName = 12;

                    Send.CommonInsertNotificationandSendNotification(paramnot);

                }
                else
                {
                    returnMsg = "Invalid appoitment.";
                }
            }
            else
            {
                returnMsg = "Invalid appoitment.";
            }

            return Json(returnMsg);
        }

        public IActionResult AppointmentAcceptedList(int Consultantid, int StatusType)
        {
            ConsultantAppointmentByStatusViewModel model = new ConsultantAppointmentByStatusViewModel();

            var getPendingAppointment = _consultantRepository.ConsultantAppointmentByStatus(SessionFactory.CurrentUserId, 2);
            model.lstconsultantAppointmentByStatus = getPendingAppointment.ToList();
            return View(model);
        }

        public IActionResult AppointmentRejectedList(int Consultantid, int StatusType)
        {
            ConsultantAppointmentByStatusViewModel model = new ConsultantAppointmentByStatusViewModel();

            var getPendingAppointment = _consultantRepository.ConsultantAppointmentByStatus(SessionFactory.CurrentUserId, 3);
            model.lstconsultantAppointmentByStatus = getPendingAppointment.ToList();
            return View(model);
        }

        public IActionResult AppointmentDetails(int appointmentId)
        {
            ConsultantApponintmentDetailByUserIdViewModel model = new ConsultantApponintmentDetailByUserIdViewModel();

            var getAppointmentDetail = _consultantRepository.ConsultantAppointmentDetailsByAppointmentId(appointmentId).FirstOrDefault();
            model.ProfilePic = getAppointmentDetail.ProfilePic;
            model.FirstName = getAppointmentDetail.FirstName;
            model.LastName = getAppointmentDetail.LastName;
            model.TypeOfService = getAppointmentDetail.TypeOfService;
            model.ApplyForCountry = getAppointmentDetail.ApplyForCountry;
            model.BelongCountry = getAppointmentDetail.BelongCountry;
            model.LanguageName = getAppointmentDetail.LanguageName;
            model.BookingTime = getAppointmentDetail.BookingTime;
            model.BookingDate = getAppointmentDetail.BookingDate;
            model.CreatedOn = getAppointmentDetail.CreatedOn;
            model.SessionTitle = getAppointmentDetail.SessionTitle;
            model.BookingMinutes = getAppointmentDetail.BookingMinutes;
            return View(model);
        }
    }
}
