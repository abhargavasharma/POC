using TAL.QuoteAndApply.DataLayer.Service;

namespace TAL.QuoteAndApply.Payment.Models
{
    public sealed class SelfManagedSuperFundPaymentDtoClassMapper : DbItemClassMapper<SelfManagedSuperFundPaymentDto>
    {
        protected override void DefineMappings(ClassMapperWrapper<SelfManagedSuperFundPaymentDto> mapper)
        {
            mapper.MapTable("SelfManagedSuperFundPayment");
        }
    }
}