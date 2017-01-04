using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TAL.QuoteAndApply.Customer.Web.Models.Api
{
    public class PaymentOptionsViewModel
    {
        public bool IsCreditCardSelected { get; set; }

        public bool IsDirectDebitSelected { get; set; }

        public CreditCardPaymentViewModel CreditCardPayment { get; set; }

        public DirectDebitPaymentViewModel DirectDebitPayment { get; set; }
    }
}