using IMMIWeb.Infrastructure;
using IMMIWeb.Service.Models;
using IMMIWeb.Service.Service.Appointment;
using IMMIWeb.Service.Service.Consultant;
using IMMIWeb.Service.Service.General;
using IMMIWeb.Service.Service.StripePay;
using IMMIWeb.Service.Service.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IMMIWeb.Areas.Consultant.Controllers
{
    [Area("Consultant")]
    [Authorize(Roles = "Consultant")]
    public class ConsultantRetainController : Controller
    {
        #region Fields
        private readonly IConsultantRepository _consultantRepository;
        private readonly IUserRepository _userRepository;
        #endregion

        #region Ctor
        public ConsultantRetainController(IConsultantRepository consultantRepository,
            IUserRepository userRepository)
        {
            _consultantRepository = consultantRepository;
            _userRepository = userRepository;
        }
        #endregion


        public IActionResult Clientlist()
        {
            var clientList = _consultantRepository.GetConsultantClientList(SessionFactory.CurrentUserId);

            return View(clientList);
        }

        public IActionResult ClientDetail(int UserId)
        {
            ConsultantRetainClientListViewModel model = _consultantRepository.GetClientDetails(UserId);

            var userDocuments = _userRepository.GetUserDocuments(UserId);

            var getCurrentConsultant = _consultantRepository.Find(x => x.Id == SessionFactory.CurrentUserId).FirstOrDefault();

            var exchangeRate = getCurrentConsultant != null ? Common.ExchangeRate(getCurrentConsultant.Country ?? 0) : 0;
            var retamount = Math.Round(model.ReatinAmount * exchangeRate, 2);
            var appamount = Math.Round(model.AppCharge * exchangeRate, 2);
            var taxamount = Math.Round(model.TaxCharge * exchangeRate, 2);

            model.ReatinAmount = retamount;
            model.AppCharge = appamount;
            model.TaxCharge = taxamount;


            var viewModel = new ClientDetailViewModel
            {
                ClientDetails = model,
                UserDocuments = userDocuments
            };

            return View(viewModel);
        }

        public IActionResult PaymentHistoryList()
        {
            var getCurrentConsultant = _consultantRepository.Find(x => x.Id == SessionFactory.CurrentUserId).FirstOrDefault();

            var exchangeRate = getCurrentConsultant != null ? Common.ExchangeRate(getCurrentConsultant.Country ?? 0) : 0;

            var paymentList = _consultantRepository.GetPaymentHistoryList(SessionFactory.CurrentUserId).ToList();

            paymentList.ForEach(payment => payment.RetainAmount = Math.Round(payment.RetainAmount*exchangeRate,2));
            paymentList.ForEach(payment => payment.Amount = Math.Round(payment.Amount * exchangeRate, 2));
            paymentList.ForEach(payment => payment.RejectionAmount = Math.Round(payment.RejectionAmount * exchangeRate, 2));

            return View(paymentList);
        }
    }
}
