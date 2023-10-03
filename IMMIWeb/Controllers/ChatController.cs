using IMMIWeb.Infrastructure;
using IMMIWeb.Service.Models;
using IMMIWeb.Service.Service.Appointment;
using IMMIWeb.Service.Service.Consultant;
using IMMIWeb.Service.Service.General;
using IMMIWeb.Service.Service.Retains;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IMMIWeb.Controllers
{
    [Authorize(Roles = "User")]
    public class ChatController : Controller
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IConsultantRepository _consultantRepository;
        private readonly IRetainRepository _retainRepository;

        public ChatController(
            IAppointmentRepository appointmentRepository, IConsultantRepository consultantRepository, IRetainRepository retainRepository)
        {
            _appointmentRepository = appointmentRepository;
            _consultantRepository = consultantRepository;
            _retainRepository = retainRepository;
        }

        public IActionResult TextChat(string appId,string rId)
        {
            
            string[] appIdVal = appId.Split('_');
            if (appIdVal[0] == "Appointment")
            {
                if (!string.IsNullOrEmpty(appId))
                {
                    string response = string.Empty;
                    if (!string.IsNullOrEmpty(appId))
                    {
                        var getAppointment = _appointmentRepository.Find(x => x.Id == Convert.ToInt32(appIdVal[1])
                         && (x.IsUserPresent == null || x.IsUserPresent == false)).FirstOrDefault();

                        if (getAppointment != null)
                        {
                            //HttpContext.Session.SetString("AppointmentSessionId", SessionId);

                            Appointment appointment = new Appointment();
                            appointment = getAppointment;
                            appointment.AppointmentStatusName = (int)MasterEnum.EAppointmentStatus.Working;
                            appointment.SlotSessionId = appId;
                            appointment.IsUserPresent = true;
                            _appointmentRepository.Update(appointment);
                        }
                    }
                }
            }

            CometchatAppointmentViewModel Model = new CometchatAppointmentViewModel();
            Model.AppointmentIdTag = appId;
            Model.rId = rId;
            return View(Model);
        }

        public IActionResult UserReceiveCall()
        {
            return View();
        }

        [HttpPost]
        public JsonResult UserChatEnd(string SessionId)
        {
            string response = string.Empty;
            if (!string.IsNullOrEmpty(SessionId))
            {
                var getAppointment = _appointmentRepository.Find(x => x.SlotSessionId == Convert.ToString(SessionId)).FirstOrDefault();

                if (getAppointment != null)
                {
                    //HttpContext.Session.SetString("AppointmentSessionId", SessionId);

                    Appointment appointment = new Appointment();
                    appointment = getAppointment;
                    appointment.AppointmentStatusName = (int)MasterEnum.EAppointmentStatus.Completed;
                    _appointmentRepository.Update(appointment);
                    response = "User start retain to consultant or call ended";
                }
            }
            return Json(new { response });
        }

        public IActionResult RetainConsultantChat()
        {
            RetainCList Model = new RetainCList();            
            Model.lstGetRetainConsultnatListForUserViewModel = _retainRepository.GetRetainConsultnatListForUser(SessionFactory.CurrentUserId);

            return View(Model);
        }

    }
}
