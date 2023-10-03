using IMMIWeb.Service.Repo;
using IMMIWeb.Service.Service.General;
using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMMIWeb.Service.Service.Appointment
{
    public class AppointmentPaymentRepository : GenericRepository<IMMIWeb.AppointmentPayment>, IAppointmentPaymentRepository
    {
        public AppointmentPaymentRepository(DbA976eeImmitestContext context) : base(context)
        {

        }
        public int BookAppointmentPayment(AppointmentPayment appointmentPayment)
        {
            int returnId = 0;

            try
            {
                _dbContext.AppointmentPayments.Add(appointmentPayment);
                _dbContext.SaveChanges();
                returnId = appointmentPayment.Id;
            }
            catch (Exception e)
            {
                returnId = appointmentPayment.Id;
                throw;
            }
            return returnId;
        }

        
    }
}
