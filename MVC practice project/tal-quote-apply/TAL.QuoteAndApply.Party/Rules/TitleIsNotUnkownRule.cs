using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.DataModel.Personal;
using TAL.QuoteAndApply.Party.Models;
using TAL.QuoteAndApply.Rules;

namespace TAL.QuoteAndApply.Party.Rules
{
    public class TitleIsNotUnkownRule : IRule<Title>
    {
        private readonly string _validationKey;

        public TitleIsNotUnkownRule(string validationKey)
        {
            _validationKey = validationKey;
        }

        public RuleResult IsSatisfiedBy(Title target)
        {
            return new RuleResult(_validationKey, target != Title.Unknown);
        }
    }
}
