using TAL.QuoteAndApply.Party.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models.Converters;

namespace TAL.QuoteAndApply.ServiceLayer.Policy
{
    public interface ICreateNewPartyDtoService
    {
        IParty Create(CreateQuoteParam createQuoteParam);
    }

    public class CreateNewPartyDtoService : ICreateNewPartyDtoService
    {
        private readonly IPartyDtoConverter _partyDtoConverter;

        public CreateNewPartyDtoService(IPartyDtoConverter partyDtoConverter)
        {
            _partyDtoConverter = partyDtoConverter;
        }

        public IParty Create(CreateQuoteParam createQuoteParam)
        {
            //create party object
            return _partyDtoConverter.CreateFrom(createQuoteParam.PersonalInformation,
                createQuoteParam.RatingFactors);
        }
    }
}