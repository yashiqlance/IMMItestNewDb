using AutoMapper;
using IMMIWeb.Infrastructure;
using IMMIWeb.Service.Models;
using IMMIWeb.Service.Service.Appointment;
using IMMIWeb.Service.Service.Consultant;
using IMMIWeb.Service.Service.General;
using IMMIWeb.Service.Service.Setting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace IMMIWeb.Areas.Consultant.Controllers
{
    [Area("Consultant")]
    [Authorize(Roles = "Consultant")]
    
    public class ConsultantSlotController : Controller
    {

        #region Fields
        private readonly IConsultantRepository _consultantRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        #endregion

        #region Ctor
        public ConsultantSlotController(            
            IConsultantRepository consultantRepository,
            IAppointmentRepository appointmentRepository
            )
        {
            _consultantRepository = consultantRepository;
            _appointmentRepository = appointmentRepository;
        }
        #endregion

        #region Method

        public IActionResult ManageSlot()
        {
            string[] result = new string[0];
            result = ListofAvailableHoursConsultant();
            ViewBag.ListofHours = result;
            
            return View();
        }
        [HttpPost]
        public ActionResult ManageSlot(List<string> SlotDate)
        {
            //string datetimeString = model.SlotDate[0].ToString();

            //string[] parts = datetimeString.Split(' ');
            //string dateString = parts[0];
            //string timeString = parts[1] + " " + parts[2];

            //DateTime parsedDate = DateTime.ParseExact(dateString, "M/d/yyyy", CultureInfo.InvariantCulture);
            //DateTime parsedTime = DateTime.ParseExact(timeString, "h:mm:ss tt", CultureInfo.InvariantCulture);

            //DateTime modifiedDatetime = new DateTime(parsedDate.Year, parsedDate.Month, parsedDate.Day, parsedTime.Hour, parsedTime.Minute, parsedTime.Second, parsedTime.Millisecond);

            //DateTime parsedDateTime = DateTime.ParseExact(datetimeString, "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
            //DateTime modifiedDatetime = new DateTime(parsedDateTime.Year, parsedDateTime.Month, parsedDateTime.Day, parsedDateTime.Hour, parsedDateTime.Minute, 0, 0);
            //DateTime isoDateTime = model.SlotDate;

            if (SlotDate.Count > 0)
            {
                _consultantRepository.AddConsultantSlotAsync(SlotDate, SessionFactory.CurrentUserId);
            }

            // return View();
            //return RedirectToAction("Index", "ConsultantHome");
            var response = new { redirect = Url.Action("Index", "ConsultantHome") };
            return Json(response);
        }

        [HttpPost]
        public ActionResult UpdateSlot(List<string> SlotDate)
        {
            if (SlotDate.Count > 0)
            {
                _consultantRepository.UpdateConsultantSlotAsync(SlotDate, SessionFactory.CurrentUserId);
            }

            //return View();
            //return RedirectToAction("Index", "ConsultantHome");

            var response = new { redirect = Url.Action("Index", "ConsultantHome") };
            return Json(response);
        }

        [HttpPost]
        public string[] ListofAvailableHoursConsultant()
        {
            string[] result = new string[0];
            result = _consultantRepository.ListofAvailableHoursConsultant(SessionFactory.CurrentUserId);

            return result;
        }

        [HttpPost]
        public List<ConsultantSlot> ListofBusinessHoursConsultant(string SlotDate)
        {
            List<ConsultantSlot> result = new List<ConsultantSlot>();
            if (!string.IsNullOrEmpty(SlotDate))
            {
                result = _consultantRepository.ListofBusinessHoursConsultant(SessionFactory.CurrentUserId, SlotDate);
            }               
            return result;
        }

        [HttpPost]
        public JsonResult UpdateAvailableStatus(int userId, bool isAvailable)
        {
            bool updateSuccess = _consultantRepository.UpdateConsultantAvailability(userId, isAvailable);

            if (updateSuccess)
            {
                return Json(new { success = true, message = "Availability status updated successfully." });
            }
            else
            {
                return Json(new { success = false, message = "User not found or an error occurred." });
            }
        }

        [HttpPost]
        public JsonResult AddSessionInAppointment(string AppointmentId, string SessionId)
        {

            string response = string.Empty;
            if (!string.IsNullOrEmpty(AppointmentId) && !string.IsNullOrEmpty(SessionId))
            {
                var getAppointment = _appointmentRepository.Find(x => x.Id == Convert.ToInt32(AppointmentId)).FirstOrDefault();

                if(getAppointment != null)
                {
                    HttpContext.Session.SetString("AppointmentSessionId", SessionId);

                    Appointment appointment = new Appointment();
                    appointment = getAppointment;
                    appointment.AppointmentStatusName = (int)MasterEnum.EAppointmentStatus.Active;
                    appointment.SlotSessionId = SessionId;
                    appointment.IsConsultantPresent = true;
                    _appointmentRepository.Update(appointment);
                    response = "update the appointment by consultant true";
                }
            }
            return Json(new { response });
        }

        [HttpPost]
        public JsonResult AppointmentCallEnded(string SessionId)
        {

            string response = string.Empty;
            //string SessionId = HttpContext.Session.GetString("AppointmentSessionId");

            if (!string.IsNullOrEmpty(SessionId))
            {
                var getAppointment = _appointmentRepository.Find(x => x.SlotSessionId == SessionId).FirstOrDefault();
                
                if (getAppointment != null)
                {
                    response = getAppointment.UserId.ToString();
                }
            }
            return Json(new { response });
        }

        #endregion
    }
}
