using TAL.QuoteAndApply.DataLayer.Service;

namespace TAL.QuoteAndApply.Payment.Models
{
    public sealed class SuperAnnuationPaymentDtoClassMapper : DbItemClassMapper<SuperAnnuationPaymentDto>
    {
        protected override void DefineMappings(ClassMapperWrapper<SuperAnnuationPaymentDto> mapper)
        {
            mapper.MapTable("SuperAnnuationPayment");
        }
    }
}