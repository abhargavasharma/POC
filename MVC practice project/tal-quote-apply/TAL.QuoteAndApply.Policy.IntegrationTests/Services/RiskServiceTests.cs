using System;
using System.Collections.Generic;
using NUnit.Framework;
using TAL.QuoteAndApply.DataModel.Personal;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Rules.Risk;
using TAL.QuoteAndApply.Policy.Service;

namespace TAL.QuoteAndApply.Policy.IntegrationTests.Services
{
    [TestFixture]
    public class RiskServiceTests
    {
        private RiskProductDefinition _riskProductDefinition;

        [TestFixtureSetUp]
        public void Setup()
        {
            _riskProductDefinition = new RiskProductDefinition();
            _riskProductDefinition.MinimumAnnualIncomeDollars = 100000;
            _riskProductDefinition.AustralianResidencyRequired = true;
            _riskProductDefinition.MaximumEntryAgeNextBirthday = 20;
            _riskProductDefinition.MinimumEntryAgeNextBirthday = 18;
        }

        [Test]
        public void IsRiskValidForInforce_RiskIsValidForInforce_ReturnsTrue()
        {
            var risk = new RiskDto
            {
                AnnualIncome = 100001,
                SmokerStatus = SmokerStatus.No,
                DateOfBirth = DateTime.Now.AddYears(-19),
                Gender = Gender.Female,
                OccupationClass = "a",
                OccupationCode = "a",
                OccupationTitle = "a",
                Residency = ResidencyStatus.Australian
            };

            var svc = GetService();
            var result = svc.IsRiskValidForInforce(_riskProductDefinition, risk);

            Assert.That(result, Is.True);
        }

        [TestCase((long)-1)]
        [TestCase((long)99999)]
        public void IsRiskValidForInforce_AnnualIncomeInvalid_ReturnsFalse(long annualIncome)
        {
            var risk = new RiskDto
            {
                AnnualIncome = annualIncome,
                SmokerStatus = SmokerStatus.No,
                DateOfBirth = DateTime.Now.AddYears(-19),
                Gender = Gender.Female,
                OccupationClass = "a",
                OccupationCode = "a",
                OccupationTitle = "a",
                Residency = ResidencyStatus.Australian
            };

            var svc = GetService();
            var result = svc.IsRiskValidForInforce(_riskProductDefinition, risk);

            Assert.That(result, Is.False);
        }

        [Test]
        public void IsRiskValidForInforce_SmokerStatusUnknown_ReturnsFalse()
        {
            var risk = new RiskDto
            {
                AnnualIncome = 100001,
                SmokerStatus = SmokerStatus.Unknown,
                DateOfBirth = DateTime.Now.AddYears(-19),
                Gender = Gender.Female,
                OccupationClass = "a",
                OccupationCode = "a",
                OccupationTitle = "a",
                Residency = ResidencyStatus.Australian
            };

            var svc = GetService();
            var result = svc.IsRiskValidForInforce(_riskProductDefinition, risk);

            Assert.That(result, Is.False);
        }

        [TestCase(17)]
        [TestCase(21)]
        public void IsRiskValidForInforce_AgeInvalid_ReturnsFalse(int age)
        {
            var risk = new RiskDto
            {
                AnnualIncome = 100001,
                SmokerStatus = SmokerStatus.No,
                DateOfBirth = DateTime.Now.AddYears(- age),
                Gender = Gender.Female,
                OccupationClass = "a",
                OccupationCode = "a",
                OccupationTitle = "a",
                Residency = ResidencyStatus.Australian
            };

            var svc = GetService();
            var result = svc.IsRiskValidForInforce(_riskProductDefinition, risk);

            Assert.That(result, Is.False);
        }

        [Test]
        public void IsRiskValidForInforce_ResidencyUnknown_ReturnsFalse()
        {
            var risk = new RiskDto
            {
                AnnualIncome = 100001,
                SmokerStatus = SmokerStatus.No,
                DateOfBirth = DateTime.Now.AddYears(-19),
                Gender = Gender.Female,
                OccupationClass = "a",
                OccupationCode = "a",
                OccupationTitle = "a",
                Residency = ResidencyStatus.Unknown
            };

            var svc = GetService();
            var result = svc.IsRiskValidForInforce(_riskProductDefinition, risk);

            Assert.That(result, Is.False);
        }

        private RiskService GetService()
        {
            return new RiskService(null, new RiskRulesService(new RiskRulesFactory()), null, null, null);
        }
    }
}
