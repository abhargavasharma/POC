using System.Collections.Generic;
using TAL.QuoteAndApply.DataModel.Personal;

namespace TAL.QuoteAndApply.Party.Postcode
{
    public class AustralianState
    {
        public State Code { get; }
        public IList<PostcodeRange> PostcodeRanges { get; }
        public bool IsValidAustralianState => Code != State.Unknown;

        public AustralianState(State code)
        {
            Code = code;
            PostcodeRanges = new List<PostcodeRange>();
        }

        public AustralianState WithPostcodeRange(int lower, int upper)
        {
            PostcodeRanges.Add(new PostcodeRange(lower, upper));
            return this;
        }

        public bool ContainsPostcode(int postcode)
        {
            foreach (var postcodeRange in PostcodeRanges)
            {
                if (postcodeRange.ContainsPostcode(postcode))
                {
                    return true;
                }
            }

            return false;
        }

    }
}