using IMMIWeb.Service.Models;
using IMMIWeb.Service.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMMIWeb.Service.Service.Appointment
{
    public interface IAppointmentRepository : IGenericRepository<IMMIWeb.Appointment>
    {

        int BookAppointment(IMMIWeb.Appointment appointment);
        List<UserRequestViewModel> UserAppointmentRequestList(int counsultantId);
        IEnumerable<AppointmentPendingViewModel> AppointmentPendingUser(int counsultantId);
        List<AppointmentPendingViewModel> AppointmentPendingUserById(int UserId);

        List<AppointmentHistoryViewModel> AppointmentHistoryUserById(int UserId);

        List<AppointmentRetainViewModel> AppointmentRetainUserById(int UserId);
        RetainDetailsByUserViewModel RetainDetailsUserById(int UserId, int ConsultantId);
    }
}
