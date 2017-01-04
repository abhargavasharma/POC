using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TAL.QuoteAndApply.DataLayer.Service;
using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.ServiceLayer.Underwriting;
using TAL.QuoteAndApply.ServiceLayer.Underwriting.Models;


namespace TAL.QuoteAndApply.ServiceLayer.IntegrationTests.Underwriting
{
    [TestFixture]
    public class RiskUnderwritingQuestionServiceTests : BaseServiceLayerTest
    {
        [SetUp]
        public void Setup()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();
        }

        [Test]
        public void GetCurrentUnderwriting_WhenCalledOnNewQuote_ReturnsSomeQuestions()
        {
            var createQuoteResult = CreateQuoteWithDefaults();
            var quote = GetPolicy(createQuoteResult.QuoteReference);

            var srv = GetServiceInstance<IRiskUnderwritingQuestionService>();
            var underwritingQuestions = srv.GetCurrentUnderwriting(quote.Risks.First().Risk.Id);

            Assert.That(underwritingQuestions, Is.Not.Null);
            Assert.That(underwritingQuestions.Questions.Any(), Is.True);
        }

        [Test]
        public void AnswerQuestion_WhenCalledWithFirstQuestion_ReturnsSuccessfullResult()
        {
            var createQuoteResult = CreateQuoteWithDefaults();
            var quote = GetPolicy(createQuoteResult.QuoteReference);

            var srv = GetServiceInstance<IRiskUnderwritingQuestionService>();
            var underwritingQuestions = srv.GetCurrentUnderwriting(quote.Risks.First().Risk.Id);

            var questionToAnswer = underwritingQuestions.Questions.First();
            var selectedAnswer = questionToAnswer.Answers.First();
            var answerQuestionResult = srv.AnswerQuestionWithoutSyncing(quote.Risks.First().Risk.Id,
                questionToAnswer.Id, new List<UnderwritingAnswer> { selectedAnswer });

            Assert.That(answerQuestionResult, Is.Not.Null);
            Assert.That(answerQuestionResult.AddedQuestions, Is.Not.Null);
            Assert.That(answerQuestionResult.ChangedQuestions, Is.Not.Null);
            Assert.That(answerQuestionResult.RemovedQuestionIds, Is.Not.Null);
        }
    }
}
