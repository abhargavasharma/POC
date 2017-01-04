using System;
using System.Collections.Generic;
using TAL.QuoteAndApply.DataModel.Interactions;

namespace TAL.QuoteAndApply.SalesPortal.Web.Models.Api
{
    public class PolicyInteractionsResponse
    {
        public PolicyInteractionsResponse()
        {
            PolicyInteractionDetailsList = new List<PolicyInteractionDetails>();
        }

        public IEnumerable<PolicyInteractionDetails> PolicyInteractionDetailsList { get; set; }
    }

    public class PolicyInteractionDetails
    {
        public string DateCreated { get; set; }
        public string InteractionType { get; set; }
        public string CreatedBy { get; set; }
    }
}