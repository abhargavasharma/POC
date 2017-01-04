using System;
using TAL.QuoteAndApply.DataModel.Interactions;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Models
{
    public class PolicyInteractions
    {
        public DateTime DateCreated { get; }
        public InteractionType InteractionType { get; }
        public string CreatedBy { get; }

        public PolicyInteractions(DateTime dateCreated, InteractionType interactionType, string createdBy)
        {
            DateCreated = dateCreated;
            InteractionType = interactionType;
            CreatedBy = createdBy;
        }
    }
}
