using IMMIWeb.Service.Models;
using IMMIWeb.Service.Models.Stripe;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMMIWeb.Service.Service.StripePay
{
    public interface IStripeRepository
    {

        
        Task<AddCardResponse> AddCustomerCardAsync(AddStripeCustomer customer, CancellationToken ct);
        Task<StripePayment> AddStripePaymentAsync(AddStripePayment payment, CancellationToken ct);
        PaymentResponse MakePayment(IMMIWeb.Service.Models.ChargeViewModel stripePaymentViewModel);
        string RefundPayment(long amount, string transactionId);
        string FundTransfer(long amount, string StripeAccountId);

    }
}
