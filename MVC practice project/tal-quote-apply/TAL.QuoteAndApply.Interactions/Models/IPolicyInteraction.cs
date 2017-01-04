using System;
using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.DataModel.Interactions;

namespace TAL.QuoteAndApply.Interactions.Models
{
    public interface IPolicyInteraction 
    {
        int PolicyId { get; set; }
        InteractionType InteractionType { get; set; }
        DateTime CreatedTS { get; set; }
        string CreatedBy { get; set; }
    }
}
