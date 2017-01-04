using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Autofac;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using TAL.Performance.Infrastructure.Core;
using TAL.QuoteAndApply.Customer.Web.Extensions;
using TAL.QuoteAndApply.Customer.Web.IntegrationTests.Clients;
using TAL.QuoteAndApply.Customer.Web.Models.View;
using TAL.QuoteAndApply.DataModel.Personal;
using TAL.QuoteAndApply.Party.Services;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.Tests.Shared.Helpers;
using TAL.QuoteAndApply.Web.Shared.HttpResults;
using Task= System.Threading.Tasks.Task;

namespace TAL.QuoteAndApply.Customer.Web.IntegrationTests.Tests
{
    [TestFixture]
    public class PolicyApiTests : BaseTestClass<PolicyClient>
    {
        public PolicyApiTests() : base(SimplePerformanceTool.CreateConsoleTimestamped())
        {
        }

        [SetUp]
        public void Setup()
        {
            QuoteSessionContext.Clear();
        }

        [Test]
        public async Task Init_OnGet_ReturnsDefaultBasicInfoViewModel_Async()
        {

            //Act
            var response = await Client.InitAsync<BasicInfoViewModel>();

            //Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.DateOfBirth, Is.Null);
            Assert.That(response.Gender, Is.Null);
            Assert.That(response.IsSmoker, Is.Null);
            Assert.That(response.AnnualIncome, Is.Null);
            Assert.That(response.Postcode, Is.Null);
        }


        [Test]
        public async Task CreateQuote_AllFieldsMissing_ReturnsModelErrors_Async()
        {
            //Act
            var response = await Client.CreateQuoteAsync<Dictionary<string, IEnumerable<string>>>(new BasicInfoViewModel());

            //Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.ContainsKey("basicInfoViewModel.gender"), Is.True);
            Assert.That(response.ContainsKey("basicInfoViewModel.isSmoker"), Is.True);
            Assert.That(response.ContainsKey("basicInfoViewModel.annualIncome"), Is.True);
            Assert.That(response.ContainsKey("basicInfoViewModel.dateOfBirth"), Is.True);
            Assert.That(response.ContainsKey("basicInfoViewModel.postcode"), Is.True);
        }

        [Test]
        public async Task CreateQuote_InvalidPostcode_ReturnsModelStateErrors_Async()
        {
            var model = new BasicInfoViewModel
            {
                AnnualIncome = 100000,
                DateOfBirth = DateTime.Today.AddYears(-30).ToFriendlyString(),
                Gender = 'M',
                IsSmoker = true,
                IndustryCode = UnderwritingHelper.IndustryCode_AdvertisingAndMarketing,
                OccupationCode = UnderwritingHelper.OccupationCode_AccountExecutive,
                Postcode = "0100"
            };

            var response = await Client.CreateQuoteAsync<Dictionary<string, IEnumerable<string>>>(model);

            Assert.That(QuoteSessionContext.HasValue(), Is.False);
            Assert.That(response.ContainsKey("basicInfoViewModel.postcode"), Is.True);
        }

        [Test]
        public async Task CreateQuote_ValidModel_RedirectToSelectCover_Async()
        {
            var model = new BasicInfoViewModel
            {
                AnnualIncome = 100000,
                DateOfBirth = DateTime.Today.AddYears(-30).ToString("dd/MM/yyyy"),
                Gender = 'M',
                IsSmoker = true,
                IndustryCode = UnderwritingHelper.IndustryCode_AdvertisingAndMarketing,
                OccupationCode = UnderwritingHelper.OccupationCode_AccountExecutive,
                Postcode = "1000"
            };

            var result = await Client.CreateQuoteAsync<RedirectToResponse> (model);

            Assert.That(QuoteSessionContext.HasValue(), Is.True);
            Assert.That(QuoteSessionContext.QuoteSession.SessionData.CallBackRequested, Is.EqualTo(false));
            Assert.That(result.RedirectTo, Is.EqualTo("/SelectCover"));
            Console.WriteLine(QuoteSessionContext.QuoteSession.QuoteReference);
        }

        [Test]
        public async Task CreateQuote_ValidModelWithPostcode_SavesPostcodeAndStateToDb_Async()
        {
            var risks = await BasicInfoLoginAsAsync(new BasicInfoViewModel
            {
                AnnualIncome = 100000,
                DateOfBirth = DateTime.Today.AddYears(-30).ToString("dd/MM/yyyy"),
                Gender = 'M',
                IsSmoker = true,
                IndustryCode = UnderwritingHelper.IndustryCode_AdvertisingAndMarketing,
                OccupationCode = UnderwritingHelper.OccupationCode_AccountExecutive,
                Postcode = "3000"
            });

            var risk = Container.Resolve<IRiskService>().GetRisk(risks.First().Id);
            var party = Container.Resolve<IPartyService>().GetParty(risk.PartyId);
            
            Assert.That(party.Postcode, Is.EqualTo("3000"));
            Assert.That(party.State, Is.EqualTo(State.VIC));
            
        }

        [Test]
        public async Task CreateQuote_ValidModel_PolicyInitialisationIsInCookie_PolicyCreatedWithPolicyAnalyticsIdentifiers_Async()
        {
            var fakeSitecoreId = Guid.NewGuid().ToString();
            var fakeResults = new JObject();
            var fakeAssumptions = new JObject();

            var policyInitialisationMetadataSessionStorageService = Container.Resolve<IPolicyInitialisationMetadataSessionStorageService>();
            policyInitialisationMetadataSessionStorageService.SaveMetadata(new PolicyInitialisationMetadata(fakeSitecoreId, fakeResults, fakeAssumptions, false, false));

            var model = new BasicInfoViewModel
            {
                AnnualIncome = 100000,
                DateOfBirth = DateTime.Today.AddYears(-30).ToString("dd/MM/yyyy"),
                Gender = 'M',
                IsSmoker = true,
                IndustryCode = UnderwritingHelper.IndustryCode_AdvertisingAndMarketing,
                OccupationCode = UnderwritingHelper.OccupationCode_AccountExecutive,
                Postcode = "1001"
            };

            await Client.CreateQuoteAsync<RedirectToResponse>(model);

            Console.WriteLine(QuoteSessionContext.QuoteSession.QuoteReference);

            var policyRepo = PolicyHelper.GetPolicyRepository();
            var policyAnalyitcsIdentifierRepo = PolicyHelper.GetPolicyAnalyticsIdentifierDtoRepository();

            var policy = policyRepo.GetPolicyByQuoteReference(QuoteSessionContext.QuoteSession.QuoteReference);
            var policyAnalyticsIdnetifier = policyAnalyitcsIdentifierRepo.GetByPolicyId(policy.Id);

            Assert.That(policyAnalyticsIdnetifier, Is.Not.Null);
            Assert.That(policyAnalyticsIdnetifier.SitecoreContactId, Is.EqualTo(fakeSitecoreId));
        }


        [Test]
        public async Task CreateQuoteFromHelpMeChoose_ValidModel_ReturnsQuoteReference_Async()
        {
            var model = new BasicInfoViewModel
            {
                AnnualIncome = 100000,
                DateOfBirth = DateTime.Today.AddYears(-30).ToString("dd/MM/yyyy"),
                Gender = 'M',
                IsSmoker = true,
                IndustryCode = UnderwritingHelper.IndustryCode_AdvertisingAndMarketing,
                OccupationCode = UnderwritingHelper.OccupationCode_AccountExecutive,
                Postcode = "1000"
            };

            var result = await Client.CreateQuoteViaHelpMeChooseAsync<string>(model);

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(QuoteSessionContext.QuoteSession.QuoteReference));
        }

        [Test]
        public async Task CreateQuoteFromHelpMeChoose_InvalidPostcode_ReturnsModelStateErrors_Async()
        {
            var model = new BasicInfoViewModel
            {
                AnnualIncome = 100000,
                DateOfBirth = DateTime.Today.AddYears(-30).ToFriendlyString(),
                Gender = 'M',
                IsSmoker = true,
                IndustryCode = UnderwritingHelper.IndustryCode_AdvertisingAndMarketing,
                OccupationCode = UnderwritingHelper.OccupationCode_AccountExecutive,
                Postcode = "0100"
            };

            var response = await Client.CreateQuoteViaHelpMeChooseAsync<Dictionary<string, IEnumerable<string>>>(model);

            Assert.That(QuoteSessionContext.HasValue(), Is.False);
            Assert.That(response.ContainsKey("basicInfoViewModel.postcode"), Is.True);
        }


        [Test]
        public async Task ValidateGeneralInformation_Valid_ReturnsSuccessfulResponse_Async()
        {
            var model = new GeneralInformationViewModel
            {
                DateOfBirth = DateTime.Today.AddYears(-30).ToFriendlyString(),
                Postcode = "1000"
            };

            var result = await Client.ValidateGeneralInformationAsync(model);
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task ValidateGeneralInformation_InvalidAge_ReturnsModelErrors_Async()
        {
            var model = new GeneralInformationViewModel
            {
                DateOfBirth = DateTime.Today.AddYears(-100).ToFriendlyString(),
                Postcode = "1000"
            };

            var response = await Client.ValidateGeneralInformationAsync<Dictionary<string, IEnumerable<string>>>(model);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.ContainsKey("basicInfoViewModel.dateOfBirth"), Is.True);
        }

        [Test]
        public async Task ValidateAge_InvalidAge_ReturnsModelErrors_Async()
        {
            var model = new ValidateAgeViewModel
            {
                DateOfBirth = DateTime.Today.AddYears(-100).ToFriendlyString()
            };

            var response = await Client.ValidateAgeAsync<Dictionary<string, IEnumerable<string>>>(model);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.ContainsKey("basicInfoViewModel.dateOfBirth"), Is.True);
        }

        [Test]
        public async Task ValidateGeneralInformation_InvalidPostcode_ReturnsModelErrors_Async()
        {
            var model = new GeneralInformationViewModel
            {
                DateOfBirth = DateTime.Today.AddYears(-30).ToFriendlyString(),
                Postcode = "0100"
            };

            var response = await Client.ValidateGeneralInformationAsync<Dictionary<string, IEnumerable<string>>>(model);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.ContainsKey("basicInfoViewModel.postcode"), Is.True);
        }
    }
}
