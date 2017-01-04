using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using TAL.QuoteAndApply.DataModel.Underwriting;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.ServiceLayer.Underwriting;
using TAL.QuoteAndApply.ServiceLayer.Underwriting.Converters;
using TAL.QuoteAndApply.Underwriting.Models.Dto;
using TAL.QuoteAndApply.Underwriting.Models.Event;
using TAL.QuoteAndApply.Underwriting.Models.Talus;

namespace TAL.QuoteAndApply.ServiceLayer.UnitTests.Underwriting
{
    [TestFixture]
    public class CoverUnderwritingSyncServiceTests
    {
        private Mock<IRiskService> _mockRiskService;
        private Mock<IPlanService> _mockPlanService;
        private Mock<ICoverService> _mockCoverService;
        private Mock<ICoverLoadingService> _mockCoverLoadingService;
        private Mock<ICoverLoadingsConverter> _mockCoverLoadingsConverter;
        private Mock<ICover> _mockCover;
        private Mock<IGetUnderwritingBenefitCodeForCoverService> _mockGetUnderwritingBenefitCodeForCoverService;

        private Mock<ICoverExclusionsService> _mockCoverExclusionsService;
        private Mock<ICoverExclusionsConverter> _mockCoverExclusionsConverter;

        [SetUp]
        public void Setup()
        {
            var mockRepo = new MockRepository(MockBehavior.Loose);
            _mockRiskService = mockRepo.Create<IRiskService>();
            _mockPlanService = mockRepo.Create<IPlanService>();
            _mockCoverService = mockRepo.Create<ICoverService>();
            _mockCoverLoadingService = mockRepo.Create<ICoverLoadingService>();
            _mockCoverLoadingsConverter = mockRepo.Create<ICoverLoadingsConverter>();
            _mockCover = mockRepo.Create<ICover>();
            _mockGetUnderwritingBenefitCodeForCoverService =
                mockRepo.Create<IGetUnderwritingBenefitCodeForCoverService>();

            _mockCoverExclusionsService = mockRepo.Create<ICoverExclusionsService>();
            _mockCoverExclusionsConverter = mockRepo.Create<ICoverExclusionsConverter>();
        }

        [TestCase(UnderwritingStatus.Accept)]
        [TestCase(UnderwritingStatus.Decline)]
        [TestCase(UnderwritingStatus.Defer)]
        [TestCase(UnderwritingStatus.Refer)]
        [TestCase(UnderwritingStatus.Incomplete)]
        [TestCase(UnderwritingStatus.MoreInfo)]
        public void Update_SameUnderwritingBenefitCodeAsCoverCode_CoverUnderwritingStatusUpdated(UnderwritingStatus uwStatus)
        {
            var mockRisk = new RiskDto() {Id=123};
            var mockPlan = new PlanDto() {Id = 987 };
            
            var inteviewId = "123";
            var coverCode = "ABC123";
            
            var mockUnderwritingBenefitResponsesChange = new UnderwritingBenefitResponsesChangeParam(inteviewId, new List<UnderwritingBenefitResponseStatus>
            {
                new UnderwritingBenefitResponseStatus(coverCode, uwStatus, new ReadOnlyTotalLoadings(new TotalLoadings()), new List<ReadOnlyExclusion> {new ReadOnlyExclusion(new Exclusion("Test", "Test"))})
            });

            _mockCover.Setup(call => call.Code).Returns(coverCode);
            
            _mockRiskService.Setup(call => call.GetRiskByInterviewId(inteviewId)).Returns(mockRisk);
            _mockPlanService.Setup(call => call.GetPlansForRisk(mockRisk.Id)).Returns(new List<IPlan>() { mockPlan });
            _mockCoverService.Setup(call => call.GetCoversForPlan(mockPlan.Id)).Returns(new List<ICover>() { _mockCover.Object });
            _mockGetUnderwritingBenefitCodeForCoverService.Setup(
                call => call.GetUnderwritingBenefitCodeFrom(_mockCover.Object)).Returns(coverCode);

            _mockCoverService.Setup(call => call.UpdateCover(_mockCover.Object));

            var svc = GetService();
            svc.Update(mockUnderwritingBenefitResponsesChange);

            _mockCover.VerifySet((cover) => cover.UnderwritingStatus = It.Is<UnderwritingStatus>(val => val == uwStatus), Times.Once);
        }

        [Test]
        public void Update_DifferentUnderwritingBenefitCodeToCoverCode_CoverUnderwritingStatusUpdated()
        {
            var mockRisk = new RiskDto() { Id = 123 };
            var mockPlan = new PlanDto() { Id = 987 };

            var inteviewId = "123";
            var coverCode = "ABC123";
            var uwBenefitCode = "UW987";
            var uwStatus = UnderwritingStatus.Accept;

            var mockUnderwritingBenefitResponsesChange = new UnderwritingBenefitResponsesChangeParam(inteviewId, new List<UnderwritingBenefitResponseStatus>
            {
                new UnderwritingBenefitResponseStatus(uwBenefitCode, uwStatus, new ReadOnlyTotalLoadings(new TotalLoadings()), new List<ReadOnlyExclusion> {new ReadOnlyExclusion(new Exclusion("Test", "Test"))})
            });

            _mockCover.Setup(call => call.Code).Returns(coverCode);

            _mockRiskService.Setup(call => call.GetRiskByInterviewId(inteviewId)).Returns(mockRisk);
            _mockPlanService.Setup(call => call.GetPlansForRisk(mockRisk.Id)).Returns(new List<IPlan>() { mockPlan });
            _mockCoverService.Setup(call => call.GetCoversForPlan(mockPlan.Id)).Returns(new List<ICover>() { _mockCover.Object });
            _mockGetUnderwritingBenefitCodeForCoverService.Setup(
                call => call.GetUnderwritingBenefitCodeFrom(_mockCover.Object)).Returns(uwBenefitCode);

            _mockCoverService.Setup(call => call.UpdateCover(_mockCover.Object));

            var svc = GetService();
            svc.Update(mockUnderwritingBenefitResponsesChange);

            _mockCover.VerifySet((cover) => cover.UnderwritingStatus = It.Is<UnderwritingStatus>(val => val == uwStatus), Times.Once);
        }

        private CoverUnderwritingSyncService GetService()
        {
            return new CoverUnderwritingSyncService(_mockRiskService.Object, 
                _mockPlanService.Object, 
                _mockCoverService.Object,
                _mockCoverLoadingService.Object,
                _mockCoverLoadingsConverter.Object,
                _mockGetUnderwritingBenefitCodeForCoverService.Object,
                _mockCoverExclusionsConverter.Object,
                _mockCoverExclusionsService.Object);
        }
    }
}
