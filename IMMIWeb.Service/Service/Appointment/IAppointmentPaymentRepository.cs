using IMMIWeb.Service.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMMIWeb.Service.Service.Appointment
{
    public interface IAppointmentPaymentRepository : IGenericRepository<IMMIWeb.AppointmentPayment>
    {
        int BookAppointmentPayment(AppointmentPayment appointmentPayment);

        
    }
}
