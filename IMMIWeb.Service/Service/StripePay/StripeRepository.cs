using IMMIWeb.Service.Models;
using IMMIWeb.Service.Models.Stripe;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IMMIWeb.Service.Service.StripePay
{
    public class StripeRepository : IStripeRepository
    {
        private readonly ChargeService _chargeService;
        private readonly CustomerService _customerService;
        private readonly TokenService _tokenService;

        public StripeRepository(
        ChargeService chargeService,
        CustomerService customerService,
        TokenService tokenService)
        {
            _chargeService = chargeService;
            _customerService = customerService;
            _tokenService = tokenService;
        }

        public async Task<AddCardResponse> AddCustomerCardAsync(AddStripeCustomer customer, CancellationToken ct)
        {
            AddCardResponse addCardResponse = new AddCardResponse();

            TokenCreateOptions tokenOptions = new TokenCreateOptions
            {
                Card = new TokenCardOptions
                {
                    Name = customer.Name,
                    Number = customer.CreditCard.CardNumber,
                    ExpYear = customer.CreditCard.ExpirationYear,
                    ExpMonth = customer.CreditCard.ExpirationMonth,
                    Cvc = customer.CreditCard.Cvc
                }
            };
            
            Token stripeToken = await _tokenService.CreateAsync(tokenOptions, null, ct);            
            CustomerCreateOptions customerOptions = new CustomerCreateOptions
            {
                Name = customer.Name,
                Email = customer.Email,
                Source = stripeToken.Id
            };
            Customer createdCustomer = await _customerService.CreateAsync(customerOptions, null, ct);

            addCardResponse.ReturnCustomerId = createdCustomer.Id;
            addCardResponse.ReturnCardId = stripeToken.Card.Id;

            return addCardResponse;
        }
        public async Task<StripePayment> AddStripePaymentAsync(AddStripePayment payment, CancellationToken ct)
        {
            // Set the options for the payment we would like to create at Stripe
            ChargeCreateOptions paymentOptions = new ChargeCreateOptions
            {
                Customer = payment.CustomerId,
                ReceiptEmail = payment.ReceiptEmail,
                Description = payment.Description,
                Currency = payment.Currency,
                Amount = payment.Amount
            };

            // Create the payment
            var createdPayment = await _chargeService.CreateAsync(paymentOptions, null, ct);

            // Return the payment to requesting method
            return new StripePayment(
              createdPayment.CustomerId,
              createdPayment.ReceiptEmail,
              createdPayment.Description,
              createdPayment.Currency,
              createdPayment.Amount,
              createdPayment.Id);
        }
        public PaymentResponse MakePayment(IMMIWeb.Service.Models.ChargeViewModel chargeViewModel)
        {
            PaymentResponse paymentResponse = new PaymentResponse();

            var options = new ChargeCreateOptions
            {
                Amount = chargeViewModel.Amount*100,
                //Currency = chargeViewModel.Currency,
                Currency = "CAD",
                Source = chargeViewModel.CardId,
                Description = chargeViewModel.Desc,
                Customer = chargeViewModel.CustomerId
            };
            var service = new ChargeService();
            var returnData = service.Create(options);

            paymentResponse.Id = returnData.Id;
            paymentResponse.Status = returnData.Status;


            return paymentResponse;
        }
        public string RefundPayment(long amount, string transactionId)
        {
            var Refundoption = new RefundCreateOptions
            {
                Amount = amount * 100,
                Charge = transactionId
            };
            var services = new RefundService();
            var refund = services.Create(Refundoption);

            return refund.Status;
        }

        public string FundTransfer(long amount, string StripeAccountId)
        {
            //StripeConfiguration.ApiKey = "sk_test_4eC39HqLyjWDarjtT1zdp7dc";

            var options = new TransferCreateOptions
            {
                Amount = amount,
                Currency = "cad",
                Destination = StripeAccountId,//"acct_1032D82eZvKYlo2C",
                //TransferGroup = "ORDER_95",
            };
            var service = new TransferService();
            var transfer = service.Create(options);
            return transfer.StripeResponse.StatusCode.ToString();

        }
    }
}
