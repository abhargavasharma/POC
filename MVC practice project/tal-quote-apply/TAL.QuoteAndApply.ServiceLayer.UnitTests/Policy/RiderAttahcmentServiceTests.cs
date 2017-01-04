using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TAL.QuoteAndApply.DataModel.Product;
using TAL.QuoteAndApply.Product.Definition;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.Plan;
using TAL.QuoteAndApply.ServiceLayer.Product;
using TAL.QuoteAndApply.Tests.Shared.Helpers;
using TAL.QuoteAndApply.Tests.Shared.Mocks;

namespace TAL.QuoteAndApply.ServiceLayer.UnitTests.Policy
{
    [TestFixture]
    public class RiderAttahcmentServiceTests
    {
        [Test]
        public void AttachRider_TRStoDTH_RiderAndCoversSelected()
        {
            var productDefintionService = new ProductDefinitionService(new ProductDefinitionBuilder(new MockProductBrandSettingsProvider()), new TestCurrentProductBrandProvider());

            var service = new RiderAttachmentService(productDefintionService);

            var planWithRiders = PlanStateParam.BuildPlanStateParam("DTH", "tal", true, 1, 1, false, 500000, false,
                PremiumType.Level, 1, 30,
                50000, null, null, OccupationDefinition.Unknown, new List<PlanStateParam>
                {
                    PlanStateParam.BuildRiderPlanStateParam("TPDDTH", "tal", false, 1, 1, false, 0, false, PremiumType.Stepped, 2, 30, 50000, new List<OptionsParam>(), new List<string>(), 500000, OccupationDefinition.AnyOccupation),
                    PlanStateParam.BuildRiderPlanStateParam("TRADTH", "tal", false, 1, 1, false, 0, false, PremiumType.Stepped, 3, 30, 50000, new List<OptionsParam>(), new List<string>(), 500000, OccupationDefinition.AnyOccupation)
                }, new List<OptionsParam>(), new List<PlanIdentityInfo>(),
                new List<string>());

            var planTpd = PlanStateParam.BuildPlanStateParam("TRS", "tal", true, 1, 1, false, 250000, false,
                PremiumType.Stepped, 1, 30, 50000, null, null, OccupationDefinition.Unknown, new List<PlanStateParam>(), new List<OptionsParam>(),
                new List<PlanIdentityInfo>(), new [] { "TRSSIC", "TRSCC" });

            service.AttachRider(planTpd, planWithRiders);

            Assert.That(planTpd.Selected, Is.False);
            var attachedRider = planWithRiders.Riders.First(r => r.PlanCode == "TRADTH");
            Assert.That(attachedRider.Selected, Is.True);
            Assert.That(attachedRider.SelectedCoverCodes, Contains.Item("TRADTHSIC"));
            Assert.That(attachedRider.SelectedCoverCodes, Contains.Item("TRADTHCC"));
            Assert.That(attachedRider.CoverAmount, Is.EqualTo(250000));
        }

        [Test]
        public void DetachhRider_TRStoDTH_RiderAndCoversSelected()
        {
            var productDefintionService = new ProductDefinitionService(new ProductDefinitionBuilder(new MockProductBrandSettingsProvider()), new TestCurrentProductBrandProvider());

            var service = new RiderAttachmentService(productDefintionService);

            var planWithRiders = PlanStateParam.BuildPlanStateParam("DTH", "tal", true, 1, 1, false, 500000, false,
                PremiumType.Level, 1, 30,
                50000, null, null, OccupationDefinition.Unknown, new List<PlanStateParam>
                {
                    PlanStateParam.BuildRiderPlanStateParam("TPDDTH", "tal", false, 1, 1, false, 0, false, PremiumType.Stepped, 2, 30, 50000, new List<OptionsParam>(), new List<string>(), 500000, OccupationDefinition.AnyOccupation),
                    PlanStateParam.BuildRiderPlanStateParam("TRADTH", "tal", true, 1, 1, false, 250000, false, PremiumType.Stepped, 3, 30, 50000, new List<OptionsParam>(), new [] {"TRADTHSIC", "TRADTHCC"}, 500000, OccupationDefinition.AnyOccupation)
                }, new List<OptionsParam>(), new List<PlanIdentityInfo>(),
                new List<string>());

            var planTpd = PlanStateParam.BuildPlanStateParam("TRS", "tal", false, 1, 1, false, 0, false,
                PremiumType.Stepped, 1, 30, 50000, null, null, OccupationDefinition.Unknown, new List<PlanStateParam>(), new List<OptionsParam>(),
                new List<PlanIdentityInfo>(), new[] { "", "" });

            service.DetachRider(planWithRiders.Riders[1], planTpd);

            Assert.That(planTpd.Selected, Is.False);
            var attachedRider = planWithRiders.Riders.First(r => r.PlanCode == "TRADTH");
            Assert.That(attachedRider.Selected, Is.False);
            Assert.That(planTpd.SelectedCoverCodes, Contains.Item("TRSSIC"));
            Assert.That(planTpd.SelectedCoverCodes, Contains.Item("TRSCC"));
            Assert.That(planTpd.CoverAmount, Is.EqualTo(250000));
        }
    }
}
