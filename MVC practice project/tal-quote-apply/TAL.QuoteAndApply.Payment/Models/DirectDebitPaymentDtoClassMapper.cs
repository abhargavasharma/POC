using TAL.QuoteAndApply.DataLayer.Service;

namespace TAL.QuoteAndApply.Payment.Models
{
    public sealed class DirectDebitPaymentDtoClassMapper : DbItemClassMapper<DirectDebitPaymentDto>
    {
        protected override void DefineMappings(ClassMapperWrapper<DirectDebitPaymentDto> mapper)
        {
            mapper.MapTable("DirectDebitPayment");
        }
    }
}