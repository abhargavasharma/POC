using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TAL.Performance.Infrastructure.Core;
using TAL.QuoteAndApply.Customer.Web.Extensions;
using TAL.QuoteAndApply.Customer.Web.IntegrationTests.Clients;
using TAL.QuoteAndApply.Customer.Web.Models.Api;
using TAL.QuoteAndApply.Customer.Web.Models.View;
using TAL.QuoteAndApply.DataLayer.Repository;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.DataModel.Product;
using TAL.QuoteAndApply.Tests.Shared.Helpers;
using Task = System.Threading.Tasks.Task;

namespace TAL.QuoteAndApply.Customer.Web.IntegrationTests.Tests
{
    [TestFixture]
    public class CoverSelectionApiTests : BaseTestClass<CoverSelectionApiClient>
    {
        public CoverSelectionApiTests() : base(SimplePerformanceTool.CreateConsoleTimestamped())
        {
        }

        [Test]
        public async Task GetPlanForRisk_WhenCalled_ReturnsPlans_Async()
        {
            //Arrange
            var risks = await BasicInfoDefaultLoginAsync();
            
            //Act
            var response = await Client.GetPlanForRiskAsync<GetPlanResponse>(risks.First().Id);

            //Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Plans, Is.Not.Null);
            Assert.That(response.Plans.Count, Is.EqualTo(4));
        }

        [Test]
        public async Task UpdatePlan_WhenCalledWithAnInValidSelectionTpdDefinition_ReturnsModelError_Async()
        {
            //Arrange
            var risks = await BasicInfoDefaultLoginAsync(UnderwritingHelper.OccupationCode_ManualDuties);

            var risk = risks.First();

            var plans = await Client.GetPlanForRiskAsync<GetPlanResponse>(risk.Id);

            var tpdPlan = plans.Plans.FirstOrDefault(p => p.PlanCode == "TPS");
            var request = new UpdatePlanRequest()
            {
                PlanCode = tpdPlan.PlanCode,
                PlanId = tpdPlan.PlanId,
                SelectedCoverAmount = 250000,
                IsSelected = true,
                PremiumType = "Stepped",
                SelectedCovers = new List<string> { "TPSAC" },
                Riders = new List<PlanRiderRequest>(),
                Options = new List<OptionConfigurationRequest>
                {
                    new OptionConfigurationRequest
                    {
                        Code = "PR",
                        IsSelected = false
                    }
                },
                Variables = new List<UpdatePlanVariableRequest>
                {
                    new UpdatePlanVariableRequest { Code = "linkedToCpi", SelectedValue = true },
                    new UpdatePlanVariableRequest {Code = "occupationDefinition", SelectedValue = "OwnOccupation"}
                },
                SelectedPlans = new List<string> { "TPS" }
            };

            //Act
            var response = await Client.UpdatePlanAsync<Dictionary<string, IEnumerable<string>>>(risk.Id, request, false);

            //Assert
            Assert.That(response.ContainsKey("tpsoccupationDefinition"), Is.True);
        }

        [Test]
        public async Task UpdatePlan_WhenCalledWithValidSelection_ReturnsPlansAndAvailability_Async()
        {
            //Arrange
            var risks = await BasicInfoDefaultLoginAsync();

            var risk = risks.First();

            var plans = await Client.GetPlanForRiskAsync<GetPlanResponse>(risk.Id);

            var lifePlan = plans.Plans.FirstOrDefault(p => p.PlanCode == "DTH");
            var request = new UpdatePlanRequest()
            {
                PlanCode = lifePlan.PlanCode,
                PlanId = lifePlan.PlanId,
                SelectedCoverAmount = 250000,
                IsSelected = true,
                PremiumType = "Stepped",
                SelectedCovers = new List<string> { "DTHIC"},
                Riders = new List<PlanRiderRequest>(),
                Options = new List<OptionConfigurationRequest>
                {
                    new OptionConfigurationRequest
                    {
                        Code = "PR",
                        IsSelected = false
                    }
                },
                Variables = new List<UpdatePlanVariableRequest>
                {
                    new UpdatePlanVariableRequest { Code = "linkedToCpi", SelectedValue = true }
                },
                SelectedPlans = new List<string> { "DTH" }
            };

            //Act
            var response = await Client.UpdatePlanAsync<GetPlanResponse>(risk.Id, request);

            //Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Plans, Is.Not.Null);
            Assert.That(response.Plans.Count, Is.EqualTo(4));
            var responsePlan = response.Plans.FirstOrDefault(p => p.PlanCode == "DTH");

            Assert.That(responsePlan, Is.Not.Null);
            Assert.That(responsePlan.IsSelected, Is.True);
            Assert.That(responsePlan.Variables.Single(v => v.Code == "linkedToCpi").SelectedValue, Is.True);
            Assert.That(responsePlan.SelectedCoverAmount, Is.EqualTo(request.SelectedCoverAmount));

            var sportsCover = responsePlan.Covers.FirstOrDefault(c => c.Code == "DTHASC");
            Assert.That(sportsCover, Is.Not.Null);
            Assert.That(sportsCover.Availability.IsAvailable, Is.False);

            var accidentCover = responsePlan.Covers.FirstOrDefault(c => c.Code == "DTHAC");
            Assert.That(accidentCover, Is.Not.Null);
            Assert.That(accidentCover.Availability.IsAvailable, Is.True);

        }

        [Test, ExpectedException(typeof(Exception))]
        public async Task AttachRider_WhenCalledWithValidSelection_ReturnsPlansAndAvailability_Async()
        {
            //Arrange
            var risks = await BasicInfoDefaultLoginAsync();

            var risk = risks.First();

            var plans = await Client.GetPlanForRiskAsync<GetPlanResponse>(risk.Id);

            var lifePlan = plans.Plans.FirstOrDefault(p => p.PlanCode == "DTH");
            var tpdPlan = plans.Plans.FirstOrDefault(p => p.PlanCode == "TPS");

            var lifeRequest = new UpdatePlanRequest()
            {
                PlanCode = lifePlan.PlanCode,
                PlanId = lifePlan.PlanId,
                SelectedCoverAmount = 250000,
                IsSelected = true,
                PremiumType = "Stepped",
                SelectedCovers = new List<string> { "DTHAC", "DTHASC" },
                Riders = new List<PlanRiderRequest>(),
                Options = new List<OptionConfigurationRequest>
                {
                    new OptionConfigurationRequest
                    {
                        Code = "PR",
                        IsSelected = false
                    }
                },
                Variables = new List<UpdatePlanVariableRequest>(),
                SelectedPlans = new List<string> { "DTH" }
            };

            var tpdRequest = new UpdatePlanRequest()
            {
                PlanCode = tpdPlan.PlanCode,
                PlanId = tpdPlan.PlanId,
                SelectedCoverAmount = 150000,
                IsSelected = true,
                PremiumType = "Stepped",
                SelectedCovers = new List<string> { "TPSAC", "TPSASC" },
                Riders = new List<PlanRiderRequest>(),
                Options = new List<OptionConfigurationRequest>(),
                SelectedPlans = new List<string> { "DTH", "TPS" }
            };

            var attachRequest = new AttachRiderRequest()
            {
                PlanToHostRiderCode = "DTH",
                PlanToBecomeRiderCode = "TPS"
            };

            //Act
            await Client.UpdatePlanAsync<GetPlanResponse>(risk.Id, lifeRequest);
            await Client.UpdatePlanAsync<GetPlanResponse>(risk.Id, tpdRequest);

            var response = await Client.AttachRiderAsync<GetPlanResponse>(risk.Id, attachRequest);

            //Assert
            Assert.That(response, Is.Not.Null);
            var responseLifePlan = response.Plans.FirstOrDefault(p => p.PlanCode == "DTH");
            var responseTpdPlan = response.Plans.FirstOrDefault(p => p.PlanCode == "TPS");

            Assert.That(responseLifePlan, Is.Not.Null);
            Assert.That(responseLifePlan.IsSelected, Is.True);

            Assert.That(responseTpdPlan, Is.Not.Null);
            Assert.That(responseTpdPlan.IsSelected, Is.False);

            var tpdRider = responseLifePlan.Riders.FirstOrDefault(r => r.PlanCode == "TPDDTH");
            Assert.That(tpdRider, Is.Not.Null);
            Assert.That(tpdRider.IsSelected, Is.True);
            Assert.That(tpdRider.Covers.Any(c => c.Code == "TPDDTHAC"), Is.True);
            Assert.That(tpdRider.Covers.Any(c => c.Code == "TPDDTHASC"), Is.True);

        }

        [Test, ExpectedException(typeof(Exception))]
        public async Task DetachRider_WhenCalledWithValidSelection_ReturnsPlansAndAvailability_Async()
        {
            //Arrange
            var risks = await BasicInfoDefaultLoginAsync();

            var risk = risks.First();

            var plans = await Client.GetPlanForRiskAsync<GetPlanResponse>(risk.Id);

            var lifePlan = plans.Plans.FirstOrDefault(p => p.PlanCode == "DTH");
            var tpdRiderPlan = lifePlan.Riders.FirstOrDefault(p => p.PlanCode == "TPDDTH");
            var ciRiderPlan = lifePlan.Riders.FirstOrDefault(p => p.PlanCode == "TRADTH");
            
            var lifeRequest = new UpdatePlanRequest()
            {
                PlanCode = lifePlan.PlanCode,
                PlanId = lifePlan.PlanId,
                SelectedCoverAmount = 250000,
                IsSelected = true,
                PremiumType = "Stepped",
                SelectedCovers = new List<string> { "DTHAC", "DTHASC" },
                Riders = new List<PlanRiderRequest>
                {
                    new PlanRiderRequest()
                    {
                        IsSelected = true,
                        Options = new List<OptionConfigurationRequest>() { new OptionConfigurationRequest {Code= "TPDDTHDBB" } },
                        PlanCode = tpdRiderPlan.PlanCode,
                        PlanId = tpdRiderPlan.PlanId,
                        SelectedCovers = new List<string> { "TPDDTHASC", "TPDDTHAC" }
                    },
                    new PlanRiderRequest()
                    {
                        IsSelected = false,
                        Options = new List<OptionConfigurationRequest>() { new OptionConfigurationRequest {Code= "TRADTHDBB" } },
                        PlanCode = ciRiderPlan.PlanCode,
                        PlanId = ciRiderPlan.PlanId,
                        SelectedCovers = new List<string> {  }
                    }
                },
                Options = new List<OptionConfigurationRequest>
                {
                    new OptionConfigurationRequest
                    {
                        Code = "PR",
                        IsSelected = false
                    }
                },
                Variables = new List<UpdatePlanVariableRequest>(),
                SelectedPlans = new List<string> { "DTH" }
            };

            var detachRequest = new DetachRiderRequest()
            {
                RiderCode = "TPDDTH"
            };

            //Act
            await Client.UpdatePlanAsync<GetPlanResponse>(risk.Id, lifeRequest);
           

            var response = await Client.DetachRiderAsync<GetPlanResponse>(risk.Id, detachRequest);
            
            //Assert
            Assert.That(response, Is.Not.Null);
            var responseLifePlan = response.Plans.FirstOrDefault(p => p.PlanCode == "DTH");
            var responseTpdPlan = response.Plans.FirstOrDefault(p => p.PlanCode == "TPS");

            Assert.That(responseLifePlan, Is.Not.Null);
            Assert.That(responseLifePlan.IsSelected, Is.True);

            Assert.That(responseTpdPlan, Is.Not.Null);
            Assert.That(responseTpdPlan.IsSelected, Is.False);

            var tpdRider = responseLifePlan.Riders.FirstOrDefault(r => r.PlanCode == "TPDDTH");
            Assert.That(tpdRider, Is.Not.Null);
            Assert.That(tpdRider.IsSelected, Is.False);

        }


        [Test]
        public async Task UpdatePlan_WhenUpdatingIPPlanVariables_ReturnsPlansAndAvailability_Async()
        {
            //Arrange
            var risks = await BasicInfoLoginAsAsync(new BasicInfoViewModel
            {
                DateOfBirth = DateTime.Now.AddYears(-32).ToFriendlyString(),
                Gender = 'M',
                IsSmoker = false,
                AnnualIncome = 100000,
                IndustryCode = UnderwritingHelper.IndustryCode_AdvertisingAndMarketing,
                OccupationCode = UnderwritingHelper.OccupationCode_AccountExecutive,
                Postcode = "1000"
            });          

            var risk = risks.First();

            var plans = await Client.GetPlanForRiskAsync<GetPlanResponse>(risk.Id);

            var lifePlan = plans.Plans.FirstOrDefault(p => p.PlanCode == "IP");
            var request = new UpdatePlanRequest()
            {
                PlanCode = lifePlan.PlanCode,
                PlanId = lifePlan.PlanId,
                SelectedCoverAmount = 3000,
                IsSelected = true,
                PremiumType = "Stepped",
                SelectedCovers = new List<string> { "IPSAC" },
                Riders = new List<PlanRiderRequest>(),
                Options = new List<OptionConfigurationRequest>
                {
                    new OptionConfigurationRequest
                    {
                        Code = "PR",
                        IsSelected = false
                    }
                },
                Variables = new List<UpdatePlanVariableRequest>
                {
                    new UpdatePlanVariableRequest { Code = "linkedToCpi", SelectedValue = false },
                    new UpdatePlanVariableRequest { Code = "waitingPeriod", SelectedValue = (long)4 },
                    new UpdatePlanVariableRequest { Code = "benefitPeriod", SelectedValue = (long)1 }
                },
                SelectedPlans = new List<string> { "IP" }
            };

            //Act
            var response = await Client.UpdatePlanAsync<GetPlanResponse>(risk.Id, request);

            //Assert
            Assert.That(response, Is.Not.Null);

            var ipPlan = response.Plans.Single(p => p.PlanCode.Equals("IP"));
            Assert.That(ipPlan, Is.Not.Null);
            Assert.That(ipPlan.Variables, Is.Not.Null);
            Assert.That(ipPlan.Variables.Single(v => v.Code == "linkedToCpi").SelectedValue, Is.False);
            Assert.That(ipPlan.Variables.Single(v => v.Code == "waitingPeriod").SelectedValue, Is.EqualTo(4));
            Assert.That(ipPlan.Variables.Single(v => v.Code == "benefitPeriod").SelectedValue, Is.EqualTo(1));
        }


        [Test]
        public async Task UpdatePlan_WhenUpdatingVariableToInvalidSelection_ReturnsInvalidModelState_Async()
        {
            //Arrange
            var risks = await BasicInfoDefaultLoginAsync();

            var risk = risks.First();

            var plans = await Client.GetPlanForRiskAsync<GetPlanResponse>(risk.Id);

            var ipPlan = plans.Plans.FirstOrDefault(p => p.PlanCode == "IP");
            var request = new UpdatePlanRequest()
            {
                PlanCode = ipPlan.PlanCode,
                PlanId = ipPlan.PlanId,
                SelectedCoverAmount = 1500,
                IsSelected = true,
                PremiumType = "Stepped",
                SelectedCovers = new List<string> { "IPSAC" },
                Riders = new List<PlanRiderRequest>(),
                Options = new List<OptionConfigurationRequest>
                {
                    new OptionConfigurationRequest
                    {
                        Code = "PR",
                        IsSelected = false
                    }
                },
                Variables = new List<UpdatePlanVariableRequest>
                {
                    new UpdatePlanVariableRequest { Code = "linkedToCpi", SelectedValue = false },
                    new UpdatePlanVariableRequest { Code = "waitingPeriod", SelectedValue = (long)99 },
                    new UpdatePlanVariableRequest { Code = "benefitPeriod", SelectedValue = (long)99 }
                },
                SelectedPlans = new List<string> { "IP" }
            };

            //Act
            var response = await Client.UpdatePlanAsync<Dictionary<string, IEnumerable<string>>>(risk.Id, request, false);

            //TODO: Assert there are Variable Option errors in response. PERSON DOING MERGE REQUEST, DO NOT ACCEPT IF THIS LINE IS HERE!!!!

            //Assert
            Assert.That(response, Is.Not.Null);
        }

        [Test, Description("This scenario covers a defect where TPD, CI and IP are not available, but when all covers are turned off they report as available")]
        public async Task UpdatePlan_WhenAllPlansAreTurnedOffAndAgeOver60_ReturnsPlansAndAvailability_Async()
        {
            //Arrange
            var risks = await BasicInfoLoginAsAsync(new BasicInfoViewModel
            {
                DateOfBirth = DateTime.Now.AddYears(-60).ToFriendlyString(),
                Gender = 'M',
                IsSmoker = false,
                AnnualIncome = 100000,
                IndustryCode = UnderwritingHelper.IndustryCode_AdvertisingAndMarketing,
                OccupationCode = UnderwritingHelper.OccupationCode_AccountExecutive,
                Postcode = "1000"
            });

            var risk = risks.First();

            var plans = await Client.GetPlanForRiskAsync<GetPlanResponse>(risk.Id);

            //Assert availability is all good
            Assert.That(plans.Plans.Single(p => p.PlanCode == "DTH").Availability.IsAvailable, Is.True);
            Assert.That(plans.Plans.Single(p => p.PlanCode == "TPS").Availability.IsAvailable, Is.False);
            Assert.That(plans.Plans.Single(p => p.PlanCode == "TRS").Availability.IsAvailable, Is.False);
            Assert.That(plans.Plans.Single(p => p.PlanCode == "IP").Availability.IsAvailable, Is.False);

            //Turn off LIFE
            var lifePlan = plans.Plans.Single(p => p.PlanCode == "DTH");
            var request = new UpdatePlanRequest()
            {
                PlanCode = lifePlan.PlanCode,
                PlanId = lifePlan.PlanId,
                SelectedCoverAmount = 100000,
                IsSelected = false,
                PremiumType = "Stepped",
                SelectedCovers = lifePlan.Covers.Select(c => c.Code).ToList(),
                Riders = new List<PlanRiderRequest>(),
                Options = new List<OptionConfigurationRequest>
                {
                    new OptionConfigurationRequest
                    {
                        Code = "PR",
                        IsSelected = false
                    }
                },
                Variables = new List<UpdatePlanVariableRequest>
                {
                    new UpdatePlanVariableRequest { Code = "linkedToCpi", SelectedValue = false },
                    new UpdatePlanVariableRequest { Code = "waitingPeriod", SelectedValue = (long)4 },
                    new UpdatePlanVariableRequest { Code = "benefitPeriod", SelectedValue = (long)1 }
                },
                SelectedPlans = new List<string>()
            };

            //Act
            var response = await Client.UpdatePlanAsync<GetPlanResponse>(risk.Id, request);

            //Assert availability still all correct
            Assert.That(response.Plans.Single(p => p.PlanCode == "DTH").Availability.IsAvailable, Is.True);
            Assert.That(response.Plans.Single(p => p.PlanCode == "TPS").Availability.IsAvailable, Is.False);
            Assert.That(response.Plans.Single(p => p.PlanCode == "TRS").Availability.IsAvailable, Is.False);
            Assert.That(response.Plans.Single(p => p.PlanCode == "IP").Availability.IsAvailable, Is.False);

            var planList = plans.Plans.ToList();

            Assert.That(RiskMarketingStatusIsEqualTo(MarketingStatus.Off, risks.First().Id), Is.True, "MarketingStatus incorrect on risk");
            Assert.That(PlanMarketingStatusIsEqualTo(MarketingStatus.Off, planList[0].PlanId), Is.True, "MarketingStatus incorrect on plan " + planList[0].PlanCode);
            Assert.That(PlanMarketingStatusIsEqualTo(MarketingStatus.Off, planList[1].PlanId), Is.True, "MarketingStatus incorrect on plan " + planList[1].PlanCode);
            Assert.That(PlanMarketingStatusIsEqualTo(MarketingStatus.Off, planList[2].PlanId), Is.True, "MarketingStatus incorrect on plan " + planList[2].PlanCode);
            Assert.That(PlanMarketingStatusIsEqualTo(MarketingStatus.Off, planList[3].PlanId), Is.True, "MarketingStatus incorrect on plan " + planList[3].PlanCode);
        }

        [Test]
        public async Task GetPlanForRisk_Response_ContainsCorrectVariablesForEachPlan()
        {
            var risks = await BasicInfoDefaultLoginAsync();

            var risk = risks.First();

            var response = await Client.GetPlanForRiskAsync<GetPlanResponse>(risk.Id);

            AssertPlanHasVariableWithOptionValues(response.Plans, "DTH", "linkedToCpi", true, false);

            AssertPlanHasVariableWithOptionValues(response.Plans, "TPS", "linkedToCpi", true, false);
            AssertPlanHasVariableWithOptionValues(response.Plans, "TPS", "occupationDefinition",
                OccupationDefinition.AnyOccupation.ToString(), OccupationDefinition.OwnOccupation.ToString());

            AssertPlanHasVariableWithOptionValues(response.Plans, "TRS", "linkedToCpi", true, false);

            AssertPlanHasVariableWithOptionValues(response.Plans, "IP", "waitingPeriod", (long)2, (long)4, (long)13, (long)104);
            AssertPlanHasVariableWithOptionValues(response.Plans, "IP", "benefitPeriod", (long)1, (long)2, (long)5);
        }

        [Test]
        public async Task GetPlanForRisk_ForCleanSkinRisk_AllVariableOptionsAreEnabled()
        {
            var risks = await BasicInfoDefaultLoginAsync();

            var risk = risks.First();

            var response = await Client.GetPlanForRiskAsync<GetPlanResponse>(risk.Id);

            AssertAllVariableOptionsForPlanAreAvailable(response.Plans, "DTH");
            AssertAllVariableOptionsForPlanAreAvailable(response.Plans, "TPS");
            AssertAllVariableOptionsForPlanAreAvailable(response.Plans, "TRS");
            AssertAllVariableOptionsForPlanAreAvailable(response.Plans, "IP");
        }


        [Test]
        public async Task GetPlanForRisk_ForOccupationClassSRA_OwnOccupationAnd2WeekWaitingPeriodNotAvailable()
        {
            var risks =
                await
                    BasicInfoDefaultLoginAsync(UnderwritingHelper.OccupationCode_WharfWorker, UnderwritingHelper.IndustryCode_Marine);

            var risk = risks.First();

            var response = await Client.GetPlanForRiskAsync<GetPlanResponse>(risk.Id);

            AssertPlanHasVariableWithOptionAvailable(response.Plans, "DTH", "linkedToCpi", true, true);
            AssertPlanHasVariableWithOptionAvailable(response.Plans, "DTH", "linkedToCpi", false, true);

            AssertPlanHasVariableWithOptionAvailable(response.Plans, "TPS", "linkedToCpi", true, true);
            AssertPlanHasVariableWithOptionAvailable(response.Plans, "TPS", "linkedToCpi", false, true);

            AssertPlanHasVariableWithOptionAvailable(response.Plans, "TPS", "occupationDefinition", OccupationDefinition.AnyOccupation.ToString(), true);
            AssertPlanHasVariableWithOptionAvailable(response.Plans, "TPS", "occupationDefinition", OccupationDefinition.OwnOccupation.ToString(), true); //TODO: when Occupation Definition hooked up to availability, this will be false

            AssertPlanHasVariableWithOptionAvailable(response.Plans, "TRS", "linkedToCpi", true, true);
            AssertPlanHasVariableWithOptionAvailable(response.Plans, "TRS", "linkedToCpi", false, true);

            AssertPlanHasVariableWithOptionAvailable(response.Plans, "IP", "waitingPeriod", (long)2, false);
            AssertPlanHasVariableWithOptionAvailable(response.Plans, "IP", "waitingPeriod", (long)4, true);
            AssertPlanHasVariableWithOptionAvailable(response.Plans, "IP", "waitingPeriod", (long)13, true);
            AssertPlanHasVariableWithOptionAvailable(response.Plans, "IP", "waitingPeriod", (long)104, true);

            AssertPlanHasVariableWithOptionAvailable(response.Plans, "IP", "benefitPeriod", (long)1, true);
            AssertPlanHasVariableWithOptionAvailable(response.Plans, "IP", "benefitPeriod", (long)2, true);
            AssertPlanHasVariableWithOptionAvailable(response.Plans, "IP", "benefitPeriod", (long)5, true);
        }


        private static void AssertPlanHasVariableWithOptionAvailable(IEnumerable<PlanSelectionResponse> plans, string planCode, string variableCode, object optionValue, bool isAvailable)
        {
            var variables = plans.Single(p => p.PlanCode == planCode).Variables;
            var variable = variables.SingleOrDefault(v => v.Code == variableCode);
            Assert.That(variable, Is.Not.Null, $"Variable code '{variableCode}' was not found on {planCode} plan");

            var option = variable.Options.SingleOrDefault(o => o.Value.Equals(optionValue));
            Assert.That(option, Is.Not.Null, $"Variable option with value '{optionValue}' was not found on variable '{variableCode}' for {planCode} plan");

            Assert.That(option.IsAvailable, Is.EqualTo(isAvailable));
        }

        private static void AssertPlanHasVariableWithOptionValues(IEnumerable<PlanSelectionResponse> plans, string planCode, string variableCode, params object[] optionValues)
        {
            var variables = plans.Single(p => p.PlanCode == planCode).Variables;
            var variable = variables.SingleOrDefault(v => v.Code == variableCode);
            Assert.That(variable, Is.Not.Null, $"Variable code '{variableCode}' was not found on {planCode} plan");

            foreach (var optionValue in optionValues)
            {
                var option = variable.Options.SingleOrDefault(o => o.Value.Equals(optionValue));
                Assert.That(option, Is.Not.Null, $"Variable option with value '{optionValue}' was not found on variable '{variableCode}' for {planCode} plan");                
            }
        }

        private static void AssertAllVariableOptionsForPlanAreAvailable(IEnumerable<PlanSelectionResponse> plans, string planCode)
        {
            var variables = plans.Single(p => p.PlanCode == planCode).Variables;
            var allLifeVariableOptions = variables.SelectMany(v => v.Options);
            Assert.That(allLifeVariableOptions.All(o => o.IsAvailable), Is.True, $"Not all variable options on {planCode} plan are available");
        }

        [Test]
        public async Task GetPlansForRisk_WhenCalledWithDefaults_CorrectRiskAndPlanMarketingStatusesStored_Async()
        {
            //Arrange
            var risks = await BasicInfoDefaultLoginAsync();

            //Act
            var response = await Client.GetPlanForRiskAsync<GetPlanResponse>(risks.First().Id);
            var plans = response.Plans.ToList();

            //Assert
            Assert.That(RiskMarketingStatusIsEqualTo(MarketingStatus.Lead, risks.First().Id), Is.True, "MarketingStatus incorrect on risk");
            Assert.That(PlanMarketingStatusIsEqualTo(MarketingStatus.Lead, plans[0].PlanId), Is.True, "MarketingStatus incorrect on plan");
            Assert.That(PlanMarketingStatusIsEqualTo(MarketingStatus.Off, plans[1].PlanId), Is.True, "MarketingStatus incorrect on plan");
            Assert.That(PlanMarketingStatusIsEqualTo(MarketingStatus.Off, plans[2].PlanId), Is.True, "MarketingStatus incorrect on plan");
            Assert.That(PlanMarketingStatusIsEqualTo(MarketingStatus.Lead, plans[3].PlanId), Is.True, "MarketingStatus incorrect on plan");
        }

        [Test]
        public async Task GetPlansForRisk_WithAirlinePilot_StoresIpPlanMarketingStatusAsOffDueToIneligibilityStored_Async()
        {
            //Arrange
            var risks = await BasicInfoDefaultLoginAsync(UnderwritingHelper.OccupationCode_CommercialAirlinePilot, UnderwritingHelper.IndustryCode_Aviation);

            //Act
            var response = await Client.GetPlanForRiskAsync<GetPlanResponse>(risks.First().Id);
            var plans = response.Plans.ToList();

            //Assert
            Assert.That(RiskMarketingStatusIsEqualTo(MarketingStatus.Lead, risks.First().Id), Is.True, "MarketingStatus incorrect on risk");
            Assert.That(PlanMarketingStatusIsEqualTo(MarketingStatus.Lead, plans[0].PlanId), Is.True, "MarketingStatus incorrect on plan");
            Assert.That(PlanMarketingStatusIsEqualTo(MarketingStatus.Off, plans[1].PlanId), Is.True, "MarketingStatus incorrect on plan");
            Assert.That(PlanMarketingStatusIsEqualTo(MarketingStatus.Off, plans[2].PlanId), Is.True, "MarketingStatus incorrect on plan");
            Assert.That(PlanMarketingStatusIsEqualTo(MarketingStatus.Off, plans[3].PlanId), Is.True, "MarketingStatus incorrect on plan");
        }

        [Test]
        public async Task GetPlansForRisk_TurnOffAllCovers_RiskMarketingStatusSetToOff_Async()
        {
            //Arrange
            var risks = await BasicInfoDefaultLoginAsync(UnderwritingHelper.OccupationCode_CommercialAirlinePilot, UnderwritingHelper.IndustryCode_Aviation);

            //Act
            var response = await Client.GetPlanForRiskAsync<GetPlanResponse>(risks.First().Id);
            var plans = response.Plans.ToList();
            await UpdatePlans<UpdatePlanResponse>(risks.First().Id, plans[0], new string[0], new string[0] );

            //Assert
            Assert.That(RiskMarketingStatusIsEqualTo(MarketingStatus.Off, risks.First().Id), Is.True, "MarketingStatus incorrect on risk");
            Assert.That(PlanMarketingStatusIsEqualTo(MarketingStatus.Off, plans[0].PlanId), Is.True, "MarketingStatus incorrect on plan");
            Assert.That(PlanMarketingStatusIsEqualTo(MarketingStatus.Off, plans[1].PlanId), Is.True, "MarketingStatus incorrect on plan");
            Assert.That(PlanMarketingStatusIsEqualTo(MarketingStatus.Off, plans[2].PlanId), Is.True, "MarketingStatus incorrect on plan");
            Assert.That(PlanMarketingStatusIsEqualTo(MarketingStatus.Off, plans[3].PlanId), Is.True, "MarketingStatus incorrect on plan");
        }
    }
}