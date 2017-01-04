using System.Collections.Generic;
using System.Linq;
using Autofac;
using NUnit.Framework;
using TAL.Performance.Infrastructure.Core;
using TAL.QuoteAndApply.Customer.Web.Extensions;
using TAL.QuoteAndApply.Customer.Web.IntegrationTests.Clients;
using TAL.QuoteAndApply.Customer.Web.Models.Api;
using TAL.QuoteAndApply.DataModel.Interactions;
using TAL.QuoteAndApply.DataModel.Personal;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.DataModel.Service;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.Policy.Service.RaisePolicy;
using TAL.QuoteAndApply.ServiceLayer.Payment.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.Risk;
using TAL.QuoteAndApply.ServiceLayer.Underwriting;
using TAL.QuoteAndApply.Tests.Shared.Helpers;
using TAL.QuoteAndApply.Web.Shared.HttpResults;
using Task = System.Threading.Tasks.Task;

namespace TAL.QuoteAndApply.Customer.Web.IntegrationTests.Tests
{
    public class PurchaseApiTests : BaseTestClass<PurchaseClient>
    {
        public PurchaseApiTests() : base(SimplePerformanceTool.CreateConsoleTimestamped())
        {
        }

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {

        }

        [Test]
        public async Task Get_GetAvailablePaymentOptionss_ReturnsResponseWithNoErrors_Async()
        {
            await BasicInfoDefaultLoginAsync();
            

            var response = await Client.GetPurchaseOptionsAsync(false);

            Assert.That(response, Is.Not.Null);
            Assert.That(response.RiskPurchaseResponses.Count(), Is.EqualTo(1));
            Assert.That(response.RiskPurchaseResponses.First().IsComplete, Is.False);
        }


        [Test]
        public async Task Update_PurchaseWithSingleCompletePersonalDetailsWithCreditCard_ReturnsResponseWithNoErrors_Async()
        {
            await BasicInfoDefaultLoginAsync();

            var purchaseAndPremium = await Client.GetPurchaseOptionsAsync();
            var firstRisk = purchaseAndPremium.RiskPurchaseResponses.First();

            var purchaseViewModel = new PurchaseRequest
            {
                Beneficiaries = new List<BeneficiaryViewModel>()
                {
                    new BeneficiaryViewModel()
                    {
                        Address = "12 Happy St",
                        DateOfBirth = "12/12/1987",
                        FirstName = "Jim",
                        IsCompleted = true,
                        BeneficiaryRelationshipId = 1,
                        Share = "100",
                        Surname = "Test",
                        Suburb = "Sunnyville",
                        State = "VIC",
                        Postcode = "4444",
                        Title = "Mr",
                        PhoneNumber = "0400000000"
                    }
                },
                PaymentOptions = CreatePaymentOptionViewModelWithCreditCardPaymentOnly(),                
                PersonalDetails = new PersonalDetailsViewModel
                {
                    DateOfBirth = "01/01/1990",
                    EmailAddress = "glenn.zheng@tal.com.au",
                    FirstName = "Glenn",
                    LastName = "Test",
                    MobileNumber = "0400000000",
                    HomeNumber = "0200000000",
                    ResidentialAddress = "1 Test street Sydney NSW 2000",
                    Postcode = "2000",
                    State = "NSW",
                    Suburb = "Blah",
                    Title = "Mr"
                },
                DisclosureNotes = new PolicyNoteResultViewModel()
                {
                    Id = null,
                    NoteText = "Test Note"
                },
                NominateLpr = false,
                DeclarationAgree = true,
                DncSelection = true,
                RiskId = firstRisk.RiskId
            };

            var response = await Client.PostPurchaseDetailsAsync<PurchaseAndPremiumResponse>(firstRisk.RiskId, purchaseViewModel);
            Assert.That(response, Is.Not.Null);
        }

        [Test]
        public async Task Update_PaymentWithInvalidBeneficiaryFields_ReturnsResponseWithErrors_Async()
        {
            await BasicInfoDefaultLoginAsync();

            var purchaseAndPremium = await Client.GetPurchaseOptionsAsync();
            var firstRisk = purchaseAndPremium.RiskPurchaseResponses.First();

            var purchaseViewModel = new PurchaseRequest
            {
                Beneficiaries = new List<BeneficiaryViewModel>()
                {
                    new BeneficiaryViewModel()
                    {
                        Address = "12 Happy St, Happyville, Melbou",
                        DateOfBirth = "12/12/1111",
                        FirstName = "2333333333333333333333333333333333",
                        IsCompleted = true,
                        BeneficiaryRelationshipId = 1,
                        Share = "50",
                        Surname = "5444444444444444444444444444444444",
                        Suburb = "766666666666666666666",
                        State = "VIC",
                        Postcode = "444",
                        Title = "Mr",
                        PhoneNumber = "040000000",
                        EmailAddress = "derp@derpy"
                    }
                },
                PaymentOptions = CreatePaymentOptionViewModelWithCreditCardPaymentOnly(),
                DisclosureNotes = new PolicyNoteResultViewModel()
                {
                    Id = null,
                    NoteText = "Test Note"
                },
                NominateLpr = false,
                DeclarationAgree = false,
                RiskId = firstRisk.RiskId
            };

            var response = await Client.PostPurchaseDetailsAsync<Dictionary<string, IEnumerable<string>>>(firstRisk.RiskId, purchaseViewModel,
                throwOnFailure: false);

            Assert.That(response, Is.Not.Null);
            Assert.That(response.ContainsKey("purchaseRequest.beneficiaries[0].firstName"), Is.True);
            var firstNameErrors = response.First(x => x.Key.Equals("purchaseRequest.beneficiaries[0].firstName")).Value.ToArray();
            Assert.That(firstNameErrors.Contains("Name must not contain numbers"));
            Assert.That(firstNameErrors.Contains("First name cannot be longer than 22 characters."));
            Assert.That(response.ContainsKey("purchaseRequest.beneficiaries[0].surname"), Is.True);
            var surnameErrors = response.First(x => x.Key.Equals("purchaseRequest.beneficiaries[0].surname")).Value.ToArray();
            Assert.That(surnameErrors.Contains("Name must not contain numbers"));
            Assert.That(surnameErrors.Contains("Last name cannot be longer than 22 characters."));
            Assert.That(response.ContainsKey("purchaseRequest.beneficiaries[0].share"), Is.True);
            Assert.That(
                response.First(x => x.Key.Equals("purchaseRequest.beneficiaries[0].share")).Value.First(),
                Is.EqualTo("Total benefit allocation across all beneficiaries must add up to 100%."));
            Assert.That(response.ContainsKey("purchaseRequest.beneficiaries[0].share"), Is.True);
            Assert.That(
                response.First(x => x.Key.Equals("purchaseRequest.beneficiaries[0].dateOfBirth")).Value.First(),
                Is.EqualTo("A valid Date of birth must be entered"));

            Assert.That(response.ContainsKey("purchaseRequest.beneficiaries[0].emailAddress"), Is.True);
            Assert.That(
                response.First(x => x.Key.Equals("purchaseRequest.beneficiaries[0].emailAddress")).Value.First(),
                Is.EqualTo("A valid Email address must be entered"));
            Assert.That(response.ContainsKey("purchaseRequest.beneficiaries[0].postcode"), Is.True);
            Assert.That(
                response.First(x => x.Key.Equals("purchaseRequest.beneficiaries[0].postcode")).Value.First(),
                Is.EqualTo("Post code must be 4 digits"));
            Assert.That(response.ContainsKey("purchaseRequest.beneficiaries[0].address"), Is.True);
            Assert.That(
                response.First(x => x.Key.Equals("purchaseRequest.beneficiaries[0].address")).Value.First(),
                Is.EqualTo("Address cannot be longer than 30 characters."));
            Assert.That(response.ContainsKey("purchaseRequest.beneficiaries[0].suburb"), Is.True);
            Assert.That(
                response.First(x => x.Key.Equals("purchaseRequest.beneficiaries[0].suburb")).Value.First(),
                Is.EqualTo("Suburb cannot be longer than 20 characters."));
            Assert.That(response.ContainsKey("purchaseRequest.beneficiaries[0].phoneNumber"), Is.True);
            Assert.That(
                response.First(x => x.Key.Equals("purchaseRequest.beneficiaries[0].phoneNumber")).Value.First(),
                Is.EqualTo("Phone number must be 10 digits"));

            Assert.That(response.ContainsKey("purchaseRequest.declarationAgree"), Is.True);
            Assert.That(
                response.First(x => x.Key.Equals("purchaseRequest.declarationAgree")).Value.First(),
                Is.EqualTo("You need to declare you have read and agree with the terms above."));
        }

        [Test]
        public async Task Update_PaymentWithInCompleteBeneficiaryPersonalDetailsFields_ReturnsResponseWithRequiredErrors_Async()
        {
            await BasicInfoDefaultLoginAsync();

            var purchaseAndPremium = await Client.GetPurchaseOptionsAsync();
            var firstRisk = purchaseAndPremium.RiskPurchaseResponses.First();

            var purchaseViewModel = new PurchaseRequest
            {
                Beneficiaries = new List<BeneficiaryViewModel>()
                {
                    new BeneficiaryViewModel()
                    {
                        Address = null,
                        DateOfBirth = null,
                        FirstName = null,
                        IsCompleted = true,
                        BeneficiaryRelationshipId = null,
                        Share = null,
                        Surname = null,

                        Suburb = null,
                        State = null,
                        Postcode = null,
                        Title = null,
                        PhoneNumber = null,
                        EmailAddress = null
                    }
                },
                PaymentOptions = CreatePaymentOptionViewModelWithCreditCardPaymentOnly(),
                DeclarationAgree = true,
                RiskId = firstRisk.RiskId
            };

            var response = await Client.PostPurchaseDetailsAsync<Dictionary<string, IEnumerable<string>>>(firstRisk.RiskId, purchaseViewModel,
                throwOnFailure: false);

            Assert.That(response, Is.Not.Null);
            Assert.That(response.ContainsKey("purchaseRequest.beneficiaries[0].firstName"), Is.True);
            Assert.That(
                response.First(x => x.Key.Equals("purchaseRequest.beneficiaries[0].firstName")).Value.First(),
                Is.EqualTo("First name is required"));
            Assert.That(response.ContainsKey("purchaseRequest.beneficiaries[0].surname"), Is.True);
            Assert.That(
                response.First(x => x.Key.Equals("purchaseRequest.beneficiaries[0].surname")).Value.First(),
                Is.EqualTo("Last name is required"));
            Assert.That(response.ContainsKey("purchaseRequest.beneficiaries[0].share"), Is.True);
            Assert.That(
                response.First(x => x.Key.Equals("purchaseRequest.beneficiaries[0].share")).Value.First(),
                Is.EqualTo("Percentage of benefit is required"));
            Assert.That(response.ContainsKey("purchaseRequest.beneficiaries[0].share"), Is.True);
            Assert.That(
                response.First(x => x.Key.Equals("purchaseRequest.beneficiaries[0].dateOfBirth")).Value.First(),
                Is.EqualTo("Date of birth is required"));
            Assert.That(
                response.First(x => x.Key.Equals("purchaseRequest.beneficiaries[0].suburb")).Value.First(),
                Is.EqualTo("Suburb is required"));
            Assert.That(
                response.First(x => x.Key.Equals("purchaseRequest.beneficiaries[0].state")).Value.First(),
                Is.EqualTo("State is required"));
            Assert.That(
                response.First(x => x.Key.Equals("purchaseRequest.beneficiaries[0].postcode")).Value.First(),
                Is.EqualTo("Postcode is required"));
            Assert.That(
                response.First(x => x.Key.Equals("purchaseRequest.beneficiaries[0].title")).Value.First(),
                Is.EqualTo("Title is required"));
        }

        [Test]
        public async Task Update_PurchaseWithSingleCompletePersonalDetailsWithDirectDebit_ReturnsResponseWithNoErrors_Async()
        {
            var risks = await BasicInfoDefaultLoginAsync();

            var riskService = Container.Resolve<IRiskService>();
            var risk = riskService.GetRisk(risks.First().Id);

            //answer residency 
            var updateRiskService = Container.Resolve<IUpdateRiskService>();
            updateRiskService.UpdateRiskRatingFactors(risk.Id, new RatingFactorsParam(risk.Gender.ToFriendlyChar().Value, risk.DateOfBirth, true, new SmokerStatusHelper(risk.SmokerStatus.ToString()), risk.OccupationCode, risk.IndustryCode, risk.AnnualIncome), true);

            var purchaseAndPremium = await Client.GetPurchaseOptionsAsync();
            var firstRisk = purchaseAndPremium.RiskPurchaseResponses.First();

            var purchaseViewModel = new PurchaseRequest
            {
                Beneficiaries = new List<BeneficiaryViewModel>()
                {
                    new BeneficiaryViewModel()
                    {
                        Address = "12 Happy St",
                        DateOfBirth = "12/12/1987",
                        FirstName = "Jim",
                        IsCompleted = true,
                        BeneficiaryRelationshipId = 1,
                        Share = "100",
                        Surname = "Test",
                        Suburb = "Sunnyville",
                        State = "VIC",
                        Postcode = "4444",
                        Title = "Mr",
                        PhoneNumber = "0400000000",
                        EmailAddress = "derp@derpy.com"
                    }
                },
                PaymentOptions = CreatePaymentOptionViewModelWithDirectDebitPaymentOnly(),
                PersonalDetails = new PersonalDetailsViewModel
                {
                    DateOfBirth = "01/01/1990",
                    EmailAddress = "glenn.zheng@tal.com.au",
                    FirstName = "Glenn",
                    LastName = "Test",
                    MobileNumber = "0400000000",
                    HomeNumber = "0200000000",
                    ResidentialAddress = "1 Test street Sydney NSW 2000",
                    Postcode = "2000",
                    State = "NSW",
                    Suburb = "Blah",
                    Title = "Mr"
                },
                DisclosureNotes = new PolicyNoteResultViewModel()
                {
                    Id = null,
                    NoteText = "Test Note"
                },
                NominateLpr = false,
                DeclarationAgree = true,
                RiskId = firstRisk.RiskId
            };

            var response = await Client.PostPurchaseDetailsAsync<RedirectToResponse>(firstRisk.RiskId, purchaseViewModel);

            Assert.That(response.RedirectTo, Is.EqualTo("/Confirmation"));

            var policySerice = Container.Resolve<IPolicyService>();
            var policy = policySerice.GetByQuoteReferenceNumber(QuoteSessionContext.QuoteSession.QuoteReference);
            Assert.That(policy.Status, Is.EqualTo(DataModel.Policy.PolicyStatus.RaisedToPolicyAdminSystem));
        }

        [Test]
        public async Task Update_SuccesfulPurchase_InvalidPolicy_SetsAsCustomerReferral_ReturnsReferRedirect_Async()
        {
            await BasicInfoDefaultLoginAsync();

            var policyWithRisksSvc = Container.Resolve<IPolicyWithRisksService>();
            var policyWithRisks = policyWithRisksSvc.GetFrom(QuoteSessionContext.QuoteSession.QuoteReference);
            var risk = policyWithRisks.Risks.First();

            //answer residency 
            var updateRiskService = Container.Resolve<IUpdateRiskService>();
            updateRiskService.UpdateRiskRatingFactors(risk.Risk.Id, new RatingFactorsParam(risk.Risk.Gender.ToFriendlyChar().Value, risk.Risk.DateOfBirth, true, new SmokerStatusHelper("Yes"), risk.Risk.OccupationCode, risk.Risk.IndustryCode, risk.Risk.AnnualIncome), true);


            //brute force a failure start - this doe
            var lifePlan = risk.Plans.First(x => x.Plan.Code == "DTH");
            lifePlan.Plan.CoverAmount = 0;
            policyWithRisksSvc.SaveAll(policyWithRisks);
            //brute force a failure end

            var purchaseAndPremium = await Client.GetPurchaseOptionsAsync();
            var firstRisk = purchaseAndPremium.RiskPurchaseResponses.First();

            var purchaseViewModel = new PurchaseRequest
            {
                Beneficiaries = new List<BeneficiaryViewModel>()
                {
                    new BeneficiaryViewModel()
                    {
                        Address = "12 Happy St",
                        DateOfBirth = "12/12/1987",
                        FirstName = "Jim",
                        IsCompleted = true,
                        BeneficiaryRelationshipId = 1,
                        Share = "100",
                        Surname = "Test",
                        Suburb = "Sunnyville",
                        State = "VIC",
                        Postcode = "4444",
                        Title = "Mr",
                        PhoneNumber = "0400000000",
                        EmailAddress = "derp@derpy.com"
                    }
                },
                PaymentOptions = CreatePaymentOptionViewModelWithDirectDebitPaymentOnly(),
                PersonalDetails = new PersonalDetailsViewModel
                {
                    DateOfBirth = "01/01/1990",
                    EmailAddress = "glenn.zheng@tal.com.au",
                    FirstName = "Glenn",
                    LastName = "Test",
                    MobileNumber = "0400000000",
                    HomeNumber = "0200000000",
                    ResidentialAddress = "1 Test street Sydney NSW 2000",
                    Postcode = "2000",
                    State = "NSW",
                    Suburb = "Blah",
                    Title = "Mr"
                },
                DisclosureNotes = new PolicyNoteResultViewModel()
                {
                    Id = null,
                    NoteText = "Test Note"
                },
                NominateLpr = false,
                DeclarationAgree = true,
                RiskId = firstRisk.RiskId
            };

            var response = await Client.PostPurchaseDetailsAsync<RedirectToResponse>(firstRisk.RiskId, purchaseViewModel);

            Assert.That(response.RedirectTo, Is.EqualTo("/Submission"));

            var policySerice = Container.Resolve<IPolicyService>();
            var policy = policySerice.GetByQuoteReferenceNumber(QuoteSessionContext.QuoteSession.QuoteReference);
            Assert.That(policy.Status, Is.EqualTo(DataModel.Policy.PolicyStatus.Incomplete));
            Assert.That(policy.SaveStatus, Is.EqualTo(PolicySaveStatus.LockedOutDueToRefer));

            var policyInteractionSvc = Container.Resolve<IPolicyInteractionService>();
            var allInteractions = policyInteractionSvc.GetInteractions(PolicyInteractionsRequest.PolicyInteractionsByQuoteReference(QuoteSessionContext.QuoteSession.QuoteReference));
            var referredToUnderwriterInteraction = allInteractions.Interactions.FirstOrDefault(x => x.InteractionType == InteractionType.Customer_Submit_Application_Referred);

            Assert.That(referredToUnderwriterInteraction, Is.Not.Null);
        }

        [Test]
        public async Task Update_PurchaseWithSingleCompletePersonalDetailsWithBothPaymentTypes_ReturnsResponseWithErrors_Async()
        {
            await BasicInfoDefaultLoginAsync();

            var purchaseAndPremium = await Client.GetPurchaseOptionsAsync();
            var firstRisk = purchaseAndPremium.RiskPurchaseResponses.First();

            var purchaseViewModel = new PurchaseRequest
            {
                Beneficiaries = new List<BeneficiaryViewModel>()
                {
                    new BeneficiaryViewModel()
                    {
                        Address = "12 Happy St",
                        DateOfBirth = "12/12/1987",
                        FirstName = "Jim",
                        IsCompleted = true,
                        BeneficiaryRelationshipId = 1,
                        Share = "100",
                        Surname = "Test",
                        Suburb = "Sunnyville",
                        State = "VIC",
                        Postcode = "4444",
                        Title = "Mr",
                        PhoneNumber = "0400000000",
                        EmailAddress = "derp@derpy.com"
                    }
                },
                PaymentOptions = CreatePaymentOptionViewModelWithBothPaymentTypes(),
                PersonalDetails = new PersonalDetailsViewModel
                {
                    DateOfBirth = "01/01/1990",
                    EmailAddress = "glenn.zheng@tal.com.au",
                    FirstName = "Glen",
                    LastName = "Zheng",
                    MobileNumber = "0400000000",
                    HomeNumber = "0200000000",
                    ResidentialAddress = "1 Test street Sydney NSW 2000",
                    Postcode = "2000",
                    State = "NSW",
                    Suburb = "Blah",
                    Title = "Mr"
                },
                DisclosureNotes = new PolicyNoteResultViewModel()
                {
                    Id = null,
                    NoteText = "Test Note"
                },
                NominateLpr = false,
                DeclarationAgree = true,
                RiskId = firstRisk.RiskId
            };

            var response = await Client.PostPurchaseDetailsAsync<Dictionary<string, IEnumerable<string>>>(firstRisk.RiskId, purchaseViewModel,
               throwOnFailure: false);

            Assert.That(response.ContainsKey("multiplePaymentTypes"), Is.True);
        }

        [Test]
        public async Task Update_PurchaseWithSingleCompletePersonalDetailsWithFieldsCharLengthLimitsExceeded_ReturnsResponseWithErrors_Async()
        {
            await BasicInfoDefaultLoginAsync();

            var purchaseAndPremium = await Client.GetPurchaseOptionsAsync();
            var firstRisk = purchaseAndPremium.RiskPurchaseResponses.First();

            var purchaseViewModel = new PurchaseRequest
            {
                Beneficiaries = new List<BeneficiaryViewModel>()
                {
                    new BeneficiaryViewModel()
                    {
                        Address = "12 Happy St",
                        DateOfBirth = "12/12/1987",
                        FirstName = "Jim",
                        IsCompleted = true,
                        BeneficiaryRelationshipId = 1,
                        Share = "100",
                        Surname = "Test",
                        Suburb = "Sunnyville",
                        State = "VIC",
                        Postcode = "4444",
                        Title = "Mr",
                        PhoneNumber = "0400000000",
                        EmailAddress = "derp@derpy.com"
                    }
                },
                PaymentOptions = CreatePaymentOptionViewModelWithBothPaymentTypes(),
                PersonalDetails = new PersonalDetailsViewModel
                {
                    DateOfBirth = "01/01/1990",
                    EmailAddress = "glenn.zheng@tal.com.auuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuu",
                    FirstName = "Glennnnnnnnnnnnnnnnnnnnnnn",
                    LastName = "Zhengggggggggggggggggggggg",
                    MobileNumber = "0400000000",
                    HomeNumber = "0200000000",
                    ResidentialAddress = "1 Test street Sydney NSW 20000000000000000000000000000000000000000000000000000000",
                    Postcode = "2000",
                    State = "NSW",
                    Suburb = "Blahhhhhhhhhhhhhhhhhh",
                    Title = "Mr"
                },
                DisclosureNotes = new PolicyNoteResultViewModel()
                {
                    Id = null,
                    NoteText = "Test Note"
                },
                NominateLpr = false,
                DeclarationAgree = true,
                RiskId = firstRisk.RiskId
            };

            var response = await Client.PostPurchaseDetailsAsync<Dictionary<string, IEnumerable<string>>>(firstRisk.RiskId, purchaseViewModel,
               throwOnFailure: false);
            
            Assert.That(response.ContainsKey("purchaseRequest.personalDetails.firstName"), Is.True);
            Assert.That(
                response.First(x => x.Key.Equals("purchaseRequest.personalDetails.firstName")).Value.First(),
                Is.EqualTo("First name cannot be longer than 22 characters."));
            Assert.That(response.ContainsKey("purchaseRequest.personalDetails.lastName"), Is.True);
            Assert.That(
                response.First(x => x.Key.Equals("purchaseRequest.personalDetails.lastName")).Value.First(),
                Is.EqualTo("Last name cannot be longer than 22 characters."));
            Assert.That(response.ContainsKey("purchaseRequest.personalDetails.suburb"), Is.True);
            Assert.That(
                response.First(x => x.Key.Equals("purchaseRequest.personalDetails.suburb")).Value.First(),
                Is.EqualTo("Suburb cannot be longer than 20 characters."));
            Assert.That(response.ContainsKey("purchaseRequest.personalDetails.residentialAddress"), Is.True);
            Assert.That(
                response.First(x => x.Key.Equals("purchaseRequest.personalDetails.residentialAddress")).Value.First(),
                Is.EqualTo("Address cannot be longer than 50 characters."));
            Assert.That(response.ContainsKey("purchaseRequest.personalDetails.emailAddress"), Is.True);
            Assert.That(
                response.First(x => x.Key.Equals("purchaseRequest.personalDetails.emailAddress")).Value.First(),
                Is.EqualTo("Email address cannot be longer than 80 characters."));
        }

        private PaymentOptionsViewModel CreatePaymentOptionViewModelWithBothPaymentTypes()
        {
            var paymentOptions = new PaymentOptionsViewModel
            {
                IsCreditCardSelected = true,
                CreditCardPayment = new CreditCardPaymentViewModel
                {
                    CardType = CreditCardType.Visa,
                    CardNumber = "4444333322221111",
                    ExpiryMonth = "9",
                    ExpiryYear = "50",
                    NameOnCard = "Test user"
                },
                IsDirectDebitSelected = true,
                DirectDebitPayment = new DirectDebitPaymentViewModel
                {
                    AccountName = "test user",
                    BSBNumber = "123321",
                    AccountNumber = "111111111"
                }
            };
            return paymentOptions;
        }

        private PaymentOptionsViewModel CreatePaymentOptionViewModelWithCreditCardPaymentOnly()
        {
            var paymentOptions = new PaymentOptionsViewModel
            {
                IsCreditCardSelected = true,
                CreditCardPayment = new CreditCardPaymentViewModel
                {
                    CardType = CreditCardType.Visa,
                    CardNumber = "4444333322221111",
                    ExpiryMonth = "9",
                    ExpiryYear = "50",
                    NameOnCard = "Test user"
                }
            };
            return paymentOptions;
        }

        private PaymentOptionsViewModel CreatePaymentOptionViewModelWithDirectDebitPaymentOnly()
        {
            var paymentOptions = new PaymentOptionsViewModel
            {
                IsDirectDebitSelected = true,
                DirectDebitPayment = new DirectDebitPaymentViewModel
                {
                    AccountName = "test user",
                    BSBNumber = "123321",
                    AccountNumber = "111111111"
                }
            };
            return paymentOptions;
        }

        [Ignore] //Ignored because the fix for DE5996 needs to be backed out because it wasn't QA'd before code freeze... derp
        [TestCase(CreditCardType.MasterCard, "5555555555554444")]
        [TestCase(CreditCardType.Visa, "4444333322221111")]
        public async Task Update_CreditCard_TokenIsPopulated_ReturnsResponseWithValidationPassing_Async(CreditCardType creditCardType, string creditCardNumber)
        {
            CreateAndLoginAsDefaultPolicy();

            var purchaseAndPremium = await Client.GetPurchaseOptionsAsync();
            var firstRisk = purchaseAndPremium.RiskPurchaseResponses.First();

            var purchaseViewModel = new PurchaseRequest
            {
                Beneficiaries = new List<BeneficiaryViewModel>()
                {
                    new BeneficiaryViewModel()
                    {
                        Address = "12 Happy St",
                        DateOfBirth = "12/12/1987",
                        FirstName = "Jim",
                        IsCompleted = true,
                        BeneficiaryRelationshipId = 1,
                        Share = "100",
                        Surname = "Test",
                        Suburb = "Sunnyville",
                        State = "VIC",
                        Postcode = "4444",
                        Title = "Mr",
                        PhoneNumber = "0400000000",
                        EmailAddress = "derp@derpy.com"
                    }
                },
                PaymentOptions = new PaymentOptionsViewModel
                {
                    IsCreditCardSelected = true,
                    CreditCardPayment = new CreditCardPaymentViewModel
                    {
                        CardType = creditCardType,
                        CardNumber = creditCardNumber,
                        ExpiryMonth = "9",
                        ExpiryYear = "50",
                        NameOnCard = "Test user"
                    }
                },
                PersonalDetails = new PersonalDetailsViewModel
                {
                    DateOfBirth = "01/01/1990",
                    EmailAddress = "glenn.zheng@tal.com.au",
                    FirstName = "Glenn",
                    LastName = "Test",
                    MobileNumber = "0400000000",
                    HomeNumber = "0200000000",
                    ResidentialAddress = "1 Test street Sydney NSW 2000",
                    Postcode = "2000",
                    State = "NSW",
                    Suburb = "Blah",
                    Title = "Mr"
                },
                DisclosureNotes = new PolicyNoteResultViewModel()
                {
                    Id = null,
                    NoteText = "Test Note"
                },
                NominateLpr = false,
                DeclarationAgree = true,
                DncSelection = true,
                RiskId = firstRisk.RiskId
            };

            var response = await Client.PostPurchaseDetailsAsync<PurchaseAndPremiumResponse>(firstRisk.RiskId, purchaseViewModel, false);
        }

        [Ignore] //Ignored because the fix for DE5996 needs to be backed out because it wasn't QA'd before code freeze... derp
        [TestCase(CreditCardType.Visa, "5555555555554444")]
        [TestCase(CreditCardType.MasterCard, "4444333322221111")]
        public async Task Update_CreditCard_TokenIsEmpty_ReturnsResponseWithCardTypeValidationFails_Async(CreditCardType creditCardType, string creditCardNumber)
        {
            CreateAndLoginAsDefaultPolicy();

            var purchaseAndPremium = await Client.GetPurchaseOptionsAsync();
            var firstRisk = purchaseAndPremium.RiskPurchaseResponses.First();

            var purchaseViewModel = new PurchaseRequest
            {
                Beneficiaries = new List<BeneficiaryViewModel>()
                {
                    new BeneficiaryViewModel()
                    {
                        Address = "12 Happy St",
                        DateOfBirth = "12/12/1987",
                        FirstName = "Jim",
                        IsCompleted = true,
                        BeneficiaryRelationshipId = 1,
                        Share = "100",
                        Surname = "Test",
                        Suburb = "Sunnyville",
                        State = "VIC",
                        Postcode = "4444",
                        Title = "Mr",
                        PhoneNumber = "0400000000",
                        EmailAddress = "derp@derpy.com"
                    }
                },
                PaymentOptions = new PaymentOptionsViewModel
                {
                    IsCreditCardSelected = true,
                    CreditCardPayment = new CreditCardPaymentViewModel
                    {
                        CardType = creditCardType,
                        CardNumber = creditCardNumber,
                        ExpiryMonth = "9",
                        ExpiryYear = "50",
                        NameOnCard = "Test user"
                    }
                },
                PersonalDetails = new PersonalDetailsViewModel
                {
                    DateOfBirth = "01/01/1990",
                    EmailAddress = "glenn.zheng@tal.com.au",
                    FirstName = "Glenn",
                    LastName = "Zheng",
                    MobileNumber = "0400000000",
                    HomeNumber = "0200000000",
                    ResidentialAddress = "1 Test street Sydney NSW 2000",
                    Postcode = "2000",
                    State = "NSW",
                    Suburb = "Blah",
                    Title = "Mr"
                },
                DisclosureNotes = new PolicyNoteResultViewModel()
                {
                    Id = null,
                    NoteText = "Test Note"
                },
                NominateLpr = false,
                DeclarationAgree = true,
                DncSelection = true,
                RiskId = firstRisk.RiskId
            };

            var response = await Client.PostPurchaseDetailsAsync<Dictionary<string, IEnumerable<string>>>(firstRisk.RiskId, purchaseViewModel,
                throwOnFailure: false);

            Assert.That(response, Is.Not.Null);
            Assert.That(response.ContainsKey("purchaseRequest.paymentOptions.creditCardPayment.cardType"), Is.True);
            Assert.That(
                response.First(x => x.Key.Equals("purchaseRequest.paymentOptions.creditCardPayment.cardType")).Value.First(),
                Is.EqualTo("Card type does not match Card number"));
        }
    }
}