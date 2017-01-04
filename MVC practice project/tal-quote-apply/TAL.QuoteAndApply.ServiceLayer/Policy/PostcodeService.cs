using System;
using TAL.QuoteAndApply.DataModel.Personal;
using TAL.QuoteAndApply.Party.Validation;

namespace TAL.QuoteAndApply.ServiceLayer.Policy
{
    public interface IPostcodeService
    {
        bool IsValidPostcode(string postcode);
        State GetStateForPostcode(string postcode);
    }

    public class PostcodeService : IPostcodeService
    {
        private readonly IAustralianStateProvider _australianStateProvider;

        public PostcodeService(IAustralianStateProvider australianStateProvider)
        {
            _australianStateProvider = australianStateProvider;
        }

        public bool IsValidPostcode(string postcode)
        {
            var postcodeState = _australianStateProvider.FromPostcode(postcode);
            return postcodeState.IsValidAustralianState;
        }

        public State GetStateForPostcode(string postcode)
        {
            var postcodeState = _australianStateProvider.FromPostcode(postcode);
            return postcodeState.Code;
        }
    }
}
