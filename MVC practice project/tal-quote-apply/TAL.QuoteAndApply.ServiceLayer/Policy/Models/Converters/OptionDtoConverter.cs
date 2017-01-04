using System.Linq;
using TAL.QuoteAndApply.Policy.Data;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Models.Converters
{
    public interface IOptionDtoConverter
    {
        OptionDto CreateFrom(PlanStateParam coverOptionsParam, bool? selected, int planId, string code);
    }
    public interface IOptionDtoUpdater
    {
        IOption UpdateFrom(IOption option, PlanStateParam updatedOptionDetails);
    }
    public class OptionDtoConverter : IOptionDtoConverter, IOptionDtoUpdater
    {
        public OptionDto CreateFrom(PlanStateParam coverOptionsParam, bool? selected, int planId, string code)
        {
            return new OptionDto
            {
                RiskId = coverOptionsParam.RiskId,
                PlanId = planId,
                Selected = selected,
                Code = code
            };
        }

        public IOption UpdateFrom(IOption option, PlanStateParam updatedCoverDetails)
        {
            //Assumption - selected is the only thing to update - in future: premium, coveramout
            if (updatedCoverDetails.PlanOptions != null && updatedCoverDetails.PlanOptions.Any())
            {
                option.Selected = updatedCoverDetails.PlanOptions.FirstOrDefault(po => po.Code == option.Code)?.Selected;
            }

            return option;
        }
    }
}
