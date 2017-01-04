using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using NUnit.Framework;
using TAL.Performance.Infrastructure.Core;
using TAL.QuoteAndApply.SalesPortal.Web.Extensions;
using TAL.QuoteAndApply.SalesPortal.Web.IntegrationTests.Clients;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.ServiceLayer.Underwriting;
using TAL.QuoteAndApply.Tests.Shared.Builders;
using TAL.QuoteAndApply.Tests.Shared.Helpers;
using Task = System.Threading.Tasks.Task;

namespace TAL.QuoteAndApply.SalesPortal.Web.IntegrationTests
{
    [TestFixture]
    public class UnderwritingApiTests : BaseTestClass<UnderwritingClient>
    {
        public UnderwritingApiTests() : base(SimplePerformanceTool.CreateConsoleTimestamped())
        {
            
        }

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
           
        }

        [Test]
        public async Task QuestionAnswered_DateOfBirthChangedToOver60_IneligiblePlansSwitchedOff_Async()
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

            var newDob = DateTime.Today.AddYears(-61);

            UnderwritingHelper.AnswerDateOfBirthQuestion(riskWrapper.Risk.InterviewId, riskWrapper.Risk.InterviewConcurrencyToken, newDob);

            var response = Client.QuestionAnsweredAsync(policyWithRisks.Policy.QuoteReference, riskWrapper.Risk.Id,
                new UnderwritingQuestionAnswerRequest
                {
                    QuestionId = QuestionIds.DateOfBirthQuestion,
                    Answers = new List<UnderwritingAnswerRequest>
                    {
                        new UnderwritingAnswerRequest {Id="*", Text = newDob.ToString("dd/MM/yyyy")}
                    }
                });

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
        public async Task QuestionAnswered_DateOfBirthChanged_RiskAndPartyUpdated_Async()
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

            var newDob = DateTime.Today.AddYears(-50);

            UnderwritingHelper.AnswerDateOfBirthQuestion(risk.InterviewId, risk.InterviewConcurrencyToken, newDob);

            var response = await Client.QuestionAnsweredAsync(createPolicyResult.Policy.QuoteReference, createPolicyResult.Risk.Id,
                new UnderwritingQuestionAnswerRequest
                {
                    QuestionId = QuestionIds.DateOfBirthQuestion,
                    Answers = new List<UnderwritingAnswerRequest>
                    {
                        new UnderwritingAnswerRequest {Id="*", Text = newDob.ToString("dd/MM/yyyy")}
                    }
                });

            var updatedParty = PartyHelper.GetParty(party.Id, true);
            var updatedRisk = RiskHelper.GetRisk(risk.Id, true);

            Assert.That(updatedRisk.DateOfBirth, Is.EqualTo(newDob));
            Assert.That(updatedParty.DateOfBirth, Is.EqualTo(newDob));
        }

        [Test]
        public async Task QuestionAnswered_AnnualIncomeChanged_RiskUpdated_Async()
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

            var newAnnualIncome = 1000;

            UnderwritingHelper.AnswerAnnualIncomeQuestion(risk.InterviewId, risk.InterviewConcurrencyToken, newAnnualIncome);

            var response = await Client.QuestionAnsweredAsync(createPolicyResult.Policy.QuoteReference, createPolicyResult.Risk.Id,
                new UnderwritingQuestionAnswerRequest
                {
                    QuestionId = QuestionIds.AnnualIncomeQuestion,
                    Answers = new List<UnderwritingAnswerRequest>
                    {
                        new UnderwritingAnswerRequest {Id="*", Text = newAnnualIncome.ToString()}
                    }
                });

            var updatedRisk = RiskHelper.GetRisk(risk.Id, true);

            Assert.That(updatedRisk.AnnualIncome, Is.EqualTo(newAnnualIncome));
        }


        [Test]
        public async Task QuestionAnswered_AnnualIncomeChangedInUnderwritingWithDecimals_RiskUpdatedWithTruncatedAnnualIncome_Async()
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

            var newAnnualIncome = 1000.25M;

            UnderwritingHelper.AnswerAnnualIncomeQuestion(risk.InterviewId, risk.InterviewConcurrencyToken, newAnnualIncome);

            var response = await Client.QuestionAnsweredAsync(createPolicyResult.Policy.QuoteReference, createPolicyResult.Risk.Id,
                new UnderwritingQuestionAnswerRequest
                {
                    QuestionId = QuestionIds.AnnualIncomeQuestion,
                    Answers = new List<UnderwritingAnswerRequest>
                    {
                        new UnderwritingAnswerRequest {Id="*", Text = newAnnualIncome.ToString()}
                    }
                });

            var updatedRisk = RiskHelper.GetRisk(risk.Id, true);

            Assert.That(updatedRisk.AnnualIncome, Is.EqualTo(1000));
        }
    }
}