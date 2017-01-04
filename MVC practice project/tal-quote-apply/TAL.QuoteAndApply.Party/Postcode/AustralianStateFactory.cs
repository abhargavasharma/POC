using System.Collections.Generic;
using TAL.QuoteAndApply.DataModel.Personal;

namespace TAL.QuoteAndApply.Party.Postcode
{

    public interface IAustralianStateFactory
    {
        IEnumerable<AustralianState> AllStates();
        AustralianState UnknownState();
    }

    public class AustralianStateFactory : IAustralianStateFactory
    {
        public IEnumerable<AustralianState> AllStates()
        {
            yield return new AustralianState(State.NSW)
                .WithPostcodeRange(1000, 1999)
                .WithPostcodeRange(2000, 2599)
                .WithPostcodeRange(2619, 2899)
                .WithPostcodeRange(2921, 2999);

            yield return new AustralianState(State.ACT)
                .WithPostcodeRange(0200, 0299)
                .WithPostcodeRange(2600, 2618)
                .WithPostcodeRange(2900, 2920);

            yield return new AustralianState(State.VIC)
                .WithPostcodeRange(3000, 3999)
                .WithPostcodeRange(8000, 8999);

            yield return new AustralianState(State.QLD)
                .WithPostcodeRange(4000, 4999)
                .WithPostcodeRange(9000, 9999);

            yield return new AustralianState(State.SA)
                .WithPostcodeRange(5000, 5799)
                .WithPostcodeRange(5800, 5999);

            yield return new AustralianState(State.WA)
                .WithPostcodeRange(6000, 6797)
                .WithPostcodeRange(6800, 6999);

            yield return new AustralianState(State.TAS)
                .WithPostcodeRange(7000, 7799)
                .WithPostcodeRange(7800, 7999);

            yield return new AustralianState(State.NT)
                .WithPostcodeRange(0800, 0899)
                .WithPostcodeRange(0900, 0999);
        }

        public AustralianState UnknownState()
        {
            return new AustralianState(State.Unknown);
        }
    }
}