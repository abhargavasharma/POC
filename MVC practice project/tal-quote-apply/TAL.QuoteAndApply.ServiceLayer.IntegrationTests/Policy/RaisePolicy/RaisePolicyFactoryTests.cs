using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using TAL.QuoteAndApply.DataLayer.Service;
using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.DataModel.Product;
using TAL.QuoteAndApply.DataModel.Underwriting;
using TAL.QuoteAndApply.Policy.Data;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.Product.Definition;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.RaisePolicy;
using TAL.QuoteAndApply.ServiceLayer.Policy.Referral;
using TAL.QuoteAndApply.ServiceLayer.Policy.Risk.Beneficiary;
using TAL.QuoteAndApply.Tests.Shared.Builders;
using PolicySource = TAL.QuoteAndApply.ServiceLayer.Models.PolicySource;

namespace TAL.QuoteAndApply.ServiceLayer.IntegrationTests.Policy.RaisePolicy
{
    [TestFixture]
    public class RaisePolicyFactoryTests : BaseServiceLayerTest
    {
        [TestFixtureSetUp]
        public void Setup()
        {
            MockLoggingService.Setup(call => call.Error(It.IsAny<Exception>())).Callback((() => Console.WriteLine("Logging service error")));
            MockLoggingService.Setup(call => call.Error(It.IsAny<string>(), It.IsAny<Exception>())).Callback((() => Console.WriteLine("Logging service error")));
            MockLoggingService.Setup(call => call.Error(It.IsAny<string>())).Callback((() => Console.WriteLine("Logging service error")));
        }

        [Test]
        public void GetFromQuoteReference_EnsurePremiumIsMonthly()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var quote = CreateQuote(new CreateQuoteBuilder()
                .WithJourneyType(PolicySource.CustomerPortalBuildMyOwn)
                .Build());

            var policyWithRisksService = GetServiceInstance<IPolicyWithRisksService>();
            var policyWithRisks = policyWithRisksService.GetFrom(quote.QuoteReference);
            
            Assert.That(policyWithRisks.Policy.Premium, Is.EqualTo(138.90m));

            var updatePolicyService = GetServiceInstance<IUpdatePolicyService>();
            updatePolicyService.UpdatePremiumFrequency(quote.QuoteReference, PremiumFrequency.Yearly);

            policyWithRisks = policyWithRisksService.GetFrom(quote.QuoteReference);
            Assert.That(policyWithRisks.Policy.Premium, Is.EqualTo(1527.89m));

            var svc = GetServiceInstance<IRaisePolicyFactory>();

            var result = svc.GetFromQuoteReference(quote.QuoteReference);

            Assert.That(result.Premium, Is.EqualTo(138.90m));
        }

        [Test]
        public void GetFromQuoteReference_EnsureOnlyActivePlansAdded()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var quote = CreateQuote(new CreateQuoteBuilder()
                .WithJourneyType(PolicySource.CustomerPortalBuildMyOwn)
                .Build());

            var policyWithRisksService = GetServiceInstance<IPolicyWithRisksService>();
            var policyWithRisks = policyWithRisksService.GetFrom(quote.QuoteReference);

            SetLifePlanAndCoversActive(policyWithRisks);
            SetOtherPlansAsInactive(policyWithRisks);
            policyWithRisksService.SaveAll(policyWithRisks);

            var svc = GetServiceInstance<IRaisePolicyFactory>();
            var result = svc.GetFromQuoteReference(quote.QuoteReference);

            Assert.That(result.PrimaryRisk.Plans.Count, Is.EqualTo(1));
            Assert.That(result.PrimaryRisk.Plans.First().Code, Is.EqualTo(ProductPlanConstants.LifePlanCode));
        }

        [Test]
        public void GetFromQuoteReference_EnsureRidersNotAddedIfLifePlanIsActivePlansAdded()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var quote = CreateQuote(new CreateQuoteBuilder()
                .WithJourneyType(PolicySource.CustomerPortalBuildMyOwn)
                .Build());

            var policyWithRisksService = GetServiceInstance<IPolicyWithRisksService>();
            var policyWithRisks = policyWithRisksService.GetFrom(quote.QuoteReference);

            SetAllParentPlansInactiveAndRidersActive(policyWithRisks);
            SetIpPlanAndCoversActive(policyWithRisks);
            policyWithRisksService.SaveAll(policyWithRisks);

            var svc = GetServiceInstance<IRaisePolicyFactory>();
            var result = svc.GetFromQuoteReference(quote.QuoteReference);

            Assert.That(result.PrimaryRisk.Plans.Count, Is.EqualTo(1));
            Assert.That(result.PrimaryRisk.Plans.First().Code, Is.EqualTo(ProductPlanConstants.IncomeProtectionPlanCode));
        }

        [Test]
        public void GetFromQuoteReference_EnsurePlanMapping()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var quote = CreateQuote(new CreateQuoteBuilder()
                .WithJourneyType(PolicySource.CustomerPortalBuildMyOwn)
                .Build());

            var policyWithRisksService = GetServiceInstance<IPolicyWithRisksService>();
            var policyWithRisks = policyWithRisksService.GetFrom(quote.QuoteReference);

            SetLifePlanAndCoversActive(policyWithRisks);
            SetOtherPlansAsInactive(policyWithRisks);

            var risk = policyWithRisks.Risks.First();
            var lifePlan = risk.Plans.First(p => p.Plan.Code == ProductPlanConstants.LifePlanCode);
            lifePlan.Plan.MultiPlanDiscountFactor = 0.8m;
            lifePlan.Plan.MultiPlanDiscount = 10;
            lifePlan.Plan.MultiCoverDiscount = 10;
            lifePlan.Plan.Premium = 1000;
            lifePlan.Plan.PremiumHoliday = true;
            lifePlan.Plan.LinkedToCpi = true;
            lifePlan.Plan.PremiumType = PremiumType.Level;
            
            policyWithRisksService.SaveAll(policyWithRisks);

            var svc = GetServiceInstance<IRaisePolicyFactory>();
            var result = svc.GetFromQuoteReference(quote.QuoteReference);

            Assert.That(result.PrimaryRisk.Plans.Count, Is.EqualTo(1));

            var raiseLifePlan = result.PrimaryRisk.Plans.First();
            Assert.That(raiseLifePlan.Code, Is.EqualTo(ProductPlanConstants.LifePlanCode));
            Assert.That(raiseLifePlan.MultiPlanDiscountFactor, Is.EqualTo(lifePlan.Plan.MultiPlanDiscountFactor));
            Assert.That(raiseLifePlan.MultiPlanDiscount, Is.EqualTo(lifePlan.Plan.MultiPlanDiscount));
            Assert.That(raiseLifePlan.MultiCoverDiscount, Is.EqualTo(lifePlan.Plan.MultiCoverDiscount));
            Assert.That(raiseLifePlan.Premium, Is.EqualTo(lifePlan.Plan.Premium));
            Assert.That(raiseLifePlan.PremiumHoliday, Is.EqualTo(lifePlan.Plan.PremiumHoliday));
            Assert.That(raiseLifePlan.LinkedToCpi, Is.EqualTo(lifePlan.Plan.LinkedToCpi));
            Assert.That(raiseLifePlan.PremiumType, Is.EqualTo(lifePlan.Plan.PremiumType));
        }

        [Test]
        public void GetFromQuoteReference_EnsureAllCoversSelectedOrNotAreIncluded()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var quote = CreateQuote(new CreateQuoteBuilder()
                .WithJourneyType(PolicySource.CustomerPortalBuildMyOwn)
                .Build());

            var policyWithRisksService = GetServiceInstance<IPolicyWithRisksService>();
            var policyWithRisks = policyWithRisksService.GetFrom(quote.QuoteReference);

            SetLifePlanAndCoversActive(policyWithRisks);
            SetOtherPlansAsInactive(policyWithRisks);
            policyWithRisksService.SaveAll(policyWithRisks);

            var svc = GetServiceInstance<IRaisePolicyFactory>();
            var result = svc.GetFromQuoteReference(quote.QuoteReference);

            Assert.That(result.PrimaryRisk.Plans.Count, Is.EqualTo(1));
            Assert.That(result.PrimaryRisk.Plans.First().Code, Is.EqualTo(ProductPlanConstants.LifePlanCode));
            Assert.That(result.PrimaryRisk.Plans.First().Covers.Count, Is.EqualTo(3));

            policyWithRisks = policyWithRisksService.GetFrom(quote.QuoteReference);
            SetLifePlanActiveAndCoversInactive(policyWithRisks);
            SetOtherPlansAsInactive(policyWithRisks);
            policyWithRisksService.SaveAll(policyWithRisks);

            result = svc.GetFromQuoteReference(quote.QuoteReference);

            Assert.That(result.PrimaryRisk.Plans.Count, Is.EqualTo(1));
            Assert.That(result.PrimaryRisk.Plans.First().Code, Is.EqualTo(ProductPlanConstants.LifePlanCode));
            Assert.That(result.PrimaryRisk.Plans.First().Covers.Count, Is.EqualTo(3));
        }

        [Test]
        public void GetFromQuoteReference_PolicyHasNoCompleteReferrals_LastCompletedReferralDateTimeIsNull()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var quote = CreateQuote(new CreateQuoteBuilder()
                .WithJourneyType(PolicySource.CustomerPortalBuildMyOwn)
                .Build());

            var svc = GetServiceInstance<IRaisePolicyFactory>();
            var result = svc.GetFromQuoteReference(quote.QuoteReference);

            Assert.That(result.LastCompletedReferralDateTime, Is.EqualTo(null));
        }

        [Test]
        public void GetFromQuoteReference_PolicyHasCompleteReferral_LastCompletedReferralDateTimeSetToCompletedDate()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var quote = CreateQuote(new CreateQuoteBuilder()
                .WithJourneyType(PolicySource.CustomerPortalBuildMyOwn)
                .Build());

            var createReferralSvc = GetServiceInstance<ICreateReferralService>();
            createReferralSvc.CreateReferralFor(quote.QuoteReference);

            var completeReferralSvc = GetServiceInstance<ICompleteReferralService>();
            completeReferralSvc.CompleteReferral(quote.QuoteReference);

            var svc = GetServiceInstance<IRaisePolicyFactory>();
            var result = svc.GetFromQuoteReference(quote.QuoteReference);

            Assert.That(result.LastCompletedReferralDateTime, Is.Not.EqualTo(null));
        }

        [Test]
        public void GetFromQuoteReference_EnsureCoverExclusionsAdded()
        {
            const string exclusionName = "TestExclusion";

            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var quote = CreateQuote(new CreateQuoteBuilder()
                .WithJourneyType(PolicySource.CustomerPortalBuildMyOwn)
                .Build());

            var policyWithRisksService = GetServiceInstance<IPolicyWithRisksService>();
            var policyWithRisks = policyWithRisksService.GetFrom(quote.QuoteReference);

            var risk = policyWithRisks.Risks.First();
            var lifePlan = risk.Plans.First(p => p.Plan.Code == ProductPlanConstants.LifePlanCode);
            var accidentCover = lifePlan.Covers.First(c => c.Code == ProductCoverConstants.LifeAccidentCover);

            var coverExclusionsSvc = GetServiceInstance<ICoverExclusionsService>();
            coverExclusionsSvc.UpdateCoverExclusions(accidentCover, new [] {new CoverExclusionDto {CoverId = accidentCover.Id, Name = exclusionName, Text = exclusionName } });

            var svc = GetServiceInstance<IRaisePolicyFactory>();

            var result = svc.GetFromQuoteReference(quote.QuoteReference);

            var raiseLifePlan = result.PrimaryRisk.Plans.First(x => x.Code == ProductPlanConstants.LifePlanCode);
            var raiseAccidentCover = raiseLifePlan.Covers.First(c => c.Code == ProductCoverConstants.LifeAccidentCover);

            Assert.That(raiseAccidentCover.Exclusions.Count, Is.EqualTo(1));
            Assert.That(raiseAccidentCover.Exclusions[0].Name, Is.EqualTo(exclusionName));
            Assert.That(raiseAccidentCover.Exclusions[0].Text, Is.EqualTo(exclusionName));
        }

        [Test]
        public void GetFromQuoteReference_EnsureCoverLoadingsAdded()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var quote = CreateQuote(new CreateQuoteBuilder()
                .WithJourneyType(PolicySource.CustomerPortalBuildMyOwn)
                .Build());

            var policyWithRisksService = GetServiceInstance<IPolicyWithRisksService>();
            var policyWithRisks = policyWithRisksService.GetFrom(quote.QuoteReference);

            var risk = policyWithRisks.Risks.First();
            var lifePlan = risk.Plans.First(p => p.Plan.Code == ProductPlanConstants.LifePlanCode);
            var accidentCover = lifePlan.Covers.First(c => c.Code == ProductCoverConstants.LifeAccidentCover);

            var coverLoadingsSvc = GetServiceInstance<ICoverLoadingService>();
            coverLoadingsSvc.UpdateLoadingsForCover(accidentCover, new[] { new CoverLoadingDto { CoverId = accidentCover.Id, LoadingType = LoadingType.PerMille, Loading = 5} });

            var svc = GetServiceInstance<IRaisePolicyFactory>();

            var result = svc.GetFromQuoteReference(quote.QuoteReference);

            var raiseLifePlan = result.PrimaryRisk.Plans.First(x => x.Code == ProductPlanConstants.LifePlanCode);
            var raiseAccidentCover = raiseLifePlan.Covers.First(c => c.Code == ProductCoverConstants.LifeAccidentCover);

            Assert.That(raiseAccidentCover.Loadings.Count, Is.EqualTo(1));
            Assert.That(raiseAccidentCover.Loadings[0].Loading, Is.EqualTo(5));
            Assert.That(raiseAccidentCover.Loadings[0].LoadingType, Is.EqualTo(LoadingType.PerMille));
        }

        [Test]
        public void GetFromQuoteReference_EnsureDocumentUrlSetCorrectly()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var quote = CreateQuote(new CreateQuoteBuilder()
                .WithJourneyType(PolicySource.CustomerPortalBuildMyOwn)
                .Build());

            var svc = GetServiceInstance<IRaisePolicyFactory>();

            var result = svc.GetFromQuoteReference(quote.QuoteReference);

            Assert.That(result.DocumentUrl, Is.EqualTo($"https://local-talsalesportal.delivery.lan/Policy/Edit/{quote.QuoteReference}"));
        }

        [Test]
        public void GetFromQuoteReference_LifeWithPremiumReliefAndRiders_EnsureRidersHavePremiumReliefSelected()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var quote = CreateQuote(new CreateQuoteBuilder()
                .WithJourneyType(PolicySource.CustomerPortalBuildMyOwn)
                .Build());

            var policyWithRisksService = GetServiceInstance<IPolicyWithRisksService>();
            var policyWithRisks = policyWithRisksService.GetFrom(quote.QuoteReference);

            SetLifePlanAndRidersActive(policyWithRisks);
            var premiumReliefOption = SetPremiumReliefForLifePlanAndReturn(policyWithRisks);
            policyWithRisksService.SaveAll(policyWithRisks);

            var optionsService = GetServiceInstance<IOptionService>();
            optionsService.UpdateOption(premiumReliefOption);

            var svc = GetServiceInstance<IRaisePolicyFactory>();

            var result = svc.GetFromQuoteReference(quote.QuoteReference);

            var raiseLifePlan = result.PrimaryRisk.Plans.First(p => p.Code == ProductPlanConstants.LifePlanCode);
            var raiseTpdRiderPlan = result.PrimaryRisk.Plans.First(p => p.Code == ProductRiderConstants.PermanentDisabilityRiderCode);
            var raiseCiRiderPlan = result.PrimaryRisk.Plans.First(p => p.Code == ProductRiderConstants.CriticalIllnessRiderCode);

            Assert.That(raiseLifePlan.Options.First(o=> o.Code == ProductOptionConstants.LifePremiumRelief).Selected, Is.True);
            Assert.That(raiseTpdRiderPlan.Options.First(o => o.Code == ProductOptionConstants.LifePremiumRelief).Selected, Is.True);
            Assert.That(raiseCiRiderPlan.Options.First(o => o.Code == ProductOptionConstants.LifePremiumRelief).Selected, Is.True);
        }

        [Test]
        public void GetFromQuoteReference_RiskIsLprBeneficiary_NoBeneficiariesAdded()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var quote = CreateQuote(new CreateQuoteBuilder()
                .WithJourneyType(PolicySource.CustomerPortalBuildMyOwn)
                .Build());

            var policyWithRisksService = GetServiceInstance<IPolicyWithRisksService>();
            var policyWithRisks = policyWithRisksService.GetFrom(quote.QuoteReference);
            var risk = policyWithRisks.Risks.First().Risk;

            var beneficiariesService = GetServiceInstance<IBeneficiaryDetailsService>();
            beneficiariesService.CreateOrUpdateBeneficiary(new RiskBeneficiaryDetailsParam {Title = "Mr"}, risk.Id);
            beneficiariesService.UpdateLprForRisk(risk.Id, true);

            var svc = GetServiceInstance<IRaisePolicyFactory>();

            var result = svc.GetFromQuoteReference(quote.QuoteReference);

            Assert.That(result.PrimaryRisk.Beneficiaries, Is.Not.Null);
            Assert.That(result.PrimaryRisk.Beneficiaries.Count, Is.EqualTo(0));
        }

        private void SetOtherPlansAsInactive(PolicyWithRisks policyWithRisks)
        {
            var risk = policyWithRisks.Risks.First();
            var otherPlans = risk.Plans.Where(p => p.Plan.Code != ProductPlanConstants.LifePlanCode);

            foreach (var p in otherPlans)
            {
                p.Plan.Selected = false;
                foreach (var cover in p.Covers)
                {
                    cover.Selected = true;
                }
            }            
        }

        private void SetLifePlanAndCoversActive(PolicyWithRisks policyWithRisks)
        {
            var risk = policyWithRisks.Risks.First();
            var lifePlan = risk.Plans.First(p => p.Plan.Code == ProductPlanConstants.LifePlanCode);
            lifePlan.Plan.Selected = true;

            foreach (var cover in lifePlan.Covers)
            {
                cover.Selected = true;
            }
        }

        private void SetLifePlanAndRidersActive(PolicyWithRisks policyWithRisks)
        {
            var risk = policyWithRisks.Risks.First();
            var lifePlan = risk.Plans.First(p => p.Plan.Code == ProductPlanConstants.LifePlanCode);
            lifePlan.Plan.Selected = true;

            foreach (var cover in lifePlan.Covers)
            {
                cover.Selected = true;
            }

            var riderPlans = risk.Plans.Where(p => p.Plan.Code == ProductRiderConstants.PermanentDisabilityRiderCode || p.Plan.Code == ProductRiderConstants.CriticalIllnessRiderCode);

            foreach (var p in riderPlans)
            {
                p.Plan.Selected = true;
                foreach (var cover in p.Covers)
                {
                    cover.Selected = true;
                }
            }
        }

        private void SetIpPlanAndCoversActive(PolicyWithRisks policyWithRisks)
        {
            var risk = policyWithRisks.Risks.First();
            var lifePlan = risk.Plans.First(p => p.Plan.Code == ProductPlanConstants.IncomeProtectionPlanCode);
            lifePlan.Plan.Selected = true;

            foreach (var cover in lifePlan.Covers)
            {
                cover.Selected = true;
            }
        }

        private void SetAllParentPlansInactiveAndRidersActive(PolicyWithRisks policyWithRisks)
        {
            var risk = policyWithRisks.Risks.First();

            foreach (var p in risk.Plans)
            {
                p.Plan.Selected = false;
            }

            var riderPlans = risk.Plans.Where(p => p.Plan.Code == ProductRiderConstants.PermanentDisabilityRiderCode || p.Plan.Code == ProductRiderConstants.CriticalIllnessRiderCode);

            foreach (var p in riderPlans)
            {
                p.Plan.Selected = true;
                foreach (var cover in p.Covers)
                {
                    cover.Selected = true;
                }
            }
        }

        private void SetLifePlanActiveAndCoversInactive(PolicyWithRisks policyWithRisks)
        {
            var risk = policyWithRisks.Risks.First();
            var lifePlan = risk.Plans.First(p => p.Plan.Code == ProductPlanConstants.LifePlanCode);
            lifePlan.Plan.Selected = true;

            foreach (var cover in lifePlan.Covers)
            {
                cover.Selected = false;
            }
        }

        private IOption SetPremiumReliefForLifePlanAndReturn(PolicyWithRisks policyWithRisks)
        {
            var risk = policyWithRisks.Risks.First();
            var lifePlan = risk.Plans.First(p => p.Plan.Code == ProductPlanConstants.LifePlanCode);

            var premiumReliefOption = lifePlan.Options.First(o => o.Code == ProductOptionConstants.LifePremiumRelief);
            premiumReliefOption.Selected = true;

            return premiumReliefOption;
        }
    }
}