using System;
using TAL.QuoteAndApply.DataModel.Personal;
using TAL.QuoteAndApply.Party.Models;
using TAL.QuoteAndApply.Policy.Models;

namespace TAL.QuoteAndApply.Tests.Shared.Builders
{
    public class RiskBuilder
    {
        private readonly RiskDto _risk;
        private readonly RiskOccupationDto _riskOccupation;

        public RiskBuilder()
        {
            _risk = new RiskDto();
            _riskOccupation = new RiskOccupationDto();
        }

        public RiskBuilder WithPolicyId(int policyId)
        {
            _risk.PolicyId = policyId;
            return this;
        }

        public RiskBuilder WithPartyId(int partyId)
        {
            _risk.PartyId = partyId;
            return this;
        }

        public RiskBuilder WithInterviewId(string interviewId)
        {
            _risk.InterviewId = interviewId;
            return this;
        }

        public RiskBuilder WithAnnualIncome(long annualIncome)
        {
            _risk.AnnualIncome = annualIncome;
            return this;
        }

        public RiskBuilder WithSmokerStatus(SmokerStatus smokerStatus)
        {
            _risk.SmokerStatus = smokerStatus;
            return this;
        }

        public RiskBuilder WithDateOfBirth(DateTime dateOfBirth)
        {
            _risk.DateOfBirth = dateOfBirth;
            return this;
        }

        public RiskBuilder WithGender(Gender gender)
        {
            _risk.Gender = gender;
            return this;
        }

        public RiskBuilder WithResidency(ResidencyStatus residencyStatus)
        {
            _risk.Residency = residencyStatus;
            return this;
        }

        public RiskBuilder WithOccupationClass(string occupationClass)
        {
            _riskOccupation.OccupationClass = occupationClass;
            return this;
        }

        public RiskBuilder WithOccupationTitle(string occupationTitle)
        {
            _riskOccupation.OccupationTitle = occupationTitle;
            return this;
        }

        public RiskBuilder WithOccupationCode(string occupationCode)
        {
            _riskOccupation.OccupationCode = occupationCode;
            return this;
        }

        public RiskBuilder WithIndustryTitle(string occupationTitle)
        {
            _riskOccupation.OccupationTitle = occupationTitle;
            return this;
        }

        public RiskBuilder WithIndustryCode(string occupationCode)
        {
            _riskOccupation.OccupationCode = occupationCode;
            return this;
        }

        public RiskBuilder Default()
        {
            _risk.SmokerStatus = SmokerStatus.No;
            _risk.AnnualIncome = 100000;
            _risk.DateOfBirth = DateTime.Today.AddYears(-30);
            _riskOccupation.OccupationClass = "AA";
            _riskOccupation.OccupationCode = "ch";
            _riskOccupation.OccupationTitle = "Account Executive";
            _riskOccupation.IndustryTitle = "Sales & Marketing";
            _riskOccupation.IndustryCode = "dh";
            _risk.Gender = Gender.Male;
            _risk.Residency = ResidencyStatus.Australian;

            return this;
        }

        public IRisk Build()
        {
            _risk.AssignOccupationProperties(_riskOccupation);
            return _risk;
        }

        public IOccupationRatingFactors BuildOccupation()
        {
            return _riskOccupation;
        }
    }
}