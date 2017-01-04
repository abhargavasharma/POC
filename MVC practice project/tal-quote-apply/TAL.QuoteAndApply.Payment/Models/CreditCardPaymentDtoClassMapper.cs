using TAL.QuoteAndApply.DataLayer.Service;

namespace TAL.QuoteAndApply.Payment.Models
{
    public sealed class CreditCardPaymentDtoClassMapper : DbItemClassMapper<CreditCardPaymentDto>
    {
        protected override void DefineMappings(ClassMapperWrapper<CreditCardPaymentDto> mapper)
        {
            mapper.MapTable("CreditCardPayment");
            mapper.MapProperty(cc => cc.CardType, "CreditCardTypeId");
        }
    }
}