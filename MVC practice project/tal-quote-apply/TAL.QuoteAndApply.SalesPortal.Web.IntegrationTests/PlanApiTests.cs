using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TAL.Performance.Infrastructure.Core;
using TAL.QuoteAndApply.DataModel.Product;
using TAL.QuoteAndApply.SalesPortal.Web.Extensions;
using TAL.QuoteAndApply.SalesPortal.Web.IntegrationTests.Clients;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;
using TAL.QuoteAndApply.ServiceLayer.Policy.Plan;
using TAL.QuoteAndApply.Tests.Shared.Helpers;
using Task = System.Threading.Tasks.Task;

namespace TAL.QuoteAndApply.SalesPortal.Web.IntegrationTests
{
    [TestFixture]
    public class PlanApiTests : BaseTestClass<PlanClient>
    {
        public PlanApiTests() : base(SimplePerformanceTool.CreateConsoleTimestamped())
        {
        }

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {

        }

      
        [Test]
        public async Task GetPlansAndCovers_Aged59_EligibleForAllPlansAndRiders()
        {
            var policyWithRisks = await CreatePolicyFromRatingFactorsAsync(new RatingFactorsRequest
            {
                AustralianResident = true,
                DateOfBirth = DateTime.Now.AddYears(-59).ToFriendlyString(),
                Gender = 'M',
                Income = 100000,
                OccupationCode = "229",
                IndustryCode = "27l"
            });

            var risk = policyWithRisks.Risks.First();
            var plansAndCovers = await Client.GetPlansAndCoversAsync(policyWithRisks.Policy.QuoteReference, risk.Risk.Id);

            foreach (var plan in plansAndCovers.Plans)
            {
                Assert.That(plan.EligibleForPlan, Is.True);
                foreach (var rider in plan.Riders)
                {
                    Assert.That(rider.EligibleForPlan, Is.True);
                }
            }
        }

        [Test]
        public async Task GetPlansAndCovers_Aged60_OnlyLifeIsEligible()
        {
            var policyWithRisks = await CreatePolicyFromRatingFactorsAsync(new RatingFactorsRequest
            {
                AustralianResident = true,
                DateOfBirth = DateTime.Now.AddYears(-60).ToFriendlyString(),
                Gender = 'M',
                Income = 100000,
                OccupationCode = "229",
                IndustryCode = "27l"
            });

            var risk = policyWithRisks.Risks.First();
            var plansAndCovers = await Client.GetPlansAndCoversAsync(policyWithRisks.Policy.QuoteReference, risk.Risk.Id);

            foreach (var plan in plansAndCovers.Plans)
            {
                if (plan.Code == "DTH")
                {
                    Assert.That(plan.EligibleForPlan, Is.True);
                }
                else
                {
                    Assert.That(plan.EligibleForPlan, Is.False);
                }

                foreach (var rider in plan.Riders)
                {
                    Assert.That(rider.EligibleForPlan, Is.False);
                }
            }
        }

        [Test]
        public async Task Update_LifePlanSelectedWithNoCovers_ReturnsResponseWithNoErrorsAndAWarning()
        {
            var policyWithRisks = await CreatePolicyFromRatingFactorsAsync(new RatingFactorsRequest
            {
                AustralianResident = true,
                DateOfBirth = DateTime.Now.AddYears(-35).ToFriendlyString(),
                Gender = 'M',
                Income = 100000,
                OccupationCode = "229",
                IndustryCode = "27l"
            });

            var risk = policyWithRisks.Risks.First();
            var plansAndCovers = await Client.GetPlansAndCoversAsync(policyWithRisks.Policy.QuoteReference, risk.Risk.Id);

            var lifePlan = plansAndCovers.Plans.Single(p => p.Code == "DTH");

            var planUpdate = new PlanUpdateRequest
            {
                CurrentActivePlan = new PlanConfigurationRequest
                {
                    CoverAmount = 100000,
                    LinkedToCpi = false,
                    PlanCode = "DTH",
                    PlanId = lifePlan.PlanId,
                    SelectedCoverCodes = new string[0],
                    SelectedOptionCodes = new OptionConfigurationRequest[0],
                    SelectedRiders = new PlanConfigurationRequest[0],
                    PremiumHoliday = false,
                    PremiumType = "Level",
                    Selected = true
                }, QuoteReferenceNumber = policyWithRisks.Policy.QuoteReference,
                RiskId = risk.Risk.Id.ToString(),
                SelectedPlanCodes = new [] {"DTH"}
            };
            
            var response = await Client.UpdateAsync<PlansUpdateResponse>(policyWithRisks.Policy.QuoteReference, risk.Risk.Id, planUpdate);

            Assert.That(response.CurrentActivePlan.Code == "DTH");
            Assert.That(response.CurrentActivePlan.Selected);
            Assert.That(response.CurrentActivePlan.Warnings.Any(), Is.True);
            AssertResponseHasWarning(response, "dth", "Life Insurance cannot be selected without any selected covers");
        }

        [Test]
        public async Task Update_PlanWithLifePlanAndTpdAndTraumaRiderSelected_ReturnsResponseWithNoErrors()
        {
            var policyWithRisks = await CreatePolicyFromRatingFactorsAsync(new RatingFactorsRequest
            {
                AustralianResident = true,
                DateOfBirth = DateTime.Now.AddYears(-35).ToFriendlyString(),
                Gender = 'M',
                Income = 100000,
                OccupationCode = "229",
                IndustryCode = "27l"
            });

            var risk = policyWithRisks.Risks.First();
            var plansAndCovers = await Client.GetPlansAndCoversAsync(policyWithRisks.Policy.QuoteReference, risk.Risk.Id);

            var lifePlan = plansAndCovers.Plans.Single(p => p.Code == "DTH");

            var planUpdate = new PlanUpdateRequest
            {
                CurrentActivePlan = new PlanConfigurationRequest
                {
                    CoverAmount = 100000,
                    LinkedToCpi = false,
                    PlanCode = "DTH",
                    PlanId = lifePlan.PlanId,
                    SelectedCoverCodes = new [] {"DTHAC"},
                    SelectedOptionCodes = new OptionConfigurationRequest[0],
                    SelectedRiders = new PlanConfigurationRequest[2] {
                        new PlanConfigurationRequest()
                        {
                            CoverAmount = 100000,
                            LinkedToCpi = false,
                            PlanCode = "TPDDTH",
                            PlanId = lifePlan.PlanId,
                            SelectedCoverCodes = new string[] {"TPDDTHAC"},
                            SelectedOptionCodes = new OptionConfigurationRequest[0],
                            SelectedRiders = new PlanConfigurationRequest[0],
                            PremiumHoliday = false,
                            PremiumType = "Level",
                            Selected = true
                        },
                        new PlanConfigurationRequest()
                        {
                            CoverAmount = 100000,
                            LinkedToCpi = false,
                            PlanCode = "TRADTH",
                            PlanId = lifePlan.PlanId,
                            SelectedCoverCodes = new string[] {"TRADTHSIN"},
                            SelectedOptionCodes = new OptionConfigurationRequest[0],
                            SelectedRiders = new PlanConfigurationRequest[0],
                            PremiumHoliday = false,
                            PremiumType = "Level",
                            Selected = true
                        }
                    },
                    PremiumHoliday = false,
                    PremiumType = "Level",
                    Selected = true
                },
                QuoteReferenceNumber = policyWithRisks.Policy.QuoteReference,
                RiskId = risk.Risk.Id.ToString(),
                SelectedPlanCodes = new[] { "DTH" },
            };

            var response = await Client.UpdateAsync<PlansUpdateResponse>(policyWithRisks.Policy.QuoteReference, risk.Risk.Id, planUpdate);

            Assert.That(response.CurrentActivePlan.Code == "DTH");
            Assert.That(response.CurrentActivePlan.Selected);
            Assert.That(response.CurrentActivePlan.Riders.First().Code == "TPDDTH");
            Assert.That(response.CurrentActivePlan.Riders.First().Selected);
            Assert.That(response.CurrentActivePlan.Riders.Any(r=>r.Code == "TRADTH"));
            Assert.That(response.CurrentActivePlan.Riders.First(r => r.Code == "TRADTH").Selected);
        }
        
        [Test]
        public async Task Update_SportsCoverWithoutAccidentCover_ReturnsModelStateErrors_Async()
        {
            var policyWithRisks = await CreatePolicyFromRatingFactorsAsync(new RatingFactorsRequest
            {
                AustralianResident = true,
                DateOfBirth = DateTime.Now.AddYears(-35).ToFriendlyString(),
                Gender = 'M',
                Income = 100000,
                OccupationCode = "229",
                IndustryCode = "27l"
            });

            var risk = policyWithRisks.Risks.First().Risk;
            var plansAndCovers = await Client.GetPlansAndCoversAsync(policyWithRisks.Policy.QuoteReference, risk.Id);

            var lifePlan = plansAndCovers.Plans.Single(p => p.Code == "DTH");

            var planUpdate = new PlanUpdateRequest
            {
                CurrentActivePlan = new PlanConfigurationRequest
                {
                    CoverAmount = 100000,
                    LinkedToCpi = false,
                    PlanCode = "DTH",
                    PlanId = lifePlan.PlanId,
                    SelectedCoverCodes = new string[] { "DTHASC" },
                    SelectedOptionCodes = new OptionConfigurationRequest[0],
                    SelectedRiders = new PlanConfigurationRequest[0],
                    PremiumHoliday = false,
                    PremiumType = "Level"
                },
                QuoteReferenceNumber = policyWithRisks.Policy.QuoteReference,
                RiskId = risk.Id.ToString(),
                SelectedPlanCodes = new[] { "DTH" }
            };

            var response = await Client.UpdateAsync<PlansUpdateResponse>(policyWithRisks.Policy.QuoteReference, risk.Id, planUpdate, throwOnFailure: false);

            Assert.That(response, Is.Not.Null);
            AssertResponseHasWarning(response, "dthasc", "Adventure Sports Cover cannot be selected without Accident Cover.");
        }

        [Test]
        public async Task Update_LifePlanWithOptionValidationErrors_ReturnsCorrectModelStateErrors()
        {
            var policyWithRisks = await CreatePolicyFromRatingFactorsAsync(new RatingFactorsRequest
            {
                AustralianResident = true,
                DateOfBirth = DateTime.Now.AddYears(-35).ToFriendlyString(),
                Gender = 'M',
                Income = 100000,
                OccupationCode = "229",
                IndustryCode = "27l"
            });

            var risk = policyWithRisks.Risks.First().Risk;
            var plansAndCovers = await Client.GetPlansAndCoversAsync(policyWithRisks.Policy.QuoteReference, risk.Id);

            var lifePlan = plansAndCovers.Plans.Single(p => p.Code == "DTH");

            var planUpdate = new PlanUpdateRequest
            {
                CurrentActivePlan = new PlanConfigurationRequest
                {
                    CoverAmount = 100000,
                    LinkedToCpi = false,
                    PlanCode = "DTH",
                    PlanId = lifePlan.PlanId,
                    SelectedCoverCodes = new string[1] { "DTHASC" },
                    SelectedOptionCodes = new OptionConfigurationRequest[] { new OptionConfigurationRequest("PR", "Premium Relief", true) },
                    SelectedRiders = new PlanConfigurationRequest[0],
                    PremiumHoliday = false,
                    PremiumType = "Level",
                    BenefitPeriod = 2,
                    WaitingPeriod = 4
                },
                QuoteReferenceNumber = policyWithRisks.Policy.QuoteReference,
                RiskId = risk.Id.ToString(),
                SelectedPlanCodes = new[] { "DTH" }
            };

            var response = await Client.UpdateAsync<PlansUpdateResponse>(policyWithRisks.Policy.QuoteReference, risk.Id, planUpdate, throwOnFailure: false);
            
            Assert.That(response, Is.Not.Null);
            AssertResponseHasWarning(response, "dthasc", "Adventure Sports Cover cannot be selected without Accident Cover.");
        }

        [Test]
        public async Task Update_TpdPlanWithOptionValidationErrors_ReturnsCorrectModelStateErrors()
        {
            var policyWithRisks = await CreatePolicyFromRatingFactorsAsync(new RatingFactorsRequest
            {
                AustralianResident = true,
                DateOfBirth = DateTime.Now.AddYears(-35).ToFriendlyString(),
                Gender = 'M',
                Income = 100000,
                OccupationCode = "229",
                IndustryCode = "27l"
            });

            var risk = policyWithRisks.Risks.First().Risk;
            var plansAndCovers = await Client.GetPlansAndCoversAsync(policyWithRisks.Policy.QuoteReference, risk.Id);

            var tpdPlan = plansAndCovers.Plans.Single(p => p.Code == "TPS");

            var planUpdate = new PlanUpdateRequest
            {
                CurrentActivePlan = new PlanConfigurationRequest
                {
                    CoverAmount = 100000,
                    LinkedToCpi = false,
                    PlanCode = "TPS",
                    PlanId = tpdPlan.PlanId,
                    SelectedCoverCodes = new string[1] { "TPSASC" },
                    SelectedOptionCodes = new OptionConfigurationRequest[] { new OptionConfigurationRequest("PR", "Premium Relief", true) },
                    SelectedRiders = new PlanConfigurationRequest[0],
                    PremiumHoliday = false,
                    PremiumType = "Level",
                    OccupationDefinition = "AnyOccupation"
                },
                QuoteReferenceNumber = policyWithRisks.Policy.QuoteReference,
                RiskId = risk.Id.ToString(),
                SelectedPlanCodes = new[] { "TPS" }
            };

            var response = await Client.UpdateAsync<PlansUpdateResponse>(policyWithRisks.Policy.QuoteReference, risk.Id, planUpdate, throwOnFailure: false);

            Assert.That(response, Is.Not.Null);
            AssertResponseHasWarning(response, "tpsasc", "Sports Cover cannot be selected without Accident Cover.");
        }

        [Test]
        public async Task Update_CiPlanWithOptionValidationErrors_ReturnsCorrectModelStateErrors()
        {
            var policyWithRisks = await CreatePolicyFromRatingFactorsAsync(new RatingFactorsRequest
            {
                AustralianResident = true,
                DateOfBirth = DateTime.Now.AddYears(-35).ToFriendlyString(),
                Gender = 'M',
                Income = 100000,
                OccupationCode = "229",
                IndustryCode = "27l"
            });

            var risk = policyWithRisks.Risks.First().Risk;
            var plansAndCovers = await Client.GetPlansAndCoversAsync(policyWithRisks.Policy.QuoteReference, risk.Id);

            var ciPlan = plansAndCovers.Plans.Single(p => p.Code == "TRS");

            var planUpdate = new PlanUpdateRequest
            {
                CurrentActivePlan = new PlanConfigurationRequest
                {
                    CoverAmount = 100000,
                    LinkedToCpi = false,
                    PlanCode = "TRS",
                    PlanId = ciPlan.PlanId,
                    SelectedCoverCodes = new string[1] { "TRSSIC" },
                    SelectedOptionCodes = new OptionConfigurationRequest[] { new OptionConfigurationRequest("PR", "Premium Relief", true) },
                    SelectedRiders = new PlanConfigurationRequest[0],
                    PremiumHoliday = false,
                    PremiumType = "Level"
                },
                QuoteReferenceNumber = policyWithRisks.Policy.QuoteReference,
                RiskId = risk.Id.ToString(),
                SelectedPlanCodes = new[] { "TRS" }
            };

            var response = await Client.UpdateAsync<PlansUpdateResponse>(policyWithRisks.Policy.QuoteReference, risk.Id, planUpdate);

            Assert.That(response, Is.Not.Null);
        }

        [Test]
        public async Task Update_IplanWithOptionValidationErrors_ReturnsCorrectModelStateErrors()
        {            
            var policyWithRisks = await CreatePolicyFromRatingFactorsAsync(new RatingFactorsRequest
            {
                AustralianResident = true,
                DateOfBirth = DateTime.Now.AddYears(-35).ToFriendlyString(),
                Gender = 'M',
                Income = 100000,
                OccupationCode = "229",
                IndustryCode = "27l"
            });

            var risk = policyWithRisks.Risks.First().Risk;
            var plansAndCovers = await Client.GetPlansAndCoversAsync(policyWithRisks.Policy.QuoteReference, risk.Id);

            var ipPlan = plansAndCovers.Plans.Single(p => p.Code == "IP");

            var planUpdate = new PlanUpdateRequest
            {
                CurrentActivePlan = new PlanConfigurationRequest
                {
                    CoverAmount = 5000,
                    LinkedToCpi = false,
                    PlanCode = "IP",
                    PlanId = ipPlan.PlanId,
                    SelectedCoverCodes = new string[1] { "IPSSC" },
                    SelectedOptionCodes = new OptionConfigurationRequest[] { new OptionConfigurationRequest("DOA", "Day One Accident", true) },
                    SelectedRiders = new PlanConfigurationRequest[0],
                    PremiumHoliday = false,
                    PremiumType = "Level",
                    BenefitPeriod = 2,
                    WaitingPeriod = 4
                },
                QuoteReferenceNumber = policyWithRisks.Policy.QuoteReference,
                RiskId = risk.Id.ToString(),
                SelectedPlanCodes = new[] { "IP" }
            };

            var response = await Client.UpdateAsync<PlansUpdateResponse>(policyWithRisks.Policy.QuoteReference, risk.Id, planUpdate, throwOnFailure: false);

            Assert.That(response, Is.Not.Null);
            AssertResponseHasWarning(response, "ipssc", "Sports Cover cannot be selected without Accident Cover.");
            AssertResponseHasWarning(response, "ipDOA", "Day One Accident cannot be selected without Accident Cover.");
        }

        [Test]
        public async Task Update_IpPlanWithInvalidWaitingPeriod_ReturnsCorrectModelStateErrors()
        {
            var policyWithRisks = await CreatePolicyFromRatingFactorsAsync(new RatingFactorsRequest
            {
                AustralianResident = true,
                DateOfBirth = DateTime.Now.AddYears(-35).ToFriendlyString(),
                Gender = 'M',
                Income = 100000,
                OccupationCode = UnderwritingHelper.OccupationCode_WharfWorker,
                IndustryCode = UnderwritingHelper.IndustryCode_Marine
            });

            var risk = policyWithRisks.Risks.First().Risk;
            var plansAndCovers = await Client.GetPlansAndCoversAsync(policyWithRisks.Policy.QuoteReference, risk.Id);

            var ipPlan = plansAndCovers.Plans.Single(p => p.Code == "IP");

            var planUpdate = new PlanUpdateRequest
            {
                CurrentActivePlan = new PlanConfigurationRequest
                {
                    CoverAmount = 5000,
                    LinkedToCpi = false,
                    PlanCode = "IP",
                    PlanId = ipPlan.PlanId,
                    SelectedCoverCodes = new string[3] { "IPSAC", "IPSIC", "IPSSC" },
                    SelectedOptionCodes = new OptionConfigurationRequest[] { new OptionConfigurationRequest("DOA", "Day One Accident", true) },
                    SelectedRiders = new PlanConfigurationRequest[0],
                    PremiumHoliday = false,
                    PremiumType = "Level",
                    BenefitPeriod = 2,
                    WaitingPeriod = 2 //2 week waiting period is invalid for this occupation
                },
                QuoteReferenceNumber = policyWithRisks.Policy.QuoteReference,
                RiskId = risk.Id.ToString(),
                SelectedPlanCodes = new[] { "IP" }
            };

            var response = await Client.UpdateAsync<PlansUpdateResponse>(policyWithRisks.Policy.QuoteReference, risk.Id, planUpdate, throwOnFailure: false);

            Assert.That(response, Is.Not.Null);
            AssertResponseHasWarning(response, "ipwaitingPeriod", "2 Weeks cannot be selected for the current occupation");
        }

        [Test]
        public async Task Update_IplanWithDoaOptionWaitingPeriodValidationErrors_ReturnsCorrectModelStateErrors()
        {
            var policyWithRisks = await CreatePolicyFromRatingFactorsAsync(new RatingFactorsRequest
            {
                AustralianResident = true,
                DateOfBirth = DateTime.Now.AddYears(-35).ToFriendlyString(),
                Gender = 'M',
                Income = 100000,
                OccupationCode = "229",
                IndustryCode = "27l"
            });

            var risk = policyWithRisks.Risks.First().Risk;
            var plansAndCovers = await Client.GetPlansAndCoversAsync(policyWithRisks.Policy.QuoteReference, risk.Id);

            var ipPlan = plansAndCovers.Plans.Single(p => p.Code == "IP");

            int waitingPeriod = 13;

            var planUpdate = new PlanUpdateRequest
            {
                CurrentActivePlan = new PlanConfigurationRequest
                {
                    CoverAmount = 5000,
                    LinkedToCpi = false,
                    PlanCode = "IP",
                    PlanId = ipPlan.PlanId,
                    SelectedCoverCodes = new string[1] { "IPSAC" },
                    SelectedOptionCodes = new OptionConfigurationRequest[] { new OptionConfigurationRequest("DOA", "Day One Accident", true) },
                    SelectedRiders = new PlanConfigurationRequest[0],
                    PremiumHoliday = false,
                    PremiumType = "Level",
                    BenefitPeriod = 2,
                    WaitingPeriod = waitingPeriod
                },
                QuoteReferenceNumber = policyWithRisks.Policy.QuoteReference,
                RiskId = risk.Id.ToString(),
                SelectedPlanCodes = new[] { "IP" }
            };

            var response = await Client.UpdateAsync<PlansUpdateResponse>(policyWithRisks.Policy.QuoteReference, risk.Id, planUpdate, throwOnFailure: false);

            Assert.That(response, Is.Not.Null);
            AssertResponseHasWarning(response, "ipDOA", $"Day One Accident cannot be selected when the Waiting Period is {waitingPeriod} weeks");
        }

        [Test]
        public async Task Update_IplanWithDoaOptionOccupationClassValidationErrors_ReturnsCorrectModelStateErrors()
        {
            //occ: Plant Operator is SRA
            var policyWithRisks = await CreatePolicyFromRatingFactorsAsync(new RatingFactorsRequest
            {
                AustralianResident = true,
                DateOfBirth = DateTime.Now.AddYears(-35).ToFriendlyString(),
                Gender = 'M',
                Income = 100000,
                OccupationCode = "1le",
                IndustryCode = "27x"
            });

            var risk = policyWithRisks.Risks.First().Risk;
            var plansAndCovers = await Client.GetPlansAndCoversAsync(policyWithRisks.Policy.QuoteReference, risk.Id);

            var ipPlan = plansAndCovers.Plans.Single(p => p.Code == "IP");

            var planUpdate = new PlanUpdateRequest
            {
                CurrentActivePlan = new PlanConfigurationRequest
                {
                    CoverAmount = 5000,
                    LinkedToCpi = false,
                    PlanCode = "IP",
                    PlanId = ipPlan.PlanId,
                    SelectedCoverCodes = new string[1] { "IPSAC" },
                    SelectedOptionCodes = new OptionConfigurationRequest[] { new OptionConfigurationRequest("DOA", "Day One Accident", true) },
                    SelectedRiders = new PlanConfigurationRequest[0],
                    PremiumHoliday = false,
                    PremiumType = "Level",
                    BenefitPeriod = 2,
                    WaitingPeriod = 4
                },
                QuoteReferenceNumber = policyWithRisks.Policy.QuoteReference,
                RiskId = risk.Id.ToString(),
                SelectedPlanCodes = new[] { "IP" }
            };

            var response = await Client.UpdateAsync<PlansUpdateResponse>(policyWithRisks.Policy.QuoteReference, risk.Id, planUpdate, throwOnFailure: false);

            Assert.That(response, Is.Not.Null);
            AssertResponseHasWarning(response, "ipDOA", "Day One Accident cannot be selected for the selected occupation");
        }

        [Test]
        public async Task Update_IplanWithIncreasingClaimsOptionWithOutLinkedToCpi_ReturnsCorrectModelStateErrors()
        {
            //occ: Plant Operator is SRA
            var policyWithRisks = await CreatePolicyFromRatingFactorsAsync(new RatingFactorsRequest
            {
                AustralianResident = true,
                DateOfBirth = DateTime.Now.AddYears(-35).ToFriendlyString(),
                Gender = 'M',
                Income = 100000,
                OccupationCode = "1le",
                IndustryCode = "27x"
            });

            var risk = policyWithRisks.Risks.First().Risk;
            var plansAndCovers = await Client.GetPlansAndCoversAsync(policyWithRisks.Policy.QuoteReference, risk.Id);

            var ipPlan = plansAndCovers.Plans.Single(p => p.Code == "IP");

            var planUpdate = new PlanUpdateRequest
            {
                CurrentActivePlan = new PlanConfigurationRequest
                {
                    CoverAmount = 5000,
                    LinkedToCpi = false,
                    PlanCode = "IP",
                    PlanId = ipPlan.PlanId,
                    SelectedCoverCodes = new string[1] { "IPSAC" },
                    SelectedOptionCodes = new OptionConfigurationRequest[] { new OptionConfigurationRequest("IC", "Increasing Claims", true) },
                    SelectedRiders = new PlanConfigurationRequest[0],
                    PremiumHoliday = false,
                    PremiumType = "Level",
                    BenefitPeriod = 2,
                    WaitingPeriod = 4
                },
                QuoteReferenceNumber = policyWithRisks.Policy.QuoteReference,
                RiskId = risk.Id.ToString(),
                SelectedPlanCodes = new[] { "IP" }
            };

            var response = await Client.UpdateAsync<PlansUpdateResponse>(policyWithRisks.Policy.QuoteReference, risk.Id, planUpdate, throwOnFailure: false);

            Assert.That(response, Is.Not.Null);
            AssertResponseHasWarning(response, "ipIC", "Increasing Claims requires Inflation Protection to be selected");
        }

        [Test]
        public async Task Update_PlanWithLifePlanAndSumInsuredGreaterThan1M_ReturnsWithMaxCoverAmounErrorMessage()
        {
            var policyWithRisks = await CreatePolicyFromRatingFactorsAsync(new RatingFactorsRequest
            {
                AustralianResident = true,
                DateOfBirth = DateTime.Now.AddYears(-30).ToFriendlyString(),
                Gender = 'M',
                Income = 100000,
                OccupationCode = "229",
                IndustryCode = "27l"
            });

            var risk = policyWithRisks.Risks.First();
            var plansAndCovers = await Client.GetPlansAndCoversAsync(policyWithRisks.Policy.QuoteReference, risk.Risk.Id);

            var lifePlan = plansAndCovers.Plans.Single(p => p.Code == "DTH");

            var planUpdate = new PlanUpdateRequest
            {
                CurrentActivePlan = new PlanConfigurationRequest
                {
                    CoverAmount = 888888888,
                    LinkedToCpi = false,
                    PlanCode = "DTH",
                    PlanId = lifePlan.PlanId,
                    SelectedCoverCodes = new [] {"DTHAC"},
                    SelectedOptionCodes = new OptionConfigurationRequest[0],
                    SelectedRiders = new PlanConfigurationRequest[0],
                    PremiumHoliday = false,
                    PremiumType = "Level",
                    Selected = true
                },
                QuoteReferenceNumber = policyWithRisks.Policy.QuoteReference,
                RiskId = risk.Risk.Id.ToString(),
                SelectedPlanCodes = new[] { "DTH" },
            };

            var response = await Client.UpdateAsync<Dictionary<string, IEnumerable<string>>>(policyWithRisks.Policy.QuoteReference, risk.Risk.Id, planUpdate, throwOnFailure: false);

            Assert.That(response, Is.Not.Null);
            Assert.That(response.ContainsKey("dthCoverAmount"), Is.True);
            Assert.That(response.First(x => x.Key.Equals("dthCoverAmount")).Value.First(),
                Is.EqualTo("Cover amount cannot exceed $1,500,000"));
        }

        [Test]
        public async Task Update_PlanWithLifePlanAndSumInsuredLessThan100000_ReturnsWithMinCoverAmounErrorMessage_Async()
        {
            var policyWithRisks = await CreatePolicyFromRatingFactorsAsync(new RatingFactorsRequest
            {
                AustralianResident = true,
                DateOfBirth = DateTime.Now.AddYears(-30).ToFriendlyString(),
                Gender = 'M',
                Income = 100000,
                OccupationCode = "229",
                IndustryCode = "27l"
            });

            var risk = policyWithRisks.Risks.First();
            var plansAndCovers = await Client.GetPlansAndCoversAsync(policyWithRisks.Policy.QuoteReference, risk.Risk.Id);

            var lifePlan = plansAndCovers.Plans.Single(p => p.Code == "DTH");

            var planUpdate = new PlanUpdateRequest
            {
                CurrentActivePlan = new PlanConfigurationRequest
                {
                    CoverAmount = 99999,
                    LinkedToCpi = false,
                    PlanCode = "DTH",
                    PlanId = lifePlan.PlanId,
                    SelectedCoverCodes = new[] { "DTHAC" },
                    SelectedOptionCodes = new OptionConfigurationRequest[0],
                    SelectedRiders = new PlanConfigurationRequest[0],
                    PremiumHoliday = false,
                    PremiumType = "Level",
                    Selected = true
                },
                QuoteReferenceNumber = policyWithRisks.Policy.QuoteReference,
                RiskId = risk.Risk.Id.ToString(),
                SelectedPlanCodes = new[] { "DTH" },
            };

            var response = await Client.UpdateAsync<Dictionary<string, IEnumerable<string>>>(policyWithRisks.Policy.QuoteReference, risk.Risk.Id, planUpdate, throwOnFailure: false);

            Assert.That(response, Is.Not.Null);
            Assert.That(response.ContainsKey("dthCoverAmount"), Is.True);
            Assert.That(response.First(x => x.Key.Equals("dthCoverAmount")).Value.First(),
                Is.EqualTo("Minimum cover is $100,000"));
        }

        [Test]
        public async Task Update_PlanWithLifePlanAndTpdRider_TpdRiderCoverAmountMoreThanLifeCoverAmount_ReturnsWithMaxCoverAmounErrorMessageForTpdRider()
        {
            var policyWithRisks = await CreatePolicyFromRatingFactorsAsync(new RatingFactorsRequest
            {
                AustralianResident = true,
                DateOfBirth = DateTime.Now.AddYears(-30).ToFriendlyString(),
                Gender = 'M',
                Income = 100000,
                OccupationCode = "229",
                IndustryCode = "27l"
            });

            var risk = policyWithRisks.Risks.First();
            var plansAndCovers = await Client.GetPlansAndCoversAsync(policyWithRisks.Policy.QuoteReference, risk.Risk.Id);

            var lifePlan = plansAndCovers.Plans.Single(p => p.Code == "DTH");

            var planUpdate = new PlanUpdateRequest
            {
                CurrentActivePlan = new PlanConfigurationRequest
                {
                    CoverAmount = 200000,
                    LinkedToCpi = false,
                    PlanCode = "DTH",
                    PlanId = lifePlan.PlanId,
                    SelectedCoverCodes = new[] { "DTHAC" },
                    SelectedOptionCodes = new OptionConfigurationRequest[0],
                    SelectedRiders = new List<PlanConfigurationRequest>
                    {
                        new PlanConfigurationRequest()
                        {
                            CoverAmount = 200001,
                            LinkedToCpi = false,
                            PlanCode = "TPDDTH",
                            PlanId = lifePlan.PlanId,
                            SelectedCoverCodes = new string[3] {"TPDDTHAC", "TPDDTHIC", "TPDDTHASC"},
                            SelectedOptionCodes = new OptionConfigurationRequest[0],
                            SelectedRiders = new PlanConfigurationRequest[0],
                            PremiumHoliday = false,
                            PremiumType = "Level",
                            Selected = true
                        }
                    },
                    PremiumHoliday = false,
                    PremiumType = "Level",
                    Selected = true
                },
                QuoteReferenceNumber = policyWithRisks.Policy.QuoteReference,
                RiskId = risk.Risk.Id.ToString(),
                SelectedPlanCodes = new[] { "DTH" },
            };

            var response = await Client.UpdateAsync<Dictionary<string, IEnumerable<string>>>(policyWithRisks.Policy.QuoteReference, risk.Risk.Id, planUpdate, throwOnFailure: false);

            Assert.That(response, Is.Not.Null);
            Assert.That(response.ContainsKey("tpddthCoverAmount"), Is.True);
            Assert.That(response.First(x => x.Key.Equals("tpddthCoverAmount")).Value.First(),
                Is.EqualTo("Cover amount cannot exceed $200,000"));
        }

        [Test]
        public async Task Update_LifePlanWithPremiumReliefOptionSelectedAndAgeNextBirthdayGreaterThan62NextBirthday_ReturnsCorrectModelStateError_Async()
        {
            var policyWithRisks = await CreatePolicyFromRatingFactorsAsync(new RatingFactorsRequest
            {
                AustralianResident = true,
                DateOfBirth = DateTime.Now.AddYears(-63).ToFriendlyString(),
                Gender = 'M',
                Income = 100000,
                OccupationCode = "229",
                IndustryCode = "27l"
            });

            var risk = policyWithRisks.Risks.First().Risk;
            var plansAndCovers = await Client.GetPlansAndCoversAsync(policyWithRisks.Policy.QuoteReference, risk.Id);

            var lifePlan = plansAndCovers.Plans.Single(p => p.Code == "DTH");

            var planUpdate = new PlanUpdateRequest
            {
                CurrentActivePlan = new PlanConfigurationRequest
                {
                    CoverAmount = 100000,
                    LinkedToCpi = false,
                    PlanCode = "DTH",
                    PlanId = lifePlan.PlanId,
                    SelectedCoverCodes = new[] { "DTHAC" },
                    SelectedOptionCodes = new OptionConfigurationRequest[] { new OptionConfigurationRequest("PR", "Premium Relief", true) },
                    SelectedRiders = new PlanConfigurationRequest[0],
                    PremiumHoliday = false,
                    PremiumType = "Stepped",
                    BenefitPeriod = 2,
                    WaitingPeriod = 4                    
                },
                QuoteReferenceNumber = policyWithRisks.Policy.QuoteReference,
                RiskId = risk.Id.ToString(),
                SelectedPlanCodes = new[] { "DTH", "IP" }
            };

            var response = await Client.UpdateAsync<PlansUpdateResponse>(policyWithRisks.Policy.QuoteReference, risk.Id, planUpdate, throwOnFailure: false);

            Assert.That(response, Is.Not.Null);
            AssertResponseHasWarning(response, "dthPR", "Premium Relief cannot be selected when the insured is older than 62 at their next birthday");
        }

        [Test]
        public async Task Update_PlanWithLifePlanNoCoversSelectedAndTpdAndTraumaRidersSelectedAndCoversSelected_ReturnsResponseWithCoverErrors_Async()
        {
            var policyWithRisks = await CreatePolicyFromRatingFactorsAsync(new RatingFactorsRequest
            {
                AustralianResident = true,
                DateOfBirth = DateTime.Now.AddYears(-30).ToFriendlyString(),
                Gender = 'M',
                Income = 100000,
                OccupationCode = "229",
                IndustryCode = "27l"
            });

            var risk = policyWithRisks.Risks.First().Risk;
            var plansAndCovers = await Client.GetPlansAndCoversAsync(policyWithRisks.Policy.QuoteReference, risk.Id);

            var lifePlan = plansAndCovers.Plans.Single(p => p.Code == "DTH");

            var planUpdate = new PlanUpdateRequest
            {
                CurrentActivePlan = new PlanConfigurationRequest
                {
                    CoverAmount = 500000,
                    LinkedToCpi = false,
                    PlanCode = "DTH",
                    PlanId = lifePlan.PlanId,
                    SelectedCoverCodes = new string[0],
                    SelectedOptionCodes = new [] { new OptionConfigurationRequest("PR", "Premium Relief", false) },
                    SelectedRiders = new PlanConfigurationRequest[2] 
                    {
                        new PlanConfigurationRequest()
                        {
                            CoverAmount = 10000,
                            LinkedToCpi = false,
                            PlanCode = "TPDDTH",
                            PlanId = lifePlan.Riders.Single(r => r.Code == "TPDDTH").PlanId,
                            SelectedCoverCodes = new string[3] {"TPDDTHAC", "TPDDTHIC", "TPDDTHASC"},
                            SelectedOptionCodes = new [] {new OptionConfigurationRequest("TPDDTHDBB", "", true) },
                            SelectedRiders = new PlanConfigurationRequest[0],
                            PremiumHoliday = false,
                            PremiumType = "Stepped",
                            Selected = true,
                            OccupationDefinition = "AnyOccupation"
                        },
                        new PlanConfigurationRequest()
                        {
                            CoverAmount = 10000,
                            LinkedToCpi = false,
                            PlanCode = "TRADTH",
                            PlanId = lifePlan.Riders.Single(r => r.Code == "TRADTH").PlanId,
                            SelectedCoverCodes = new string[3] { "TRADTHSIN", "TRADTHCC", "TRADTHSIC"},
                            SelectedOptionCodes = new [] {new OptionConfigurationRequest("TRADTHDBB", "", true) },
                            SelectedRiders = new PlanConfigurationRequest[0],
                            PremiumHoliday = false,
                            PremiumType = "Stepped",
                            Selected = true,
                            OccupationDefinition = "Unknown"
                       }
                    },
                    PremiumHoliday = false,
                    PremiumType = "Stepped",                    
                    Selected = true,
                    OccupationDefinition = "Unknown"
                },
                QuoteReferenceNumber = policyWithRisks.Policy.QuoteReference,
                RiskId = risk.Id.ToString(),
                SelectedPlanCodes = new[] { "DTH" },
            };

            var response = await Client.UpdateAsync<PlansUpdateResponse>(policyWithRisks.Policy.QuoteReference, risk.Id, planUpdate, throwOnFailure: false);

            AssertResponseHasWarning(response, "tpddthac", "Accident Cover cannot be selected without selecting Accident Cover for Life");
            AssertResponseHasWarning(response, "tpddthasc", "Sports Cover cannot be selected without selecting Adventure Sports and Accident Cover for Life");
            AssertResponseHasWarning(response, "tpddthic", "Illness Cover cannot be selected without selecting Illness Cover for Life");
            AssertResponseHasWarning(response, "tradthsin", "Serious Injury Cover cannot be selected without selecting Accident Cover for Life");
            AssertResponseHasWarning(response, "tradthcc", "Cancer Cover cannot be selected without selecting Illness Cover for Life");
            AssertResponseHasWarning(response, "tradthsic", "Serious Illness Cover cannot be selected without selecting Illness Cover for Life");
        }

        [Test]
        public async Task Update_PlanWithLifePlanNoCoversSelectedAndTpdAndTraumaRidersSelectedAndCoversSelectedAndOver60_ReturnsResponseWithCoverErrors_Async()
        {
            var policyWithRisks = await CreatePolicyFromRatingFactorsAsync(new RatingFactorsRequest
            {
                AustralianResident = true,
                DateOfBirth = DateTime.Now.AddYears(-61).ToFriendlyString(),
                Gender = 'M',
                Income = 100000,
                OccupationCode = "229",
                IndustryCode = "27l"
            });

            var risk = policyWithRisks.Risks.First().Risk;
            var plansAndCovers = await Client.GetPlansAndCoversAsync(policyWithRisks.Policy.QuoteReference, risk.Id);

            var lifePlan = plansAndCovers.Plans.Single(p => p.Code == "DTH");

            var planUpdate = new PlanUpdateRequest
            {
                CurrentActivePlan = new PlanConfigurationRequest
                {
                    CoverAmount = 500000,
                    LinkedToCpi = false,
                    PlanCode = "DTH",
                    PlanId = lifePlan.PlanId,
                    SelectedCoverCodes = new string[0],
                    SelectedOptionCodes = new[] { new OptionConfigurationRequest("PR", "Premium Relief", false) },
                    SelectedRiders = new PlanConfigurationRequest[2]
                    {
                        new PlanConfigurationRequest()
                        {
                            CoverAmount = 10000,
                            LinkedToCpi = false,
                            PlanCode = "TPDDTH",
                            PlanId = lifePlan.Riders.Single(r => r.Code == "TPDDTH").PlanId,
                            SelectedCoverCodes = new string[3] {"TPDDTHAC", "TPDDTHIC", "TPDDTHASC"},
                            SelectedOptionCodes = new [] {new OptionConfigurationRequest("TPDDTHDBB", "", true) },
                            SelectedRiders = new PlanConfigurationRequest[0],
                            PremiumHoliday = false,
                            PremiumType = "Stepped",
                            Selected = true,
                            OccupationDefinition = "AnyOccupation"
                        },
                        new PlanConfigurationRequest()
                        {
                            CoverAmount = 10000,
                            LinkedToCpi = false,
                            PlanCode = "TRADTH",
                            PlanId = lifePlan.Riders.Single(r => r.Code == "TRADTH").PlanId,
                            SelectedCoverCodes = new string[3] { "TRADTHSIN", "TRADTHCC", "TRADTHSIC"},
                            SelectedOptionCodes = new [] {new OptionConfigurationRequest("TRADTHDBB", "", true) },
                            SelectedRiders = new PlanConfigurationRequest[0],
                            PremiumHoliday = false,
                            PremiumType = "Stepped",
                            Selected = true,
                            OccupationDefinition = "Unknown"
                       }
                    },
                    PremiumHoliday = false,
                    PremiumType = "Stepped",
                    Selected = true,
                    OccupationDefinition = "Unknown"
                },
                QuoteReferenceNumber = policyWithRisks.Policy.QuoteReference,
                RiskId = risk.Id.ToString(),
                SelectedPlanCodes = new[] { "DTH" },
            };

            var response = await Client.UpdateAsync<Dictionary<string, IEnumerable<string>>>(policyWithRisks.Policy.QuoteReference, risk.Id, planUpdate, throwOnFailure: false);

            //TODO: error code/message will change when we put in proper testing for age eligibility on riders
            Assert.That(response.ContainsKey("tpddthCoverAmount"), Is.True);
            Assert.That(response.First(x => x.Key.Equals("tpddthCoverAmount")).Value.First(),
                Is.EqualTo("Max sum insured $0 does not reach minimum for this product."));

            Assert.That(response.ContainsKey("tradthCoverAmount"), Is.True);
            Assert.That(response.First(x => x.Key.Equals("tradthCoverAmount")).Value.First(),
                Is.EqualTo("Max sum insured $0 does not reach minimum for this product."));

        }


        [Test]
        public async Task Edit_UpdateSelectedPlans_SetsCorrectSelectedStatusOnPlans_Async()
        {

            var policyWithRisks = await CreatePolicyFromRatingFactorsAsync(new RatingFactorsRequest
            {
                AustralianResident = true,
                DateOfBirth = DateTime.Now.AddYears(-35).ToFriendlyString(),
                Gender = 'M',
                Income = 100000,
                OccupationCode = "229",
                IndustryCode = "27l"
            });

            var risk = policyWithRisks.Risks.First();
            var plansAndCovers = await Client.GetPlansAndCoversAsync(policyWithRisks.Policy.QuoteReference, risk.Risk.Id);

            var lifePlan = plansAndCovers.Plans.Single(p => p.Code == "DTH");

            var planUpdate = new PlanUpdateRequest
            {
                CurrentActivePlan = new PlanConfigurationRequest
                {
                    CoverAmount = 200000,
                    LinkedToCpi = false,
                    PlanCode = "DTH",
                    PlanId = lifePlan.PlanId,
                    SelectedCoverCodes = new[] { "DTHAC" },
                    SelectedOptionCodes = new OptionConfigurationRequest[0],
                    SelectedRiders = new PlanConfigurationRequest[0],
                    PremiumHoliday = false,
                    PremiumType = "Level",
                    Selected = true
                },
                QuoteReferenceNumber = policyWithRisks.Policy.QuoteReference,
                RiskId = risk.Risk.Id.ToString(),
                SelectedPlanCodes = new[] {"DTH", "TRS", "TPS"}
            };

            await Client.UpdateAsync<PlansUpdateResponse>(policyWithRisks.Policy.QuoteReference, risk.Risk.Id, planUpdate);

            plansAndCovers = await Client.GetPlansAndCoversAsync(policyWithRisks.Policy.QuoteReference, risk.Risk.Id);

            Assert.That(plansAndCovers, Is.Not.Null);
            Assert.That(plansAndCovers.Plans.Single(p => p.Code == "DTH").Selected, Is.True);
            Assert.That(plansAndCovers.Plans.Single(p => p.Code == "TRS").Selected, Is.True);
            Assert.That(plansAndCovers.Plans.Single(p => p.Code == "TPS").Selected, Is.True);
            Assert.That(plansAndCovers.Plans.Single(p => p.Code == "IP").Selected, Is.False);
        }

        [Test]
        public async Task Edit_UpdatePlanCoverAmount_OnlyUpdatesSelectedPlanCoverAmount_Async()
        {
            var policyWithRisks = await CreatePolicyFromRatingFactorsAsync(new RatingFactorsRequest
            {
                AustralianResident = true,
                DateOfBirth = DateTime.Now.AddYears(-35).ToFriendlyString(),
                Gender = 'M',
                Income = 100000,
                OccupationCode = "229",
                IndustryCode = "27l"
            });

            var risk = policyWithRisks.Risks.First();
            var plansAndCovers = await Client.GetPlansAndCoversAsync(policyWithRisks.Policy.QuoteReference, risk.Risk.Id);

            var lifePlan = plansAndCovers.Plans.Single(p => p.Code == "DTH");

            var planUpdate = new PlanUpdateRequest
            {
                CurrentActivePlan = new PlanConfigurationRequest
                {
                    CoverAmount = 200000,
                    LinkedToCpi = false,
                    PlanCode = "DTH",
                    PlanId = lifePlan.PlanId,
                    SelectedCoverCodes = new[] { "DTHAC" },
                    SelectedOptionCodes = new OptionConfigurationRequest[0],
                    SelectedRiders = new PlanConfigurationRequest[0],
                    PremiumHoliday = false,
                    PremiumType = "Level",
                    Selected = true
                },
                QuoteReferenceNumber = policyWithRisks.Policy.QuoteReference,
                RiskId = risk.Risk.Id.ToString(),
                SelectedPlanCodes = new[] { "DTH" },
            };

            await Client.UpdateAsync<PlansUpdateResponse>(policyWithRisks.Policy.QuoteReference, risk.Risk.Id, planUpdate);

            plansAndCovers = await Client.GetPlansAndCoversAsync(policyWithRisks.Policy.QuoteReference, risk.Risk.Id);

            Assert.That(plansAndCovers, Is.Not.Null);
            Assert.That(plansAndCovers.Plans.Single(p => p.Code == "DTH").CoverAmount, Is.EqualTo(200000));
            Assert.That(plansAndCovers.Plans.Single(p => p.Code == "TRS").CoverAmount, Is.EqualTo(100000));
            Assert.That(plansAndCovers.Plans.Single(p => p.Code == "TPS").CoverAmount, Is.EqualTo(375000));
            Assert.That(plansAndCovers.Plans.Single(p => p.Code == "IP").CoverAmount, Is.EqualTo(6250));
        }

        [Test]
        public async Task Update_PlanWithLifeTpdAndTraumaPlansSelectedAndTpdAndTraumaRidersSelected_ReturnsResponseWithNoStandAloneErrors_Async()
        {
            var policyWithRisks = await CreatePolicyFromRatingFactorsAsync(new RatingFactorsRequest
            {
                AustralianResident = true,
                DateOfBirth = DateTime.Now.AddYears(-35).ToFriendlyString(),
                Gender = 'M',
                Income = 100000,
                OccupationCode = "229",
                IndustryCode = "27l"
            });

            var risk = policyWithRisks.Risks.First();
            var plansAndCovers = await Client.GetPlansAndCoversAsync(policyWithRisks.Policy.QuoteReference, risk.Risk.Id);

            var lifePlan = plansAndCovers.Plans.Single(p => p.Code == "DTH");

            var planUpdate = new PlanUpdateRequest
            {
                CurrentActivePlan = new PlanConfigurationRequest
                {
                    CoverAmount = 100000,
                    LinkedToCpi = false,
                    PlanCode = "DTH",
                    PlanId = lifePlan.PlanId,
                    SelectedCoverCodes = new[] { "DTHAC" },
                    SelectedOptionCodes = new OptionConfigurationRequest[0],
                    SelectedRiders = new PlanConfigurationRequest[2] {
                        new PlanConfigurationRequest()
                        {
                            CoverAmount = 0,
                            LinkedToCpi = false,
                            PlanCode = "TPDDTH",
                            PlanId = lifePlan.Riders.Single(r => r.Code == "TPDDTH").PlanId,
                            SelectedCoverCodes = new string[0],
                            SelectedOptionCodes = new [] {new OptionConfigurationRequest("TPDDTHDBB", "", true) },
                            SelectedRiders = new PlanConfigurationRequest[0],
                            PremiumHoliday = false,
                            PremiumType = "Level",
                            Selected = true,
                            OccupationDefinition = "AnyOccupation"
                        },
                        new PlanConfigurationRequest()
                        {
                            CoverAmount = 0,
                            LinkedToCpi = false,
                            PlanCode = "TRADTH",
                            PlanId = lifePlan.Riders.Single(r => r.Code == "TRADTH").PlanId,
                            SelectedCoverCodes = new string[0],
                            SelectedOptionCodes = new [] {new OptionConfigurationRequest("TRADTHDBB", "", true) },
                            SelectedRiders = new PlanConfigurationRequest[0],
                            PremiumHoliday = false,
                            PremiumType = "Level",
                            Selected = true,
                            OccupationDefinition = "AnyOccupation"
                        }
                    },
                    PremiumHoliday = false,
                    PremiumType = "Level",
                    Selected = true
                },
                QuoteReferenceNumber = policyWithRisks.Policy.QuoteReference,
                RiskId = risk.Risk.Id.ToString(),
                SelectedPlanCodes = new[] { "DTH", "TRS", "TPS" }
            };
            
            var response = await Client.UpdateAsync<PlansUpdateResponse>(policyWithRisks.Policy.QuoteReference, risk.Risk.Id, planUpdate, throwOnFailure: false);

            AssertResponseHasWarning(response, "tpddth", "TPD must be attached to Life or taken standalone");
            AssertResponseHasWarning(response, "tradth", "Recovery Insurance must be attached to Life or taken standalone");
        }

        [Test]
        public async Task Update_OnIpPlanWithPremiumReliefOnLifeSelectedAndDeselectionOfIp_ReturnsCorrectModelStateErrors_Async()
        {
            var policyWithRisks = await CreatePolicyFromRatingFactorsAsync(new RatingFactorsRequest
            {
                AustralianResident = true,
                DateOfBirth = DateTime.Now.AddYears(-35).ToFriendlyString(),
                Gender = 'M',
                Income = 100000,
                OccupationCode = "229",
                IndustryCode = "27l"
            });

            var risk = policyWithRisks.Risks.First();
            var plansAndCovers = await Client.GetPlansAndCoversAsync(policyWithRisks.Policy.QuoteReference, risk.Risk.Id);

            var lifePlan = plansAndCovers.Plans.Single(p => p.Code == "DTH");
            var ipPlan = plansAndCovers.Plans.Single(p => p.Code == "IP");

            var planUpdate = new PlanUpdateRequest
            {
                CurrentActivePlan = new PlanConfigurationRequest
                {
                    CoverAmount = 200000,
                    LinkedToCpi = false,
                    PlanCode = "DTH",
                    PlanId = lifePlan.PlanId,
                    SelectedCoverCodes = new[] { "DTHAC" },
                    SelectedOptionCodes = new [] { new OptionConfigurationRequest("PR", "Premium Relief", true) },
                    SelectedRiders = new PlanConfigurationRequest[0],
                    PremiumHoliday = false,
                    PremiumType = "Stepped",
                    Selected = true,
                    OccupationDefinition = "AnyOccupation"
                },
                QuoteReferenceNumber = policyWithRisks.Policy.QuoteReference,
                RiskId = risk.Risk.Id.ToString(),
                SelectedPlanCodes = new[] { "DTH", "IP" },
            };

            await Client.UpdateAsync<PlansUpdateResponse>(policyWithRisks.Policy.QuoteReference, risk.Risk.Id, planUpdate);

            planUpdate = new PlanUpdateRequest
            {
                CurrentActivePlan = new PlanConfigurationRequest
                {
                    CoverAmount = 5000,
                    LinkedToCpi = false,
                    PlanCode = "IP",
                    PlanId = ipPlan.PlanId,
                    SelectedCoverCodes = new[] { "IPSAC" },
                    SelectedOptionCodes = new OptionConfigurationRequest[0],
                    SelectedRiders = new PlanConfigurationRequest[0],
                    PremiumHoliday = false,
                    PremiumType = "Level",
                    WaitingPeriod = 4,
                    BenefitPeriod = 1
                },
                QuoteReferenceNumber = policyWithRisks.Policy.QuoteReference,
                RiskId = risk.Risk.Id.ToString(),
                SelectedPlanCodes = new[] { "DTH"}
            };

            var ipResponse = await Client.UpdateAsync<PlansUpdateResponse>(policyWithRisks.Policy.QuoteReference, risk.Risk.Id, planUpdate);

            Assert.That(ipResponse.CurrentActivePlan.Code, Is.EqualTo("IP"));
            Assert.That(ipResponse.CurrentActivePlan.Status, Is.EqualTo(PlanStatus.Completed));

            //US22049 - Premium Relief doesn't need IP plan to be selected, so commenting out the assert below
            //Assert.That(ipResponse.OverallStatus, Is.EqualTo(PlanStatus.Warning));

            var nonActiveLifePlan = ipResponse.Plans.First(x => x.Code == "DTH");
            Assert.That(nonActiveLifePlan.Status, Is.EqualTo(PlanStatus.Completed));
        }

        [Test]
        public async Task Update_PlanWithLifePlanAndTpdRider_TpdRiderTpdDefinitionSetToOwnOccupation_ReturnsWithCorrectVariableSetForTpdRiderTpdDefinition_Async()
        {
            var policyWithRisks = await CreatePolicyFromRatingFactorsAsync(new RatingFactorsRequest
            {
                AustralianResident = true,
                DateOfBirth = DateTime.Now.AddYears(-30).ToFriendlyString(),
                Gender = 'M',
                Income = 100000,
                OccupationCode = "229",
                IndustryCode = "27l"
            });

            var risk = policyWithRisks.Risks.First();
            var plansAndCovers = await Client.GetPlansAndCoversAsync(policyWithRisks.Policy.QuoteReference, risk.Risk.Id);

            var lifePlan = plansAndCovers.Plans.Single(p => p.Code == "DTH");

            Assert.That(lifePlan, Is.Not.Null);
            Assert.That(lifePlan.OccupationDefinition, Is.Not.Null);
            Assert.That(lifePlan.OccupationDefinition, Is.EqualTo(OccupationDefinition.Unknown));
            Assert.That(lifePlan.Riders.First().Code, Is.EqualTo("TPDDTH"));
            Assert.That(lifePlan.Riders.First().OccupationDefinition, Is.EqualTo(OccupationDefinition.AnyOccupation));
            Assert.That(lifePlan.Riders.First().Variables.Single(p => p.Code == "occupationDefinition").Name, Is.EqualTo("TPD Occupation Type"));

            var planUpdate = new PlanUpdateRequest
            {
                CurrentActivePlan = new PlanConfigurationRequest
                {
                    CoverAmount = 200000,
                    LinkedToCpi = false,
                    PlanCode = "DTH",
                    PlanId = lifePlan.PlanId,
                    SelectedCoverCodes = new[] { "DTHAC" },
                    SelectedOptionCodes = new OptionConfigurationRequest[0],
                    SelectedRiders = new List<PlanConfigurationRequest>
                    {
                        new PlanConfigurationRequest()
                        {
                            CoverAmount = 100000,
                            LinkedToCpi = false,
                            PlanCode = "TPDDTH",
                            PlanId = lifePlan.PlanId,
                            SelectedCoverCodes = new string[0] {},
                            SelectedOptionCodes = new OptionConfigurationRequest[0],
                            SelectedRiders = new PlanConfigurationRequest[0],
                            PremiumHoliday = false,
                            PremiumType = "Level",
                            Selected = true,
                            OccupationDefinition = OccupationDefinition.OwnOccupation.ToString()
                        }
                    },
                    PremiumHoliday = false,
                    PremiumType = "Level",
                    Selected = true
                },
                QuoteReferenceNumber = policyWithRisks.Policy.QuoteReference,
                RiskId = risk.Risk.Id.ToString(),
                SelectedPlanCodes = new[] { "DTH" },
            };

            var response = await Client.UpdateAsync<PlansUpdateResponse>(policyWithRisks.Policy.QuoteReference, risk.Risk.Id, planUpdate);

            Assert.That(response, Is.Not.Null);
            Assert.That(response.CurrentActivePlan.Riders.First().Code, Is.EqualTo("TPDDTH"));
            Assert.That(response.CurrentActivePlan.Riders.First().OccupationDefinition, Is.EqualTo(OccupationDefinition.OwnOccupation.ToString()));
        }

        [Test]
        public async Task Update_TpdPlanWithTpdDefinitionSetToOwnOccupation_ReturnsWithCorrectVariableSetForTpdDefinition_Async()
        {
            var policyWithRisks = await CreatePolicyFromRatingFactorsAsync(new RatingFactorsRequest
            {
                AustralianResident = true,
                DateOfBirth = DateTime.Now.AddYears(-30).ToFriendlyString(),
                Gender = 'M',
                Income = 100000,
                OccupationCode = "229",
                IndustryCode = "27l"
            });

            var risk = policyWithRisks.Risks.First();
            var plansAndCovers = await Client.GetPlansAndCoversAsync(policyWithRisks.Policy.QuoteReference, risk.Risk.Id);

            var tpdPlan = plansAndCovers.Plans.Single(p => p.Code == "TPS");

            Assert.That(tpdPlan, Is.Not.Null);
            Assert.That(tpdPlan.OccupationDefinition, Is.Not.Null);
            Assert.That(tpdPlan.OccupationDefinition, Is.EqualTo(OccupationDefinition.AnyOccupation));
            Assert.That(tpdPlan.Variables.Single(p => p.Code == "occupationDefinition").Name, Is.EqualTo("TPD Occupation Type"));

            var planUpdate = new PlanUpdateRequest
            {
                CurrentActivePlan = new PlanConfigurationRequest
                {
                    CoverAmount = 200000,
                    LinkedToCpi = false,
                    PlanCode = "TPS",
                    PlanId = tpdPlan.PlanId,
                    SelectedCoverCodes = new [] { "TPSAC" },
                    SelectedOptionCodes = new OptionConfigurationRequest[0],
                    SelectedRiders = new List<PlanConfigurationRequest>(),
                    PremiumHoliday = false,
                    PremiumType = "Level",
                    Selected = true,
                    OccupationDefinition = OccupationDefinition.OwnOccupation.ToString()
                },
                QuoteReferenceNumber = policyWithRisks.Policy.QuoteReference,
                RiskId = risk.Risk.Id.ToString(),
                SelectedPlanCodes = new[] { "TPS" },
            };

            var response = await Client.UpdateAsync<PlansUpdateResponse>(policyWithRisks.Policy.QuoteReference, risk.Risk.Id, planUpdate);

            Assert.That(response, Is.Not.Null);
            Assert.That(response.CurrentActivePlan.Code, Is.EqualTo("TPS"));
            Assert.That(response.CurrentActivePlan.OccupationDefinition, Is.EqualTo(OccupationDefinition.OwnOccupation.ToString()));
        }

        [Test]
        public async Task Update_ActivePlanWithNoCoverAmount_ErrorReturned_Async()
        {
            var policyWithRisks = await CreatePolicyFromRatingFactorsAsync(new RatingFactorsRequest
            {
                AustralianResident = true,
                DateOfBirth = DateTime.Now.AddYears(-35).ToFriendlyString(),
                Gender = 'M',
                Income = 100000,
                OccupationCode = "229",
                IndustryCode = "27l"
            });

            var risk = policyWithRisks.Risks.First();
            var plansAndCovers = await Client.GetPlansAndCoversAsync(policyWithRisks.Policy.QuoteReference, risk.Risk.Id);

            var lifePlan = plansAndCovers.Plans.Single(p => p.Code == "DTH");

            var planUpdate = new PlanUpdateRequest
            {
                CurrentActivePlan = new PlanConfigurationRequest
                {
                    CoverAmount = 0,
                    LinkedToCpi = false,
                    PlanCode = "DTH",
                    PlanId = lifePlan.PlanId,
                    SelectedCoverCodes = new[] { "DTHAC" },
                    SelectedOptionCodes = new OptionConfigurationRequest[0],
                    SelectedRiders = new PlanConfigurationRequest[0],
                    PremiumHoliday = false,
                    PremiumType = "Level",
                    Selected = true
                },
                QuoteReferenceNumber = policyWithRisks.Policy.QuoteReference,
                RiskId = risk.Risk.Id.ToString(),
                SelectedPlanCodes = new[] { "DTH" }
            };

            var response = await Client.UpdateAsync<Dictionary<string, IEnumerable<string>>>(policyWithRisks.Policy.QuoteReference, risk.Risk.Id, planUpdate, throwOnFailure:false);
            Assert.That(response.ContainsKey("dthCoverAmount"), Is.True);
        }

        private void AssertResponseHasWarning(PlansUpdateResponse response, string code, string message)
        {
            Assert.That(response.CurrentActivePlan.Warnings.ContainsKey(code), Is.True);
            Assert.That(response.CurrentActivePlan.Warnings.First(x => x.Key.Equals(code)).Value.First(),
                Is.EqualTo(message));
        }
    }
}