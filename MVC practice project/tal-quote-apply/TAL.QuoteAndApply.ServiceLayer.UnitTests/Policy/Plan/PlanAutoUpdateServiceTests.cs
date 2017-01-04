using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.DataModel.Product;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.ServiceLayer.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.MarketingStatus;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.Plan;
using TAL.QuoteAndApply.ServiceLayer.Product;
using TAL.QuoteAndApply.ServiceLayer.Product.Rules;

namespace TAL.QuoteAndApply.ServiceLayer.UnitTests.Policy.Plan
{
    [TestFixture]
    public class PlanAutoUpdateServiceTests
    {
        private Mock<IAvailablePlanOptionsProvider> _mockAvailabilityPlanOptionsProvider;
        private Mock<IPlanService> _mockPlanService;
        private Mock<IPlanEligibilityService> _mockPlanEligiblityService;
        private Mock<ICoverService> _mockCoverService;
        private Mock<ICoverEligibilityService> _mockCoverEligibilityService;
        private Mock<IRiskService> _mockRiskService;
        private Mock<IPlanCoverAmountService> _mockPlanCoverAmountService;
        private Mock<IUpdateMarketingStatusService> _mockMarketingStatusService;

        [TestFixtureSetUp]
        public void Setup()
        {
            var mockRepo = new MockRepository(MockBehavior.Strict);

            _mockAvailabilityPlanOptionsProvider = mockRepo.Create<IAvailablePlanOptionsProvider>();
            _mockPlanService = mockRepo.Create<IPlanService>();
            _mockPlanEligiblityService = mockRepo.Create<IPlanEligibilityService>();
            _mockCoverService = mockRepo.Create<ICoverService>();
            _mockCoverEligibilityService = mockRepo.Create<ICoverEligibilityService>();
            _mockRiskService = mockRepo.Create<IRiskService>();
            _mockPlanCoverAmountService = mockRepo.Create<IPlanCoverAmountService>();
            _mockMarketingStatusService = mockRepo.Create<IUpdateMarketingStatusService>();
        }

        private IPlanAutoUpdateService GetService()
        {
            return new PlanAutoUpdateService(_mockAvailabilityPlanOptionsProvider.Object, _mockPlanService.Object,
                _mockPlanEligiblityService.Object, _mockCoverService.Object, _mockCoverEligibilityService.Object, 
                _mockRiskService.Object, _mockPlanCoverAmountService.Object, _mockMarketingStatusService.Object);
        }

        [Test]
        public void UpdatePlanStateToConformToProductRules_PlanStateThatIsValidForAvailability_NoChangeToPlanState()
        {
            // Setup
            var availableOptions = new[] {"MR"};
            var availableCovers = new[] { "C1", "C2", "C3" };
            var availablePlans = new[] { "P1", "P2", "P3" };
            var availableRiders = new[]
            {
                new AvailableRiderOptionsAndConfigResult()
                {
                    AvailableCovers = new[] {"RC1", "RC2", "RC3"},
                    AvailableOptions = new[] {"RMR"},
                    RiderCode = "R1",
                    UnavailableFeatures = new AvailableFeature[0]
                }
            };

            var availabilityResult = new AvailablePlanOptionsAndConfigResult("P1", availableOptions, availableCovers,
                availablePlans, availableRiders, new AvailableFeature[0], new AvailableFeature[0]);


            _mockAvailabilityPlanOptionsProvider.Setup(call => call.GetForPlan(It.IsAny<AvailabilityPlanStateParam>()))
                .Returns(availabilityResult);

            var service = GetService();

            var planStateParam = PlanStateParam.BuildPlanStateParam("P1", "tal", true, 1, 1, null, 250000, null, PremiumType.Stepped, 1, 30,
                100000, null, null, OccupationDefinition.AnyOccupation,
                new[]
                {
                    PlanStateParam.BuildRiderPlanStateParam("R1", "tal", true, 1, 1, null, 0, null, PremiumType.Stepped, 2, 30, 100000,
                        new[] {new OptionsParam("RMR", true)}, new [] {"RC1", "RC2", "RC3"}, 250000, OccupationDefinition.AnyOccupation)
                },
                new[] {new OptionsParam("MR", true)}, new[]
                {
                    new PlanIdentityInfo(1, "P1", true),
                    new PlanIdentityInfo(1, "P2", true),
                    new PlanIdentityInfo(1, "P3", true),
                },
                new[] {"C1", "C2", "C3"});

            var mockRisk = new RiskDto();
            _mockRiskService.Setup(call => call.GetRisk(planStateParam.RiskId)).Returns(mockRisk);

            // Action
            var result = service.UpdatePlanStateToConformToProductRules(planStateParam);
            
            //Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.UpdatedPlanState, Is.Not.Null);
            Assert.That(result.UpdatedPlanState.SelectedCoverCodes, Is.Not.Null);
            Assert.That(result.UpdatedPlanState.SelectedCoverCodes, Contains.Item("C1"));
            Assert.That(result.UpdatedPlanState.SelectedCoverCodes, Contains.Item("C2"));
            Assert.That(result.UpdatedPlanState.SelectedCoverCodes, Contains.Item("C3"));

            Assert.That(result.UpdatedPlanState.AllPlans.Where(p => p.Selected).Select(p => p.PlanCode), Contains.Item("P1"));
            Assert.That(result.UpdatedPlanState.AllPlans.Where(p => p.Selected).Select(p => p.PlanCode), Contains.Item("P2"));
            Assert.That(result.UpdatedPlanState.AllPlans.Where(p => p.Selected).Select(p => p.PlanCode), Contains.Item("P3"));

            Assert.That(result.UpdatedPlanState.PlanOptions.Where(po => po.Selected.Value).Select(po => po.Code), Contains.Item("MR"));

            Assert.That(result.UpdatedPlanState.Riders, Is.Not.Null);
            Assert.That(result.UpdatedPlanState.Riders.First().Selected, Is.True);
            Assert.That(result.UpdatedPlanState.Riders.First().SelectedCoverCodes, Contains.Item("RC1"));
            Assert.That(result.UpdatedPlanState.Riders.First().SelectedCoverCodes, Contains.Item("RC3"));
            Assert.That(result.UpdatedPlanState.Riders.First().SelectedCoverCodes, Contains.Item("RC2"));

            Assert.That(result.UpdatedPlanState.Riders.First().PlanOptions.Where(po => po.Selected.Value).Select(po => po.Code), Contains.Item("RMR"));
        }

        [Test]
        public void UpdatePlanStateToConformToProductRules_PlanStateThatIsNotValidForAvailability_ChangeToPlanState()
        {
            // Setup
            var availableOptions = new[] { "MR" };
            var availableCovers = new[] { "C1", "C2", "C3" };
            var availablePlans = new[] { "P1", "P2", "P3" };
            var availableRiders = new[]
            {
                new AvailableRiderOptionsAndConfigResult()
                {
                    AvailableCovers = new[] {"RC1", "RC2"},
                    AvailableOptions = new[] {"RMR"},
                    RiderCode = "R1",
                    UnavailableFeatures = new AvailableFeature[0]
                }
            };

            var availabilityResult = new AvailablePlanOptionsAndConfigResult("P1", availableOptions, availableCovers,
                availablePlans, availableRiders, new AvailableFeature[0], new AvailableFeature[0]);


            _mockAvailabilityPlanOptionsProvider.Setup(call => call.GetForPlan(It.IsAny<AvailabilityPlanStateParam>()))
                .Returns(availabilityResult);

            var service = GetService();

            var planStateParam = PlanStateParam.BuildPlanStateParam("P1", "tal", true, 1, 1, null, 250000, null, PremiumType.Stepped, 1, 30,
                100000, null, null, OccupationDefinition.AnyOccupation,
                new[]
                {
                    PlanStateParam.BuildRiderPlanStateParam("R1", "tal", true, 1, 1, null, 0, null, PremiumType.Stepped, 2, 30, 100000,
                        new[] {new OptionsParam("RMR", true)}, new [] {"RC1", "RC2", "RC3"}, 250000, OccupationDefinition.AnyOccupation)
                },
                new[] { new OptionsParam("MR", true) }, new[]
                {
                    new PlanIdentityInfo(1, "P1", true),
                    new PlanIdentityInfo(1, "P2", true),
                    new PlanIdentityInfo(1, "P3", true),
                },
                new[] { "C1", "C2", "C3" });

            var mockRisk = new RiskDto();
            _mockRiskService.Setup(call => call.GetRisk(planStateParam.RiskId)).Returns(mockRisk);

            // Action
            var result = service.UpdatePlanStateToConformToProductRules(planStateParam);

            //Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.UpdatedPlanState, Is.Not.Null);
            Assert.That(result.UpdatedPlanState.SelectedCoverCodes, Is.Not.Null);
            Assert.That(result.UpdatedPlanState.SelectedCoverCodes, Contains.Item("C1"));
            Assert.That(result.UpdatedPlanState.SelectedCoverCodes, Contains.Item("C2"));
            Assert.That(result.UpdatedPlanState.SelectedCoverCodes, Contains.Item("C3"));

            Assert.That(result.UpdatedPlanState.AllPlans.Where(p => p.Selected).Select(p => p.PlanCode), Contains.Item("P1"));
            Assert.That(result.UpdatedPlanState.AllPlans.Where(p => p.Selected).Select(p => p.PlanCode), Contains.Item("P2"));
            Assert.That(result.UpdatedPlanState.AllPlans.Where(p => p.Selected).Select(p => p.PlanCode), Contains.Item("P3"));

            Assert.That(result.UpdatedPlanState.PlanOptions.Where(po => po.Selected.Value).Select(po => po.Code), Contains.Item("MR"));

            Assert.That(result.UpdatedPlanState.Riders, Is.Not.Null);
            Assert.That(result.UpdatedPlanState.Riders.First().SelectedCoverCodes, Contains.Item("RC1"));
            Assert.That(result.UpdatedPlanState.Riders.First().SelectedCoverCodes, Contains.Item("RC2"));
            Assert.That(result.UpdatedPlanState.Riders.First().SelectedCoverCodes, Is.Not.Contains("RC3"));

            Assert.That(result.UpdatedPlanState.Riders.First().PlanOptions.Where(po => po.Selected.Value).Select(po => po.Code), Contains.Item("RMR"));
        }

        [Test]
        public void UpdatePlansToConformWithPlanEligiblityRules_IneligiblePlan_Saved()
        {
            var mockRisk = new RiskDto {Id = 111};
            var mockPlan = new PlanDto {Id = 987, Selected = true};
            var allPlans = new[] { mockPlan };
            var mockCoverElegibilityResults = new List<CoverEligibilityResult>
            {
                CoverEligibilityResult.Eligible("COVER")
            };
            var mockEligibility = new AvailableFeature("MOCK", false, new List<string> {"I'm invalid yo"});
            var mockCovers = new List<ICover>
            {
                new CoverDto {Code = "COVER"}
            };

            _mockPlanService.Setup(call => call.GetPlansForRisk(mockRisk.Id)).Returns(allPlans);
            _mockPlanService.Setup(call => call.UpdatePlan(mockPlan));
            _mockCoverService.Setup(call => call.GetCoversForPlan(987)).Returns(mockCovers);
            _mockPlanEligiblityService.Setup(call => call.IsRiskEligibleForPlan(mockRisk, mockPlan, mockCoverElegibilityResults)).Returns(mockEligibility);
            _mockCoverEligibilityService.Setup(call => call.GetCoverEligibilityResults(mockCovers)).Returns(mockCoverElegibilityResults);
            _mockPlanCoverAmountService.Setup(
                call => call.ChangePlanCoverAmountToMinOrMaxIfApplicable(mockRisk, mockPlan, allPlans))
                .Returns(new PlanCoverAmountToMaxResult(mockPlan, false));

            _mockMarketingStatusService.Setup(call => call.UpdateMarketingStatusForRisk(mockRisk.Id));

            var svc = GetService();
            svc.UpdatePlansToConformWithPlanEligiblityRules(mockRisk);
        }
    }
}