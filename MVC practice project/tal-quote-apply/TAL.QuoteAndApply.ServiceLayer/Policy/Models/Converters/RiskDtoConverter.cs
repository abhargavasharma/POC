using System;
using TAL.QuoteAndApply.DataModel.Personal;
using TAL.QuoteAndApply.Party.Models;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Models.Converters;
using TAL.QuoteAndApply.ServiceLayer.Underwriting;
using TAL.QuoteAndApply.Underwriting.Models.Dto;


namespace TAL.QuoteAndApply.ServiceLayer.Policy.Models.Converters
{
    public interface IRiskDtoConverter
    {
        IRisk CreateFrom(IPolicy policy, IParty party, RatingFactorsParam ratingFactors, InterviewReferenceInformation interview);
        IRisk UpdateFrom(IRisk risk, RatingFactorsParam ratingFactors, InterviewReferenceInformation interview);
    }

    public class RiskDtoConverter : IRiskDtoConverter
    {
        private readonly IOccupationQuestionProvider _occupationQuestionProvider;
        private readonly IResidencyStatusConverter _residencyStatusConverter;

        public RiskDtoConverter(IOccupationQuestionProvider occupationQuestionProvider, 
            IResidencyStatusConverter residencyStatusConverter)
        {
            _occupationQuestionProvider = occupationQuestionProvider;
            _residencyStatusConverter = residencyStatusConverter;
        }

        public IRisk CreateFrom(IPolicy policy, IParty party, RatingFactorsParam ratingFactors, InterviewReferenceInformation interview)
        {
            string occupationTitle = null;
            string occupationClass = null;
            string pasCode = null;
            string industryTitle = null;
            string industryCode = null;
            bool isTpdAny = false;
            bool isTpdOwn = false;
            decimal? tpdLoading = null;

            if (!string.IsNullOrEmpty(ratingFactors.OccupationCode))
            {
                var occupationClassAndText = _occupationQuestionProvider.GetForInterview(interview);
                occupationTitle = occupationClassAndText.OccupationText;
                occupationClass = occupationClassAndText.OccupationClass;
                pasCode = occupationClassAndText.PasCode;
                industryTitle = occupationClassAndText.IndustryText;
                industryCode = occupationClassAndText.IndustryCode;
                isTpdAny = occupationClassAndText.IsTpdAny;
                isTpdOwn = occupationClassAndText.IsTpdOwn;
                tpdLoading = occupationClassAndText.TpdLoading;
            }

            var retVal = new RiskDto
            {
                AnnualIncome = ratingFactors.Income,
                DateOfBirth = ratingFactors.DateOfBirth,
                Gender = ratingFactors.Gender == 'M' ? Gender.Male : Gender.Female,
                OccupationClass = occupationClass,
                OccupationTitle = occupationTitle,
                OccupationCode = ratingFactors.OccupationCode,
                IndustryTitle = industryTitle,
                IndustryCode = industryCode,
                PasCode = pasCode,
                IsTpdAny = isTpdAny,
                IsTpdOwn = isTpdOwn,
                TpdLoading = tpdLoading,
                SmokerStatus = ratingFactors.SmokerStatus.Status,
                Residency = _residencyStatusConverter.GetResidency(ratingFactors.IsAustralianResident),
                InterviewId = interview.InterviewIdentifier,
                InterviewConcurrencyToken = interview.ConcurrencyToken,
                InterviewTemplateVersion = interview.TemplateVersion,
                PartyId = party.Id,
                PolicyId = policy.Id,
                LprBeneficiary = true
            };

            return retVal;
        }

        public IRisk UpdateFrom(IRisk risk, RatingFactorsParam ratingFactors, InterviewReferenceInformation interview)
        {
            string occupationTitle = risk.OccupationTitle;
            string occupationClass = risk.OccupationClass;
            string pasCode = risk.PasCode;
            string industryTitle = risk.IndustryTitle;
            string industryCode = risk.IndustryCode;
            bool isTpdAny = risk.IsTpdAny;
            bool isTpdOwn = risk.IsTpdOwn;
            decimal? tpdLoading = risk.TpdLoading;

            if (!string.IsNullOrEmpty(ratingFactors.OccupationCode))
            {
                var occupationClassAndText = _occupationQuestionProvider.GetForInterview(interview);
                if (occupationClassAndText != null)
                {
                    occupationTitle = occupationClassAndText.OccupationText;
                    occupationClass = occupationClassAndText.OccupationClass;
                    pasCode = occupationClassAndText.PasCode;
                    industryTitle = occupationClassAndText.IndustryText;
                    industryCode = occupationClassAndText.IndustryCode;
                    isTpdAny = occupationClassAndText.IsTpdAny;
                    isTpdOwn = occupationClassAndText.IsTpdOwn;
                    tpdLoading = occupationClassAndText.TpdLoading;
                }
            }

            risk.InterviewConcurrencyToken = interview.ConcurrencyToken;       
            risk.DateOfBirth = ratingFactors.DateOfBirth;
            risk.Gender = ratingFactors.Gender == 'M' ? Gender.Male : Gender.Female;
            risk.OccupationClass = occupationClass;
            risk.OccupationTitle = occupationTitle;
            risk.IndustryTitle = industryTitle;
            risk.IndustryCode = industryCode;
            risk.PasCode = pasCode;
            risk.IsTpdAny = isTpdAny;
            risk.IsTpdOwn = isTpdOwn;
            risk.TpdLoading = tpdLoading;
            risk.OccupationCode = ratingFactors.OccupationCode;
            risk.SmokerStatus = ratingFactors.SmokerStatus.Status;
            risk.Residency = _residencyStatusConverter.GetResidency(ratingFactors.IsAustralianResident);
            risk.AnnualIncome = ratingFactors.Income;

            return risk;
        }
    }
}
