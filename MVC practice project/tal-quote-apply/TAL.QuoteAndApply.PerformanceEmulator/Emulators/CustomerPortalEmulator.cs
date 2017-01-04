using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using TAL.Performance.Infrastructure.Core;
using TAL.Performance.Infrastructure.Core.ApiClients;
using TAL.Performance.Infrastructure.Core.WebServers;
using TAL.QuoteAndApply.Customer.Web.IntegrationTests;
using TAL.QuoteAndApply.Customer.Web.IntegrationTests.Clients;
using TAL.QuoteAndApply.Customer.Web.Models.Api;
using TAL.QuoteAndApply.Customer.Web.Models.View;
using TAL.QuoteAndApply.Customer.Web.Services;
using TAL.QuoteAndApply.DataModel.Underwriting;
using TAL.QuoteAndApply.Infrastructure.Extensions;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.Tests.Shared.WebIntegration;
using TAL.QuoteAndApply.Web.Shared.HttpResults;
using Task = System.Threading.Tasks.Task;

namespace TAL.QuoteAndApply.PerformanceEmulator.Emulators
{
    public class CustomerPortalEmulator
    {
        private readonly IWebServer _webServer;
        private readonly IPerformanceTestTool _performanceTestTool;

        public CustomerPortalEmulator(IWebServer webServer, IPerformanceTestTool performanceTestTool)
        {
            _webServer = webServer;
            _performanceTestTool = performanceTestTool;
        }

        public async Task Vanilla(ScenarioData data, Action<string> setCookieCallback)
        {
            const string workflowCreateLife = "Workflow: Create life policy with all covers in Customer portal";
            _performanceTestTool.StartTransaction(workflowCreateLife);

            _performanceTestTool.LogInformation(string.Join("|", 
                data.Title, data.FirstName,data.LastName,
                data.DateOfBirth, data.Gender, data.AnnualIncome, data.Smoker ? "Smoker" : "Nonsmoker", string.Join(",", data.Search),
                data.StreetAddress, data.Town, data.State, data.PostCode));

            var policyClient = GetClient<PolicyClient>();
            var searchClient = GetClient<SearchClient>();
            var coverClient = GetClient<CoverSelectionApiClient>();
            var qualificationClient = GetClient<QualificationApiClient>();
            var reviewClient = GetClient<ReviewClient>();
            var chatClient = GetClient<ChatClient>();
            var purchaseClient = GetClient<PurchaseClient>();
            var referenceClient = GetClient<ReferenceClient>();
            var saveClient = GetClient<SaveClient>();

            var initResponse = await policyClient.InitAsync<BasicInfoViewModel>();

            var searchResponse = await searchClient.SearchAsync(data.Search[0]);
            foreach (var search in data.Search.Skip(1))
                searchResponse = await searchClient.SearchAsync(search, pause: false);

            var searchCodes = searchResponse.OrderByDescending(sr => sr.Score).First();


            var model = GetBasicInfoViewModel(data, searchCodes);

            var createResponse = await policyClient.CreateQuoteAsync<RedirectToResponse>(model, setCookieCallback: setCookieCallback);
            Assert.That(createResponse, Is.Not.Null);
            
            var risks = await qualificationClient.GetUnderwritingRisksAsync<IEnumerable<RiskResponse>>();
            var riskId = risks.Single().Id;

            //Call to GetContactDetails below simulates opening up save gate which does a fetch of risk to pre-fill details
            await saveClient.GetContactDetails<SaveCustomerParam>(riskId);
            await saveClient.SaveDetailsAsync<SaveCustomerResponse>(riskId, new SaveCustomerRequest()
            {
                EmailAddress = $"{data.FirstName}.{data.LastName.Replace(" ", "")}@tal.com.au",
                FirstName = data.FirstName,
                ExpressConsent = false,
                LastName = data.LastName,
                PhoneNumber = "0400000000"
            });
            await saveClient.SavePasswordAsync(riskId, new CreateLoginRequest() { Password = "Pa55word!" });

            var plans = await coverClient.GetPlanForRiskAsync<GetPlanResponse>(riskId);

            var updatePlanRequestIP = GetUpdatePlanRequestIp(plans);
            var updatePlanRequestLIFE = GetUpdatePlanRequestLife(plans);
            var coverUpdateResponseIP = await coverClient.UpdatePlanAsync<GetPlanResponse>(riskId, updatePlanRequestIP);
            var coverUpdateResponseLIFE = await coverClient.UpdatePlanAsync<GetPlanResponse>(riskId, updatePlanRequestLIFE);
            
            var validateResponse = await reviewClient.ValidateReviewForRiskAsync(riskId);

            await qualificationClient.GetUnderwritingRisksAsync<IEnumerable<RiskResponse>>();

            var uwHelper = new CustomerRiskUnderwritingHelper(riskId, qualificationClient);
            await uwHelper.QuestionByContainsText("relevant medical, lifestyle, occupation and income information we request").AnswerYesAsync();
            await uwHelper.QuestionByContainsText("Your Duty of Disclosure").AnswerYesAsync();
            await uwHelper.QuestionByContainsText("Gender").AnswerByTextAsync(data.Gender);

            var validateDoB = await policyClient.ValidateGeneralInformationAsync(new GeneralInformationViewModel {DateOfBirth = model.DateOfBirth, Postcode = model.Postcode});
            await uwHelper.QuestionByContainsText("What is your date of birth").AnswerByTextAsync(model.DateOfBirth);
            Assert.That(validateDoB, Is.True);

            //
            await uwHelper.QuestionByContainsText("Are you an Australian citizen or permanent Australian Resident").AnswerYesAsync();
            await uwHelper.QuestionByContainsText("How tall are you").AnswerByTextAsync("185 cm (6 ft 0.8 in)");
            await uwHelper.QuestionByContainsText("What is your weight").AnswerByTextAsync("80 kg (12 st 8 lbs)");
            await uwHelper.QuestionByContainsText("Have you smoked tobacco").AnswerByTextAsync(data.Smoker ? "Yes" : "No");
            if (data.Smoker)
                await uwHelper.QuestionByContainsText("Do you smoke more than 30 cigarettes per day?").AnswerNoAsync();
            
            await uwHelper.QuestionByContainsText("Do you have any other Life").AnswerNoAsync();
            await uwHelper.QuestionByContainsText("In the last 10 years have you received a claim payment").AnswerNoAsync();
            await uwHelper.QuestionByContainsText("In the last 10 years, have any of your life insurance applications been declined or issued with a health loading or exclusion").AnswerNoAsync();

            await uwHelper.QuestionByContainsText("What Industry do you work in?").AnswerByTextAsync("Advertising & Marketing");
            await uwHelper.QuestionByContainsText("What is your current Occupation?").AnswerByTextAsync("Account Executive");

            await uwHelper.QuestionByContainsText("Do you have more than one occupation").AnswerNoAsync();

            var validateIncome = await policyClient.ValidateIncomeAsync(new IncomeViewModel { AnnualIncome = model.AnnualIncome });
            await uwHelper.QuestionByContainsText("What is your current annual income before tax").AnswerByTextAsync(model.AnnualIncome.ToString());
            Assert.That(validateIncome, Is.True);

            await uwHelper.QuestionByContainsText("What percentage of your work duties involves either manual work").AnswerByTextAsync("10% or less manual duties");
            await uwHelper.QuestionByContainsText("Are you currently receiving any Government income support").AnswerNoAsync();
            await uwHelper.QuestionByContainsText("Which of the following best describes your current employment status").AnswerByTextAsync("I'm an employee of a company");
            await uwHelper.QuestionByContainsText("Have you been in your current employment").AnswerYesAsync();
            await uwHelper.QuestionByContainsText("In the next 12 months do you have plans to change your occupation").AnswerNoAsync();

            //await uwHelper.QuestionByContainsText("Have you had any periods of unemployment in the last 12 months").AnswerNoAsync();
            
            await uwHelper.QuestionByContainsText("Are you currently off work or working on a restricted basis due to illness or injury").AnswerNoAsync();
            await uwHelper.QuestionByContainsText("What is the average number of hours that you work per week").AnswerByTextAsync("Between 20 and 70 hours");
            await uwHelper.QuestionByContainsText("Do you play or intend to engage in any sport or recreational activities").AnswerNoAsync();
            await uwHelper.QuestionByContainsText("do you have definite plans to travel or live overseas, or are you required to travel overseas on a regular basis for work").AnswerNoAsync();

            foreach (var medicalQuestion in uwHelper.QuestionsMatchingIdRoot("Medical and Lifestyle?"))
            {
                var answers = medicalQuestion.Answers;
                if (answers.Contains("No"))
                    await medicalQuestion.AnswerNoAsync();
                else
                    await medicalQuestion.AnswerByTextAsync(answers.First());
            }
           
            await uwHelper.QuestionByContainsText("Other than what you have already answered, in the last 5 years have you").AnswerNoAsync();
            await uwHelper.QuestionByContainsText("Other than what you have already answered:").AnswerNoAsync();

            // continue page
            var validateRiskResponse = await qualificationClient.ValidateRisk(riskId);
            var chatAvailable = await chatClient.GetAvailabilityAsync();
            await qualificationClient.GetUnderwritingRisksAsync<IEnumerable<RiskResponse>>(pause: false);
            var review = await reviewClient.GetReviewForRiskAsync(riskId, pause: false);
            if (review.ReviewWorkflowStatus != ReviewWorkflowStatus.Accept)
            {
                await uwHelper.GetLatestUnderwritingForRiskAsync();
                var unanswered = uwHelper.GetUnansweredQuestions();
                foreach(var q in unanswered)
                    _performanceTestTool.LogWarning($"Found unanswered question '{q.Id}' '{q.Text}'");
            }
            Assert.That(review.ReviewWorkflowStatus, Is.EqualTo(ReviewWorkflowStatus.Accept));

            // purchase cover
            var purchaseOptions = await purchaseClient.GetPurchaseOptionsAsync();
            await chatClient.GetAvailabilityAsync();
            var beneficiaryRelationships = await referenceClient.GetBeneficiaryRelationships(pause: false);
            Assert.That(purchaseOptions, Is.Not.Null);
            Assert.That(beneficiaryRelationships, Is.Not.Null);

            // submit
            var purchaseViewModel = GetPurchaseRequest(data, riskId);
            var purchaseResponse = await purchaseClient.PostPurchaseDetailsAsync<RedirectToResponse>(riskId, purchaseViewModel);
            await chatClient.GetAvailabilityAsync();

            Assert.That(purchaseResponse, Is.Not.Null);

            _performanceTestTool.EndTransaction(workflowCreateLife);
        }

        private static PurchaseRequest GetPurchaseRequest(ScenarioData data, int riskId)
        {
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
                        Surname = "Barnes",
                        Suburb = "Sunnyville",
                        State = "VIC",
                        Postcode = "4444",
                        Title = "Mr",
                        PhoneNumber = "0400000000",
                        EmailAddress = "tal.test@tal.com.com"
                    }
                },
                PaymentOptions = new PaymentOptionsViewModel
                {
                    IsDirectDebitSelected = true,
                    DirectDebitPayment = new DirectDebitPaymentViewModel
                    {
                        AccountName = $"{data.FirstName} {data.LastName}",
                        BSBNumber = "062000",
                        AccountNumber = "598266548"
                    }
                },
                PersonalDetails = new PersonalDetailsViewModel
                {
                    DateOfBirth = data.DateOfBirth,
                    FirstName = data.FirstName,
                    LastName = data.LastName,
                    EmailAddress = $"{data.FirstName}.{data.LastName.Replace(" ", "")}@tal.com.au",
                    MobileNumber = "0400000000",
                    HomeNumber = "0200000000",
                    ResidentialAddress = data.StreetAddress,
                    Postcode = data.PostCode,
                    State = data.State,
                    Suburb = data.Town,
                    Title = data.Title
                },
                DisclosureNotes = new PolicyNoteResultViewModel()
                {
                    Id = null,
                    NoteText = "Test Note"
                },
                NominateLpr = false,
                DeclarationAgree = true,
                DncSelection = true,
                RiskId = riskId
            };
            return purchaseViewModel;
        }

        private static UpdatePlanRequest GetUpdatePlanRequestLife(GetPlanResponse plans)
        {
            var updatePlanRequestLIFE = new UpdatePlanRequest()
            {
                PlanCode = "DTH",
                PlanId = plans.Plans.Single(p => p.PlanCode == "DTH").PlanId,
                SelectedCoverAmount = 250000,
                IsSelected = true,
                PremiumType = "Stepped",
                SelectedCovers = new List<string> {"DTHAC", "DTHIC"},
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
                    new UpdatePlanVariableRequest {Code = "linkedToCpi", SelectedValue = true}
                },
                SelectedPlans = new List<string> {"DTH", "IP"}
            };
            return updatePlanRequestLIFE;
        }

        private static UpdatePlanRequest GetUpdatePlanRequestIp(GetPlanResponse plans)
        {
            var updatePlanRequestIP = new UpdatePlanRequest()
            {
                PlanCode = "IP",
                PlanId = plans.Plans.Single(p => p.PlanCode == "IP").PlanId,
                IsSelected = true,
                PremiumType = "Stepped",
                SelectedCoverAmount = 2345,
                PremiumHoliday = true,
                SelectedPlans = new List<string> {"DTH", "IP"},
                SelectedCovers = new List<string> {"IPSAC", "IPSIC"},
                Options = new List<OptionConfigurationRequest>
                {
                    new OptionConfigurationRequest
                    {
                        Code = "IC",
                        IsSelected = false
                    },
                    new OptionConfigurationRequest
                    {
                        Code = "DOA",
                        IsSelected = false
                    }
                },
                Riders = new List<PlanRiderRequest>(),
                Variables = new List<UpdatePlanVariableRequest>
                {
                    new UpdatePlanVariableRequest {Code = "linkedToCpi", SelectedValue = true},
                    new UpdatePlanVariableRequest {Code = "waitingPeriod", SelectedValue = 13},
                    new UpdatePlanVariableRequest {Code = "benefitPeriod", SelectedValue = 1},
                },
            };
            return updatePlanRequestIP;
        }

        private static BasicInfoViewModel GetBasicInfoViewModel(ScenarioData data, SearchResult searchCodes)
        {
            var model = new BasicInfoViewModel
            {
                AnnualIncome = data.AnnualIncome,
                DateOfBirth = data.DateOfBirth,
                Gender = data.Gender[0],
                IsSmoker = data.Smoker,
                IndustryCode = searchCodes.ParentResponseId,
                OccupationCode = searchCodes.ResponseId,
                Postcode = data.PostCode
            };
            return model;
        }

        private T GetClient<T>() where T: CustomerPortalApiClient, new()
        {
            var client = new T();

            client.AssignDependencies(new WebApiClient(_webServer, new Json()), _performanceTestTool);

            return client;
        }
    }

    public class ScenarioData
    {
        public string FirstName { get; }
        public string LastName { get; }
        public string Title { get; }
        public long AnnualIncome { get; }
        public string DateOfBirth { get; }
        public string Gender { get; }
        public bool Smoker { get; }
        public IReadOnlyList<string> Search { get; }
        public string StreetAddress { get; }
        public string PostCode { get; }
        public string State { get; }
        public string Town { get; }

        public ScenarioData(string firstName, string lastName, string title, long annualIncome, string dateOfBirth, string gender, bool smoker, IReadOnlyList<string> search, string streetAddress, string postCode, string state, string town)
        {
            FirstName = firstName;
            LastName = lastName;
            Title = title;
            AnnualIncome = annualIncome;
            DateOfBirth = dateOfBirth;
            Gender = gender;
            Smoker = smoker;
            Search = search;
            StreetAddress = streetAddress;
            PostCode = postCode;
            State = state;
            Town = town;
        }
    }
}
