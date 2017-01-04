using TAL.QuoteAndApply.DataLayer.Service;

namespace TAL.QuoteAndApply.Payment.Models
{
    public sealed class PolicyPaymentDtoClassMapper : DbItemClassMapper<PolicyPaymentDto>
    {
        protected override void DefineMappings(ClassMapperWrapper<PolicyPaymentDto> mapper)
        {
            mapper.MapTable("PolicyPayment");
            mapper.MapProperty(cc => cc.PaymentType, "PaymentTypeId");
        }
    }
}