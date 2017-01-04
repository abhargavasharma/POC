using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using NUnit.Framework;
using TAL.Performance.Infrastructure.Core;
using TAL.QuoteAndApply.DataModel.Personal;
using TAL.QuoteAndApply.DataModel.Service;
using TAL.QuoteAndApply.Infrastructure.Extensions;
using TAL.QuoteAndApply.DataModel.Underwriting;
using TAL.QuoteAndApply.Party.Services;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.SalesPortal.Web.Extensions;
using TAL.QuoteAndApply.SalesPortal.Web.IntegrationTests.Clients;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Underwriting;
using TAL.QuoteAndApply.Tests.Shared.Builders;
using TAL.QuoteAndApply.Tests.Shared.Helpers;
using Task = System.Threading.Tasks.Task;

namespace TAL.QuoteAndApply.SalesPortal.Web.IntegrationTests
{
    [TestFixture]
    public class RiskApiTests : BaseTestClass<RiskClient>
    {
       
        public RiskApiTests() : base(SimplePerformanceTool.CreateConsoleTimestamped())
        {
        }

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {

        }

        [Test]
        public async Task RatingFactors_GetRiskWithAllRatingFactorsSet_RatingFactorsReturnedAndIsCompleted_Async()
        {
            var party = new PartyBuilder()
                .Default()
                .Build();

            var riskBuilder = new RiskBuilder()
                .Default()
                .WithDateOfBirth(party.DateOfBirth)
                .WithGender(party.Gender);

            var risk = riskBuilder.Build();
            var riskOccupation = riskBuilder.BuildOccupation();

            risk.OccupationClass = riskOccupation.OccupationClass;
            risk.OccupationCode = riskOccupation.OccupationCode;
            risk.OccupationTitle = riskOccupation.OccupationTitle;
            var createPolicyResult = PolicyHelper.CreatePolicy(risk, party);

            var ratingFactorsResponse = await Client.RatingFactorsAsync(createPolicyResult.Policy.QuoteReference,
                createPolicyResult.Risk.Id);

            Assert.That(ratingFactorsResponse, Is.Not.Null);
            Assert.That(ratingFactorsResponse.IsCompleted, Is.True);
            Assert.That(ratingFactorsResponse.RatingFactors.SmokerStatus, Is.EqualTo(risk.SmokerStatus.ToString()));
            Assert.That(ratingFactorsResponse.RatingFactors.AustralianResident, Is.EqualTo(risk.Residency == ResidencyStatus.Australian));
            Assert.That(ratingFactorsResponse.RatingFactors.DateOfBirth, Is.EqualTo(risk.DateOfBirth.ToString("dd/MM/yyyy")));
            Assert.That(ratingFactorsResponse.RatingFactors.Gender, Is.EqualTo(risk.Gender.ToFriendlyChar()));
            Assert.That(ratingFactorsResponse.RatingFactors.Income, Is.EqualTo(risk.AnnualIncome));
            Assert.That(ratingFactorsResponse.RatingFactors.OccupationCode, Is.EqualTo(riskOccupation.OccupationCode));
            Assert.That(ratingFactorsResponse.RatingFactors.OccupationTitle, Is.EqualTo(riskOccupation.OccupationTitle));
        }

        [Test]
        public async Task RatingFactors_GetRiskWithSomeRatingFactorsSet_RatingFactorsReturnedAndNotIsCompleted_Async()
        {
            var party = new PartyBuilder()
                .Default()
                .Build();

            var riskBuilder = new RiskBuilder()
                .Default()
                .WithAnnualIncome(75000)
                .WithSmokerStatus(SmokerStatus.Unknown)
                .WithDateOfBirth(party.DateOfBirth)
                .WithGender(party.Gender);

            var risk = riskBuilder.Build();
            var riskOccupation = riskBuilder.BuildOccupation();
            risk.OccupationClass = riskOccupation.OccupationClass;
            risk.OccupationCode = riskOccupation.OccupationCode;
            risk.OccupationTitle = riskOccupation.OccupationTitle;

            var createPolicyResult = PolicyHelper.CreatePolicy(risk, party);
            
            var ratingFactorsResponse = await Client.RatingFactorsAsync(createPolicyResult.Policy.QuoteReference,
                createPolicyResult.Risk.Id);

            Assert.That(ratingFactorsResponse, Is.Not.Null);
            Assert.That(ratingFactorsResponse.IsCompleted, Is.False);
            Assert.That(ratingFactorsResponse.RatingFactors.SmokerStatus, Is.Null);
            Assert.That(ratingFactorsResponse.RatingFactors.AustralianResident, Is.EqualTo(risk.Residency == ResidencyStatus.Australian));
            Assert.That(ratingFactorsResponse.RatingFactors.DateOfBirth, Is.EqualTo(risk.DateOfBirth.ToString("dd/MM/yyyy")));
            Assert.That(ratingFactorsResponse.RatingFactors.Gender, Is.EqualTo(risk.Gender.ToFriendlyChar()));
            Assert.That(ratingFactorsResponse.RatingFactors.Income, Is.EqualTo(risk.AnnualIncome));
            Assert.That(ratingFactorsResponse.RatingFactors.OccupationCode, Is.EqualTo(riskOccupation.OccupationCode));
            Assert.That(ratingFactorsResponse.RatingFactors.OccupationTitle, Is.EqualTo(riskOccupation.OccupationTitle));
        }

        [Test]
        public async Task RatingFactors_PostNotAustralianResident_ModelStateErrorReturned_Async()
        {
            var party = new PartyBuilder()
                .Default()
                .Build();

            var risk = new RiskBuilder()
                .Default()
                .WithDateOfBirth(party.DateOfBirth)
                .WithGender(party.Gender)
                .Build();

            var createPolicyResult = PolicyHelper.CreatePolicy(risk, party);

            var newDob = DateTime.Today.AddYears(-55);

            var ratingFactorsRequest = new RatingFactorsRequest
            {
                DateOfBirth = newDob.ToString("dd/MM/yyyy"),
                Gender = 'F',
                SmokerStatus = SmokerStatus.Yes.ToString(),
                OccupationCode = "lg",
                OccupationTitle = "Housekeeper/Chambermaid",
                AustralianResident = false,
                Income = risk.AnnualIncome
            };

            var modelStateErrors = await Client.RatingFactorsAsync<Dictionary<string, IEnumerable<string>>>(createPolicyResult.Policy.QuoteReference,
                createPolicyResult.Risk.Id, ratingFactorsRequest);

            Assert.That(modelStateErrors, Is.Not.Null);
            Assert.That(modelStateErrors.ContainsKey("ratingFactorsRequest.australianResident"), Is.True);

        }

        [Test]
        public async Task RatingFactors_PostAgeUnder18_ModelStateErrorReturned_Async()
        {
            var party = new PartyBuilder()
                .Default()
                .Build();

            var risk = new RiskBuilder()
                .Default()
                .WithDateOfBirth(party.DateOfBirth)
                .WithGender(party.Gender)
                .Build();

            var createPolicyResult = PolicyHelper.CreatePolicy(risk, party);

            var newDob = DateTime.Today.AddYears(-17);

            var ratingFactorsRequest = new RatingFactorsRequest
            {
                DateOfBirth = newDob.ToString("dd/MM/yyyy"),
                Gender = 'F',
                SmokerStatus = SmokerStatus.Yes.ToString(),
                OccupationCode = "lg",
                OccupationTitle = "Housekeeper/Chambermaid",
                AustralianResident = true,
                Income = risk.AnnualIncome
            };

            var modelStateErrors = await Client.RatingFactorsAsync<Dictionary<string, IEnumerable<string>>>(createPolicyResult.Policy.QuoteReference,
                createPolicyResult.Risk.Id, ratingFactorsRequest);

            Assert.That(modelStateErrors, Is.Not.Null);
            Assert.That(modelStateErrors.ContainsKey("ratingFactorsRequest.dateOfBirth"), Is.True);

        }

        [Test(Description = "MB: Needed to ignore occupation stuff for now because the new template doesnt have an occuaptions configured")]
        public async Task RatingFactors_PostAllFields_ChangesSavedToRiskAndPartyAndInterview_Async()
        {
            var party = new PartyBuilder()
                .Default()
                .Build();

            var risk = new RiskBuilder()
                .Default()
                .WithDateOfBirth(party.DateOfBirth)
                .WithGender(party.Gender)
                .Build();

            var createPolicyResult = PolicyHelper.CreatePolicy(risk, party);

            var newDob = DateTime.Today.AddYears(-55);

            var ratingFactorsRequest = new RatingFactorsRequest
            {
                DateOfBirth = newDob.ToString("dd/MM/yyyy"),
                Gender = 'F',
                SmokerStatus = SmokerStatus.Yes.ToString(),
                OccupationCode = "229",
                IndustryCode = "27l",
                AustralianResident = true,
                Income = risk.AnnualIncome
            };

            var ratingFactorsResponse = await Client.RatingFactorsAsync<RatingFactorsResponse>(createPolicyResult.Policy.QuoteReference,
                createPolicyResult.Risk.Id, ratingFactorsRequest);

            var updatedRisk = RiskHelper.GetRisk(risk.Id, true);
            var updatedParty = PartyHelper.GetParty(party.Id, true);

            Assert.That(updatedParty, Is.Not.Null);
            Assert.That(updatedRisk, Is.Not.Null);

            //todo: cannot be done until occupation work is done
            //Assert.That(ratingFactorsResult.IsCompleted, Is.True);

            Assert.That(updatedRisk.SmokerStatus, Is.EqualTo(SmokerStatus.Yes));

            //Assert.That(updatedRisk.OccupationCode, Is.EqualTo(ratingFactorsRequest.OccupationCode));
            //Assert.That(updatedRisk.OccupationTitle, Is.EqualTo(ratingFactorsRequest.OccupationTitle));
            //var occQuestion = UnderwritingHelper.GetAnswerToQuestionInInterview(updatedRisk.InterviewId,
            //    QuestionIds.EmploymentQuestion);
            //Assert.That(occQuestion.ResponseId, Is.EqualTo(ratingFactorsRequest.OccupationCode));
            //Assert.That(occQuestion.SelectedText, Is.EqualTo(ratingFactorsRequest.OccupationTitle));


            Assert.That(updatedRisk.DateOfBirth, Is.EqualTo(newDob));
            Assert.That(updatedParty.DateOfBirth, Is.EqualTo(newDob));
            var dobQuestion = UnderwritingHelper.GetAnswerToQuestionInInterview(updatedRisk.InterviewId, updatedRisk.InterviewConcurrencyToken,
                QuestionIds.DateOfBirthQuestion);
            Assert.That(dobQuestion.SelectedText, Is.EqualTo(ratingFactorsRequest.DateOfBirth));

            Assert.That(updatedRisk.Gender, Is.EqualTo(Gender.Female));
            Assert.That(updatedParty.Gender, Is.EqualTo(Gender.Female));
            var genderQuestion = UnderwritingHelper.GetAnswerToQuestionInInterview(updatedRisk.InterviewId, updatedRisk.InterviewConcurrencyToken,
                QuestionIds.GenderQuestion);
            Assert.That(genderQuestion.SelectedText, Is.EqualTo("Female"));

        }

        [Test]
        public async Task RatingFactors_SetOccupationThatResultsInUnderwritingDecline_UnderwritingStatusSavedToCover_Async()
        {
            var party = new PartyBuilder()
                .Default()
                .Build();

            var risk = new RiskBuilder()
                .Default()
                .WithDateOfBirth(party.DateOfBirth)
                .WithGender(party.Gender)
                .Build();

            var createPolicyResult = PolicyHelper.CreatePolicy(risk, party);

            var newDob = DateTime.Today.AddYears(-55);

            var ratingFactorsRequest = new RatingFactorsRequest
            {
                DateOfBirth = newDob.ToString("dd/MM/yyyy"),
                Gender = 'F',
                SmokerStatus = SmokerStatus.Yes.ToString(),
                OccupationCode = "22t",
                OccupationTitle = "Agricultural Pilot / Crop Duster",
                IndustryCode = "27m",
                IndustryTitle = "Agriculture, Forestry & Animals",
                AustralianResident = true,
                Income = risk.AnnualIncome
            };

            await Client.RatingFactorsAsync<RatingFactorsResponse>(createPolicyResult.Policy.QuoteReference,
                createPolicyResult.Risk.Id, ratingFactorsRequest);

            var planService = Container.Resolve<IPlanService>();
            var coverService = Container.Resolve<ICoverService>();
            var plans = planService.GetPlansForRisk(createPolicyResult.Risk.Id);
            var tpdPlan = plans.Single(p => p.Code == "TPS");
            var tpdCovers = coverService.GetCoversForPlan(tpdPlan.Id);

            Assert.That(tpdCovers.All(c => c.UnderwritingStatus == UnderwritingStatus.Decline));
        }

        [Test]
        public async Task RatingFactors_PostDateOfBirthOver60_IneligiblePlansSwitchedOff_Async()
        {
            var policyWithRisks = await CreatePolicyFromRatingFactorsAsync(new RatingFactorsRequest
            {
                AustralianResident = true,
                DateOfBirth = DateTime.Now.AddYears(-45).ToFriendlyString(),
                Gender = 'M',
                Income = 100000,
                OccupationCode = "229",
                IndustryCode = "27l",
            });

            var policyWithRisksSvc = Container.Resolve<IPolicyWithRisksService>();
            var riskWrapper = policyWithRisks.Risks.First();

            foreach (var p in riskWrapper.Plans)
            {
                p.Plan.Selected = true;
            }

            policyWithRisksSvc.SaveAll(policyWithRisks);

            var ratingFactorsRequest = new RatingFactorsRequest
            {
                DateOfBirth = DateTime.Now.AddYears(-61).ToFriendlyString(),
                Gender = riskWrapper.Risk.Gender.ToFriendlyChar(),
                SmokerStatus = riskWrapper.Risk.SmokerStatus.ToString(),
                AustralianResident = riskWrapper.Risk.Residency == ResidencyStatus.Australian,
                Income = riskWrapper.Risk.AnnualIncome,
                OccupationCode = "229",
                IndustryCode = "27l",
            };

            await Client.RatingFactorsAsync<RatingFactorsResponse>(policyWithRisks.Policy.QuoteReference,
               riskWrapper.Risk.Id, ratingFactorsRequest);

            policyWithRisks = policyWithRisksSvc.GetFrom(policyWithRisks.Policy.QuoteReference);
            riskWrapper = policyWithRisks.Risks.First();

            foreach (var p in riskWrapper.Plans)
            {
                if (p.Plan.Code == "DTH")
                {
                    Assert.That(p.Plan.Selected, Is.True);
                }
                else
                {
                    Assert.That(p.Plan.Selected, Is.False);
                }
            }
        }

        [Test]
        public async Task RatingFactors_PostALowAnnualIncome_MinCoverMoreThanMaxForIp_IneligiblePlansSwitchedOff_Async()
        {
            var policyWithRisks = await CreatePolicyFromRatingFactorsAsync(new RatingFactorsRequest
            {
                AustralianResident = true,
                DateOfBirth = DateTime.Now.AddYears(-45).ToFriendlyString(),
                Gender = 'M',
                Income = 100000,
                OccupationCode = "229",
                IndustryCode = "27l",
            });

            var policyWithRisksSvc = Container.Resolve<IPolicyWithRisksService>();
            var riskWrapper = policyWithRisks.Risks.First();

            foreach (var p in riskWrapper.Plans)
            {
                p.Plan.Selected = true;
            }

            policyWithRisksSvc.SaveAll(policyWithRisks);

            foreach (var p in riskWrapper.Plans)
            {
                Assert.That(p.Plan.Selected, Is.True);
            }

            var ratingFactorsRequest = new RatingFactorsRequest
            {
                DateOfBirth = riskWrapper.Risk.DateOfBirth.ToFriendlyString(),
                Gender = riskWrapper.Risk.Gender.ToFriendlyChar(),
                SmokerStatus = riskWrapper.Risk.SmokerStatus.ToString(),
                AustralianResident = riskWrapper.Risk.Residency == ResidencyStatus.Australian,
                Income = 10000,
                OccupationCode = "229",
                IndustryCode = "27l",
            };

            await Client.RatingFactorsAsync<RatingFactorsResponse>(policyWithRisks.Policy.QuoteReference,
               riskWrapper.Risk.Id, ratingFactorsRequest);

            policyWithRisks = policyWithRisksSvc.GetFrom(policyWithRisks.Policy.QuoteReference);
            riskWrapper = policyWithRisks.Risks.First();

            foreach (var p in riskWrapper.Plans)
            {
                if (p.Plan.Code == "IP")
                {
                    Assert.That(p.Plan.Selected, Is.False);
                }
                else
                {
                    Assert.That(p.Plan.Selected, Is.True);
                }
            }
        }

        [Test]
        public async Task RatingFactors_PostALowAnnualIncome_MaxCoverAmountHasChanged_LifePlanCoverAmountScaledToMaxCoverAmount_Async()
        {
            var policyWithRisks = await CreatePolicyFromRatingFactorsAsync(new RatingFactorsRequest
            {
                AustralianResident = true,
                DateOfBirth = DateTime.Now.AddYears(-36).ToFriendlyString(),
                Gender = 'M',
                Income = 100000,
                OccupationCode = "229",
                IndustryCode = "27l",
            });

            var policyWithRisksSvc = Container.Resolve<IPolicyWithRisksService>();
            var riskWrapper = policyWithRisks.Risks.First();

            foreach (var p in riskWrapper.Plans)
            {
                p.Plan.Selected = true;

                if (p.Plan.Code == "DTH")
                {
                    p.Plan.CoverAmount = 500000;
                }
            }

            policyWithRisksSvc.SaveAll(policyWithRisks);

            foreach (var p in riskWrapper.Plans)
            {
                Assert.That(p.Plan.Selected, Is.True);
            }

            var ratingFactorsRequest = new RatingFactorsRequest
            {
                DateOfBirth = riskWrapper.Risk.DateOfBirth.ToFriendlyString(),
                Gender = riskWrapper.Risk.Gender.ToFriendlyChar(),
                SmokerStatus = riskWrapper.Risk.SmokerStatus.ToString(),
                AustralianResident = riskWrapper.Risk.Residency == ResidencyStatus.Australian,
                Income = 0,
                OccupationCode = "229",
                IndustryCode = "27l",
            };

            await Client.RatingFactorsAsync<RatingFactorsResponse>(policyWithRisks.Policy.QuoteReference,
               riskWrapper.Risk.Id, ratingFactorsRequest);

            policyWithRisks = policyWithRisksSvc.GetFrom(policyWithRisks.Policy.QuoteReference);
            riskWrapper = policyWithRisks.Risks.First();

            foreach (var p in riskWrapper.Plans)
            {
                if (p.Plan.Code == "DTH")
                {
                    Assert.That(p.Plan.CoverAmount, Is.EqualTo(500000));
                }
            }
        }

        [Test]
        public async Task PartyConsent_PostUpdate_ChangesSaved_Async()
        {
            var party = new PartyBuilder()
                .Default()
                .Build();

            var risk = new RiskBuilder()
                .Default()
                .WithDateOfBirth(party.DateOfBirth)
                .WithGender(party.Gender)
                .Build();

            var createPolicyResult = PolicyHelper.CreatePolicy(risk, party);

            var partyConsentRequest = new PartyConsentRequest()
            {
                Consents = new List<string>() {
                    ConsentType.DncEmail.ToString().ToCamelCase(),
                    ConsentType.DncMobile.ToString().ToCamelCase(),
                    ConsentType.DncHomeNumber.ToString().ToCamelCase(),
                    ConsentType.DncPostalMail.ToString().ToCamelCase()},
                ExpressConsent = true
            };

            await Client.PartyConsentAsync(createPolicyResult.Policy.QuoteReference,
                createPolicyResult.Risk.Id, partyConsentRequest);
        }

        [Test]
        public async Task PartyConsent_GetConsent_ChangesSavedAndReturnCorrectConsent_Async()
        {
            var party = new PartyBuilder()
                .Default()
                .Build();

            var risk = new RiskBuilder()
                .Default()
                .WithDateOfBirth(party.DateOfBirth)
                .WithGender(party.Gender)
                .Build();

            var createPolicyResult = PolicyHelper.CreatePolicy(risk, party);

            var partyConsentRequest = new PartyConsentRequest()
            {
                Consents = new List<string>() {
                    ConsentType.DncEmail.ToString().ToCamelCase(),
                    ConsentType.DncMobile.ToString().ToCamelCase(),
                    ConsentType.DncHomeNumber.ToString().ToCamelCase(),
                    ConsentType.DncPostalMail.ToString().ToCamelCase()},
                ExpressConsent = true
            };

            await Client.PartyConsentAsync(createPolicyResult.Policy.QuoteReference,
                createPolicyResult.Risk.Id, partyConsentRequest);

            var updatedConsents = await Client.PartyConsentAsync(createPolicyResult.Policy.QuoteReference,
                createPolicyResult.Risk.Id);

            Assert.That(updatedConsents, Is.Not.Null);
            Assert.That(updatedConsents.Consents.Contains(ConsentType.DncEmail.ToString().ToCamelCase()), Is.EqualTo(true));
            Assert.That(updatedConsents.Consents.Contains(ConsentType.DncMobile.ToString().ToCamelCase()), Is.EqualTo(true));
            Assert.That(updatedConsents.Consents.Contains(ConsentType.DncHomeNumber.ToString().ToCamelCase()), Is.EqualTo(true));
            Assert.That(updatedConsents.Consents.Contains(ConsentType.DncPostalMail.ToString().ToCamelCase()), Is.EqualTo(true));
            Assert.That(updatedConsents.ExpressConsent, Is.True);
        }

        [Test]
        public async Task PartyConsent_GetConsent_ChangesSavedAndReturnCorrectExpressConsentSetFalse_Async()
        {
            var party = new PartyBuilder()
                .Default()
                .Build();

            var risk = new RiskBuilder()
                .Default()
                .WithDateOfBirth(party.DateOfBirth)
                .WithGender(party.Gender)
                .Build();

            var createPolicyResult = PolicyHelper.CreatePolicy(risk, party);

            var partyConsentRequest = new PartyConsentRequest()
            {
                Consents = new List<string>() {
                    ConsentType.DncEmail.ToString().ToCamelCase(),
                    ConsentType.DncMobile.ToString().ToCamelCase(),
                    ConsentType.DncHomeNumber.ToString().ToCamelCase(),
                    ConsentType.DncPostalMail.ToString().ToCamelCase()},
                ExpressConsent = false
            };

            await Client.PartyConsentAsync(createPolicyResult.Policy.QuoteReference,
                createPolicyResult.Risk.Id, partyConsentRequest);

            var updatedConsents = await Client.PartyConsentAsync(createPolicyResult.Policy.QuoteReference,
                createPolicyResult.Risk.Id);

            Assert.That(updatedConsents, Is.Not.Null);
            Assert.That(updatedConsents.ExpressConsent, Is.False);
        }

        [Test]
        public async Task LifeInsuredPersonalDetails_GetAndSetConsentLifeInsuredPersonalDetails_CorrectlySaves_Async()
        {
            var createQuoteService = Container.Resolve<ICreateQuoteService>();
            var personalInformationParam = new PersonalInformationParam("Mr", "Michael", "Test", "0400000000",
                "0800000000", "VIC", "Victor@tal.com.au", null, "Kensington", "12 Happy St", "1234");

            var response = createQuoteService.CreateQuote(
                new CreateQuoteParam(new RatingFactorsParam('M', DateTime.Now.AddYears(-30), true,
                    new SmokerStatusHelper(true), "229", "27l", 600000),
                    personalInformationParam, true, ServiceLayer.Models.PolicySource.SalesPortal, "TAL"));

            var policyClient = GetSalesPortalPolicyClient();

            var policyWithRisksSvc = Container.Resolve<IPolicyWithRisksService>();

            var policyWithRisks = policyWithRisksSvc.GetFrom(response.QuoteReference);

            await policyClient.UpdatePolicyOwnerTypeAsync<PolicyOwnerDetailsParam>(response.QuoteReference, "SelfManagedSuperFund");

            var lifeInsured = await Client.LifeInsuredPersonalDetailsAsync(response.QuoteReference, policyWithRisks.Risks.First().Risk.Id);

            Assert.That(lifeInsured, Is.Not.Null);

            var lifeInsuredDetailsRequest = new LifeInsuredDetailsRequest()
            {
                ExpressConsent = true,
                FirstName = "Firstest",
                Postcode = "1234",
                Surname = "Lastest",
                Title = "Mr"
            };

            var lifeInsuredPersonalDetails = await Client.LifeInsuredPersonalDetailsAsync<UpdateRiskPersonalDetailsResult>(response.QuoteReference, policyWithRisks.Risks.First().Risk.Id, lifeInsuredDetailsRequest);

            Assert.That(lifeInsuredPersonalDetails, Is.Not.Null);
            Assert.That(lifeInsuredPersonalDetails.IsPersonalDetailsValidForInforce, Is.False);

            lifeInsured = await Client.LifeInsuredPersonalDetailsAsync(response.QuoteReference, policyWithRisks.Risks.First().Risk.Id);
            Assert.That(lifeInsured, Is.Not.Null);
            Assert.That(lifeInsured.ExpressConsent, Is.True);
            Assert.That(lifeInsured.FirstName, Is.EqualTo(lifeInsuredDetailsRequest.FirstName));
            Assert.That(lifeInsured.IsCompleted, Is.False);
            Assert.That(lifeInsured.Postcode, Is.EqualTo(lifeInsuredDetailsRequest.Postcode));
            Assert.That(lifeInsured.State, Is.EqualTo("UNKNOWN"));
            Assert.That(lifeInsured.Surname, Is.EqualTo(lifeInsuredDetailsRequest.Surname));
            Assert.That(lifeInsured.Title, Is.EqualTo(lifeInsuredDetailsRequest.Title));
        }

        [Test]
        public async Task LifeInsuredPersonalDetails_SetLifeInsuredPersonalDetailsWithMaxCharLengthsExceeded_CorrectErrorMessagesReturned_Async()
        {
            var createQuoteService = Container.Resolve<ICreateQuoteService>();
            var personalInformationParam = new PersonalInformationParam("Mr", "Michael", "Test", "0400000000",
                "0800000000", "VIC", "Victor@tal.com.au", null, "Kensington", "12 Happy St", "1234");

            var response = createQuoteService.CreateQuote(
                new CreateQuoteParam(new RatingFactorsParam('M', DateTime.Now.AddYears(-30), true,
                    new SmokerStatusHelper(true), "229", "27l", 600000),
                    personalInformationParam, true, ServiceLayer.Models.PolicySource.SalesPortal, "TAL"));

            var policyClient = GetSalesPortalPolicyClient();

            var policyWithRisksSvc = Container.Resolve<IPolicyWithRisksService>();

            var policyWithRisks = policyWithRisksSvc.GetFrom(response.QuoteReference);

            await policyClient.UpdatePolicyOwnerTypeAsync<PolicyOwnerDetailsParam>(response.QuoteReference, "SelfManagedSuperFund");

            var lifeInsured = await Client.LifeInsuredPersonalDetailsAsync(response.QuoteReference, policyWithRisks.Risks.First().Risk.Id);

            Assert.That(lifeInsured, Is.Not.Null);

            var lifeInsuredDetailsRequest = new LifeInsuredDetailsRequest()
            {
                ExpressConsent = true,
                FirstName = "Testtttttttttttttttttttttt",
                Surname = "Testtttttttttttttttttttttt",
                Postcode = "12345",
                Title = "Mr"
            };

            var lifeInsuredPersonalDetails = await Client.LifeInsuredPersonalDetailsAsync<Dictionary<string, IEnumerable<string>>>(response.QuoteReference, policyWithRisks.Risks.First().Risk.Id, lifeInsuredDetailsRequest);

            Assert.That(lifeInsuredPersonalDetails, Is.Not.Null);
            Assert.That(lifeInsuredPersonalDetails.ContainsKey("lifeInsuredDetailsRequest.firstName"), Is.True);
            Assert.That(lifeInsuredPersonalDetails.Last(x => x.Key.Equals("lifeInsuredDetailsRequest.firstName")).Value.Last(), Is.EqualTo("First name cannot be longer than 22 characters."));
            Assert.That(lifeInsuredPersonalDetails.ContainsKey("lifeInsuredDetailsRequest.surname"), Is.True);
            Assert.That(lifeInsuredPersonalDetails.Last(x => x.Key.Equals("lifeInsuredDetailsRequest.surname")).Value.Last(), Is.EqualTo("Surname cannot be longer than 22 characters."));
            Assert.That(lifeInsuredPersonalDetails.ContainsKey("lifeInsuredDetailsRequest.postcode"), Is.True);
            Assert.That(lifeInsuredPersonalDetails.Last(x => x.Key.Equals("lifeInsuredDetailsRequest.postcode")).Value.Last(), Is.EqualTo("Postcode must be 4 digits"));
        }

        [Test]
        public async Task LifeInsuredPersonalDetails_SetLifeInsuredPersonalDetailsWithPostcodeOnlyThreeChars_PostcodeLengthErrorMessageReturned_Async()
        {
            var createQuoteService = Container.Resolve<ICreateQuoteService>();
            var personalInformationParam = new PersonalInformationParam("Mr", "Michael", "Test", "0400000000",
                "0800000000", "VIC", "Victor@tal.com.au", null, "Kensington", "12 Happy St", "1234");

            var response = createQuoteService.CreateQuote(
                new CreateQuoteParam(new RatingFactorsParam('M', DateTime.Now.AddYears(-30), true,
                    new SmokerStatusHelper(true), "229", "27l", 600000),
                    personalInformationParam, true, ServiceLayer.Models.PolicySource.SalesPortal, "TAL"));

            var policyClient = GetSalesPortalPolicyClient();

            var policyWithRisksSvc = Container.Resolve<IPolicyWithRisksService>();

            var policyWithRisks = policyWithRisksSvc.GetFrom(response.QuoteReference);

            await policyClient.UpdatePolicyOwnerTypeAsync<PolicyOwnerDetailsParam>(response.QuoteReference, "SelfManagedSuperFund");

            var lifeInsured = await Client.LifeInsuredPersonalDetailsAsync(response.QuoteReference, policyWithRisks.Risks.First().Risk.Id);

            Assert.That(lifeInsured, Is.Not.Null);

            var lifeInsuredDetailsRequest = new LifeInsuredDetailsRequest()
            {
                ExpressConsent = true,
                FirstName = "Test",
                Surname = "Test",
                Postcode = "123",
                Title = "Mr"
            };

            var lifeInsuredPersonalDetails = await Client.LifeInsuredPersonalDetailsAsync<Dictionary<string, IEnumerable<string>>>(response.QuoteReference, policyWithRisks.Risks.First().Risk.Id, lifeInsuredDetailsRequest);

            Assert.That(lifeInsuredPersonalDetails, Is.Not.Null);
            Assert.That(lifeInsuredPersonalDetails.ContainsKey("lifeInsuredDetailsRequest.postcode"), Is.True);
            Assert.That(lifeInsuredPersonalDetails.Last(x => x.Key.Equals("lifeInsuredDetailsRequest.postcode")).Value.Last(), Is.EqualTo("Postcode must be 4 digits"));
        }

        [Test]
        public async Task LifeInsuredPersonalDetails_SetLifeInsuredPersonalDetailsWithInvalidNamesAndNumbers_CorrectErrorMessagesReturned_Async()
        {
            var createQuoteService = Container.Resolve<ICreateQuoteService>();
            var personalInformationParam = new PersonalInformationParam("Mr", "Michael", "Test", "0400000000",
                "0800000000", "VIC", "Victor@tal.com.au", null, "Kensington", "12 Happy St", "1234");

            var response = createQuoteService.CreateQuote(
                new CreateQuoteParam(new RatingFactorsParam('M', DateTime.Now.AddYears(-30), true,
                    new SmokerStatusHelper(true), "229", "27l", 600000),
                    personalInformationParam, true, ServiceLayer.Models.PolicySource.SalesPortal, "TAL"));

            var policyClient = GetSalesPortalPolicyClient();

            var policyWithRisksSvc = Container.Resolve<IPolicyWithRisksService>();

            var policyWithRisks = policyWithRisksSvc.GetFrom(response.QuoteReference);

            await policyClient.UpdatePolicyOwnerTypeAsync<PolicyOwnerDetailsParam>(response.QuoteReference, "SelfManagedSuperFund");

            var lifeInsured = await Client.LifeInsuredPersonalDetailsAsync(response.QuoteReference, policyWithRisks.Risks.First().Risk.Id);

            Assert.That(lifeInsured, Is.Not.Null);

            var lifeInsuredDetailsRequest = new LifeInsuredDetailsRequest()
            {
                ExpressConsent = true,
                FirstName = "Testttttttttttttttttt12",
                Surname = "Testttttttttttttttttt12",
                Postcode = "Abc",
                Title = "Mr"
            };

            var lifeInsuredPersonalDetails = await Client.LifeInsuredPersonalDetailsAsync<Dictionary<string, IEnumerable<string>>>(response.QuoteReference, policyWithRisks.Risks.First().Risk.Id, lifeInsuredDetailsRequest);

            Assert.That(lifeInsuredPersonalDetails, Is.Not.Null);
            Assert.That(lifeInsuredPersonalDetails.ContainsKey("lifeInsuredDetailsRequest.firstName"), Is.True);
            Assert.That(lifeInsuredPersonalDetails.First(x => x.Key.Equals("lifeInsuredDetailsRequest.firstName")).Value.First(), Is.EqualTo("Name must not contain numbers"));
            Assert.That(lifeInsuredPersonalDetails.ContainsKey("lifeInsuredDetailsRequest.surname"), Is.True);
            Assert.That(lifeInsuredPersonalDetails.First(x => x.Key.Equals("lifeInsuredDetailsRequest.surname")).Value.First(), Is.EqualTo("Name must not contain numbers"));
            Assert.That(lifeInsuredPersonalDetails.ContainsKey("lifeInsuredDetailsRequest.postcode"), Is.True);
            Assert.That(lifeInsuredPersonalDetails.First(x => x.Key.Equals("lifeInsuredDetailsRequest.postcode")).Value.First(), Is.EqualTo("Postcode must not contain any non numeric values"));
        }

        private string AppendChange(string str)
        {
            return str + "Test";
        }
    }
}