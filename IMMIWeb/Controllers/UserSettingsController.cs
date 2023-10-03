using IMMIWeb.Infrastructure;
using IMMIWeb.Service.Models;
using IMMIWeb.Service.Models.Stripe;
using IMMIWeb.Service.Service.StripePay;
using IMMIWeb.Service.Service.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using Stripe;

namespace IMMIWeb.Controllers
{
    [Authorize(Roles = "User")]
    public class UserSettingsController : Controller
    {
        #region Fields

        private readonly IStripeRepository _stripeRepository;
        private readonly IUserRepository _userRepository;

        #endregion

        #region Ctor

        public UserSettingsController(
            IStripeRepository stripeRepository,
            IUserRepository userRepository)
        {
            _stripeRepository = stripeRepository;
            _userRepository = userRepository;
        }

        #endregion

        #region Method

        public IActionResult UserAddPaymentCard()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> UserAddPaymentCard(AddStripeCustomer AddStripeCustomer, CancellationToken ct)
        {

            string returnMsg = string.Empty;

            var getCardResponse = await _stripeRepository.AddCustomerCardAsync(AddStripeCustomer, ct);

            if (getCardResponse != null && !string.IsNullOrEmpty(getCardResponse.ReturnCustomerId) && getCardResponse.ReturnCustomerId.Length > 0)
            {
                UserCardsDetail userCardsDetail = new UserCardsDetail();


                if (_userRepository.IsUserCardExist(SessionFactory.CurrentUserId))
                    userCardsDetail.IsPrimary = false;
                else
                    userCardsDetail.IsPrimary = true;

                userCardsDetail.CardName = AddStripeCustomer.Name;
                userCardsDetail.CardNumber = AddStripeCustomer.CreditCard.CardNumber.Replace(" ", "");
                userCardsDetail.Cvv = Convert.ToInt32(AddStripeCustomer.CreditCard.Cvc);
                userCardsDetail.ExpMonth = AddStripeCustomer.CreditCard.ExpirationMonth;
                userCardsDetail.ExpYear = AddStripeCustomer.CreditCard.ExpirationYear;
                userCardsDetail.StripeCustomerId = getCardResponse.ReturnCustomerId;
                userCardsDetail.StripeCardId = getCardResponse.ReturnCardId;
                userCardsDetail.Id = SessionFactory.CurrentUserId;

                int returnId = _userRepository.AddUserCard(userCardsDetail);

                returnMsg = "Card succesfully added";

                RedirectToAction("BookConsultant", "Home");
            }
            else
            {
                returnMsg = "Invalid Process Try After Sometime";
            }
            return Json(returnMsg);
        }

        public IActionResult PaymentCardList()
        {
            int userId = (int)SessionFactory.CurrentUserId;

            UserCardDetails model = new UserCardDetails();

            model.CardList = _userRepository.GetUserAllCardsDetail(userId);

            return View(model);
        }


        //[HttpPost]
        //public IActionResult RemoveCard(int cardId)
        //{
        //    var deleteCard = _userRepository.RemoveUserCardsDetail(cardId);
        //    return deleteCard;
        //}

        [HttpPost]
        public IActionResult RemoveUserCardsDetail(int cardId, int id)
        {
            try
            {
                var param = new Removecarddetailparam();
                param.Cardid = cardId;
                param.id = id;

                _userRepository.RemoveUserCardsDetail(param);

                RedirectToAction("PaymentCardList", "UserSettings");

                return Json(new { success = true, message = "Card removed successfully" });
                
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = "Error occurred during card removal" });
            }
        }

        [HttpPost]
        public IActionResult SetPrimayCard(int id, int cardId)
        {
            try
            {
                _userRepository.SetPrimaryCard(cardId, id);
                RedirectToAction("PaymentCardList", "UserSettings");
                return Json(new { success = true, message = "Card set a successfully" });

            }
            catch (Exception e)
            {
                return Json(new { success = false, message = "Error occurred while setting the primary card" });
            }
        }

        #endregion
    }
}
