using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.Underwriting.Models.Event;

namespace TAL.QuoteAndApply.Underwriting.Models.Dto
{
    public class InterviewReferenceInformation
    {
        public InterviewReferenceInformation(string interviewIdentifier, string templateVersion, string concurrencyToken,
            InterviewOccupationInformation occupationInformation,
            IEnumerable<UnderwritingBenefitResponseStatus> benefitStatuses)
        {
            InterviewIdentifier = interviewIdentifier;
            TemplateVersion = templateVersion;
            ConcurrencyToken = concurrencyToken;
            OccupationInformation = occupationInformation;
            BenefitStatuses = benefitStatuses;
        }

        public string InterviewIdentifier { get; private set; }

        // for some reason template version is used when you create and templateid is used when getting an interview
        public string TemplateVersion { get; private set; }
        public string ConcurrencyToken { get; private set; }
        public InterviewOccupationInformation OccupationInformation { get; private set; }
        public IEnumerable<UnderwritingBenefitResponseStatus> BenefitStatuses { get; private set; }
    }

    public class InterviewOccupationInformation
    {
        public string OccupationText { get; set; }
        public string IndustryText { get; set; }
        public IEnumerable<string> OccupationTags { get; set; }
        public string OccupationAnswerId { get; set; }
        public string IndustryAnswerId { get; set; }
        public IEnumerable<string> IndustryTags { get; set; }

        public InterviewOccupationInformation(ReadOnlyQuestion industryQuestion, string industryAnswerId,
            ReadOnlyQuestion occupationQuestion, string occupationAnswerId)
        {
            if (industryQuestion != null)
            {
                var industryAnswer = industryQuestion.Answers.Single(a => a.ResponseId == industryAnswerId);
                IndustryText = industryAnswer.Text;
                IndustryTags = industryAnswer.Tags.ToArray();
                IndustryAnswerId = industryAnswerId;
            }
            if (occupationQuestion != null)
            {
                var occupationAnswer = occupationQuestion.Answers.Single(a => a.ResponseId == occupationAnswerId);
                OccupationText = occupationAnswer.Text;
                OccupationTags = occupationAnswer.Tags.ToArray();
                OccupationAnswerId = occupationAnswerId;
            }
        }

    }
}