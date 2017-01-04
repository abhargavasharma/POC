using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Rules.Cover;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Plan
{
    public interface ICoverEligibilityService
    {
        CoverEligibilityResult GetCoverEligibilityResult(ICover cover);
        IEnumerable<CoverEligibilityResult> GetCoverEligibilityResults(IEnumerable<ICover> covers);
    }

    public class CoverEligibilityService : ICoverEligibilityService
    {
        private readonly ICoverRulesFactory _coverRulesFactory;
        private readonly IPlanErrorMessageService _planErrorMessages;

        public CoverEligibilityService(ICoverRulesFactory coverRulesFactory, IPlanErrorMessageService planErrorMessages)
        {
            _coverRulesFactory = coverRulesFactory;
            _planErrorMessages = planErrorMessages;
        }

        public CoverEligibilityResult GetCoverEligibilityResult(ICover cover)
        {
            var notDeclineRule = _coverRulesFactory.GetCoverNotUnderwritingDeclinedRule();

            if (notDeclineRule.IsSatisfiedBy(cover).IsSatisfied)
            {
                return CoverEligibilityResult.Eligible(cover.Code);
            }

            return CoverEligibilityResult.Ineligible(cover.Code,
                _planErrorMessages.GetSelectedCoverUndwritingDeclinedMessage());
        }

        public IEnumerable<CoverEligibilityResult> GetCoverEligibilityResults(IEnumerable<ICover> covers)
        {
            return covers.Select(GetCoverEligibilityResult);
        }
    }
}
