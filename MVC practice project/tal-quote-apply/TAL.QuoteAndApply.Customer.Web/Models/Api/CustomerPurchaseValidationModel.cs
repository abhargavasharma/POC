using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;

namespace TAL.QuoteAndApply.Customer.Web.Models.Api
{
    public class CustomerPurchaseValidationModel
    {
        public ICollection<RiskBeneficiaryValidationModel> Beneficiaries { get; set; }

        public string PaymentErrorMessage { get; set; }
    }
}