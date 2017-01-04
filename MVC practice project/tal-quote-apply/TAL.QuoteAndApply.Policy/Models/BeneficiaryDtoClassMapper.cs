using TAL.QuoteAndApply.DataLayer.Service;

namespace TAL.QuoteAndApply.Policy.Models
{
    public sealed class BeneficiaryDtoClassMapper : DbItemClassMapper<BeneficiaryDto>
    {
        protected override void DefineMappings(ClassMapperWrapper<BeneficiaryDto> mapper)
        {
            mapper.MapTable("Beneficiary");            
            mapper.MapProperty(beneficiary => beneficiary.Gender, "GenderId");
            mapper.MapProperty(beneficiary => beneficiary.Country, "CountryId");
            mapper.MapProperty(beneficiary => beneficiary.Title, "TitleId");
            mapper.MapProperty(beneficiary => beneficiary.State, "StateId");
        }
    }
}