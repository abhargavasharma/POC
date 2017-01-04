using System.Linq;
using System.Monads;
using TAL.QuoteAndApply.ServiceLayer.Search.Underwriting;
using TAL.QuoteAndApply.ServiceLayer.Underwriting.Models;
using TAL.QuoteAndApply.Underwriting;
using TAL.QuoteAndApply.Underwriting.Extensions;
using TAL.QuoteAndApply.Underwriting.Models.Dto;
using TAL.QuoteAndApply.Underwriting.Service;

namespace TAL.QuoteAndApply.ServiceLayer.Underwriting
{
    public interface IOccupationQuestionProvider
    {
        IndustryOccuptionClassAndText GetForInterview(InterviewReferenceInformation interview);
    }

    public class OccupationQuestionProvider : IOccupationQuestionProvider
    {
        private readonly IUnderwritingTagMetaDataService _metaDataService;

        public OccupationQuestionProvider(IUnderwritingTagMetaDataService metaDataService)
        {
            _metaDataService = metaDataService;
        }

        public IndustryOccuptionClassAndText GetForInterview(InterviewReferenceInformation interview)
        {
            if (interview.OccupationInformation.IndustryText == null ||
                interview.OccupationInformation.OccupationText == null)
            {
                return null;
            }

            return new IndustryOccuptionClassAndText(interview.OccupationInformation.OccupationText,
                _metaDataService.GetFirstOrDefault(interview.OccupationInformation.OccupationTags,
                    AnswerTagConstants.TalConsumerOccupationClass, ""),
                interview.OccupationInformation.IndustryText,
                interview.OccupationInformation.IndustryAnswerId,
                _metaDataService.GetFirstOrDefault(interview.OccupationInformation.OccupationTags, AnswerTagConstants.TalConsumerPasCode, ""),
                _metaDataService.TagAndValueExists(interview.OccupationInformation.OccupationTags, AnswerTagConstants.TalConsumerOccupationTpdOwnAnyKey,  AnswerTagConstants.TalConsumerOccupationTpdOwnValue),
                _metaDataService.TagAndValueExists(interview.OccupationInformation.OccupationTags, AnswerTagConstants.TalConsumerOccupationTpdOwnAnyKey, AnswerTagConstants.TalConsumerOccupationTpdAnyValue),
                _metaDataService.GetFirstOrDefault(interview.OccupationInformation.OccupationTags, AnswerTagConstants.TalConsumerOccupationTpdLoading, (decimal?)null)
                );
        }
    }
}
