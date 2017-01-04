using TAL.QuoteAndApply.DataLayer.Service;

namespace TAL.QuoteAndApply.Interactions.Models
{
    public sealed class PolicyInteractionDtoClassMapper : DbItemClassMapper<PolicyInteractionDto>
    {
        protected override void DefineMappings(ClassMapperWrapper<PolicyInteractionDto> mapper)
        {
            mapper.MapTable("PolicyInteraction");
            mapper.MapProperty(policyInteraction  => policyInteraction.InteractionType, "InteractionTypeId");
        }
    }
}
