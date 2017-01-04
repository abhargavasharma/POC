using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using TAL.QuoteAndApply.DataModel.Personal;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.DataModel.Product;
using TAL.QuoteAndApply.PremiumCalculation.Data;
using TAL.QuoteAndApply.PremiumCalculation.Models;
using TAL.QuoteAndApply.PremiumCalculation.Services;

namespace TAL.QuoteAndApply.PremiumCalculation.UnitTests.Services
{
    [TestFixture]
    public class GetFactorBCalculatorInputServiceTests
    {
        private Mock<IModalFrequencyFactorDtoRepository> _modalFrequencyFactorDtoRepository;
        private Mock<IPremiumReliefFactorDtoRepository> _premiumReliefFactorRepository;

        const int BrandId = 1;

        [TestFixtureSetUp]
        public void Setup()
        {
            var mockRepo = new MockRepository(MockBehavior.Strict);
            _modalFrequencyFactorDtoRepository = mockRepo.Create<IModalFrequencyFactorDtoRepository>();
            _premiumReliefFactorRepository = mockRepo.Create<IPremiumReliefFactorDtoRepository>();
        }

        [Test, ExpectedException(typeof(ApplicationException))]
        public void GetFactorBCalculatorInput_NullPremiumReliefFactor_ExceptionThrown()
        {
            var mockPremCalcRequest = GetMockPremiumCalculatorFactors(true);

            _premiumReliefFactorRepository.Setup(call => call.GetPremiumReliefFactor(true, BrandId))
                .Returns(()=> null);
            
            var svc = GetService();
            svc.GetFactorBCalculatorInput(mockPremCalcRequest);
        }

        [Test, ExpectedException(typeof(ApplicationException))]
        public void GetFactorBCalculatorInput_NullModalFrequency_ExceptionThrown()
        {
            var mockPremCalcRequest = GetMockPremiumCalculatorFactors(true);

            _premiumReliefFactorRepository.Setup(call => call.GetPremiumReliefFactor(true, BrandId))
                .Returns(new PremiumReliefFactorDto {Factor = .5m});

            _modalFrequencyFactorDtoRepository.Setup(
                call => call.GetModalFrequencyFactorForPremiumFrequency(mockPremCalcRequest.PolicyFactors.PremiumFrequency, BrandId)).Returns(() => null);

            var svc = GetService();
            svc.GetFactorBCalculatorInput(mockPremCalcRequest);
        }

        [TestCase(true, true)]
        [TestCase(false, false)]
        [TestCase(null, false)]
        public void GetFactorBCalculatorInput_WithPremiumReliefOption_FactorBInputReturned(bool? premiumReliefSelectedValue, bool expectedPremiumReliefOption)
        {
            var mockPremCalcRequest = GetMockPremiumCalculatorFactors(premiumReliefSelectedValue);

            _premiumReliefFactorRepository.Setup(call => call.GetPremiumReliefFactor(expectedPremiumReliefOption, BrandId))
                .Returns(new PremiumReliefFactorDto { Factor = .5m });

            _modalFrequencyFactorDtoRepository.Setup(
                call => call.GetModalFrequencyFactorForPremiumFrequency(mockPremCalcRequest.PolicyFactors.PremiumFrequency, BrandId)).Returns(new ModalFrequencyFactorDto {Factor = 1m});

            var svc = GetService();
            var result = svc.GetFactorBCalculatorInput(mockPremCalcRequest);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ModalFrequencyFactor, Is.EqualTo(1m));
            Assert.That(result.PremiumReliefOptionFactor, Is.EqualTo(.5m));
        }

        private PremiumCalculatorFactors GetMockPremiumCalculatorFactors(bool? premiumReliefOptionSelected)
        {
            var brandId = 1;
            var premCalcRequest = new PremiumCalculationRequest(PremiumFrequency.Monthly,
                new List<RiskCalculationRequest>
                {
                    new RiskCalculationRequest(1, 18, Gender.Male, true, null, new List<PlanCalculationRequest>
                    {
                        new PlanCalculationRequest("ABC", true, true, 100000, PremiumType.Level, null, null, null, OccupationDefinition.Unknown, null, premiumReliefOptionSelected,false, false,
                            new List<CoverCalculationRequest> {new CoverCalculationRequest("123", true, true, true, true, new Loadings(0, 0), brandId) }),
                    })
                }, brandId);

            var risk = premCalcRequest.Risks.First();
            var plan = risk.Plans.First();
            var cover = plan.Covers.First();
            return new PremiumCalculatorFactors(premCalcRequest, risk, plan, cover);
        }

        private GetFactorBCalculatorInputService GetService()
        {
            return new GetFactorBCalculatorInputService(_modalFrequencyFactorDtoRepository.Object, _premiumReliefFactorRepository.Object);
        }
    }
}