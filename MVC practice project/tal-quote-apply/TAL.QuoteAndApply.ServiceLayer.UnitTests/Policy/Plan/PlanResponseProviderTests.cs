using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.DataModel.Underwriting;
using TAL.QuoteAndApply.Policy.Data;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.Plan;
using TAL.QuoteAndApply.ServiceLayer.Product;

namespace TAL.QuoteAndApply.ServiceLayer.UnitTests.Policy.Plan
{
    [TestFixture]
    public class PlanResponseProviderTests
    {
        private Mock<ICoverService> _mockCoverService;
        private Mock<IOptionService> _mockOptionService;
        private Mock<IPolicyService> _mockPolicyService;
        private Mock<IPlanVariableResponseConverter> _mockPlanResponseConverter;
        private Mock<ICoverExclusionsService> _mockCoverExclusionsService;

        [TestFixtureSetUp]
        public void Setup()
        {
            var mockRepo = new MockRepository(MockBehavior.Strict);
            _mockCoverService = mockRepo.Create<ICoverService>();
            _mockOptionService = mockRepo.Create<IOptionService>();
            _mockPolicyService = mockRepo.Create<IPolicyService>();
            _mockPlanResponseConverter = mockRepo.Create<IPlanVariableResponseConverter>();
            _mockCoverExclusionsService = mockRepo.Create<ICoverExclusionsService>();
        }

        private IPlanResponseProvider GetProvider()
        {
            return new PlanResponseProvider(_mockCoverService.Object, _mockOptionService.Object,
                _mockPolicyService.Object, _mockPlanResponseConverter.Object, _mockCoverExclusionsService.Object);
        }

        [Test]
        public void MapStoredPlansToProduct_RiderSelected_PremiumIncludingRidersIncludesRiderPremiums()
        {
            //Arrange
            var provider = GetProvider();
            var mockData = SetupDefaultMockData(riderSelected: true);

            //Act
            provider.MapStoredPlansToProduct(mockData.QuoteReference, mockData.PlanResponses, mockData.SavedPlans);

            //Assert
            Assert.That(mockData.PlanResponses.First().Premium, Is.EqualTo(2));
            Assert.That(mockData.PlanResponses.First().PremiumIncludingRiders, Is.EqualTo(5));
        }

        [Test]
        public void MapStoredPlansToProduct_RiderNotSelected_PremiumIncludingRidersDoesNotIncludeRiderPremiums()
        {
            //Arrange
            var provider = GetProvider();
            var mockData = SetupDefaultMockData(riderSelected: false);

            //Act
            provider.MapStoredPlansToProduct(mockData.QuoteReference, mockData.PlanResponses, mockData.SavedPlans);

            //Assert
            Assert.That(mockData.PlanResponses.First().Premium, Is.EqualTo(2));
            Assert.That(mockData.PlanResponses.First().PremiumIncludingRiders, Is.EqualTo(2));
        }

        [Test]
        public void MapStoredPlansToProduct_CoverHasExclusions_ExclusionsAreMappedToCoversAndRiders()
        {
            //Arrange
            var provider = GetProvider();
            var mockData = SetupDefaultMockData(riderSelected: false);
            var exclusions = new List<ICoverExclusion>
            {
                new CoverExclusionDto {Name = "Exclusion", Text = "Exclusion Text"}
            };

            _mockCoverExclusionsService.Setup(s => s.GetExclusionsForCover(It.IsAny<ICover>()))
                .Returns(exclusions);

            //Act
            provider.MapStoredPlansToProduct(mockData.QuoteReference, mockData.PlanResponses, mockData.SavedPlans);

            //Assert
            AssertAllCoversHaveDefaultExclusions(mockData.PlanResponses.First());

            foreach (var rider in mockData.PlanResponses.First().Riders)
            {
                AssertAllCoversHaveDefaultExclusions(rider);
            }
        }

        private static void AssertAllCoversHaveDefaultExclusions(PlanResponse plan)
        {
            //Is the default exclusion attached to all covers on a plan
            foreach (var cover in plan.Covers)
            {
                Assert.That(cover.Exclusions.Count(), Is.EqualTo(1));
                Assert.That(cover.Exclusions.First().Name, Is.EqualTo("Exclusion"));
                Assert.That(cover.Exclusions.First().Description, Is.EqualTo("Exclusion Text"));
            }
        }

        private class PlanResponseMockData
        {
            public string QuoteReference { get; set; }
            public IEnumerable<PlanResponse> PlanResponses { get; set; }
            public IEnumerable<PlanOverviewResult> SavedPlans { get; set; }
        }

        private PlanResponseMockData SetupDefaultMockData(bool riderSelected)
        {
            /*
                - This sets up a mock plan with one cover. The plan has one rider that has one cover.
                - Mocks saved plan data with some premiums and the plan selected
                - You can optionally pass in if rider on the plan is selected or not
                - Will also setup mock calls for MapStoredPlansToProduct to run properly
                TODO: If going to write more tests then turn PlanResponse and PlanOverviewResult into builder pattern to be of more use in mocking up different test scenarios
            */

            const int mainPlanId = 1;
            const int riderPlanId = 2;
            const int planCoverId = 3;
            const int riderCoverId = 4;

            var policyDto = new PolicyDto
            {
                PremiumFrequency = PremiumFrequency.Monthly,
                Progress = PolicyProgress.Unknown
            };

            var planCovers = new List<ICover>
            {
                new CoverDto {PlanId = planCoverId, Code = "PlanCover1", UnderwritingStatus = UnderwritingStatus.Accept, Premium = 2}
            };

            var riderCovers = new List<ICover>
            {
                new CoverDto {PlanId = riderCoverId, Code = "RiderCover1", UnderwritingStatus = UnderwritingStatus.Accept, Selected = true, Premium = 3}
            };

            var mockData = new PlanResponseMockData()
            {
                QuoteReference = "RANDY",
                PlanResponses = new List<PlanResponse>
                {
                    new PlanResponse
                    {
                        PlanId = mainPlanId,
                        Code = "Plan1",
                        Covers = new List<CoverResponse>
                        {
                            new CoverResponse {Code = "PlanCover1"}
                        },
                        Riders = new List<PlanResponse>
                        {
                            new PlanResponse
                            {
                                PlanId = riderPlanId,
                                Code = "Rider1",
                                Covers = new List<CoverResponse>
                                {
                                    new CoverResponse {Code = "RiderCover1"}
                                },
                                Options = new List<OptionResponse>()
                            }
                        },
                        Options = new List<OptionResponse>()
                    }
                },
                SavedPlans = new List<PlanOverviewResult>
                {
                    new PlanOverviewResult {PlanId = mainPlanId, Code = "Plan1", Selected = true, Premium = planCovers.Sum(c => c.Premium)},
                    new PlanOverviewResult {PlanId = riderPlanId, Code = "Rider1", ParentPlanId = 1, Selected = riderSelected, Premium = riderCovers.Sum(r => r.Premium)}
                }
            };

            var options = new List<OptionDto>();

            _mockPlanResponseConverter.Setup(s => s.From(It.IsAny<PlanResponse>(), It.IsAny<PlanOverviewResult>()))
                .Returns(new List<VariableResponse>());
            _mockPolicyService.Setup(s => s.GetByQuoteReferenceNumber(mockData.QuoteReference)).Returns(policyDto);
            _mockCoverService.Setup(s => s.GetCoversForPlan(mainPlanId)).Returns(planCovers);
            _mockCoverService.Setup(s => s.GetCoversForPlan(riderPlanId)).Returns(riderCovers);
            _mockOptionService.Setup(s => s.GetOptionsForPlan(It.IsAny<int>())).Returns(options);
            _mockCoverExclusionsService.Setup(s => s.GetExclusionsForCover(It.IsAny<ICover>()))
                .Returns(new List<ICoverExclusion>());

            return mockData;

        }


    }
}
