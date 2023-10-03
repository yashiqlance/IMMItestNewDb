using IMMIWeb.Service.Service.Appointment;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using IMMIWeb.Infrastructure;
using IMMIWeb.Service.Models;
using System.Security.Cryptography;
using IMMIWeb.Service.Service.General;
using IMMIWeb.Service.Service.Retains;

namespace IMMIWeb.Areas.Consultant.Controllers
{
    [Area("Consultant")]
    [Authorize(Roles = "Consultant")]
    public class ConsultantChatController : Controller
    {

        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IRetainRepository _retainRepository;

        public ConsultantChatController(
           IAppointmentRepository appointmentRepository, IRetainRepository retainRepository
            )
        {
            _appointmentRepository = appointmentRepository;
            _retainRepository = retainRepository;
        }

        public IActionResult TextChatConsultant(string appId,string rId)
        {
            string[] appIdVal = appId.Split('_');
            
            if (appIdVal[0]== "Appointment")
            {
                if (!string.IsNullOrEmpty(appId))
                {
                    string response = string.Empty;
                    if (!string.IsNullOrEmpty(appId))
                    {
                        var getAppointment = _appointmentRepository.Find(x => x.Id == Convert.ToInt32(appIdVal[1])
                         && (x.IsConsultantPresent == null || x.IsConsultantPresent == false)).FirstOrDefault();

                        if (getAppointment != null)
                        {
                            //HttpContext.Session.SetString("AppointmentSessionId", SessionId);

                            Appointment appointment = new Appointment();
                            appointment = getAppointment;
                            appointment.AppointmentStatusName = (int)MasterEnum.EAppointmentStatus.Active;
                            appointment.SlotSessionId = appId;
                            appointment.IsConsultantPresent = true;
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
        public IActionResult ConsultantReceiveCall()
        {
            return View();
        }

        public IActionResult RetainUserChat()
        {
            RetainCList Model = new RetainCList();
            Model.lstGetRetainConsultnatListForUserViewModel = _retainRepository.GetRetainUserListForConsultnat(SessionFactory.CurrentUserId);

            return View(Model);
        }

    }
}
