using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;

namespace TAL.QuoteAndApply.SalesPortal.Web.Services.Converters
{
    public interface IPolicyProgressViewModelConverter
    {
        PolicyProgressViewModel From(PolicyProgressParam progress);
    }

    public class PolicyProgressViewModelConverter : IPolicyProgressViewModelConverter
    {
        public PolicyProgressViewModel From(PolicyProgressParam progress)
        {
            return new PolicyProgressViewModel()
            {
                Progress = progress.Progress.ToString()
            };
        }
    }
}