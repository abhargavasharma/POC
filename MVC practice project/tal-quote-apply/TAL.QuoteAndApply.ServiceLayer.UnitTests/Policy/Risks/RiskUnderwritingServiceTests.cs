using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using TAL.QuoteAndApply.DataModel.Underwriting;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.ServiceLayer.Policy.Risk;

namespace TAL.QuoteAndApply.ServiceLayer.UnitTests.Policy.Risks
{
    [TestFixture]
    public class RiskUnderwritingServiceTests
    {
        private Mock<IRiskService> _mockRiskService;
        private Mock<IPlanService> _mockPlanService;
        private Mock<ICoverService> _mockCoverService;
        
        [TestFixtureSetUp]
        public void Setup()
        {
            var mockRepo = new MockRepository(MockBehavior.Strict);
            _mockRiskService = mockRepo.Create<IRiskService>();
            _mockPlanService = mockRepo.Create<IPlanService>();
            _mockCoverService = mockRepo.Create<ICoverService>();
        }

        [TestCase(UnderwritingStatus.Accept, Validation.ValidationType.Success, true)]
        [TestCase(UnderwritingStatus.Decline, Validation.ValidationType.Error, false)]
        [TestCase(UnderwritingStatus.Incomplete, Validation.ValidationType.Warning, false)]
        [TestCase(UnderwritingStatus.Refer, Validation.ValidationType.Warning, false)]
        [TestCase(UnderwritingStatus.Defer, Validation.ValidationType.Warning, false)]
        [TestCase(UnderwritingStatus.MoreInfo, Validation.ValidationType.Warning, false)]
        public void GetRiskUnderwritingStatus_CoverUnderwritingStatus_MappedToIsCompletedAndValidationType(UnderwritingStatus uwStatus, Validation.ValidationType expectedValidationType, bool expectedIsCompleted)
        {
            var riskId = 123;

            var mockRisk = new RiskDto { Id = 123 };
            var mockPlan = new PlanDto { Id = 987, Selected = true};
            var mockCover = new CoverDto { Id = 123, Selected = true, UnderwritingStatus = uwStatus };

            _mockRiskService.Setup(call => call.GetRisk(riskId)).Returns(mockRisk);
            _mockPlanService.Setup(call => call.GetPlansForRisk(mockRisk.Id)).Returns(new List<IPlan>() { mockPlan });
            _mockCoverService.Setup(call => call.GetCoversForPlan(mockPlan.Id)).Returns(new List<ICover>() { mockCover });

            var svc = GetService();
            var result = svc.GetRiskUnderwritingStatus(riskId);
            var coverResult = result.CoverUnderwritingCompleteResults.ToList()[0];

            Assert.That(coverResult.IsUnderwritingComplete, Is.EqualTo(expectedIsCompleted));
            Assert.That(coverResult.ValidationType, Is.EqualTo(expectedValidationType));
        }

        [Test]
        public void GetRiskUnderwritingStatus_NoSelectedCovers_RiskUnderwritingComplete()
        {
            var riskId = 123;

            var mockRisk = new RiskDto { Id = 123 };
            var mockPlan = new PlanDto { Id = 987, Selected = true };
            var mockCover1 = new CoverDto { Id = 123, Selected = false, UnderwritingStatus = UnderwritingStatus.Accept };
            var mockCover2 = new CoverDto { Id = 987, Selected = false, UnderwritingStatus = UnderwritingStatus.Decline }; //not complete

            _mockRiskService.Setup(call => call.GetRisk(riskId)).Returns(mockRisk);
            _mockPlanService.Setup(call => call.GetPlansForRisk(mockRisk.Id)).Returns(new List<IPlan>() { mockPlan });
            _mockCoverService.Setup(call => call.GetCoversForPlan(mockPlan.Id)).Returns(new List<ICover>() { mockCover1, mockCover2 });

            var svc = GetService();
            var result = svc.GetRiskUnderwritingStatus(riskId);

            Assert.That(result.IsUnderwritingCompleteForRisk, Is.True);
        }

        [Test]
        public void GetRiskUnderwritingStatus_AllCoversUnderwritingComplete_RiskUnderwritingComplete()
        {
            var riskId = 123;

            var mockRisk = new RiskDto { Id = 123 };
            var mockPlan = new PlanDto { Id = 987, Selected = true };
            var mockCover1 = new CoverDto { Id = 123, Selected = true, UnderwritingStatus = UnderwritingStatus.Accept };
            var mockCover2 = new CoverDto { Id = 987, Selected = true, UnderwritingStatus = UnderwritingStatus.Accept };

            _mockRiskService.Setup(call => call.GetRisk(riskId)).Returns(mockRisk);
            _mockPlanService.Setup(call => call.GetPlansForRisk(mockRisk.Id)).Returns(new List<IPlan>() { mockPlan });
            _mockCoverService.Setup(call => call.GetCoversForPlan(mockPlan.Id)).Returns(new List<ICover>() { mockCover1, mockCover2 });

            var svc = GetService();
            var result = svc.GetRiskUnderwritingStatus(riskId);

            Assert.That(result.IsUnderwritingCompleteForRisk, Is.True);
        }

        [Test]
        public void GetRiskUnderwritingStatus_AnyCoverUnderwritingNotComplete_RiskUnderwritingIncomplete()
        {
            var riskId = 123;

            var mockRisk = new RiskDto { Id = 123 };
            var mockPlan = new PlanDto { Id = 987, Selected = true };
            var mockCover1 = new CoverDto { Id = 123, Selected = true, UnderwritingStatus = UnderwritingStatus.Accept };
            var mockCover2 = new CoverDto { Id = 987, Selected = true, UnderwritingStatus = UnderwritingStatus.Decline }; //not complete

            _mockRiskService.Setup(call => call.GetRisk(riskId)).Returns(mockRisk);
            _mockPlanService.Setup(call => call.GetPlansForRisk(mockRisk.Id)).Returns(new List<IPlan>() { mockPlan });
            _mockCoverService.Setup(call => call.GetCoversForPlan(mockPlan.Id)).Returns(new List<ICover>() { mockCover1, mockCover2 });

            var svc = GetService();
            var result = svc.GetRiskUnderwritingStatus(riskId);

            Assert.That(result.IsUnderwritingCompleteForRisk, Is.False);
        }


        [Test]
        public void GetRiskUnderwritingStatus_AnyCoverHasValidationError_RiskHasValidationError()
        {
            var riskId = 123;

            var mockRisk = new RiskDto { Id = 123 };
            var mockPlan = new PlanDto { Id = 987, Selected = true };
            var mockCover1 = new CoverDto { Id = 123, Selected = true, UnderwritingStatus = UnderwritingStatus.Accept };
            var mockCover2 = new CoverDto { Id = 987, Selected = true, UnderwritingStatus = UnderwritingStatus.Decline }; //error!

            _mockRiskService.Setup(call => call.GetRisk(riskId)).Returns(mockRisk);
            _mockPlanService.Setup(call => call.GetPlansForRisk(mockRisk.Id)).Returns(new List<IPlan>() { mockPlan });
            _mockCoverService.Setup(call => call.GetCoversForPlan(mockPlan.Id)).Returns(new List<ICover>() { mockCover1, mockCover2 });

            var svc = GetService();
            var result = svc.GetRiskUnderwritingStatus(riskId);

            Assert.That(result.ValidationTypeForRisk, Is.EqualTo(Validation.ValidationType.Error));
        }

        [Test]
        public void GetRiskUnderwritingStatus_NoCoversHaveAnError_AnyCoverHasValidationWarning_RiskHasValidationWarning()
        {
            var riskId = 123;

            var mockRisk = new RiskDto { Id = 123 };
            var mockPlan = new PlanDto { Id = 987, Selected = true };
            var mockCover1 = new CoverDto { Id = 123, Selected = true, UnderwritingStatus = UnderwritingStatus.Accept };
            var mockCover2 = new CoverDto { Id = 987, Selected = true, UnderwritingStatus = UnderwritingStatus.Refer }; //warning!

            _mockRiskService.Setup(call => call.GetRisk(riskId)).Returns(mockRisk);
            _mockPlanService.Setup(call => call.GetPlansForRisk(mockRisk.Id)).Returns(new List<IPlan>() { mockPlan });
            _mockCoverService.Setup(call => call.GetCoversForPlan(mockPlan.Id)).Returns(new List<ICover>() { mockCover1, mockCover2 });

            var svc = GetService();
            var result = svc.GetRiskUnderwritingStatus(riskId);

            Assert.That(result.ValidationTypeForRisk, Is.EqualTo(Validation.ValidationType.Warning));
        }

        [Test]
        public void GetRiskUnderwritingStatus_AllCoversHaveValidationSuccecss_RiskHasValidationSuccecss()
        {
            var riskId = 123;

            var mockRisk = new RiskDto { Id = 123 };
            var mockPlan = new PlanDto { Id = 987, Selected = true };
            var mockCover1 = new CoverDto { Id = 123, Selected = true, UnderwritingStatus = UnderwritingStatus.Accept };
            var mockCover2 = new CoverDto { Id = 987, Selected = true, UnderwritingStatus = UnderwritingStatus.Accept };

            _mockRiskService.Setup(call => call.GetRisk(riskId)).Returns(mockRisk);
            _mockPlanService.Setup(call => call.GetPlansForRisk(mockRisk.Id)).Returns(new List<IPlan>() { mockPlan });
            _mockCoverService.Setup(call => call.GetCoversForPlan(mockPlan.Id)).Returns(new List<ICover>() { mockCover1, mockCover2 });

            var svc = GetService();
            var result = svc.GetRiskUnderwritingStatus(riskId);

            Assert.That(result.ValidationTypeForRisk, Is.EqualTo(Validation.ValidationType.Success));
        }

        public RiskUnderwritingService GetService()
        {
            return new RiskUnderwritingService(_mockRiskService.Object, _mockPlanService.Object, _mockCoverService.Object);
        }
    }
}
