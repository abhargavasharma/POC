using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.DataModel.Interactions;

namespace TAL.QuoteAndApply.Interactions.Models
{
    public class PolicyInteractionDto : DbItem, IPolicyInteraction
    {
        public int PolicyId { get; set; }
        public InteractionType InteractionType { get; set; }
    }
}
