using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAL.QuoteAndApply.DataModel.Underwriting;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Plan.Models.Mappers;
using TAL.QuoteAndApply.Underwriting.Models.Dto;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Models.Converters
{
    public interface ICoverDtoConverter
    {
        CoverDto CreateFrom(PlanStateParam coverOptionsParam, bool selected, int planId, string code);
    }
    public interface ICoverDtoUpdater
    {
        ICover UpdateFrom(ICover plan, PlanStateParam updatedCoverDetails);
        ICover UpdateFrom(ICover cover, InterviewReferenceInformation interview);
    }
    public class CoverDtoConverter : ICoverDtoConverter, ICoverDtoUpdater
    {
        public CoverDto CreateFrom(PlanStateParam coverOptionsParam, bool selected, int planId, string code)
        {
            return new CoverDto
            {
                PolicyId = coverOptionsParam.PolicyId,
                RiskId = coverOptionsParam.RiskId,
                PlanId = planId,
                Selected = selected,
                Code = code,
                UnderwritingStatus = UnderwritingStatus.Incomplete
            };
        }

        public ICover UpdateFrom(ICover cover, PlanStateParam updatedCoverDetails)
        {
            //Assumption - selected is the only thing to update - in future: premium, coveramout
            cover.Selected = updatedCoverDetails.SelectedCoverCodes.Contains(cover.Code);

            return cover;
        }

        public ICover UpdateFrom(ICover cover, InterviewReferenceInformation interview)
        {
            throw new NotImplementedException();
        }
    }
}
