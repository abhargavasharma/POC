using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TAL.QuoteAndApply.Customer.Web.IntegrationTests.Clients;
using TAL.QuoteAndApply.Customer.Web.Models.Api.Underwriting;
using TAL.QuoteAndApply.ServiceLayer.Underwriting;

namespace TAL.QuoteAndApply.Customer.Web.IntegrationTests
{
    public class CustomerRiskUnderwritingHelper
    {
        private readonly int _riskId;
        private readonly QualificationApiClient _qualificationApiClient;
        private CustomerUnderwritingResponse _underwritingResponse;

        public CustomerRiskUnderwritingHelper(int riskId, QualificationApiClient qualificationApiClient)
        {
            _riskId = riskId;
            _qualificationApiClient = qualificationApiClient;
            Task.Run(() => GetLatestUnderwritingForRiskAsync()).Wait();
        }

        public IReadOnlyList<QuestionResponse> GetUnansweredQuestions()
        {
            return _underwritingResponse.Questions.Where(q => !q.IsAnswered).ToList().AsReadOnly();
        }

        public async Task GetLatestUnderwritingForRiskAsync()
        {
            _underwritingResponse = await _qualificationApiClient.GetUnderwritingForRiskAsync<CustomerUnderwritingResponse>(_riskId);
        }

        public RiskUnderwritingQuestionHelper QuestionById(string questionId)
        {
            var question =
                _underwritingResponse.Questions.Single(
                    q => q.Id.Equals(questionId, StringComparison.OrdinalIgnoreCase));

            return CreateQuestionHelper(question);
        }

        public RiskUnderwritingQuestionHelper QuestionByText(string questionText)
        {
            var question =
                _underwritingResponse.Questions.Single(
                    q => q.Text.Equals(questionText, StringComparison.OrdinalIgnoreCase));

            return CreateQuestionHelper(question);
        }

        public RiskUnderwritingQuestionHelper QuestionByContainsText(string questionText)
        {
            var question = _underwritingResponse
                .Questions
                .Single(q => q.Text.IndexOf(questionText, StringComparison.OrdinalIgnoreCase) >= 0);

            return CreateQuestionHelper(question);
        }

        public IReadOnlyList<RiskUnderwritingQuestionHelper> QuestionsMatchingIdRoot(string questionIdRoot)
        {
            var questions = _underwritingResponse
                .Questions
                .Where(q => q.Id.StartsWith(questionIdRoot))
                .Select(CreateQuestionHelper)
                .ToList();

            return questions.AsReadOnly();
        }

        //TODO: question lookups based on TAL Consumer Underwriting template, maybe make more flexible based on UW template

        public RiskUnderwritingQuestionHelper DisclosureQuestion()
        {
            var disclosureQuestion =
                _underwritingResponse.Questions.Single(q => q.Id == "Duty of Disclosure Statement?e3");
            return CreateQuestionHelper(disclosureQuestion);
        }

        public RiskUnderwritingQuestionHelper PrivacyQuestion()
        {
            var disclosureQuestion =
                _underwritingResponse.Questions.Single(q => q.Id == "Privacy Statement?11j");
            return CreateQuestionHelper(disclosureQuestion);
        }

        public RiskUnderwritingQuestionHelper ResidencyQuestion()
        {
            var residencyQuestion =
                _underwritingResponse.Questions.Single(q => q.Id == "Personal Details?e7");
            return CreateQuestionHelper(residencyQuestion);
        }

        public RiskUnderwritingQuestionHelper SmokerQuestion()
        {
            var question =
                _underwritingResponse.Questions.Single(q => q.Tags.Contains(QuestionTagConstants.SmokerQuestionTag));
            return CreateQuestionHelper(question);
        }

        public async Task MakeUnderwritingGlobalDecline()
        {
            //Non-resident will get declined for everything
            await ResidencyQuestion().AnswerNoAsync();
        }

        public async Task AnswerCleanFullUnderwriting()
        {
            await QuestionById("Privacy Statement?11j").AnswerYesAsync(); //Are you happy and able to do this?
            await DisclosureQuestion().AnswerYesAsync();
            await QuestionByText("Are you an Australian Citizen or permanent Australian Resident?").AnswerYesAsync();
            await QuestionByText("How tall are you?").AnswerByTextAsync("188 cm (6 ft 2 in)");
            await QuestionByText("What is your weight?").AnswerByTextAsync("80 kg (12 st 8 lbs)");
            await QuestionByContainsText("Do you have any other Life Insurance").AnswerNoAsync();
            await QuestionByContainsText("In the last 10 years have you received a claim payment").AnswerNoAsync();
            await QuestionByContainsText("In the last 10 years, have any of your life insurance applications been declined").AnswerNoAsync();
            await QuestionByText("Do you have more than one occupation?").AnswerNoAsync();
            await QuestionByContainsText("What percentage of your work duties involves either manual work").AnswerByTextAsync("10% or less manual duties");
            await QuestionByContainsText("Are you currently receiving any Government income support").AnswerNoAsync();
            await QuestionByText("Which of the following best describes your current employment status?").AnswerByTextAsync("I'm an employee of a company");
            await QuestionByText("Have you been in your current employment  for 12 months or more?").AnswerYesAsync();
            await QuestionByContainsText("In the next 12 months do you have plans to change your occupation").AnswerNoAsync();
            await QuestionByText("Are you currently off work or working on a restricted basis due to illness or injury?").AnswerNoAsync();
            await QuestionByText("What is the average number of hours that you work per week?").AnswerByTextAsync("Between 20 and 70 hours");
            await QuestionByText("Do you play or intend to engage in any sport or recreational activities?").AnswerNoAsync();
            await QuestionByContainsText("In the next 12 months, do you have definite plans to travel or live overseas").AnswerNoAsync();
            await QuestionByContainsText("Have you ever had or received medical advice or treatment for any heart condition").AnswerNoAsync();
            await QuestionByContainsText("Have any of your immediate family (mother, father, brother or sister) been diagnosed").AnswerNoAsync();
            await QuestionByContainsText("Have you ever had or received medical advice or treatment for any cancer, tumour, lump").AnswerNoAsync();
            await QuestionByContainsText("Have you ever had or received medical advice or treatment for diabetes").AnswerNoAsync();
            await QuestionByContainsText("Have you ever had or received medical advice or treatment for epilepsy or seizures").AnswerNoAsync();
            await QuestionByContainsText("Have you ever had or received medical advice or treatment for rheumatoid arthritis").AnswerNoAsync();
            await QuestionByText("Have you ever tested positive for HIV/AIDS or are you awaiting results of an HIV test?").AnswerNoAsync();
            await QuestionByText("In the last ten years, have you engaged in any activity reasonably expected to increase the risk of exposure to the HIV/AIDS virus?").AnswerNoAsync();
            await QuestionByContainsText("In the last ten years, have you had or received medical advice or treatment for any gall bladder").AnswerNoAsync();
            await QuestionByContainsText("In the last ten years, have you had or received medical advice or treatment for asthma").AnswerNoAsync();
            await QuestionByContainsText("In the last ten years have you had or received medical advice or treatment for depression").AnswerNoAsync();
            await QuestionByContainsText("In the last ten years, have you had or received medical advice or treatment for whiplash").AnswerNoAsync();
            await QuestionByContainsText("In the last ten years, have you had or received medical advice or treatment for any joint").AnswerNoAsync();
            await QuestionByContainsText("any disorder of your eyes, ears, or any skin condition?").AnswerNoAsync();
            await QuestionByContainsText("any blood disorder, thyroid disorder, anaemia, or lupus?").AnswerNoAsync();
            await QuestionByContainsText("any kidney, bladder, urinary or prostate condition?").AnswerNoAsync();
            await QuestionByText("How many standard units of alcohol do you drink in an average week?").AnswerByTextAsync("1 to 10 units");
            await QuestionByContainsText("any drug other than as medically directed, or received counselling from a health professional for excess alcohol consumption?").AnswerNoAsync();
            await QuestionByContainsText("seen a doctor or other health professional for any other medical condition which has lasted more than 14 days").AnswerNoAsync();
            await QuestionByContainsText("do you have any other ongoing medical conditions").AnswerNoAsync();
        }


        public async Task AnswerToGetKidneyStonesLoadingAsync()
        {
            await QuestionById("Medical and Lifestyle?nu").AnswerYesAsync(); //"In the last ten years, have you had or received medical advice or treatment for any kidney, bladder, urinary or prostate condition?"
            await QuestionById("Medical and Lifestyle?nu?2?vh").AnswerByTextAsync("Kidney stone"); //"Please specify which condition/s you have?"
            await QuestionByText("Have you had any complications from this or is your kidney function abnormal?").AnswerNoAsync();
            await QuestionByText("Have you had more than one kidney stone?").AnswerYesAsync();
            await QuestionByText("Have stones been present in the last 2 years?").AnswerYesAsync();
        }

        public async Task<RiskUnderwritingQuestionHelper> GetAsthmaExclusionLoadingQuestionAsync()
        {
            await SmokerQuestion().AnswerYesAsync();
            await QuestionByText("Do you smoke more than 30 cigarettes per day?").AnswerNoAsync();

            await QuestionById("Medical and Lifestyle?nv")//"Have you ever had or received medical advice or treatment for any heart condition, including high blood pressure, high cholesterol, a heart murmur, chest pain or palpitations?")
                .AnswerYesAsync();
            await QuestionById("Medical and Lifestyle?nv?2?fi").AnswerByTextAsync("Asthma"); //"Please select which condition/s you have?"
            await QuestionByText("Was your asthma confirmed as childhood asthma only?").AnswerNoAsync();
            await QuestionByText("Have you been hospitalised or required corticosteroids (excluding inhalers) due to your asthma in the last 2 years?")
                .AnswerNoAsync();
            await QuestionByText("In the past 2 years have your daily activities been restricted or have you required more than 5 consecutive days off work due to your asthma?")
                .AnswerNoAsync();
            await QuestionByText("On average, how often do you experience acute episodes of asthma?").AnswerByTextAsync("Up to 1 per month");

            return QuestionById("Medical and Lifestyle?nv?2?fi?1e?DD-52?10d?3?10e?3?1az?3?1b0?2ka?3?y?gd"); //"<p>You can choose to be covered for your Asthma condition at an additional cost. Refer to the help text for more information</p> Which would you prefer?");
        }

        public async Task<RiskUnderwritingQuestionHelper> GetHangGlidingExclusionLoadingChoiceQuestionAsync()
        {
            await QuestionByText("Do you play or intend to engage in any sport or recreational activities?").AnswerYesAsync();
            await QuestionByText("Please select from the following list").AnswerByTextAsync("Hang gliding");
            await QuestionByText("On average how many hours do you participate in hang gliding?").AnswerByTextAsync("25 to 100 hrs pa");
            return QuestionByText("Based on your Hang Gliding activities we can offer you two options for your Life and Recovery Insurance:");
        }

        public async Task AnswerToGetFibromyalgiaOutcome()
        {
            await QuestionByContainsText("treatment for rheumatoid arthritis, psoriatic arthritis, fibromyalgia, osteoporosis or other bone disease?").AnswerYesAsync();
            await QuestionById("Medical and Lifestyle?191?2?1ay").AnswerByTextAsync("Fibromyalgia"); //Please select which condition/s you have?
            await QuestionByText("Have you been fully recovered for more than 3 years without any ongoing symptoms or treatment for this condition?").AnswerNoAsync();
            await QuestionById("Medical and Lifestyle?191?2?1ay?6a?DD-27?j1?3?kq").AnswerYesAsync(); //Do you experience symptoms of irritable bowel, anxiety, depression or sleep disturbance or require continuous medication for this condition?
        }

        public async Task AnswerToGetWeightLoading()
        {
            await QuestionByText("How tall are you?").AnswerByTextAsync("130 cm (4 ft 3.2 in)");
            await QuestionByText("What is your weight?").AnswerByTextAsync("60 kg (9 st 6 lbs)");
        }

        public async Task AnswerToGetHyperthyroidismLoading()
        {
            await QuestionByContainsText("any blood disorder, thyroid disorder, anaemia, or lupus?").AnswerYesAsync();
            await QuestionById("Medical and Lifestyle?nz?2?w9").AnswerByTextAsync("Hyperthyroidism");
            await QuestionByText("Has your thyroid function been stable for more than six months with no ongoing symptoms?").AnswerYesAsync();
            await QuestionByText("Do you have high blood pressure, high cholesterol or any associated heart condition?").AnswerNoAsync();
            await QuestionByText("Are you currently on medication for overactive thyroid?").AnswerYesAsync();
        }

        public async Task AnswerToGetCholesterolLoading()
        {
            await QuestionByContainsText("any heart condition, including high blood pressure, high cholesterol, a heart murmur, chest pain or palpitations?").AnswerYesAsync();
            await QuestionById("Medical and Lifestyle?hd?2?he").AnswerByTextAsync("High Cholesterol");
            await QuestionByText("When was your high cholesterol diagnosed?").AnswerByTextAsync("More than 3 months ago");
            await QuestionByText("Has your cholesterol been checked since being diagnosed?").AnswerYesAsync();
            await QuestionByText("What type of treatment are you taking for your high cholesterol?").AnswerByTextAsync("Medication (oral)");
            await QuestionByText("Has your cholesterol level been checked within the last 12 months?").AnswerYesAsync();
            await QuestionByText("Do you know your latest cholesterol reading?").AnswerYesAsync();
            await QuestionByText("What was your latest cholesterol reading?").AnswerByTextAsync("Between 6.6 - 7.0mmol/L ");
        }

        private RiskUnderwritingQuestionHelper CreateQuestionHelper(QuestionResponse question)
        {
            return new RiskUnderwritingQuestionHelper(_qualificationApiClient, this, question, _riskId);
        }
    }

    public class RiskUnderwritingQuestionHelper
    {
        private readonly QualificationApiClient _qualificationApiClient;
        private readonly CustomerRiskUnderwritingHelper _underwritingHelper;
        private readonly QuestionResponse _question;
        private readonly int _riskId;

        public IReadOnlyList<string> Answers { get { return _question.Answers.Select(a => a.Text).ToList().AsReadOnly(); } }

        public RiskUnderwritingQuestionHelper(QualificationApiClient qualificationApiClient,
            CustomerRiskUnderwritingHelper underwritingHelper, QuestionResponse question, int riskId)
        {
            _qualificationApiClient = qualificationApiClient;
            _underwritingHelper = underwritingHelper;
            _question = question;
            _riskId = riskId;
        }


        public async Task<AnswerQuestionResponse> AnswerByTextAsync(string answerText)
        {
            var selectedAnswer = _question.Answers.Single(a => a.Text.Equals(answerText));

            var answerQuestionRequest = new UpdateQuestionRequest
            {
                QuestionId = _question.Id,
                SelectedAnswers =
                    new List<UpdateQuestionAnswerRequest>
                    {
                        new UpdateQuestionAnswerRequest {Id = selectedAnswer.Id, Text = selectedAnswer.Text}
                    }
            };

            var response = await _qualificationApiClient.AnswerQuestionForRiskAsync<AnswerQuestionResponse>(_riskId,
                answerQuestionRequest);

            if(response.AddedQuestions?.Any() == true || response.RemovedQuestionIds?.Any() == true)
                await _underwritingHelper.GetLatestUnderwritingForRiskAsync();

            return response;
        }

        public Task<AnswerQuestionResponse> AnswerYesAsync()
        {
            return AnswerByTextAsync("Yes");
        }

        public Task<AnswerQuestionResponse> AnswerNoAsync()
        {
            return AnswerByTextAsync("No");
        }

    }
}
