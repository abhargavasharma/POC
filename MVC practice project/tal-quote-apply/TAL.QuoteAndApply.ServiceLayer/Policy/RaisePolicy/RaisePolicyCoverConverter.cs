using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.RaisePolicy.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.RaisePolicy
{
    public interface IRaisePolicyCoverConverter
    {
        RaisePolicyCover From(ICover cover, IEnumerable<ICoverLoading> coverLoadings, IEnumerable<ICoverExclusion> coverExclusions);
    }

    public class RaisePolicyCoverConverter : IRaisePolicyCoverConverter
    {
        public RaisePolicyCover From(ICover cover, IEnumerable<ICoverLoading> coverLoadings, IEnumerable<ICoverExclusion> coverExclusions)
        {
            return new RaisePolicyCover
            {
                Code = cover.Code,
                CoverAmount = cover.CoverAmount,
                Selected = cover.Selected,
                Id = cover.Id,
                RiskId = cover.RiskId,
                PolicyId = cover.PolicyId,
                PlanId = cover.PlanId,
                UnderwritingStatus = cover.UnderwritingStatus,
                Premium = cover.Premium,
                Loadings = coverLoadings.Select(From).ToList(),
                Exclusions = coverExclusions.Select(From).ToList()
            };
        }

        private RaisePolicyCoverExclusion From(ICoverExclusion coverExclusion)
        {
            return new RaisePolicyCoverExclusion
            {
                CoverId = coverExclusion.CoverId,
                Name = coverExclusion.Name,
                Text = coverExclusion.Text
            };
        }

        public RaisePolicyCoverLoading From(ICoverLoading coverLoading)
        {
            return new RaisePolicyCoverLoading
            {
                CoverId = coverLoading.CoverId,
                Loading = coverLoading.Loading,
                LoadingType = coverLoading.LoadingType
            };
        }
    }
}