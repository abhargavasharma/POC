using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using NUnit.Framework;
using TAL.Performance.Infrastructure.Core;
using TAL.QuoteAndApply.DataModel.Personal;
using TAL.QuoteAndApply.SalesPortal.Web.IntegrationTests.Clients;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.Web.Shared.HttpResults;
using Task = System.Threading.Tasks.Task;

namespace TAL.QuoteAndApply.SalesPortal.Web.IntegrationTests
{
    [TestFixture]
    public class CorrespondenceApiTests : BaseTestClass<CorrespondenceClient>
    {
        public CorrespondenceApiTests() : base(SimplePerformanceTool.CreateConsoleTimestamped())
        {
        }

        [TestCase("", "Test", "text@tal.com.au")]
        [TestCase("Testfirstname", "", "text@tal.com.au")]
        [TestCase("Testfirstname", "Test", "")]
        public async Task GetCorrespondenceSummary_WithMissingMiniumFieldsAndBothRiders_ReturnsValidCorresondenceFalseInSummaryAsync(string firstName, string lastName, string emailAddress)
        {
            var planClient = GetSalesPortalPlanClient();
            var policyClient = GetSalesPortalPolicyClient();

            var quoteRef = await SetUpQuote();

            var policyWithRisksSvc = Container.Resolve<IPolicyWithRisksService>();

            var policyWithRisks = policyWithRisksSvc.GetFrom(quoteRef);

            //check the risk
            var risk = policyWithRisks.Risks.First();
            var plansAndCovers = await planClient.GetPlansAndCoversAsync(policyWithRisks.Policy.QuoteReference, risk.Risk.Id);

            var lifePlan = plansAndCovers.Plans.Single(p => p.Code == "DTH");

            var planUpdate = new PlanUpdateRequest
            {
                CurrentActivePlan = new PlanConfigurationRequest
                {
                    CoverAmount = 100000,
                    LinkedToCpi = true,
                    PlanCode = "DTH",
                    PlanId = lifePlan.PlanId,
                    SelectedCoverCodes = new[] { "DTHAC", "DTHIC", "DTHASC" },
                    SelectedOptionCodes = new []
                    {
                        new OptionConfigurationRequest("PR", "Premium Relief", false)
                    },
                    SelectedRiders = new [] {
                        new PlanConfigurationRequest()
                        {
                            CoverAmount = 100000,
                            LinkedToCpi = true,
                            PlanCode = "TPDDTH",
                            PlanId = lifePlan.Riders.Single(p => p.Code == "TPDDTH").PlanId,
                            SelectedCoverCodes = new [] {"TPDDTHAC", "TPDDTHIC", "TPDDTHASC"},
                            SelectedOptionCodes = new []
                            {
                                new OptionConfigurationRequest("TPDDTHDBB", "Life Buy Back", true)
                            },
                            SelectedRiders = new PlanConfigurationRequest[0],
                            OccupationDefinition = "AnyOccupation",
                            PremiumHoliday = null,
                            PremiumType = "Level",
                            Selected = true,
                            
                        },
                        new PlanConfigurationRequest()
                        {
                            CoverAmount = 100000,
                            LinkedToCpi = false,
                            PlanCode = "TRADTH",
                            PlanId = lifePlan.Riders.Single(p => p.Code == "TRADTH").PlanId,
                            SelectedCoverCodes = new [] {"TRADTHSIN"},
                            SelectedOptionCodes = new []
                            {
                                new OptionConfigurationRequest("TRADTHDBB", "Life Buy Back", true)
                            },
                            SelectedRiders = new PlanConfigurationRequest[0],
                            PremiumHoliday = false,
                            PremiumType = "Level",
                            Selected = true
                        }
                    },
                    PremiumHoliday = true,
                    PremiumType = "Level",
                    Selected = true, 
                    OccupationDefinition = "Unknown"
                },
                QuoteReferenceNumber = policyWithRisks.Policy.QuoteReference,
                RiskId = risk.Risk.Id.ToString(),
                SelectedPlanCodes = new[] { "DTH", "IP" }
            };

            await planClient.UpdateAsync<PlansUpdateResponse>(policyWithRisks.Policy.QuoteReference, risk.Risk.Id, planUpdate);

            var personalDetailsRequest = new PolicyOwnerDetailsRequest()
            {
                Title = Title.Dr.ToString(),
                FirstName = firstName,
                Surname = lastName,
                Address = "TestAddress",
                Suburb = "TestSuburb",
                State = State.ACT.ToString(),
                Postcode = "9999",
                EmailAddress = emailAddress,
                MobileNumber = "0400000000",
                PartyConsents = new List<string>()
            };

            await policyClient.UpdateOwnerDetailsAsync<PolicyOwnerDetailsRequest>(policyWithRisks.Policy.QuoteReference, personalDetailsRequest);

            var corroResponse = await Client.GetCorrespondenceSummaryAsync<PolicyCorrespondenceViewModel>(policyWithRisks.Policy.QuoteReference, risk.Risk.Id);

            //Assert
            var fullName = firstName + " " + lastName;
            Assert.That(corroResponse, Is.Not.Null);
            Assert.That(corroResponse.ClientFullName, Is.EqualTo((firstName == "" || lastName == "") ? null : fullName));
            Assert.That(corroResponse.CustomerEmailAddress, Is.EqualTo(emailAddress));
            Assert.That(corroResponse.PremiumFrequency, Is.EqualTo("Month"));
            Assert.That(corroResponse.UserEmailAddress, Is.EqualTo("mmd@tal.com.au"));
            Assert.That(corroResponse.IsValidForEmailCorrespondence, Is.EqualTo(false));
            Assert.That(corroResponse.EmailSent, Is.EqualTo(false));
            Assert.That(corroResponse.PlanSummaries.Count(), Is.EqualTo(4));
        }

        [Test]
        public async Task GetCorrespondenceSummary_WithMinimumCorrespondence_ReturnValidCorresondenceSummaryAsync()
        {
            var policyClient = GetSalesPortalPolicyClient();
            var quoteRef = await SetUpQuote();

            var policyWithRisksSvc = Container.Resolve<IPolicyWithRisksService>();

            var policyWithRisks = policyWithRisksSvc.GetFrom(quoteRef);

            //check the risk
            var risk = policyWithRisks.Risks.First();

            await SetUpPersonalDetailsOnPolicy(quoteRef, risk.Risk.Id, policyClient);

            var corroResponse = await Client.GetCorrespondenceSummaryAsync<PolicyCorrespondenceViewModel>(quoteRef, risk.Risk.Id);

            //Assert
            Assert.That(corroResponse, Is.Not.Null);
            Assert.That(corroResponse.ClientFullName, Is.EqualTo("Testfirstname Test"));
            Assert.That(corroResponse.CustomerEmailAddress, Is.EqualTo("change@changed.com"));
            Assert.That(corroResponse.PremiumFrequency, Is.EqualTo("Month"));
            Assert.That(corroResponse.UserEmailAddress, Is.EqualTo("mmd@tal.com.au"));
            Assert.That(corroResponse.IsValidForEmailCorrespondence, Is.EqualTo(true));
            Assert.That(corroResponse.EmailSent, Is.EqualTo(false));
            Assert.That(corroResponse.PlanSummaries.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetCorrespondenceSummary_WithMinimumCorrespondence_SendEmail_EmailSentAsync()
        {
            var policyClient = GetSalesPortalPolicyClient();
            var quoteRef = await SetUpQuote();

            var policyWithRisksSvc = Container.Resolve<IPolicyWithRisksService>();

            var policyWithRisks = policyWithRisksSvc.GetFrom(quoteRef);

            //check the risk
            var risk = policyWithRisks.Risks.First();

            await SetUpPersonalDetailsOnPolicy(quoteRef, risk.Risk.Id, policyClient);

            var sendCorrespondenceResponse = await Client.SendCorrespondenceAsync(policyWithRisks.Policy.QuoteReference, risk.Risk.Id);

            //Assert
            Assert.That(sendCorrespondenceResponse, Is.Not.Null);
            Assert.That(sendCorrespondenceResponse, Is.EqualTo(true));
        }

        public async Task<string> SetUpQuote()
        {
            var policyClient = GetSalesPortalPolicyClient();

            var createClientRequest = new CreateClientRequest
            {
                PolicyOwner = new ClientRequest
                {
                    RatingFactors = new RatingFactorsRequest
                    {
                        DateOfBirth = DateTime.Now.AddYears(-20).ToString("dd/MM/yyyy"),
                        Gender = 'M',
                        AustralianResident = true,
                        Income = 100000,
                        OccupationCode = "229",
                        IndustryCode = "27l"
                    }
                }
            };

            var response = await policyClient.CreateAsync<RedirectToResponse>(createClientRequest);

            var parts = response.RedirectTo.Split('/');
            var quoteRef = parts.Last();

            if (quoteRef.Contains("?"))
            {
                parts = quoteRef.Split('?');
                quoteRef = parts.First();
            }

            return quoteRef;
        }

        public async Task SetUpPersonalDetailsOnPolicy(string quoteRef, int riskId, PolicyClient policyClient)
        {
            var personalDetailsRequest = new PolicyOwnerDetailsRequest()
            {
                Title = Title.Dr.ToString(),
                FirstName = "TestFirstName",
                Surname = "Test",
                Address = "TestAddress",
                Suburb = "TestSuburb",
                State = State.ACT.ToString(),
                Postcode = "9999",
                EmailAddress = "change@changed.com",
                MobileNumber = "0400000000"
            };

            await policyClient.UpdateOwnerDetailsAsync<PolicyOwnerDetailsParam>(quoteRef, personalDetailsRequest);
        }
    }
}
