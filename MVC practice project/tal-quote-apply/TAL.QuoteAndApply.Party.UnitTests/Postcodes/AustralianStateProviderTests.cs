using NUnit.Framework;
using TAL.QuoteAndApply.DataModel.Personal;
using TAL.QuoteAndApply.Party.Postcode;
using TAL.QuoteAndApply.Party.Validation;

namespace TAL.QuoteAndApply.Party.UnitTests.Postcodes
{
    [TestFixture]
    public class AustralianStateProviderTests
    {
        //Test edge cases
        [TestCase("1000", Result = State.NSW)]
        [TestCase("1999", Result = State.NSW)]
        [TestCase("2000", Result = State.NSW)]
        [TestCase("2599", Result = State.NSW)]
        [TestCase("2619", Result = State.NSW)]
        [TestCase("2899", Result = State.NSW)]
        [TestCase("2921", Result = State.NSW)]
        [TestCase("2999", Result = State.NSW)]
        [TestCase("0200", Result = State.ACT)]
        [TestCase("0299", Result = State.ACT)]
        [TestCase("2600", Result = State.ACT)]
        [TestCase("2618", Result = State.ACT)]
        [TestCase("2900", Result = State.ACT)]
        [TestCase("2920", Result = State.ACT)]
        [TestCase("3000", Result = State.VIC)]
        [TestCase("3999", Result = State.VIC)]
        [TestCase("8000", Result = State.VIC)]
        [TestCase("8999", Result = State.VIC)]
        [TestCase("4000", Result = State.QLD)]
        [TestCase("4999", Result = State.QLD)]
        [TestCase("9000", Result = State.QLD)]
        [TestCase("9999", Result = State.QLD)]
        [TestCase("5000", Result = State.SA)]
        [TestCase("5799", Result = State.SA)]
        [TestCase("5800", Result = State.SA)]
        [TestCase("5999", Result = State.SA)]
        [TestCase("6000", Result = State.WA)]
        [TestCase("6797", Result = State.WA)]
        [TestCase("6800", Result = State.WA)]
        [TestCase("6999", Result = State.WA)]
        [TestCase("7000", Result = State.TAS)]
        [TestCase("7799", Result = State.TAS)]
        [TestCase("7800", Result = State.TAS)]
        [TestCase("7999", Result = State.TAS)]
        [TestCase("0800", Result = State.NT)]
        [TestCase("0899", Result = State.NT)]
        [TestCase("0900", Result = State.NT)]
        [TestCase("0999", Result = State.NT)]

        //Some out-of-range cases
        [TestCase("", Result = State.Unknown)]
        [TestCase("0", Result = State.Unknown)]
        [TestCase("-1", Result = State.Unknown)]
        [TestCase("0199", Result = State.Unknown)]
        [TestCase("6798", Result = State.Unknown)]
        [TestCase("6799", Result = State.Unknown)]
        [TestCase("0300", Result = State.Unknown)]
        [TestCase("0799", Result = State.Unknown)]
        [TestCase("10000", Result = State.Unknown)]
        [TestCase("blah", Result = State.Unknown)]
        public State StateFromPostcode_ReturnsValidState(string postcode)
        {
            var provider = new AustralianStateProvider(new AustralianStateFactory());
            var aussieState = provider.FromPostcode(postcode);
            return aussieState.Code;
        } 
    }
}
