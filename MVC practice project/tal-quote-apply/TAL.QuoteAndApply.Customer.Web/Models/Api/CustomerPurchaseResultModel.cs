using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;

namespace TAL.QuoteAndApply.Customer.Web.Models.Api
{
    public class CustomerPurchaseResultModel
    {
        public PolicyNoteResultViewModel PolicyNoteResult { get; set; }
        public List<BeneficiaryViewModel> Beneficiaries { get; set; }
        public bool NominateLpr { get; set; }
        public PersonalDetailsViewModel PersonalDetails { get; set; }
    }
}