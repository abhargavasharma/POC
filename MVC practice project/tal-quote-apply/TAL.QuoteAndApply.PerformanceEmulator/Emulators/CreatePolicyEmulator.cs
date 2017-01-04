using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TAL.Performance.Infrastructure.Core;
using TAL.Performance.Infrastructure.Core.Data;
using TAL.Performance.Infrastructure.Core.WebServers;
using TAL.QuoteAndApply.DataLayer.Encrpytion;
using TAL.QuoteAndApply.DataLayer.Factory;
using TAL.QuoteAndApply.DataLayer.Repository;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.Infrastructure.Cache;
using TAL.QuoteAndApply.Infrastructure.Http;
using TAL.QuoteAndApply.PerformanceEmulator.Utility;
using TAL.QuoteAndApply.Policy.Configuration;
using TAL.QuoteAndApply.Policy.Data;
using TAL.QuoteAndApply.Policy.Events;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Rules.Risk;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.SalesPortal.Web.Extensions;
using TAL.QuoteAndApply.SalesPortal.Web.IntegrationTests.Clients;
using TAL.QuoteAndApply.SalesPortal.Web.Models;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;
using TAL.QuoteAndApply.ServiceLayer.Payment.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Search.AgentDashboard.Models;
using TAL.QuoteAndApply.Tests.Shared.Helpers;
using TAL.QuoteAndApply.Tests.Shared.Mocks;
using TAL.QuoteAndApply.Underwriting.Configuration;
using TAL.QuoteAndApply.Underwriting.Models.Talus;
using TAL.QuoteAndApply.Underwriting.Service;
using TAL.QuoteAndApply.Web.Shared.HttpResults;
using Task = System.Threading.Tasks.Task;

namespace TAL.QuoteAndApply.PerformanceEmulator.Emulators
{
    public class CreatePolicyEmulator : BaseEmulator
    {
        private readonly IRiskDtoRepository _riskRepo;

        private readonly HttpClientService _httpClient;
        private readonly UnderwritingWebServiceUrlProvider _underwritingWebServiceUrlProvider;

        public CreatePolicyEmulator(IWebServer webserver, IPerformanceTestTool performanceTestTool,
            IPolicyConfigurationProvider policyConfigurationProvider, IUnderwritingConfigurationProvider underwritingConfigurationProvider)
            : base(webserver, performanceTestTool)
        {
            _riskRepo = GetRiskRepository(policyConfigurationProvider);
            _httpClient = new HttpClientService(new HttpResponseMessageSerializer(), new MimeTypeProvider(), new HttpRequestMessageSerializer());

            _underwritingWebServiceUrlProvider =
                new UnderwritingWebServiceUrlProvider(underwritingConfigurationProvider);
        }

        public async Task CreateLifePolicyWithAllCoversAsync(TestPerson testApplicant, long income, bool smoker, TestPerson testBeneficiary)
        {
            const string workflowCreateLife = "Workflow: Create life policy with all covers in Sales portal";
            PerformanceTestTool.StartTransaction(workflowCreateLife);

            PerformanceTestTool.LogInformation(string.Join("|", 
                testApplicant.Title, testApplicant.FirstName, testApplicant.MiddleName, testApplicant.LastName, testApplicant.DateOfBirth.ToString("dd/MM/yyyy"),
                testApplicant.Gender, income, smoker ? "Smoker" : "Nonsmoker", 
                testApplicant.Address.StreetAddress, testApplicant.Address.Town, testApplicant.Address.State, testApplicant.Address.PostCode));

            var loginClient = GetSalesPortalLoginClient();
            var policyClient = GetSalesPortalPolicyClient();
            var riskClient = GetSalesPortalRiskClient();
            var underwritingClient = GetSalesPortalUnderwritingClient();
            var paymentClient = GetSalesPortalPaymentClient();
            var planClient = GetSalesPortalPlanClient();
            var referenceClient = GetSalesPortalReferenceClient();
            var beneficiaryClient = GetSalesPortalBeneficiaryClient();
            var dashboardClient = GetSalesPortalDasbhoardClient();
            var searchClient = GetSalesPortalSearchClient();
            const string sectionLogin = "Section: Login";
            //login
            PerformanceTestTool.StartTransaction(sectionLogin);
            var loginRequest = new LoginRequest
            {
                UserName = "TalConsumerAgent_QA",
                Password = "Transform2001!",
                UseWindowsAuth = false
            };

            await loginClient.AttemptLoginAsync<RedirectToResponse>(loginRequest, false);
            PerformanceTestTool.EndTransaction(sectionLogin);

            //dashboard
            PerformanceTestTool.StartTransaction("Section: Dashboard");

            var agentDashboardRequest = new AgentDashboardRequest()
            {
                ClosedCantContact = false,
                ClosedNoSale = false,
                ClosedTriage = false,
                ClosedSale = false,
                InProgressPreUw = false,
                InProgressCantContact = false,
                InProgressUwReferral = false,
                InProgressRecommendation = false,
                EndDate = DateTime.Now,
                StartDate = DateTime.Now.AddMonths(-1),
                PageNumber = 1
            };

            await dashboardClient.GetQuotesAsync<AgentDashboardDetailsResponse>(agentDashboardRequest);

            PerformanceTestTool.EndTransaction("Section: Dashboard");

            //search for client
            PerformanceTestTool.StartTransaction("Section: Search for clients");

            var searchClientsRequest = new SearchClientsRequest
            {
                SearchOnParty = true,
                FirstName = testApplicant.FirstName,
                Surname = testApplicant.LastName
            };

            await searchClient.SearchClientsAsync<SearchClientsResponse>(searchClientsRequest);

            PerformanceTestTool.EndTransaction("Section: Search for clients");

            const string sectionCreateNewClient = "Section: Create New Client";
            PerformanceTestTool.StartTransaction(sectionCreateNewClient);
            await policyClient.InitAsync();

            var createClientRequest = new CreateClientRequest
            {
                PolicyOwner = new ClientRequest
                {
                    RatingFactors = new RatingFactorsRequest
                    {
                        DateOfBirth = testApplicant.DateOfBirth.ToFriendlyString(),
                        Gender = GetGenderChar(testApplicant.Gender),
                        AustralianResident = true,
                        SmokerStatus = smoker ? "Yes" : "No",
                        Income = income,
                        IndustryCode = "27w",
                        OccupationCode = "2d1",
                        OccupationTitle = "Barrister/Solicitor/Lawyer/Attorney"
                    }
                }
            };

            await searchClient.SearchOccupationsAsync("Ba", 45);
            await searchClient.SearchOccupationsAsync("Bar", 45);

            var policyRedirectAction = await policyClient.CreateAsync<RedirectToResponse>(createClientRequest, true);
            PerformanceTestTool.EndTransaction(sectionCreateNewClient);

            var parts = policyRedirectAction.RedirectTo.Split('/');
            var quoteRef = parts.Last();

            if (quoteRef.Contains("?"))
            {
                parts = quoteRef.Split('?');
                quoteRef = parts.First();
            }

            const string sectionEditQuoteLoad = "Section: Edit Quote Load";
            PerformanceTestTool.StartTransaction(sectionEditQuoteLoad);

            var policySummary = await policyClient.EditAsync(quoteRef, QuoteEditSource.Created);
            var riskId = policySummary.Risks.First().RiskId;

            //All things that happen when moving to the edit page
            var premiumFrequency = await policyClient.GetPolicyPremiumFrequencyAsync(quoteRef);
            var premium = await riskClient.GetPremiumAsync(quoteRef, riskId);
            var ratingFactors = await riskClient.RatingFactorsAsync(quoteRef, riskId);
            var personalDetails = await policyClient.GetOwnerDetailsAsync(quoteRef);
            var underwritingStatus = await underwritingClient.GetUnderwritingStatusAsync(quoteRef, riskId);
            var paymentOptions = await paymentClient.GetAvailablePaymentOptionsAsync(quoteRef, riskId);
            var beneficiaryRelationShips = await referenceClient.GetBeneficiaryRelationshipsAsync();
            var beneficiaries = await beneficiaryClient.GetBeneficiariesAsync(quoteRef, riskId);
            var beneficiaryOptions = await beneficiaryClient.GetOptionsAsync(quoteRef, riskId);

            var getPlansAndCovers = await planClient.GetPlansAndCoversAsync(quoteRef, riskId);

            var lifePlan = getPlansAndCovers.Plans.First(x => x.Code == "DTH");
            lifePlan.Selected = true;

            var planUpdateRequest = new PlanUpdateRequest
            {
                CurrentActivePlan = PlanConfigurationRequestFrom(lifePlan),
                QuoteReferenceNumber = quoteRef,
                RiskId = riskId.ToString(),
                SelectedPlanCodes = getPlansAndCovers.Plans.Where(x => x.Selected).Select(x => x.Code)
            };

            var updatedPlans = await planClient.UpdateAsync<PlansUpdateResponse>(quoteRef, riskId, planUpdateRequest);

            //page load now done, start filling out sections
            PerformanceTestTool.EndTransaction(sectionEditQuoteLoad);

            await policyClient.UpdatePolicyProgressAsync<PolicyProgressViewModel>(quoteRef, new PolicyProgressViewModel { Progress = PolicyProgress.InProgressPreUw.ToString() });

            //move to personal details
            const string sectionPersonalDetails = "Section: Personal Details";
            PerformanceTestTool.StartTransaction("Personal Details Section");
            await CompletePersonalDetailsSectionAsync(quoteRef, testApplicant, policyClient);
            PerformanceTestTool.EndTransaction("Personal Details Section");

            //move to rating factors
            const string sectionRatingFactorsSection = "Section: Rating Factors";
            PerformanceTestTool.StartTransaction(sectionRatingFactorsSection);
            await CompleteRatingFactorsSectionAsync(quoteRef, riskId, riskClient, planClient, underwritingClient);
            PerformanceTestTool.EndTransaction(sectionRatingFactorsSection);

            //move to beneficiaries
            const string sectionBeneficiariesSection = "Section: Beneficiaries";
            PerformanceTestTool.StartTransaction(sectionBeneficiariesSection);
            await CompleteBeneficiarySectionAsync(quoteRef, riskId, beneficiaryClient, referenceClient, testBeneficiary);
            PerformanceTestTool.EndTransaction(sectionBeneficiariesSection);

            //move to payment details section
            const string sectionPayment = "Section: Payment";
            PerformanceTestTool.StartTransaction(sectionPayment);
            //todo: should we radmonise between direct debit and credit card?
            await CompletePaymentOptionsSectionAsync(quoteRef, riskId, testApplicant, paymentClient);
            PerformanceTestTool.EndTransaction(sectionPayment);

            //move to out underwriting
            const string sectionUnderwriting = "Section: Underwriting";
            PerformanceTestTool.StartTransaction(sectionUnderwriting);
            //todo: finish the rest of the questions
            await CompleteUnderwritingSectionAsync(quoteRef, riskId, smoker, testApplicant.Gender, planClient, riskClient, underwritingClient);
            PerformanceTestTool.EndTransaction(sectionUnderwriting);

            //move to cover selection section
            const string sectionCoverSelection = "Section: Cover Selection";
            PerformanceTestTool.StartTransaction(sectionCoverSelection);
            await CompleteCoverSelectionSectionAsync(quoteRef, riskId, planClient, riskClient, underwritingClient);
            PerformanceTestTool.EndTransaction(sectionCoverSelection);

            await policyClient.UpdatePolicyProgressAsync<PolicyProgressViewModel>(quoteRef, new PolicyProgressViewModel { Progress = PolicyProgress.ClosedSale.ToString() });

            //todo: set inforce

            PerformanceTestTool.EndTransaction(workflowCreateLife);
        }

        private async Task CompleteUnderwritingSectionAsync(string quoteRef, int riskId, bool smoker, TestPerson.GenderType gender, PlanClient planClient, RiskClient riskClient, UnderwritingClient underwritingClient)
        {
            var underwritingStatus = await underwritingClient.GetUnderwritingStatusAsync(quoteRef, riskId);
            var talusUiUrl = await underwritingClient.GetTalusUiUrlAsync(quoteRef, riskId);

            //need to get to get the initial concurrency token and interviewId
            var interviewId = _riskRepo.GetRisk(riskId, true).InterviewId;
            var token = await GetCurrentInterviewConcurrencyTokenAsync(interviewId);

            //answer in talus
            token = await AnswerQuestionAsync(quoteRef, riskId, interviewId, token, "Privacy Statement?11j", "2", "Yes", underwritingClient); // To give you a quote and find out if we can offer you this product, you will need to provide the relevant medical, lifestyle, occupation and income information we request. Are you happy and able to do this?
            token = await AnswerQuestionAsync(quoteRef, riskId, interviewId, token, "Duty of Disclosure Statement?e3", "2", "Yes", underwritingClient); // <div class="question-align-left">                 <h6>Your Duty of Disclosure</h6>                 <p>You have a duty to tell us anything that you know, or could reasonably be expected to know, that may affect our decision to provide insurance and on what terms. You have this duty until we agree to provide the insurance. Read your full duty <a href="https://qa1-talcustomer.delivery.lan/" target="_blank">here</a></p>                 <p>Any necessary medical, financial and employment records will be obtained at the time of a claim to confirm the information given in this application is honest and complete.</p>                 <p>If you do not tell us anything you are required to, and we would not have provided the insurance if you had told us, we may reduce or refuse to pay a claim or we may cancel the policy.</p>                 <p><strong>Do you understand this?</strong></p>             </div>
            //token = await AnswerQuestionAsync(quoteRef, riskId, interviewId, token, "Personal Details?Gender", "h2", "Female", underwritingClient); // Gender
            //token = await AnswerQuestionAsync(quoteRef, riskId, interviewId, token, "Personal Details?wb", "*", "12/06/1991", underwritingClient); // What is your date of birth?
            token = await AnswerQuestionAsync(quoteRef, riskId, interviewId, token, "Personal Details?e7", "2", "Yes", underwritingClient); // Are you an Australian Citizen or permanent Australian Resident?
            token = await AnswerQuestionAsync(quoteRef, riskId, interviewId, token, "Personal Details?19e", "jx", "180 cm (5 ft 10.9 in)", underwritingClient); // How tall are you?
            token = await AnswerQuestionAsync(quoteRef, riskId, interviewId, token, "Personal Details?eb", "ol", "75 kg (11 st 11 lbs)", underwritingClient); // What is your weight?

            if (smoker)
            {
                // smoker yes
                token = await AnswerQuestionAsync(quoteRef, riskId, interviewId, token, "Personal Details?e8", "2", "Yes", underwritingClient); // Have you smoked tobacco (e.g. in a cigarette, cigar or pipe) or used e-cigarettes in the last 12 months?
                token = await AnswerQuestionAsync(quoteRef, riskId, interviewId, token, "Personal Details?e8?2?e9", "3", "No", underwritingClient); // Do you smoke more than 30 cigarettes per day?
            }
            else
            {
                // smoker no
                token = await AnswerQuestionAsync(quoteRef, riskId, interviewId, token, "Personal Details?e8", "3", "No", underwritingClient); // Have you smoked tobacco (e.g. in a cigarette, cigar or pipe) or used e-cigarettes in the last 12 months?
            }

            token = await AnswerQuestionAsync(quoteRef, riskId, interviewId, token, "Personal Details?181", "3", "No", underwritingClient); // Do you have any other Life Insurance, Total Permanent Disability Insurance (TPD), Recovery Insurance (also known as Trauma or Critical Illness) or Income Protection that you will keep in addition to this application?
            token = await AnswerQuestionAsync(quoteRef, riskId, interviewId, token, "Personal Details?vg", "3", "No", underwritingClient); // In the last 10 years have you received a claim payment or are you currently making a claim for Workers Compensation or other insurance payments for an accident, sickness or disability?
            token = await AnswerQuestionAsync(quoteRef, riskId, interviewId, token, "Personal Details?182", "3", "No", underwritingClient); // In the last 10 years, have any of your life insurance applications been declined or issued with a health loading or exclusion?
            token = await AnswerQuestionAsync(quoteRef, riskId, interviewId, token, "Employment?1a9", "27l", "Advertising & Marketing", underwritingClient); // What Industry do you work in?
            token = await AnswerQuestionAsync(quoteRef, riskId, interviewId, token, "Employment?1a9?27l?1aa", "229", "Account Executive", underwritingClient); // What is your current Occupation?
            token = await AnswerQuestionAsync(quoteRef, riskId, interviewId, token, "Employment?18m", "3", "No", underwritingClient); // Do you have more than one occupation?
            //token = await AnswerQuestionAsync(quoteRef, riskId, interviewId, token, "Employment?g3", "*", "300000", underwritingClient); // What is your current annual income before tax?
            token = await AnswerQuestionAsync(quoteRef, riskId, interviewId, token, "Employment?19w", "2tx", "10% or less manual duties", underwritingClient); // What percentage of your work duties involves either manual work (e.g. using tools, operating machinery) or working outdoors?
            token = await AnswerQuestionAsync(quoteRef, riskId, interviewId, token, "Employment?gy", "3", "No", underwritingClient); // Are you currently receiving any Government income support (not including family benefits)?
            token = await AnswerQuestionAsync(quoteRef, riskId, interviewId, token, "Employment?wh", "2k3", "I'm an employee of a company", underwritingClient); // Which of the following best describes your current employment status?
            token = await AnswerQuestionAsync(quoteRef, riskId, interviewId, token, "Employment?183", "2", "Yes", underwritingClient); // Have you been in your current employment  for 12 months or more?
            token = await AnswerQuestionAsync(quoteRef, riskId, interviewId, token, "Employment?184", "3", "No", underwritingClient); // In the next 12 months do you have plans to change your occupation, reduce your working hours or take more than 3 months off work?
            token = await AnswerQuestionAsync(quoteRef, riskId, interviewId, token, "Employment?185", "3", "No", underwritingClient); // Are you currently off work or working on a restricted basis due to illness or injury?
            token = await AnswerQuestionAsync(quoteRef, riskId, interviewId, token, "Employment?sr", "1z7", "Between 20 and 70 hours", underwritingClient); // What is the average number of hours that you work per week?
            token = await AnswerQuestionAsync(quoteRef, riskId, interviewId, token, "Sports and Travel?uj", "3", "No", underwritingClient); // Do you play or intend to engage in any sport or recreational activities?
            token = await AnswerQuestionAsync(quoteRef, riskId, interviewId, token, "Sports and Travel?18t", "3", "No", underwritingClient); // In the next 12 months, do you have definite plans to travel or live overseas, or are you required to travel overseas on a regular basis for work?
            token = await AnswerQuestionAsync(quoteRef, riskId, interviewId, token, "Medical and Lifestyle?hd", "3", "No", underwritingClient); // Have you ever had or received medical advice or treatment for any heart condition, including high blood pressure, high cholesterol, a heart murmur, chest pain or palpitations?
            token = await AnswerQuestionAsync(quoteRef, riskId, interviewId, token, "Medical and Lifestyle?mh", "3", "No", underwritingClient); // <p>Have any of your immediate family (mother, father, brother or sister) been diagnosed with any of the following before the age of 65:</p> <ul> <li>Cancer</li> <li>Heart Disease</li> <li>Heart Attack</li> <li>Polycystic Kidney Disease</li> <li>Diabetes</li> <li>Huntingtons disease, multiple sclerosis, motor neurone disease, Parkinsons disease or any other hereditary disorder</li> </ul>
            token = await AnswerQuestionAsync(quoteRef, riskId, interviewId, token, "Medical and Lifestyle?fd", "3", "No", underwritingClient); // Have you ever had or received medical advice or treatment for any cancer, tumour, lump, skin cancer, cyst, polyp or growth?
            token = await AnswerQuestionAsync(quoteRef, riskId, interviewId, token, "Medical and Lifestyle?j2", "3", "No", underwritingClient); // Have you ever had or received medical advice or treatment for diabetes, raised blood sugar levels or hepatitis B or C?
            token = await AnswerQuestionAsync(quoteRef, riskId, interviewId, token, "Medical and Lifestyle?k8", "3", "No", underwritingClient); // Have you ever had or received medical advice or treatment for epilepsy or seizures, multiple sclerosis, paralysis, embolism, stroke, tremor, chronic headaches, chronic fatigue, or any symptoms of a brain, neurological or circulatory system condition?
            token = await AnswerQuestionAsync(quoteRef, riskId, interviewId, token, "Medical and Lifestyle?191", "3", "No", underwritingClient); // Have you ever had or received medical advice or treatment for rheumatoid arthritis, psoriatic arthritis, fibromyalgia, osteoporosis or other bone disease?
            token = await AnswerQuestionAsync(quoteRef, riskId, interviewId, token, "Medical and Lifestyle?16b", "3", "No", underwritingClient); // Have you ever tested positive for HIV/AIDS or are you awaiting results of an HIV test?
            token = await AnswerQuestionAsync(quoteRef, riskId, interviewId, token, "Medical and Lifestyle?190", "3", "No", underwritingClient); // In the last ten years, have you engaged in any activity reasonably expected to increase the risk of exposure to the HIV/AIDS virus?
            token = await AnswerQuestionAsync(quoteRef, riskId, interviewId, token, "Medical and Lifestyle?nu", "3", "No", underwritingClient); // In the last ten years, have you had or received medical advice or treatment for any gall bladder, hernia, liver, bowel or stomach condition?
            token = await AnswerQuestionAsync(quoteRef, riskId, interviewId, token, "Medical and Lifestyle?nv", "3", "No", underwritingClient); // In the last ten years, have you had or received medical advice or treatment for asthma, sleep apnoea or any respiratory or lung condition?
            token = await AnswerQuestionAsync(quoteRef, riskId, interviewId, token, "Medical and Lifestyle?fj", "3", "No", underwritingClient); // In the last ten years have you had or received medical advice or treatment for depression, anxiety, panic attacks, stress, bipolar disorder, post-natal depression, post-traumatic stress disorder or other mental health condition?
            token = await AnswerQuestionAsync(quoteRef, riskId, interviewId, token, "Medical and Lifestyle?192", "3", "No", underwritingClient); // In the last ten years, have you had or received medical advice or treatment for whiplash, sciatica, scoliosis, other back or neck pain, strain, surgery, injury or disorder?
            token = await AnswerQuestionAsync(quoteRef, riskId, interviewId, token, "Medical and Lifestyle?193", "3", "No", underwritingClient); // In the last ten years, have you had or received medical advice or treatment for any joint, muscle, cartilage, tendon or bone pain, fracture, surgery, injury or disorder, osteoarthritis, and gout?
            token = await AnswerQuestionAsync(quoteRef, riskId, interviewId, token, "Medical and Lifestyle?nx", "3", "No", underwritingClient); // In the last ten years, have you had or received medical advice or treatment for any disorder of your eyes, ears, or any skin condition?
            token = await AnswerQuestionAsync(quoteRef, riskId, interviewId, token, "Medical and Lifestyle?nz", "3", "No", underwritingClient); // In the last ten years, have you had or received medical advice or treatment for any blood disorder, thyroid disorder, anaemia, or lupus?
            token = await AnswerQuestionAsync(quoteRef, riskId, interviewId, token, "Medical and Lifestyle?ny", "3", "No", underwritingClient); // In the last ten years, have you had or received medical advice or treatment for any kidney, bladder, urinary or prostate condition?
            
            if(gender == TestPerson.GenderType.Female)
                token = await AnswerQuestionAsync(quoteRef, riskId, interviewId, token, "Medical and Lifestyle?o?Female?tj", "3", "No", underwritingClient); // In the last ten years, have you had or received medical advice or treatment for an abnormal breast check or pap smear, or for any other condition of the cervix, ovaries, uterus or endometrium, or are you currently pregnant?

            token = await AnswerQuestionAsync(quoteRef, riskId, interviewId, token, "Medical and Lifestyle?26?y?1bn", "2wi", "0 units", underwritingClient); // How many standard units of alcohol do you drink in an average week?
            token = await AnswerQuestionAsync(quoteRef, riskId, interviewId, token, "Medical and Lifestyle?26?y?1bo", "3", "No", underwritingClient); // In the last ten years have you smoked or taken recreational drugs or any drug other than as medically directed, or received counselling from a health professional for excess alcohol consumption?
            token = await AnswerQuestionAsync(quoteRef, riskId, interviewId, token, "Medical and Lifestyle?26?y?fl", "3", "No", underwritingClient); // <p>Other than what you have already answered, in the last 5 years have you:</p>           <ul>           <li>been admitted to hospital</li>           <li>seen a doctor or other health professional for any other medical condition which has lasted more than 14 days, or</li>           <li>been prescribed medication for more than 14 days?</li>           </ul>
            token = await AnswerQuestionAsync(quoteRef, riskId, interviewId, token, "Medical and Lifestyle?26?y?fn", "3", "No", underwritingClient); // <p>Other than what you have already answered:</p>           <ul>            <li>do you have any other ongoing medical conditions</li>           <li>do you intend seeking or have you been advised to seek medical advice or treatment for any current medical concern, or</li>           <li>are you awaiting the results of any medical tests or investigations? </li>           </ul>
            token = await AnswerQuestionAsync(quoteRef, riskId, interviewId, token, "Medical and Lifestyle?26?y?fl", "3", "No", underwritingClient); // <p>Other than what you have already answered, in the last 5 years have you:</p>           <ul>           <li>been admitted to hospital</li>           <li>seen a doctor or other health professional for any other medical condition which has lasted more than 14 days, or</li>           <li>been prescribed medication for more than 14 days?</li>           </ul>
            token = await AnswerQuestionAsync(quoteRef, riskId, interviewId, token, "Medical and Lifestyle?26?y?fn", "3", "No", underwritingClient); // <p>Other than what you have already answered:</p>           <ul>            <li>do you have any other ongoing medical conditions</li>           <li>do you intend seeking or have you been advised to seek medical advice or treatment for any current medical concern, or</li>           <li>are you awaiting the results of any medical tests or investigations? </li>           </ul>
        }

        private async Task<string> AnswerQuestionAsync(string quoteRef, int riskId, string interviewId, string token, string questionId, string responseId, string responseText, UnderwritingClient underwritingClient)
        {
            var concurrencyToken = await UpdateUnderwritingQuestionDirectlyInTalusAsync(interviewId, token, questionId, responseId, responseText);
            await underwritingClient.QuestionAnsweredAsync(quoteRef, riskId, new UnderwritingQuestionAnswerRequest
            {
                QuestionId = questionId,
                Answers = new List<UnderwritingAnswerRequest>
                {
                    new UnderwritingAnswerRequest {Id = responseId, Text = responseText}
                },
                ConcurrencyToken = concurrencyToken
            });
            return concurrencyToken;
        }

        private async Task CompleteCoverSelectionSectionAsync(string quoteRef, int riskId, PlanClient planClient, RiskClient riskClient, UnderwritingClient underwritingClient)
        {
            var getPlansAndCovers = await planClient.GetPlansAndCoversAsync(quoteRef, riskId);

            var lifePlan = getPlansAndCovers.Plans.First(x => x.Code == "DTH");

            var planUpdateRequest = new PlanUpdateRequest
            {
                CurrentActivePlan = PlanConfigurationRequestFrom(lifePlan),
                QuoteReferenceNumber = quoteRef,
                RiskId = riskId.ToString(),
                SelectedPlanCodes = getPlansAndCovers.Plans.Where(x => x.Selected).Select(x => x.Code)
            };

            await planClient.UpdateAsync<PlansUpdateResponse>(quoteRef, riskId, planUpdateRequest);

            var underwritingStatus = await underwritingClient.GetUnderwritingStatusAsync(quoteRef, riskId);

            //cover ammount
            planUpdateRequest.CurrentActivePlan.CoverAmount = 240000;
            await planClient.UpdateAsync<PlansUpdateResponse>(quoteRef, riskId, planUpdateRequest);
            underwritingStatus = await underwritingClient.GetUnderwritingStatusAsync(quoteRef, riskId);
            var premium = await riskClient.GetPremiumAsync(quoteRef, riskId);

            //turn on accident
            planUpdateRequest.CurrentActivePlan.SelectedCoverCodes = new[] { "DTHAC" };
            await planClient.UpdateAsync<PlansUpdateResponse>(quoteRef, riskId, planUpdateRequest);
            underwritingStatus = await underwritingClient.GetUnderwritingStatusAsync(quoteRef, riskId);
            premium = await riskClient.GetPremiumAsync(quoteRef, riskId);

            //turn on illness
            planUpdateRequest.CurrentActivePlan.SelectedCoverCodes = new[] { "DTHAC", "DTHIC" };
            await planClient.UpdateAsync<PlansUpdateResponse>(quoteRef, riskId, planUpdateRequest);
            underwritingStatus = await underwritingClient.GetUnderwritingStatusAsync(quoteRef, riskId);
            premium = await riskClient.GetPremiumAsync(quoteRef, riskId);

            //turn on sports
            planUpdateRequest.CurrentActivePlan.SelectedCoverCodes = new[] { "DTHAC", "DTHIC", "DTHASC" };
            await planClient.UpdateAsync<PlansUpdateResponse>(quoteRef, riskId, planUpdateRequest);
            underwritingStatus = await underwritingClient.GetUnderwritingStatusAsync(quoteRef, riskId);
            premium = await riskClient.GetPremiumAsync(quoteRef, riskId);

            //turn on inflation protection
            planUpdateRequest.CurrentActivePlan.LinkedToCpi = true;
            await planClient.UpdateAsync<PlansUpdateResponse>(quoteRef, riskId, planUpdateRequest);
            underwritingStatus = await underwritingClient.GetUnderwritingStatusAsync(quoteRef, riskId);
            premium = await riskClient.GetPremiumAsync(quoteRef, riskId);

            //turn on premium holiday
            planUpdateRequest.CurrentActivePlan.PremiumHoliday = true;
            await planClient.UpdateAsync<PlansUpdateResponse>(quoteRef, riskId, planUpdateRequest);
            underwritingStatus = await underwritingClient.GetUnderwritingStatusAsync(quoteRef, riskId);
            premium = await riskClient.GetPremiumAsync(quoteRef, riskId);
        }

        private static async Task CompletePaymentOptionsSectionAsync(string quoteRef, int riskId, TestPerson testApplicant, PaymentClient paymentClient)
        {
            var paymentOptions = await paymentClient.GetAvailablePaymentOptionsAsync(quoteRef, riskId);

            var paymentUpdate = new DirectDebitPaymentViewModel
            {
                AccountName = $"{testApplicant.FirstName} {testApplicant.LastName}",
                AccountNumber = "598266548",
                BsbNumber = "062000",
                IsValidForInforce = false
            };
            await paymentClient.PayViaDirectDebitAsync<PaymentOptionsParam>(quoteRef, riskId, paymentUpdate);
        }

        private async Task CompleteRatingFactorsSectionAsync(string quoteRef, int riskId, RiskClient riskClient, PlanClient planClient, UnderwritingClient underwritingClient)
        {
            var ratingFactors = await riskClient.RatingFactorsAsync(quoteRef, riskId);

            var ratingFactorsRequest = new RatingFactorsRequest()
            {
                AustralianResident = true,
                DateOfBirth = ratingFactors.RatingFactors.DateOfBirth,
                Gender = ratingFactors.RatingFactors.Gender,
                Income = ratingFactors.RatingFactors.Income,
                OccupationCode = "229",
                IndustryCode = "27l"
            };

            var ratingFactorsResponse = await riskClient.RatingFactorsAsync<RatingFactorsResponse>(quoteRef, riskId, ratingFactorsRequest);

            await OnRefreshEventsAsync(quoteRef, riskId, riskClient, planClient, underwritingClient);

            ratingFactorsRequest = new RatingFactorsRequest()
            {
                AustralianResident = ratingFactorsResponse.RatingFactors.AustralianResident,
                DateOfBirth = ratingFactorsResponse.RatingFactors.DateOfBirth,
                Gender = ratingFactorsResponse.RatingFactors.Gender,
                Income = ratingFactorsResponse.RatingFactors.Income,
                OccupationCode = ratingFactorsResponse.RatingFactors.OccupationCode,
                OccupationTitle = ratingFactorsResponse.RatingFactors.OccupationTitle,
                SmokerStatus = SmokerStatus.Yes.ToString()
            };
            ratingFactorsResponse = await riskClient.RatingFactorsAsync<RatingFactorsResponse>(quoteRef, riskId, ratingFactorsRequest);

            await OnRefreshEventsAsync(quoteRef, riskId, riskClient, planClient, underwritingClient);
        }

        private async Task OnRefreshEventsAsync(string quoteRef, int riskId, RiskClient riskClient, PlanClient planClient, UnderwritingClient underwritingClient)
        {
            var getPlansAndCovers = await planClient.GetPlansAndCoversAsync(quoteRef, riskId);
            var lifePlan = getPlansAndCovers.Plans.First(x => x.Code == "DTH");
            var planUpdateRequest = new PlanUpdateRequest
            {
                CurrentActivePlan = PlanConfigurationRequestFrom(lifePlan),
                QuoteReferenceNumber = quoteRef,
                RiskId = riskId.ToString(),
                SelectedPlanCodes = getPlansAndCovers.Plans.Where(x => x.Selected).Select(x => x.Code)
            };
            var updatedPlans = await planClient.UpdateAsync<PlansUpdateResponse>(quoteRef, riskId, planUpdateRequest);
            var premium = await riskClient.GetPremiumAsync(quoteRef, riskId);
            var underwritingStatus = await underwritingClient.GetUnderwritingStatusAsync(quoteRef, riskId);
        }

        private async Task CompletePersonalDetailsSectionAsync(string quoteRef, TestPerson testApplicant, PolicyClient policyClient)
        {
            var policyOwnerDetailsRequest = new PolicyOwnerDetailsRequest { PartyConsents = new List<string>() };
            var personalDetailsResult = await policyClient.UpdateOwnerDetailsAsync<PolicyOwnerDetailsParam>(quoteRef, policyOwnerDetailsRequest);

            policyOwnerDetailsRequest = new PolicyOwnerDetailsRequest
            {
                Title = testApplicant.Title,
                PartyConsents = personalDetailsResult.PartyConsentsParam.Consents,
            };

            personalDetailsResult = await policyClient.UpdateOwnerDetailsAsync<PolicyOwnerDetailsParam>(quoteRef, policyOwnerDetailsRequest);

            policyOwnerDetailsRequest = new PolicyOwnerDetailsRequest
            {
                Title = personalDetailsResult.Title,
                PartyConsents = personalDetailsResult.PartyConsentsParam.Consents,
                FirstName = testApplicant.FirstName
            };
            personalDetailsResult = await policyClient.UpdateOwnerDetailsAsync<PolicyOwnerDetailsParam>(quoteRef, policyOwnerDetailsRequest);

            policyOwnerDetailsRequest = new PolicyOwnerDetailsRequest
            {
                Title = personalDetailsResult.Title,
                PartyConsents = personalDetailsResult.PartyConsentsParam.Consents,
                FirstName = personalDetailsResult.FirstName,
                Surname = testApplicant.LastName
            };
            personalDetailsResult = await policyClient.UpdateOwnerDetailsAsync<PolicyOwnerDetailsParam>(quoteRef, policyOwnerDetailsRequest);

            policyOwnerDetailsRequest = new PolicyOwnerDetailsRequest
            {
                Title = personalDetailsResult.Title,
                PartyConsents = personalDetailsResult.PartyConsentsParam.Consents,
                FirstName = personalDetailsResult.FirstName,
                Surname = personalDetailsResult.Surname,
                Address = testApplicant.Address.StreetAddress
            };
            personalDetailsResult = await policyClient.UpdateOwnerDetailsAsync<PolicyOwnerDetailsParam>(quoteRef, policyOwnerDetailsRequest);

            policyOwnerDetailsRequest = new PolicyOwnerDetailsRequest
            {
                Title = personalDetailsResult.Title,
                PartyConsents = personalDetailsResult.PartyConsentsParam.Consents,
                FirstName = personalDetailsResult.FirstName,
                Surname = personalDetailsResult.Surname,
                Address = personalDetailsResult.Address,
                Suburb = testApplicant.Address.Town
            };
            personalDetailsResult = await policyClient.UpdateOwnerDetailsAsync<PolicyOwnerDetailsParam>(quoteRef, policyOwnerDetailsRequest);

            policyOwnerDetailsRequest = new PolicyOwnerDetailsRequest
            {
                Title = personalDetailsResult.Title,
                PartyConsents = personalDetailsResult.PartyConsentsParam.Consents,
                FirstName = personalDetailsResult.FirstName,
                Surname = personalDetailsResult.Surname,
                Address = personalDetailsResult.Address,
                Suburb = personalDetailsResult.Suburb,
                State = testApplicant.Address.State.ToString()
            };
            personalDetailsResult = await policyClient.UpdateOwnerDetailsAsync<PolicyOwnerDetailsParam>(quoteRef, policyOwnerDetailsRequest);

            policyOwnerDetailsRequest = new PolicyOwnerDetailsRequest
            {
                Title = personalDetailsResult.Title,
                PartyConsents = personalDetailsResult.PartyConsentsParam.Consents,
                FirstName = personalDetailsResult.FirstName,
                Surname = personalDetailsResult.Surname,
                Address = personalDetailsResult.Address,
                Suburb = personalDetailsResult.Suburb,
                State = personalDetailsResult.State,
                Postcode = testApplicant.Address.PostCode.ToString()
            };
            personalDetailsResult = await policyClient.UpdateOwnerDetailsAsync<PolicyOwnerDetailsParam>(quoteRef, policyOwnerDetailsRequest);

            policyOwnerDetailsRequest = new PolicyOwnerDetailsRequest
            {
                Title = personalDetailsResult.Title,
                PartyConsents = personalDetailsResult.PartyConsentsParam.Consents,
                FirstName = personalDetailsResult.FirstName,
                Surname = personalDetailsResult.Surname,
                Address = personalDetailsResult.Address,
                Suburb = personalDetailsResult.Suburb,
                State = personalDetailsResult.State,
                Postcode = personalDetailsResult.Postcode,
                MobileNumber = "0412345678",
                HomeNumber = "0812345678"
            };
            personalDetailsResult = await policyClient.UpdateOwnerDetailsAsync<PolicyOwnerDetailsParam>(quoteRef, policyOwnerDetailsRequest);

            policyOwnerDetailsRequest = new PolicyOwnerDetailsRequest
            {
                Title = personalDetailsResult.Title,
                PartyConsents = personalDetailsResult.PartyConsentsParam.Consents,
                FirstName = personalDetailsResult.FirstName,
                Surname = personalDetailsResult.Surname,
                Address = personalDetailsResult.Address,
                Suburb = personalDetailsResult.Suburb,
                State = personalDetailsResult.State,
                Postcode = personalDetailsResult.Postcode,
                MobileNumber = personalDetailsResult.MobileNumber,
                HomeNumber = personalDetailsResult.HomeNumber,
                EmailAddress = $"{personalDetailsResult.FirstName}.{personalDetailsResult.Surname.Replace(" ", "")}@tal.com.au"
            };
            personalDetailsResult = await policyClient.UpdateOwnerDetailsAsync<PolicyOwnerDetailsParam>(quoteRef, policyOwnerDetailsRequest);

            policyOwnerDetailsRequest = new PolicyOwnerDetailsRequest
            {
                Title = personalDetailsResult.Title,
                PartyConsents = personalDetailsResult.PartyConsentsParam.Consents,
                FirstName = personalDetailsResult.FirstName,
                Surname = personalDetailsResult.Surname,
                Address = personalDetailsResult.Address,
                Suburb = personalDetailsResult.Suburb,
                State = personalDetailsResult.State,
                Postcode = personalDetailsResult.Postcode,
                MobileNumber = personalDetailsResult.MobileNumber,
                HomeNumber = personalDetailsResult.HomeNumber,
                EmailAddress = personalDetailsResult.EmailAddress
            };
            await policyClient.UpdateOwnerDetailsAsync<PolicyOwnerDetailsParam>(quoteRef, policyOwnerDetailsRequest);
        }

        private async Task CompleteBeneficiarySectionAsync(string quoteRef, int riskId, BeneficiaryClient beneficiaryClient, ReferenceClient referenceClient, TestPerson testBeneficiary)
        {
            var beneficiaryRelationShips = await referenceClient.GetBeneficiaryRelationshipsAsync();
            var beneficiaries = await beneficiaryClient.GetBeneficiariesAsync(quoteRef, riskId);
            var beneficiaryOptions = beneficiaryClient.GetOptionsAsync(quoteRef, riskId);

            IEnumerable<BeneficiaryDetailsRequest> beneficiaryDetailsRequest = new List<BeneficiaryDetailsRequest>()
            {
                new BeneficiaryDetailsRequest()
                {
                    Share = "100",
                    Title = testBeneficiary.Title
                }
            };

            beneficiaryDetailsRequest = await beneficiaryClient.CreateUpdateBeneficiariesAsync<IEnumerable<BeneficiaryDetailsRequest>>(quoteRef, riskId, beneficiaryDetailsRequest);

            beneficiaryDetailsRequest.First().FirstName = testBeneficiary.FirstName;
            beneficiaryDetailsRequest = await beneficiaryClient.CreateUpdateBeneficiariesAsync<IEnumerable<BeneficiaryDetailsRequest>>(quoteRef, riskId, beneficiaryDetailsRequest);

            beneficiaryDetailsRequest.First().Surname = testBeneficiary.LastName;
            beneficiaryDetailsRequest = await beneficiaryClient.CreateUpdateBeneficiariesAsync<IEnumerable<BeneficiaryDetailsRequest>>(quoteRef, riskId, beneficiaryDetailsRequest);

            beneficiaryDetailsRequest.First().DateOfBirth = testBeneficiary.DateOfBirth.ToFriendlyString();
            beneficiaryDetailsRequest = await beneficiaryClient.CreateUpdateBeneficiariesAsync<IEnumerable<BeneficiaryDetailsRequest>>(quoteRef, riskId, beneficiaryDetailsRequest);

            beneficiaryDetailsRequest.First().BeneficiaryRelationshipId = 1;
            beneficiaryDetailsRequest = await beneficiaryClient.CreateUpdateBeneficiariesAsync<IEnumerable<BeneficiaryDetailsRequest>>(quoteRef, riskId, beneficiaryDetailsRequest);

            beneficiaryDetailsRequest.First().Address = testBeneficiary.Address.StreetAddress;
            beneficiaryDetailsRequest = await beneficiaryClient.CreateUpdateBeneficiariesAsync<IEnumerable<BeneficiaryDetailsRequest>>(quoteRef, riskId, beneficiaryDetailsRequest);

            beneficiaryDetailsRequest.First().Suburb = testBeneficiary.Address.Town;
            beneficiaryDetailsRequest = await beneficiaryClient.CreateUpdateBeneficiariesAsync<IEnumerable<BeneficiaryDetailsRequest>>(quoteRef, riskId, beneficiaryDetailsRequest);

            beneficiaryDetailsRequest.First().State = testBeneficiary.Address.State.ToString();
            beneficiaryDetailsRequest = await beneficiaryClient.CreateUpdateBeneficiariesAsync<IEnumerable<BeneficiaryDetailsRequest>>(quoteRef, riskId, beneficiaryDetailsRequest);

            beneficiaryDetailsRequest.First().Postcode = testBeneficiary.Address.PostCode.ToString();
            beneficiaryDetailsRequest = await beneficiaryClient.CreateUpdateBeneficiariesAsync<IEnumerable<BeneficiaryDetailsRequest>>(quoteRef, riskId, beneficiaryDetailsRequest);

            beneficiaryDetailsRequest.First().PhoneNumber = "0212345678";
            beneficiaryDetailsRequest = await beneficiaryClient.CreateUpdateBeneficiariesAsync<IEnumerable<BeneficiaryDetailsRequest>>(quoteRef, riskId, beneficiaryDetailsRequest);

            beneficiaryDetailsRequest.First().EmailAddress = $"{testBeneficiary.FirstName}.{testBeneficiary.LastName}@tal.com.au";
            await beneficiaryClient.CreateUpdateBeneficiariesAsync<IEnumerable<BeneficiaryDetailsRequest>>(quoteRef, riskId, beneficiaryDetailsRequest);

            //TODO: not working at the moment because client doesnt support GET with not content
            //beneficiaryClient.Validate(quoteRef, riskId);
        }

        private char GetGenderChar(TestPerson.GenderType gender)
        {
            if (gender == TestPerson.GenderType.Male)
            {
                return 'M';
            }

            return 'F';
        }

        private PlanConfigurationRequest PlanConfigurationRequestFrom(PlanDetailResponse planDetail)
        {
            var planConfig = new PlanConfigurationRequest
            {
                CoverAmount = planDetail.CoverAmount,
                LinkedToCpi = planDetail.LinkedToCpi,
                PlanCode = planDetail.Code,
                PlanId = planDetail.PlanId,
                PremiumHoliday = planDetail.PremiumHoliday,
                PremiumType = planDetail.PremiumType,
                Selected = planDetail.Selected,
                SelectedCoverCodes = new string[] { },
            };


            if (planDetail.Options != null)
            {
                planConfig.SelectedOptionCodes =
                    planDetail.Options.Select(
                        x => new OptionConfigurationRequest(x.Code, x.Name, x.Selected.GetValueOrDefault(false))).ToList();
            }
            else
            {
                planConfig.SelectedOptionCodes = new List<OptionConfigurationRequest>();
            }


            if (planDetail.Riders != null)
            {
                planConfig.SelectedRiders = planDetail.Riders.Select(PlanConfigurationRequestFrom).ToList();
            }
            else
            {
                planConfig.SelectedRiders = new List<PlanConfigurationRequest>();
            }


            return planConfig;
        }

        private async Task<string> UpdateUnderwritingQuestionDirectlyInTalusAsync(string interviewId, string concurrencyToken, string questionId, string responseId, string responseText)
        {
            var request = new UpdateInterviewRequest(new AnswerQuestionRequest(questionId, new[] { new AnswerSubmission { ResponseId = responseId, Text = responseText } }, "PerformanceTest.User"));

            var urlString = _underwritingWebServiceUrlProvider.GetInterviewUrl(interviewId);

            var putRequest = new PutOrPostRequest(new Uri(urlString), request)
                .WithEtag(concurrencyToken);

            var updatedUnderwritingInterview = await _httpClient.PutAsync<UpdatedUnderwritingInterview>(putRequest);

            return updatedUnderwritingInterview.ConcurrencyToken;
        }

        private async Task<string> GetCurrentInterviewConcurrencyTokenAsync(string interviewId)
        {
            var urlString = _underwritingWebServiceUrlProvider.GetInterviewUrl(interviewId);

            var underwritingInterview = await _httpClient.GetAsync<UnderwritingInterview>(new GetRequest(new Uri(urlString)));

            return underwritingInterview.ConcurrencyToken;
        }


        private RiskDtoRepository GetRiskRepository(IPolicyConfigurationProvider policyConfigurationProvider)
        {
            var mockCurrentUserProvider = new MockCurrentUserProvider();

            return new RiskDtoRepository(policyConfigurationProvider, mockCurrentUserProvider,
                new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()), new CachingWrapper(new MockHttpProvider()), new RiskChangeSubject());
        }
    }
}
