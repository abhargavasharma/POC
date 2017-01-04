using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TAL.QuoteAndApply.DataLayer.Service;
using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.DataModel.Product;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.Plan;
using TAL.QuoteAndApply.Tests.Shared.Builders;
using PolicySource = TAL.QuoteAndApply.ServiceLayer.Models.PolicySource;
using Status = TAL.QuoteAndApply.DataModel.Policy.MarketingStatus;

namespace TAL.QuoteAndApply.ServiceLayer.IntegrationTests.Policy.MarketingStatus
{
    [TestFixture]
    public class MarketingStatusServiceTests : BaseServiceLayerTest
    {
        [Test]
        public void UpdateRiskMarketingStatus_WithDefaultSelectionForQuoteCreation_DefaultMarketingStatusesCorrect()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            //Arrange
            var quote = CreateQuote(new CreateQuoteBuilder()
                .AsAge(30)
                .WithJourneyType(PolicySource.CustomerPortalBuildMyOwn)
                .Build());
            var policyWithRisks = GetPolicy(quote.QuoteReference);
            var risk = policyWithRisks.Risks.First();

            //Act
            var riskSrv = GetServiceInstance<IRiskMarketingStatusService>();
            var planSrv = GetServiceInstance<IPlanMarketingStatusService>();
            var coverSrv = GetServiceInstance<ICoverMarketingStatusService>();

            var riskMarketingStatus = riskSrv.GetRiskMarketingStatusByRiskId(risk.Risk.Id);

            var plans = risk.Plans.ToList();
            var lifePlanMarketingStatus = planSrv.GetPlanMarketingStatusByPlanId(plans[0].Plan.Id);
            var tpdPlanMarketingStatus = planSrv.GetPlanMarketingStatusByPlanId(plans[3].Plan.Id);
            var riPlanMarketingStatus = planSrv.GetPlanMarketingStatusByPlanId(plans[4].Plan.Id);
            var ipPlanMarketingStatus = planSrv.GetPlanMarketingStatusByPlanId(plans[5].Plan.Id);

            var lifePlanCovers = plans[0].Covers.ToList();

            var lifeAccidentCoverMarketingStatus = coverSrv.GetCoverMarketingStatusByCoverId(lifePlanCovers[0].Id);
            var lifeIllnessCoverMarketingStatus = coverSrv.GetCoverMarketingStatusByCoverId(lifePlanCovers[1].Id);
            var lifeSportsCoverMarketingStatus = coverSrv.GetCoverMarketingStatusByCoverId(lifePlanCovers[2].Id);

            var tpdPlanCovers = plans[3].Covers.ToList();

            var tpdAccidentCoverMarketingStatus = coverSrv.GetCoverMarketingStatusByCoverId(tpdPlanCovers[0].Id);
            var tpdIllnessCoverMarketingStatus = coverSrv.GetCoverMarketingStatusByCoverId(tpdPlanCovers[1].Id);
            var tpdSportsCoverMarketingStatus = coverSrv.GetCoverMarketingStatusByCoverId(tpdPlanCovers[2].Id);

            var riPlanCovers = plans[4].Covers.ToList();

            var riAccidentCoverMarketingStatus = coverSrv.GetCoverMarketingStatusByCoverId(riPlanCovers[0].Id);
            var riIllnessCoverMarketingStatus = coverSrv.GetCoverMarketingStatusByCoverId(riPlanCovers[1].Id);
            var riSportsCoverMarketingStatus = coverSrv.GetCoverMarketingStatusByCoverId(riPlanCovers[2].Id);

            var ipPlanCovers = plans[5].Covers.ToList();

            var ipAccidentCoverMarketingStatus = coverSrv.GetCoverMarketingStatusByCoverId(ipPlanCovers[0].Id);
            var ipIllnessCoverMarketingStatus = coverSrv.GetCoverMarketingStatusByCoverId(ipPlanCovers[1].Id);
            var ipSportsCoverMarketingStatus = coverSrv.GetCoverMarketingStatusByCoverId(ipPlanCovers[2].Id);

            //Assert
            Assert.That(riskMarketingStatus.MarketingStatusId, Is.EqualTo(Status.Lead));
            Assert.That(lifePlanMarketingStatus.MarketingStatusId, Is.EqualTo(Status.Lead));
            Assert.That(tpdPlanMarketingStatus.MarketingStatusId, Is.EqualTo(Status.Off));
            Assert.That(riPlanMarketingStatus.MarketingStatusId, Is.EqualTo(Status.Off));
            Assert.That(ipPlanMarketingStatus.MarketingStatusId, Is.EqualTo(Status.Lead));

            Assert.That(lifeAccidentCoverMarketingStatus.MarketingStatusId, Is.EqualTo(Status.Lead));
            Assert.That(lifeIllnessCoverMarketingStatus.MarketingStatusId, Is.EqualTo(Status.Lead));
            Assert.That(lifeSportsCoverMarketingStatus.MarketingStatusId, Is.EqualTo(Status.Lead));

            Assert.That(tpdAccidentCoverMarketingStatus.MarketingStatusId, Is.EqualTo(Status.Lead));
            Assert.That(tpdIllnessCoverMarketingStatus.MarketingStatusId, Is.EqualTo(Status.Lead));
            Assert.That(tpdSportsCoverMarketingStatus.MarketingStatusId, Is.EqualTo(Status.Lead));

            Assert.That(riAccidentCoverMarketingStatus.MarketingStatusId, Is.EqualTo(Status.Lead));
            Assert.That(riIllnessCoverMarketingStatus.MarketingStatusId, Is.EqualTo(Status.Lead));
            Assert.That(riSportsCoverMarketingStatus.MarketingStatusId, Is.EqualTo(Status.Lead));

            Assert.That(ipAccidentCoverMarketingStatus.MarketingStatusId, Is.EqualTo(Status.Lead));
            Assert.That(ipIllnessCoverMarketingStatus.MarketingStatusId, Is.EqualTo(Status.Lead));
            Assert.That(ipSportsCoverMarketingStatus.MarketingStatusId, Is.EqualTo(Status.Lead));
        }

        [Test]
        public void UpdateRiskMarketingStatus_QuoteCreatedFromSalesPortal_RiskMarketingStatusSetToUnknown()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            //Arrange
            var quote = CreateQuote(new CreateQuoteBuilder()
                .AsAge(30)
                .WithJourneyType(PolicySource.SalesPortal)
                .Build());
            var policyWithRisks = GetPolicy(quote.QuoteReference);
            var risk = policyWithRisks.Risks.First();

            //Act
            var riskSrv = GetServiceInstance<IRiskMarketingStatusService>();
            
            var riskMarketingStatus = riskSrv.GetRiskMarketingStatusByRiskId(risk.Risk.Id);

            //Assert
            Assert.That(riskMarketingStatus.MarketingStatusId, Is.EqualTo(Status.Unknown));
        }

        [Test]
        public void UpdateRiskMarketingStatus_AllPlansAndCoversSelected_AllMarketingStatusesSetToLead()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            //Arrange
            var quote = CreateQuote(new CreateQuoteBuilder()
                .AsAge(30)
                .WithJourneyType(PolicySource.CustomerPortalBuildMyOwn)
                .Build());
            var policyWithRisks = GetPolicy(quote.QuoteReference);
            var risk = policyWithRisks.Risks.First();
            var riskDto = GetRisk(risk.Risk.Id);

            var lifePlan = risk.Plans.FirstOrDefault(x => x.Plan.Code == "DTH");
            var tpdPlan = risk.Plans.FirstOrDefault(x => x.Plan.Code == "TPS");
            var riPlan = risk.Plans.FirstOrDefault(x => x.Plan.Code == "TRS");
            var ipPlan = risk.Plans.FirstOrDefault(x => x.Plan.Code == "IP");

            //Act
            var srv = GetServiceInstance<IPlanAutoUpdateService>();
            var riskSrv = GetServiceInstance<IRiskMarketingStatusService>();
            var planSrv = GetServiceInstance<IPlanMarketingStatusService>();
            var updatePlanSrv = GetServiceInstance<IUpdatePlanService>();

            var allPlansIdentityInfo = new List<PlanIdentityInfo>()
            {
                new PlanIdentityInfo(){PlanCode = lifePlan.Plan.Code,PlanId = lifePlan.Plan.Id,Selected = true,Status = PlanStatus.Completed},
                new PlanIdentityInfo(){PlanCode = tpdPlan.Plan.Code,PlanId = tpdPlan.Plan.Id,Selected = true,Status = PlanStatus.Completed},
                new PlanIdentityInfo(){PlanCode = riPlan.Plan.Code,PlanId = riPlan.Plan.Id,Selected = true,Status = PlanStatus.Completed},
                new PlanIdentityInfo(){PlanCode = ipPlan.Plan.Code,PlanId = ipPlan.Plan.Id,Selected = true,Status = PlanStatus.Completed}
            };

            //Select tpd plan
            var updateTpdPlanStateParam = PlanStateParam.BuildPlanStateParam(tpdPlan.Plan.Code, "TAL", true, policyWithRisks.Policy.Id, risk.Risk.Id, false, 375000, false,
                PremiumType.Level, tpdPlan.Plan.Id, 30,
                50000, null, null, OccupationDefinition.AnyOccupation, new List<PlanStateParam>().ToArray(), new List<OptionsParam>() { new OptionsParam("PR", true) }, 
                allPlansIdentityInfo, new List<string>() { "TPSAC", "TPSIC", "TPSASC" });

            var updatedTpdPlan = updatePlanSrv.UpdateActivePlanAndCalculatePremium(quote.QuoteReference, updateTpdPlanStateParam).ToArray();

            //Select ri plan
            var updateRiPlanStateParam = PlanStateParam.BuildPlanStateParam(riPlan.Plan.Code, "TAL", true, policyWithRisks.Policy.Id, risk.Risk.Id, false, 100000, false,
                PremiumType.Stepped, riPlan.Plan.Id, 30,
                50000, null, null, OccupationDefinition.OwnOccupation, new List<PlanStateParam>().ToArray(), new List<OptionsParam>() { new OptionsParam("PR", true) }, 
                allPlansIdentityInfo, new List<string>() { "TRSSIN", "TRSSIC", "TRSCC" });

            var updatedRiPlan = updatePlanSrv.UpdateActivePlanAndCalculatePremium(quote.QuoteReference, updateRiPlanStateParam).ToArray();

            srv.UpdatePlansToConformWithPlanEligiblityRules(riskDto);

            var riskMarketingStatus = riskSrv.GetRiskMarketingStatusByRiskId(risk.Risk.Id);

            var plans = risk.Plans.ToList();
            var lifePlanMarketingStatus = planSrv.GetPlanMarketingStatusByPlanId(plans[0].Plan.Id);
            var tpdPlanMarketingStatus = planSrv.GetPlanMarketingStatusByPlanId(plans[3].Plan.Id);
            var riPlanMarketingStatus = planSrv.GetPlanMarketingStatusByPlanId(plans[4].Plan.Id);
            var ipPlanMarketingStatus = planSrv.GetPlanMarketingStatusByPlanId(plans[5].Plan.Id);

            //Assert
            Assert.That(riskMarketingStatus.MarketingStatusId, Is.EqualTo(Status.Lead));
            Assert.That(lifePlanMarketingStatus.MarketingStatusId, Is.EqualTo(Status.Lead));
            Assert.That(tpdPlanMarketingStatus.MarketingStatusId, Is.EqualTo(Status.Lead));
            Assert.That(riPlanMarketingStatus.MarketingStatusId, Is.EqualTo(Status.Lead));
            Assert.That(ipPlanMarketingStatus.MarketingStatusId, Is.EqualTo(Status.Lead));
        }
    }
}
