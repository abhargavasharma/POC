using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using NUnit.Framework;
using TAL.Performance.Infrastructure.Core;
using TAL.QuoteAndApply.DataModel.Interactions;
using TAL.QuoteAndApply.DataModel.Personal;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.DataModel.Product;
using TAL.QuoteAndApply.Infrastructure.Extensions;
using TAL.QuoteAndApply.Party.Configuration;
using TAL.QuoteAndApply.Party.Leads.LeadsService;
using TAL.QuoteAndApply.Party.Models;
using TAL.QuoteAndApply.Party.Services;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.SalesPortal.Web.IntegrationTests.Clients;
using TAL.QuoteAndApply.SalesPortal.Web.Models;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.Tests.Shared.Builders;
using TAL.QuoteAndApply.Tests.Shared.Helpers;
using TAL.QuoteAndApply.Web.Shared.HttpResults;
using Task = System.Threading.Tasks.Task;

namespace TAL.QuoteAndApply.SalesPortal.Web.IntegrationTests
{
    [TestFixture]
    public class PolicyApiTests : BaseTestClass<PolicyClient>
    {
        public PolicyApiTests() : base(SimplePerformanceTool.CreateConsoleTimestamped())
        {
            
        }

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
          
        }

        [Test]
        public async Task Init_EmptyObjectReturned_Async()
        {
            var createClientRequest = await Client.InitAsync();

            Assert.That(createClientRequest, Is.Not.Null);
        }

        [Test]
        public async Task Create_EmptyObjectPosted_RequiredFieldErrorsReturned_Async()
        {
            var createClientRequest = new CreateClientRequest();

            var response = await Client.CreateAsync<Dictionary<string, IEnumerable<string>>>(createClientRequest);

            Assert.That(response, Is.Not.Null);
            Assert.That(response.ContainsKey("createClientRequest.policyOwner.ratingFactors.gender"), Is.True);
            Assert.That(response.ContainsKey("createClientRequest.policyOwner.ratingFactors.dateOfBirth"), Is.True);
            Assert.That(response.ContainsKey("createClientRequest.policyOwner.ratingFactors.australianResident"), Is.True);
            Assert.That(response.ContainsKey("createClientRequest.policyOwner.ratingFactors.income"), Is.True);
        }

        [Test]
        public async Task Create_AgeUnder18_ErrorReturned_Async()
        {
            var createClientRequest = new CreateClientRequest
            {
                PolicyOwner = new ClientRequest
                {
                    RatingFactors = new RatingFactorsRequest
                    {
                        DateOfBirth = DateTime.Now.AddYears(-17).ToString("dd/MM/yyyy"),
                        Gender = 'M',
                        AustralianResident = true,
                        Income=100000,
                        OccupationCode = "229",
                        IndustryCode = "27l"
                    }
                }
            };

            var response = await Client.CreateAsync<Dictionary<string, IEnumerable<string>>>(createClientRequest);

            Assert.That(response, Is.Not.Null);
            Assert.That(response.ContainsKey("createClientRequest.policyOwner.ratingFactors.dateOfBirth"), Is.True);
        }

        [Test]
        public async Task Create_NotAustralianResident_ErrorReturned_Async()
        {
            var createClientRequest = new CreateClientRequest
            {
                PolicyOwner = new ClientRequest
                {
                    RatingFactors = new RatingFactorsRequest
                    {
                        DateOfBirth = DateTime.Now.AddYears(-20).ToString("dd/MM/yyyy"),
                        Gender = 'M',
                        AustralianResident = false,
                        Income = 100000,
                        OccupationCode = "229",
                        IndustryCode = "27l"
                    }
                }
            };

            var response = await Client.CreateAsync<Dictionary<string, IEnumerable<string>>>(createClientRequest);

            Assert.That(response, Is.Not.Null);
            Assert.That(response.ContainsKey("createClientRequest.policyOwner.ratingFactors.australianResident"), Is.True);
        }

        [Test]
        public async Task Create_ValidRequiredFieldsPosted_PolicyCreated_Async()
        {
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

            var response = await Client.CreateAsync<RedirectToResponse>(createClientRequest);

            Assert.That(response, Is.Not.Null);
            Assert.That(response.RedirectTo, Is.Not.Null);

            var parts = response.RedirectTo.Split('/');
            var quoteRef = parts.Last();

            if (quoteRef.Contains("?"))
            {
                parts = quoteRef.Split('?');
                quoteRef = parts.First();
            }

            var policyWithRisksSvc = Container.Resolve<IPolicyWithRisksService>();

            var policyWithRisks = policyWithRisksSvc.GetFrom(quoteRef);

            //check the policy
            Assert.That(policyWithRisks.Policy.QuoteReference, Is.EqualTo(quoteRef));
            Assert.That(policyWithRisks.Policy.BrandKey, Is.EqualTo("TAL"));
            Assert.That(policyWithRisks.Policy.OrganisationId, Is.EqualTo(1));

            //check the risk
            var riskWrapper = policyWithRisks.Risks.First();
            Assert.That(riskWrapper.Risk.DateOfBirth, Is.EqualTo(createClientRequest.PolicyOwner.RatingFactors.DateOfBirth.ToDateExcactDdMmYyyy()));
            Assert.That(riskWrapper.Risk.Gender, Is.EqualTo(Gender.Male));

            //check the plans
            var plans = riskWrapper.Plans.ToList();
            Assert.That(plans.Count, Is.EqualTo(6));

            Assert.That(plans.All(x=> x.Plan.PremiumType == PremiumType.Stepped), Is.EqualTo(true));

            //check there are two riders
            var riders = riskWrapper.Plans.Where(x => x.Plan.ParentPlanId.HasValue).ToList();
            Assert.That(riders.Count, Is.EqualTo(2));

            //check DTH is selected
            var selectedPlans = plans.Where(x => x.Plan.Selected).ToList();
            Assert.That(selectedPlans.Count, Is.EqualTo(2));
            Assert.That(selectedPlans[0].Plan.Code, Is.EqualTo("DTH"));

            //check the covers
            var covers = riskWrapper.Plans.SelectMany(x => x.Covers).ToList();
            Assert.That(covers.Count, Is.EqualTo(18));
            Assert.That(covers.Count(x => x.Selected), Is.EqualTo(12));
        }

        [Test]
        public async Task GetPolicyForPremiumFrequency_ReturnsPremiumFrequencyForPolicy_Async()
        {
            await LogOutAndLoginWithBrand("TAL");

            var createQuoteService = Container.Resolve<ICreateQuoteService>();

            var response = createQuoteService.CreateQuote(
                new CreateQuoteParam(new RatingFactorsParam('M', DateTime.Now.AddYears(-30), true,
                    new SmokerStatusHelper(true), "229","27l", 600000),
                    new PersonalInformationParam("Mr", "Michael", "Test", "0400000000", "0800000000", "VIC", "Victor@tal.com.au", null, "Kensington", "12 Happy St", "1234"), true, ServiceLayer.Models.PolicySource.SalesPortal, "TAL"));

            var premiumTypeResponse = await Client.GetPolicyPremiumFrequencyAsync(response.QuoteReference);

            Assert.That(premiumTypeResponse, Is.Not.Null);
            Assert.That(premiumTypeResponse.PremiumFrequency, Is.EqualTo(PremiumFrequency.Monthly.ToString()));
        }

        [Test]
        public async Task GetPolicyForPremiumFrequencyForExternalCustomer_ReturnsPremiumFrequencyForPolicy_Async()
        {
            await LogOutAndLoginWithBrand("TAL");

            var createQuoteService = Container.Resolve<ICreateQuoteService>();

            var response = createQuoteService.CreateQuote(
                new CreateQuoteParam(new RatingFactorsParam('M', DateTime.Now.AddYears(-30), true,
                    new SmokerStatusHelper(true), "229", "27l", 600000),
                    new PersonalInformationParam("Mr", "Michael", "Test", "0400000000", "0800000000", "VIC", "Victor@tal.com.au", null, "Kensington", "12 Happy St", "1234", "123"), true, ServiceLayer.Models.PolicySource.SalesPortal, "TAL"));

            var premiumTypeResponse = await Client.GetPolicyPremiumFrequencyAsync(response.QuoteReference);

            Assert.That(premiumTypeResponse, Is.Not.Null);
            Assert.That(premiumTypeResponse.PremiumFrequency, Is.EqualTo(PremiumFrequency.Monthly.ToString()));
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task UpdatePolicyPremiumFrequency_ReturnsPremiumFrequencyForPolicy_Async(bool validateResidency)
        {
            var createQuoteService = Container.Resolve<ICreateQuoteService>();

            var response = createQuoteService.CreateQuote(
                new CreateQuoteParam(new RatingFactorsParam('M', DateTime.Now.AddYears(-30), true,
                    new SmokerStatusHelper(true), "229", "27l", 600000),
                    new PersonalInformationParam("Mr", "Michael", "Test", "0400000000", "0800000000", "VIC", "Victor@tal.com.au", null, "Kensington", "12 Happy St", "1234"), validateResidency, ServiceLayer.Models.PolicySource.SalesPortal, "TAL"));

            var updatePremiumFrequency = new PolicyFremiumFrequencyViewModel
            {
                PremiumFrequency = PremiumFrequency.Yearly.ToString(),
                QuoteReferenceNumber = response.QuoteReference
            };

            await Client.UpdatePolicyPremiumFrequencyAsync<PolicyFremiumFrequencyViewModel>(response.QuoteReference, updatePremiumFrequency);

            var policyService = Container.Resolve<IPolicyService>();
            var policy = policyService.GetByQuoteReferenceNumber(response.QuoteReference);
            Assert.That(policy.PremiumFrequency, Is.EqualTo(PremiumFrequency.Yearly));
        }

        [Test]
        public async Task Create_CreateFromAdobeLead_QuoteCreatedSuccessfully_WithCorrectLeadsServiceFieldsStored_Async()
        {
            //Create adobe lead
            var party = new PartyDto()
            {
                Gender = Gender.Female,
                DateOfBirth = new DateTime(1992, 05, 01),
                Title = Title.Dr,
                FirstName = "First",
                Surname = "Test",
                Address = "12 Derp St",
                Suburb = "Derpburb",
                State = State.ACT,
                Postcode = "1234",
                Country = Country.Australia,
                MobileNumber = "0400000000",
                HomeNumber = "0200000000",
                EmailAddress = "testman@test.com",
                ExternalCustomerReference = "123"
            };

            var policySourceType = PolicySource.CustomerPortalBuildMyOwn;

            var converter = new PartyToLeadsMessageConverter(new LeadConfigurationProvider());

            var createLeadMsg = converter.From(party, policySourceType);

            var httpLeadsService = Container.Resolve<IHttpLeadsService>();

            var lead = httpLeadsService.CreateLead(createLeadMsg);

            Assert.That(lead, Is.Not.Null, "Test failed to create an adobe lead");

            party.LeadId = lead.AdobeId;

            //Create new quote with adobe lead
            var createClientRequest = new CreateClientRequest
            {
                PolicyOwner = new ClientRequest
                {
                    RatingFactors = new RatingFactorsRequest
                    {
                        DateOfBirth = party.DateOfBirth.ToString("dd/MM/yyyy"), //These are two-way binded in front-end
                        Gender = party.Gender.ToString().ToCharArray()[0], //These are two-way binded in front-end
                        AustralianResident = true,
                        Income = 100000,
                        OccupationCode = "229",
                        IndustryCode = "27l"
                    },
                    PersonalDetails = new PersonalDetailsRequest()
                    {
                        LeadId = lead.AdobeId //Will do lookup of lead and communication preferences with this
                    }
                }
            };

            var response = await Client.CreateAsync<RedirectToResponse>(createClientRequest);

            Assert.That(response, Is.Not.Null);
            Assert.That(response.RedirectTo, Is.Not.Null);

            var parts = response.RedirectTo.Split('/');
            var quoteRef = parts.Last();

            if (quoteRef.Contains("?"))
            {
                parts = quoteRef.Split('?');
                quoteRef = parts.First();
            }

            var policyWithRisksSvc = Container.Resolve<IPolicyWithRisksService>();

            var policyWithRisks = policyWithRisksSvc.GetFrom(quoteRef);

            //check the policy
            Assert.That(policyWithRisks.Policy.QuoteReference, Is.EqualTo(quoteRef));

            var policyOwnerSvc = Container.Resolve<IPolicyOwnerService>();

            var policyOwnerPartyId = policyOwnerSvc.GetPolicyOwnerPartyId(policyWithRisks.Policy.Id);

            var partySvc = Container.Resolve<IPartyService>();

            var partyDto = partySvc.GetParty((int)policyOwnerPartyId);

            Assert.That(partyDto.LeadId.Value, Is.EqualTo(lead.AdobeId));
            Assert.That(partyDto.DateOfBirth, Is.EqualTo(party.DateOfBirth));
            Assert.That(partyDto.Gender, Is.EqualTo(party.Gender));
            Assert.That(partyDto.Title, Is.EqualTo(party.Title));
            Assert.That(partyDto.FirstName, Is.EqualTo(party.FirstName));
            Assert.That(partyDto.Surname, Is.EqualTo(party.Surname));
            Assert.That(partyDto.Address, Is.EqualTo(party.Address));
            Assert.That(partyDto.Suburb, Is.EqualTo(party.Suburb.ToUpper()));
            Assert.That(partyDto.State, Is.EqualTo(party.State));
            Assert.That(partyDto.Postcode, Is.EqualTo(party.Postcode));
            Assert.That(partyDto.Country, Is.EqualTo(party.Country));
            Assert.That(partyDto.MobileNumber, Is.EqualTo(party.MobileNumber));
            Assert.That(partyDto.HomeNumber, Is.EqualTo(party.HomeNumber));
            Assert.That(partyDto.EmailAddress, Is.EqualTo(party.EmailAddress));
            Assert.That(partyDto.ExternalCustomerReference, Is.EqualTo(party.ExternalCustomerReference));
        }

        [Test]
        public async Task Create_CreateFromAdobeLead_QuoteCreatedSuccessfully_WithCorrectCommunicationPreferencesServiceFieldsStored_Async()
        {
            //Create adobe lead
            var party = new PartyDto()
            {
                Gender = Gender.Female,
                DateOfBirth = new DateTime(1992, 05, 01),
                Title = Title.Dr,
                FirstName = "First",
                Surname = "Test",
                Address = "12 Derp St",
                Suburb = "Derpburb",
                State = State.ACT,
                Postcode = "1234",
                Country = Country.Australia,
                MobileNumber = "0400000000",
                HomeNumber = "0200000000",
                EmailAddress = "testman@test.com",
                ExternalCustomerReference = "123"
            };

            var policySourceType = PolicySource.CustomerPortalBuildMyOwn;

            var converter = new PartyToLeadsMessageConverter(new LeadConfigurationProvider());

            var createLeadMsg = converter.From(party, policySourceType);

            var httpLeadsService = Container.Resolve<IHttpLeadsService>();

            var lead = httpLeadsService.CreateLead(createLeadMsg);

            Assert.That(lead, Is.Not.Null, "Creation of adobe lead failed.");

            var partyConsentDto = new PartyConsentDto()
            {
                DncEmail = true,
                DncHomeNumber = false,
                DncMobile = true,
                DncPostalMail = false,
                ExpressConsent = true,
                ExpressConsentUpdatedTs = DateTime.Now
            };

            var consentConverter = new PartyCommunicationMessageConverter(new LeadConfigurationProvider());

            party.LeadId = lead.AdobeId;

            Assert.That(lead, Is.Not.Null, "Test failed to create an adobe lead");

            var updateCommPrefsMsg = consentConverter.From(party, partyConsentDto);

            httpLeadsService.UpdateCommunicationPreferences(updateCommPrefsMsg);

            var createClientRequest = new CreateClientRequest
            {
                PolicyOwner = new ClientRequest
                {
                    RatingFactors = new RatingFactorsRequest
                    {
                        DateOfBirth = party.DateOfBirth.ToString("dd/MM/yyyy"), //These are two-way binded in front-end
                        Gender = party.Gender.ToString().ToCharArray()[0], //These are two-way binded in front-end
                        AustralianResident = true,
                        Income = 100000,
                        OccupationCode = "229",
                        IndustryCode = "27l"
                    },
                    PersonalDetails = new PersonalDetailsRequest()
                    {
                        LeadId = lead.AdobeId
                    }
                }
            };

            var response = await Client.CreateAsync<RedirectToResponse>(createClientRequest);

            Assert.That(response, Is.Not.Null);
            Assert.That(response.RedirectTo, Is.Not.Null);

            var parts = response.RedirectTo.Split('/');
            var quoteRef = parts.Last();

            if (quoteRef.Contains("?"))
            {
                parts = quoteRef.Split('?');
                quoteRef = parts.First();
            }

            var policyWithRisksSvc = Container.Resolve<IPolicyWithRisksService>();

            var policyWithRisks = policyWithRisksSvc.GetFrom(quoteRef);

            //check the policy
            Assert.That(policyWithRisks.Policy.QuoteReference, Is.EqualTo(quoteRef));
            var policyOwnerSvc = Container.Resolve<IPolicyOwnerService>();
            var policyOwnerPartyId = policyOwnerSvc.GetPolicyOwnerPartyId(policyWithRisks.Policy.Id);
            var partyConsentSvc = Container.Resolve<IPartyConsentService>();

            var partyConsent = partyConsentSvc.GetPartyConsentByPartyId((int)policyOwnerPartyId);
            Assert.That(partyConsent, Is.Not.Null);
            Assert.That(partyConsent.ExpressConsent, Is.EqualTo(partyConsentDto.ExpressConsent));
            Assert.That(partyConsent.DncMobile, Is.EqualTo(partyConsentDto.DncMobile));
            Assert.That(partyConsent.DncEmail, Is.EqualTo(partyConsentDto.DncEmail));
            Assert.That(partyConsent.DncHomeNumber, Is.EqualTo(partyConsentDto.DncHomeNumber));
            Assert.That(partyConsent.DncPostalMail, Is.EqualTo(partyConsentDto.DncPostalMail));
            Assert.That(partyConsent.ExpressConsentUpdatedTs.Value.Date, Is.EqualTo(partyConsentDto.ExpressConsentUpdatedTs.Value.Date));
        }

        [Test]
        public async void GetPolicyProgress_ReturnsCorrectPolicyProgress_Async()
        {
            var createQuoteService = Container.Resolve<ICreateQuoteService>();

            var response = createQuoteService.CreateQuote(
                new CreateQuoteParam(new RatingFactorsParam('M', DateTime.Now.AddYears(-30), true,
                    new SmokerStatusHelper(true), "229", "27l", 600000),
                    new PersonalInformationParam("Mr", "Michael", "Test", "0400000000", "0800000000", "VIC", "Victor@tal.com.au", null, "Kensington", "12 Happy St", "1234"), true, ServiceLayer.Models.PolicySource.SalesPortal, "TAL"));

            var premiumTypeResponse = await Client.GetPolicyProgressAsync(response.QuoteReference);

            Assert.That(premiumTypeResponse, Is.Not.Null);
            Assert.That(premiumTypeResponse.Progress, Is.EqualTo(PolicyProgress.Unknown.ToString()));
        }

        public async void GetPolicyProgress_ReturnsCorrectPolicyProgressForExternalCustomer_Async()
        {
            var createQuoteService = Container.Resolve<ICreateQuoteService>();

            var response = createQuoteService.CreateQuote(
                new CreateQuoteParam(new RatingFactorsParam('M', DateTime.Now.AddYears(-30), true,
                    new SmokerStatusHelper(true), "229", "27l", 600000),
                    new PersonalInformationParam("Mr", "Michael", "Test", "0400000000", "0800000000", "VIC", "Victor@tal.com.au", null, "Kensington", "12 Happy St", "1234", "123"), true, ServiceLayer.Models.PolicySource.SalesPortal, "TAL"));

            var premiumTypeResponse = await Client.GetPolicyProgressAsync(response.QuoteReference);

            Assert.That(premiumTypeResponse, Is.Not.Null);
            Assert.That(premiumTypeResponse.Progress, Is.EqualTo(PolicyProgress.Unknown.ToString()));
        }
        [Test]
        public async void UpdatePolicyProgress_ReturnsProgressForPolicy_Async()
        {
            var createQuoteService = Container.Resolve<ICreateQuoteService>();

            var response = createQuoteService.CreateQuote(
                new CreateQuoteParam(new RatingFactorsParam('M', DateTime.Now.AddYears(-30), true,
                    new SmokerStatusHelper(true), "229", "27l", 600000),
                    new PersonalInformationParam("Mr", "Michael", "Test", "0400000000", "0800000000", "VIC", "Victor@tal.com.au", null, "Kensington", "12 Happy St", "1234"), true, ServiceLayer.Models.PolicySource.SalesPortal, "TAL"));

            var updatePolicyProgress = new PolicyProgressViewModel
            {
                Progress = PolicyProgress.ClosedCantContact.ToString()
            };

            await Client.UpdatePolicyProgressAsync<PolicyProgressViewModel>(response.QuoteReference, updatePolicyProgress);

            var policyService = Container.Resolve<IPolicyService>();
            var policy = policyService.GetByQuoteReferenceNumber(response.QuoteReference);
            Assert.That(policy.Progress, Is.EqualTo(PolicyProgress.ClosedCantContact));

            var policyInteractionService = Container.Resolve<IPolicyInteractionService>();

            var policyInteractionsRequest = PolicyInteractionsRequest.PolicyInteractionsByQuoteReference(response.QuoteReference);
            var policyInteractions = policyInteractionService.GetInteractions(policyInteractionsRequest);
            var progressInteraction =
                policyInteractions.Interactions.First(i => i.InteractionType == InteractionType.Pipeline_Status_Change);
            Assert.That(progressInteraction, Is.Not.Null);
        }

        [Test]
        public async void UpdatePolicyProgress_ReturnsProgressForPolicyForExternalCustomer_Async()
        {
            var createQuoteService = Container.Resolve<ICreateQuoteService>();

            var response = createQuoteService.CreateQuote(
                new CreateQuoteParam(new RatingFactorsParam('M', DateTime.Now.AddYears(-30), true,
                    new SmokerStatusHelper(true), "229", "27l", 600000),
                    new PersonalInformationParam("Mr", "Michael", "Test", "0400000000", "0800000000", "VIC", "Victor@tal.com.au", null, "Kensington", "12 Happy St", "1234", "123"), true, ServiceLayer.Models.PolicySource.SalesPortal, "TAL"));

            var updatePolicyProgress = new PolicyProgressViewModel
            {
                Progress = PolicyProgress.ClosedCantContact.ToString()
            };

            await Client.UpdatePolicyProgressAsync<PolicyProgressViewModel>(response.QuoteReference, updatePolicyProgress);

            var policyService = Container.Resolve<IPolicyService>();
            var policy = policyService.GetByQuoteReferenceNumber(response.QuoteReference);
            Assert.That(policy.Progress, Is.EqualTo(PolicyProgress.ClosedCantContact));

            var policyInteractionService = Container.Resolve<IPolicyInteractionService>();

            var policyInteractionsRequest = PolicyInteractionsRequest.PolicyInteractionsByQuoteReference(response.QuoteReference);
            var policyInteractions = policyInteractionService.GetInteractions(policyInteractionsRequest);
            var progressInteraction =
                policyInteractions.Interactions.First(i => i.InteractionType == InteractionType.Pipeline_Status_Change);
            Assert.That(progressInteraction, Is.Not.Null);
        }
        [Test]
        public async Task Create_PolicyCreated_WithSalesPortalSetAsPolicySourceAndCreatedBySalesPortalInteractionAdded_Async()
        {
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

            var response = await Client.CreateAsync<RedirectToResponse>(createClientRequest);

            var parts = response.RedirectTo.Split('/');
            var quoteRef = parts.Last();

            if (quoteRef.Contains("?"))
            {
                parts = quoteRef.Split('?');
                quoteRef = parts.First();
            }

            var policyWithRisksSvc = Container.Resolve<IPolicyWithRisksService>();

            var policyWithRisks = policyWithRisksSvc.GetFrom(quoteRef);

            Assert.That(policyWithRisks.Policy.Source, Is.EqualTo(PolicySource.SalesPortal));

            var policyInteractionService = Container.Resolve<IPolicyInteractionService>();
            var policyInteractions = policyInteractionService.GetInteractions(
                    PolicyInteractionsRequest.PolicyInteractionsByQuoteReference(quoteRef)).Interactions.ToList();
            var customerPortalInteraction =
                policyInteractions.FirstOrDefault(x => x.InteractionType == InteractionType.Created_By_Agent);
            Assert.That(customerPortalInteraction, Is.Not.Null);
        }

        [Test]
        public async Task Edit_SourceIsCreated_Async()
        {
            var createQuoteService = Container.Resolve<ICreateQuoteService>();

            var createQuoteResponse = createQuoteService.CreateQuote(
                new CreateQuoteParam(new RatingFactorsParam('M', DateTime.Now.AddYears(-30), true,
                    new SmokerStatusHelper(true), "229", "27l", 600000),
                    new PersonalInformationParam("Mr", "Michael", "Test", "0400000000", "0800000000", "VIC", "Victor@tal.com.au", null, "Kensington", "12 Happy St", "1234"), true, ServiceLayer.Models.PolicySource.SalesPortal, "TAL"));

            var response = await Client.EditAsync(createQuoteResponse.QuoteReference, QuoteEditSource.Created);

            Assert.That(response.QuoteReferenceNumber, Is.EqualTo(createQuoteResponse.QuoteReference));


            var policyInteractionSvc = Container.Resolve<IPolicyInteractionService>();
            var allInteractions = policyInteractionSvc.GetInteractions(PolicyInteractionsRequest.PolicyInteractionsByQuoteReference(createQuoteResponse.QuoteReference));
            var quoteAccessedInteraction = allInteractions.Interactions.FirstOrDefault(x => x.InteractionType == InteractionType.Quote_Accessed);

            Assert.That(quoteAccessedInteraction, Is.Not.Null);
        }

        [Test]
        public async Task Edit_SourceIsCreatedForExternalCustomer_Async()
        {
            var createQuoteService = Container.Resolve<ICreateQuoteService>();

            var createQuoteResponse = createQuoteService.CreateQuote(
                new CreateQuoteParam(new RatingFactorsParam('M', DateTime.Now.AddYears(-30), true,
                    new SmokerStatusHelper(true), "229", "27l", 600000),
                    new PersonalInformationParam("Mr", "Michael", "Test", "0400000000", "0800000000", "VIC", "Victor@tal.com.au", null, "Kensington", "12 Happy St", "1234", "123"), true, ServiceLayer.Models.PolicySource.SalesPortal, "TAL"));

            var response = await Client.EditAsync(createQuoteResponse.QuoteReference, QuoteEditSource.Created);

            Assert.That(response.QuoteReferenceNumber, Is.EqualTo(createQuoteResponse.QuoteReference));


            var policyInteractionSvc = Container.Resolve<IPolicyInteractionService>();
            var allInteractions = policyInteractionSvc.GetInteractions(PolicyInteractionsRequest.PolicyInteractionsByQuoteReference(createQuoteResponse.QuoteReference));
            var quoteAccessedInteraction = allInteractions.Interactions.FirstOrDefault(x => x.InteractionType == InteractionType.Quote_Accessed);

            Assert.That(quoteAccessedInteraction, Is.Not.Null);
        }

        [Test]
        public async Task Edit_SourceIsRetrieved_Async()
        {
            var createQuoteService = Container.Resolve<ICreateQuoteService>();

            var createQuoteResponse = createQuoteService.CreateQuote(
                new CreateQuoteParam(new RatingFactorsParam('M', DateTime.Now.AddYears(-30), true,
                    new SmokerStatusHelper(true), "229", "27l", 600000),
                    new PersonalInformationParam("Mr", "Michael", "Test", "0400000000", "0800000000", "VIC", "Victor@tal.com.au", null, "Kensington", "12 Happy St", "1234"), true, ServiceLayer.Models.PolicySource.SalesPortal, "TAL"));

            var response = await Client.EditAsync(createQuoteResponse.QuoteReference, QuoteEditSource.Retrieved);

            Assert.That(response.QuoteReferenceNumber, Is.EqualTo(createQuoteResponse.QuoteReference));

            var policyInteractionSvc = Container.Resolve<IPolicyInteractionService>();
            var allInteractions = policyInteractionSvc.GetInteractions(PolicyInteractionsRequest.PolicyInteractionsByQuoteReference(createQuoteResponse.QuoteReference));
            var quoteAccessedInteraction = allInteractions.Interactions.FirstOrDefault(x => x.InteractionType == InteractionType.Quote_Accessed);

            Assert.That(quoteAccessedInteraction, Is.Not.Null);
        }

        [Test]
        public async Task Edit_SourceIsRetrievedForExternalCustomers_Async()
        {
            var createQuoteService = Container.Resolve<ICreateQuoteService>();

            var createQuoteResponse = createQuoteService.CreateQuote(
                new CreateQuoteParam(new RatingFactorsParam('M', DateTime.Now.AddYears(-30), true,
                    new SmokerStatusHelper(true), "229", "27l", 600000),
                    new PersonalInformationParam("Mr", "Michael", "Test", "0400000000", "0800000000", "VIC", "Victor@tal.com.au", null, "Kensington", "12 Happy St", "1234", "123"), true, ServiceLayer.Models.PolicySource.SalesPortal, "TAL"));

            var response = await Client.EditAsync(createQuoteResponse.QuoteReference, QuoteEditSource.Retrieved);

            Assert.That(response.QuoteReferenceNumber, Is.EqualTo(createQuoteResponse.QuoteReference));

            var policyInteractionSvc = Container.Resolve<IPolicyInteractionService>();
            var allInteractions = policyInteractionSvc.GetInteractions(PolicyInteractionsRequest.PolicyInteractionsByQuoteReference(createQuoteResponse.QuoteReference));
            var quoteAccessedInteraction = allInteractions.Interactions.FirstOrDefault(x => x.InteractionType == InteractionType.Quote_Accessed);

            Assert.That(quoteAccessedInteraction, Is.Not.Null);
        }

        [Test]
        public async Task Create_YbQuoteCreated_YbIdIsStoredAgainstBrandAndOrganisation()
        {
            await LogOutAndLoginWithBrand("YB");

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
                        IndustryCode = "27l",
                    }
                }
            };

            var response = await Client.CreateAsync<RedirectToResponse>(createClientRequest);

            var parts = response.RedirectTo.Split('/');
            var quoteRef = parts.Last();

            var policyWithRisksSvc = Container.Resolve<IPolicyWithRisksService>();

            var policyWithRisks = policyWithRisksSvc.GetFrom(quoteRef.Split('?').First());

            Assert.That(policyWithRisks.Policy.BrandKey, Is.EqualTo("YB"));
            Assert.That(policyWithRisks.Policy.OrganisationId, Is.EqualTo(2));
        }

        [Test]
        public async Task GetOwnerDetailsAsync_GetRiskWithAllPersonalDetailsSet_ReturnsRiskPersonalDetailsAndIsCompleted_Async()
        {
            await LogOutAndLoginWithBrand("TAL");

            var createQuoteService = Container.Resolve<ICreateQuoteService>();

            var response = createQuoteService.CreateQuote(
                new CreateQuoteParam(new RatingFactorsParam('M', DateTime.Now.AddYears(-30), true,
                    new SmokerStatusHelper(true), "229", "27l", 600000),
                    new PersonalInformationParam("Mr", "Michael", "Test", "0400000000", "0800000000", "VIC", "Victor@tal.com.au", null, "Kensington", "12 Happy St", "1234"), true, ServiceLayer.Models.PolicySource.SalesPortal, "TAL"));
           
            var policyWithRisksSvc = Container.Resolve<IPolicyWithRisksService>();

            var policyWithRisks = policyWithRisksSvc.GetFrom(response.QuoteReference);

            var partySvc = Container.Resolve<IPartyService>();

            var party = partySvc.GetParty(policyWithRisks.Risks.First().Risk.PartyId);

            var personalDetailsRequest = await Client.GetOwnerDetailsAsync(response.QuoteReference);

            Assert.That(personalDetailsRequest, Is.Not.Null);
            Assert.That(personalDetailsRequest.IsCompleted, Is.True);
            Assert.That(personalDetailsRequest.Title, Is.EqualTo(party.Title.ToString()));
            Assert.That(personalDetailsRequest.FirstName, Is.EqualTo(party.FirstName));
            Assert.That(personalDetailsRequest.Surname, Is.EqualTo(party.Surname));
            Assert.That(personalDetailsRequest.Address, Is.EqualTo(party.Address));
            Assert.That(personalDetailsRequest.Suburb, Is.EqualTo(party.Suburb));
            Assert.That(personalDetailsRequest.State, Is.EqualTo(party.State.ToString()));
            Assert.That(personalDetailsRequest.Postcode, Is.EqualTo(party.Postcode));
            Assert.That(personalDetailsRequest.MobileNumber, Is.EqualTo(party.MobileNumber));
            Assert.That(personalDetailsRequest.HomeNumber, Is.EqualTo(party.HomeNumber));
            Assert.That(personalDetailsRequest.EmailAddress, Is.EqualTo(party.EmailAddress));
        }

        [Test]
        public async Task GetOwnerDetailsAsync_GetRiskWithSomePersonalDetailsSet_ReturnsRiskPersonalDetailsAndNotIsCompleted_Async()
        {
            var createQuoteService = Container.Resolve<ICreateQuoteService>();

            var response = createQuoteService.CreateQuote(
                new CreateQuoteParam(new RatingFactorsParam('M', DateTime.Now.AddYears(-30), true,
                    new SmokerStatusHelper(true), "229", "27l", 600000),
                    new PersonalInformationParam("Mr", "Michael", "Test", "0400000000", "0800000000", "Unknown", "Victor@tal.com.au", null, null, "12 Happy St", "1234"), true, ServiceLayer.Models.PolicySource.SalesPortal, "TAL"));

            var policyWithRisksSvc = Container.Resolve<IPolicyWithRisksService>();

            var policyWithRisks = policyWithRisksSvc.GetFrom(response.QuoteReference);

            var partySvc = Container.Resolve<IPartyService>();

            var party = partySvc.GetParty(policyWithRisks.Risks.First().Risk.PartyId);

            var personalDetailsRequest = await Client.GetOwnerDetailsAsync(response.QuoteReference);

            Assert.That(personalDetailsRequest, Is.Not.Null);
            Assert.That(personalDetailsRequest.IsCompleted, Is.False);
            Assert.That(personalDetailsRequest.Title, Is.EqualTo(party.Title.ToString()));
            Assert.That(personalDetailsRequest.FirstName, Is.EqualTo(party.FirstName));
            Assert.That(personalDetailsRequest.Surname, Is.EqualTo(party.Surname));
            Assert.That(personalDetailsRequest.Address, Is.EqualTo(party.Address));
            Assert.That(personalDetailsRequest.Suburb, Is.Null);
            Assert.That(personalDetailsRequest.State, Is.EqualTo("Unknown"));
            Assert.That(personalDetailsRequest.Postcode, Is.EqualTo(party.Postcode));
            Assert.That(personalDetailsRequest.MobileNumber, Is.EqualTo(party.MobileNumber));
            Assert.That(personalDetailsRequest.HomeNumber, Is.EqualTo(party.HomeNumber));
            Assert.That(personalDetailsRequest.EmailAddress, Is.EqualTo(party.EmailAddress));

        }
        [Test]
        public async Task GetOwnerDetailsAsync_SaveRiskWithExternalCustRefDetail_ReturnsRiskPersonalDetailsWithExternalCustRefValue_Async()
        {
            //Arrange
            var createQuoteService = Container.Resolve<ICreateQuoteService>();

            var request = new CreateQuoteParam(new RatingFactorsParam('M', DateTime.Now.AddYears(-30), true,
                new SmokerStatusHelper(true), "229", "27l", 600000),
                new PersonalInformationParam("Mr", "Michael", "Test", "0400000000", "0800000000", "Unknown",
                    "Victor@tal.com.au", null, null, "12 Happy St", "1234", "PASSME"), true,
                ServiceLayer.Models.PolicySource.SalesPortal, "TAL");

            //Act
            var createQuoteRequest = createQuoteService.CreateQuote(request);
            var response = await Client.GetOwnerDetailsAsync(createQuoteRequest.QuoteReference);


            //Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.IsCompleted, Is.False);
            Assert.That(response.Title, Is.EqualTo(request.PersonalInformation.Title.ToString()));
            Assert.That(response.FirstName, Is.EqualTo(request.PersonalInformation.Firstname));
            Assert.That(response.Surname, Is.EqualTo(request.PersonalInformation.Surname));
            Assert.That(response.Address, Is.EqualTo(request.PersonalInformation.Address));
            Assert.That(response.Suburb, Is.Null);
            Assert.That(response.State, Is.EqualTo("Unknown"));
            Assert.That(response.Postcode, Is.EqualTo(request.PersonalInformation.Postcode));
            Assert.That(response.MobileNumber, Is.EqualTo(request.PersonalInformation.MobileNumber));
            Assert.That(response.HomeNumber, Is.EqualTo(request.PersonalInformation.HomeNumber));
            Assert.That(response.EmailAddress, Is.EqualTo(request.PersonalInformation.EmailAddress));
            Assert.That(response.ExternalCustomerReference, Is.EqualTo(request.PersonalInformation.ExternalCustomerReference));

        }
        [Test]
        public async Task UpdateOwnerDetailsAsync_PostInvalidObjectWithNumbersInNames_ModelStateErrorsReturned_Async()
        {
            var createQuoteService = Container.Resolve<ICreateQuoteService>();

            var response = createQuoteService.CreateQuote(
                new CreateQuoteParam(new RatingFactorsParam('M', DateTime.Now.AddYears(-30), true,
                    new SmokerStatusHelper(true), "229", "27l", 600000),
                    new PersonalInformationParam("Mr", "Michael", "Test", "0400000000", "0800000000", "VIC", "Victor@tal.com.au", null, "Kensington", "12 Happy St", "1234"), true, ServiceLayer.Models.PolicySource.SalesPortal, "TAL"));
            
            var personalDetailsRequest = new PolicyOwnerDetailsRequest()
            {
                FirstName = "123456789",
                Surname = "123456789"
            };

            var modelStateErrors = await Client.UpdateOwnerDetailsAsync<Dictionary<string, IEnumerable<string>>>(response.QuoteReference, personalDetailsRequest);

            Assert.That(modelStateErrors, Is.Not.Null);
            Assert.That(modelStateErrors.ContainsKey("ownerDetailsRequest.firstName"), Is.True);
            Assert.That(modelStateErrors.First(x => x.Key.Equals("ownerDetailsRequest.firstName")).Value.First(), Is.EqualTo("Name must not contain numbers"));
            Assert.That(modelStateErrors.ContainsKey("ownerDetailsRequest.surname"), Is.True);
            Assert.That(modelStateErrors.First(x => x.Key.Equals("ownerDetailsRequest.surname")).Value.First(), Is.EqualTo("Name must not contain numbers"));
        }

        [Test]
        public async Task UpdateOwnerDetailsAsync_PostInvalidObjectWithMaxLengthLimitExceededForFields_ModelStateErrorsReturned_Async()
        {
            var createQuoteService = Container.Resolve<ICreateQuoteService>();

            var response = createQuoteService.CreateQuote(
                new CreateQuoteParam(new RatingFactorsParam('M', DateTime.Now.AddYears(-30), true,
                    new SmokerStatusHelper(true), "229", "27l", 600000),
                    new PersonalInformationParam("Mr", "Michael", "Test", "0400000000", "0800000000", "VIC", "Victor@tal.com.au", null, "Kensington", "12 Happy St", "1234"), true, ServiceLayer.Models.PolicySource.SalesPortal, "TAL"));
            
            var personalDetailsRequest = new PolicyOwnerDetailsRequest()
            {
                FirstName = "Testtttttttttttttttttttttt",
                Surname = "Testtttttttttttttttttttttt",
                Suburb = "TestSuburbbbbbbbbbbbb",
                Address = "TestAddressssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssss",
                EmailAddress = "test@test.testttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttttt"
            };

            var modelStateErrors = await Client.UpdateOwnerDetailsAsync<Dictionary<string, IEnumerable<string>>>(response.QuoteReference, personalDetailsRequest);

            Assert.That(modelStateErrors, Is.Not.Null);
            Assert.That(modelStateErrors.ContainsKey("ownerDetailsRequest.firstName"), Is.True);
            Assert.That(modelStateErrors.Last(x => x.Key.Equals("ownerDetailsRequest.firstName")).Value.Last(), Is.EqualTo("First name cannot be longer than 22 characters."));
            Assert.That(modelStateErrors.ContainsKey("ownerDetailsRequest.surname"), Is.True);
            Assert.That(modelStateErrors.Last(x => x.Key.Equals("ownerDetailsRequest.surname")).Value.Last(), Is.EqualTo("Surname cannot be longer than 22 characters."));
            Assert.That(modelStateErrors.ContainsKey("ownerDetailsRequest.suburb"), Is.True);
            Assert.That(modelStateErrors.Last(x => x.Key.Equals("ownerDetailsRequest.suburb")).Value.Last(), Is.EqualTo("Suburb cannot be longer than 20 characters."));
            Assert.That(modelStateErrors.ContainsKey("ownerDetailsRequest.address"), Is.True);
            Assert.That(modelStateErrors.Last(x => x.Key.Equals("ownerDetailsRequest.address")).Value.Last(), Is.EqualTo("Address cannot be longer than 50 characters."));
            Assert.That(modelStateErrors.ContainsKey("ownerDetailsRequest.emailAddress"), Is.True);
            Assert.That(modelStateErrors.Last(x => x.Key.Equals("ownerDetailsRequest.emailAddress")).Value.Last(), Is.EqualTo("Email address cannot be longer than 80 characters."));
        }


        [Test]
        public async Task PersonalDetails_PostAllFieldsExceptHomePhoneNumber_ChangesSavedAndIsCompleted_Async()
        {
            await LogOutAndLoginWithBrand("TAL");

            var createQuoteService = Container.Resolve<ICreateQuoteService>();
            var personalInformationParam = new PersonalInformationParam("Mr", "Michael", "Test", "0400000000",
                "0800000000", "VIC", "Victor@tal.com.au", null, "Kensington", "12 Happy St", "1234", "EXTERN123");

            var response = createQuoteService.CreateQuote(
                new CreateQuoteParam(new RatingFactorsParam('M', DateTime.Now.AddYears(-30), true,
                    new SmokerStatusHelper(true), "229", "27l", 600000),
                    personalInformationParam, true, ServiceLayer.Models.PolicySource.SalesPortal, "TAL"));
            
            var personalDetailsRequest = new PolicyOwnerDetailsRequest()
            {
                Title = Title.Dr.ToString(),
                FirstName = AppendChange(personalInformationParam.Firstname),
                Surname = AppendChange(personalInformationParam.Surname),
                Address = AppendChange(personalInformationParam.Address),
                Suburb = AppendChange(personalInformationParam.Suburb),
                State = State.ACT.ToString(),
                Postcode = "9999",
                EmailAddress = "change@changed.com",
                MobileNumber = "0400000000",
                PartyConsents = new List<string>(),
                ExternalCustomerReference = "NEWEXTERN"
            };

            var personalDetailsResult = await Client.UpdateOwnerDetailsAsync<PolicyOwnerDetailsParam>(response.QuoteReference, personalDetailsRequest);

            var updatedParty = await Client.GetOwnerDetailsAsync(response.QuoteReference);

            Assert.That(updatedParty, Is.Not.Null);
            Assert.That(personalDetailsResult.IsCompleted, Is.True);
            Assert.That(updatedParty.Title, Is.EqualTo(personalDetailsRequest.Title));
            Assert.That(updatedParty.FirstName, Is.EqualTo(personalDetailsRequest.FirstName));
            Assert.That(updatedParty.Surname, Is.EqualTo(personalDetailsRequest.Surname));
            Assert.That(updatedParty.Address, Is.EqualTo(personalDetailsRequest.Address));
            Assert.That(updatedParty.Suburb, Is.EqualTo(personalDetailsRequest.Suburb));
            Assert.That(updatedParty.State, Is.EqualTo(personalDetailsRequest.State));
            Assert.That(updatedParty.Postcode, Is.EqualTo(personalDetailsRequest.Postcode));
            Assert.That(updatedParty.EmailAddress, Is.EqualTo(personalDetailsRequest.EmailAddress));
            Assert.That(updatedParty.MobileNumber, Is.EqualTo(personalDetailsRequest.MobileNumber));
            Assert.That(updatedParty.HomeNumber, Is.EqualTo(personalDetailsRequest.HomeNumber));
            Assert.That(updatedParty.ExternalCustomerReference, Is.EqualTo(personalDetailsRequest.ExternalCustomerReference));
        }

        [Test]
        public async Task PersonalDetails_PostSomeFields_ChangesSavedAndNotIsCompleted_Async()
        {
            var createQuoteService = Container.Resolve<ICreateQuoteService>();
            var personalInformationParam = new PersonalInformationParam("Mr", "Michael", "Test", "0400000000",
                "0800000000", "VIC", "Victor@tal.com.au", null, "Kensington", "12 Happy St", "1234", "TESTEXT");

            var response = createQuoteService.CreateQuote(
                new CreateQuoteParam(new RatingFactorsParam('M', DateTime.Now.AddYears(-30), true,
                    new SmokerStatusHelper(true), "229", "27l", 600000),
                    personalInformationParam, true, ServiceLayer.Models.PolicySource.SalesPortal, "TAL"));

            var personalDetailsRequest = new PolicyOwnerDetailsRequest()
            {
                Title = Title.Dr.ToString(),
                Address = AppendChange(personalInformationParam.Address),
                Suburb = AppendChange(personalInformationParam.Suburb),
                State = State.ACT.ToString(),
                Postcode = "9999",
                EmailAddress = "change@changed.com",
                MobileNumber = "0400000000",
                HomeNumber = "0800000000",
                PartyConsents = new List<string>(),
                ExternalCustomerReference = "CHANGED"
            };

            var personalDetailsResult = await Client.UpdateOwnerDetailsAsync<PolicyOwnerDetailsParam>(response.QuoteReference, personalDetailsRequest);

            var updatedParty = await Client.GetOwnerDetailsAsync(response.QuoteReference);

            Assert.That(updatedParty, Is.Not.Null);
            Assert.That(personalDetailsResult.IsCompleted, Is.False);
            Assert.That(updatedParty.Title, Is.EqualTo(personalDetailsRequest.Title));
            Assert.That(updatedParty.FirstName, Is.Null);
            Assert.That(updatedParty.Surname, Is.Null);
            Assert.That(updatedParty.Address, Is.EqualTo(personalDetailsRequest.Address));
            Assert.That(updatedParty.Suburb, Is.EqualTo(personalDetailsRequest.Suburb));
            Assert.That(updatedParty.State, Is.EqualTo(personalDetailsRequest.State));
            Assert.That(updatedParty.Postcode, Is.EqualTo(personalDetailsRequest.Postcode));
            Assert.That(updatedParty.EmailAddress, Is.EqualTo(personalDetailsRequest.EmailAddress));
            Assert.That(updatedParty.MobileNumber, Is.EqualTo(personalDetailsRequest.MobileNumber));
            Assert.That(updatedParty.HomeNumber, Is.EqualTo(personalDetailsRequest.HomeNumber));
            Assert.That(updatedParty.ExternalCustomerReference, Is.EqualTo(personalDetailsRequest.ExternalCustomerReference));
        }

        [TestCase("Ordinary", PolicyOwnerType.Ordinary, false)]
        [TestCase("SelfManagedSuperFund", PolicyOwnerType.SelfManagedSuperFund, false)]
        [TestCase("SuperannuationFund", PolicyOwnerType.SuperannuationFund, false)]
        public async Task UpdatePolicyOwnerType_PostSomeFields_ChangesSavedAndNotIsCompleted_Async(string policyOwnerString, PolicyOwnerType policyOwnerType, bool isCompleted)
        {
            await LogOutAndLoginWithBrand("YB");

            var createQuoteService = Container.Resolve<ICreateQuoteService>();
            var personalInformationParam = new PersonalInformationParam("Mr", "Michael", "Test", "0400000000",
                "0800000000", "VIC", "Victor@tal.com.au", null, "Kensington", "12 Happy St", "1234");

            var response = createQuoteService.CreateQuote(
                new CreateQuoteParam(new RatingFactorsParam('M', DateTime.Now.AddYears(-30), true,
                    new SmokerStatusHelper(true), "229", "27l", 600000),
                    personalInformationParam, true, ServiceLayer.Models.PolicySource.SalesPortal, "YB"));

            var updatePolicyOwnerTypeResult = await Client.UpdatePolicyOwnerTypeAsync<PolicyOwnerDetailsParam>(response.QuoteReference, policyOwnerString);
            
            Assert.That(updatePolicyOwnerTypeResult.IsCompleted, Is.EqualTo(isCompleted));
            Assert.That(PolicyHasOwnerType(response.QuoteReference, policyOwnerType), Is.True);
        }

        private string AppendChange(string str)
        {
            return str + "Test";
        }
    }
}
