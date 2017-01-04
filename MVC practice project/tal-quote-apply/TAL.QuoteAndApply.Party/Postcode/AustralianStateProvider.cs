using System.Collections.Generic;
using TAL.QuoteAndApply.Party.Postcode;

namespace TAL.QuoteAndApply.Party.Validation
{
    public interface IAustralianStateProvider
    {
        AustralianState FromPostcode(string postcode);
    }

    public class AustralianStateProvider : IAustralianStateProvider
    {
        private readonly IAustralianStateFactory _australianStateFactory;

        public AustralianStateProvider(IAustralianStateFactory australianStateFactory)
        {
            _australianStateFactory = australianStateFactory;
        }

        public AustralianState FromPostcode(string postcode)
        {
            int postcodeInt;
            if (!int.TryParse(postcode, out postcodeInt))
            {
                return _australianStateFactory.UnknownState();
            }

            var allAustralianStates = _australianStateFactory.AllStates();

            foreach (var state in allAustralianStates)
            {
                if (state.ContainsPostcode(postcodeInt))
                {
                    return state;
                }
            }

            return _australianStateFactory.UnknownState();
        }
    }
}
