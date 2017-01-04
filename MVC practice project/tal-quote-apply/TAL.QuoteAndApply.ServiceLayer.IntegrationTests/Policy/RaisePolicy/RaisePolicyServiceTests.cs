using System;
using System.Linq;
using NUnit.Framework;
using TAL.QuoteAndApply.DataLayer.Service;
using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.DataModel.Interactions;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.Policy.Data;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Models;
using TAL.QuoteAndApply.ServiceLayer.Payment;
using TAL.QuoteAndApply.ServiceLayer.Payment.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.RaisePolicy;
using TAL.QuoteAndApply.ServiceLayer.Policy.Risk.Beneficiary;
using TAL.QuoteAndApply.ServiceLayer.Underwriting.Events;
using TAL.QuoteAndApply.Tests.Shared.Builders;
using TAL.QuoteAndApply.Underwriting.Service;
using PolicyStatus = TAL.QuoteAndApply.DataModel.Policy.PolicyStatus;

namespace TAL.QuoteAndApply.ServiceLayer.IntegrationTests.Policy.RaisePolicy
{
    public class RaisePolicyServiceTests : BaseServiceLayerTest
    {
        [Test]
        public void PostPolicy_EnsureService_PostsPolicyWithNoException()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var quoteReference = CreateCompletePolicyAndReturnQuoteReference();

            //Act
            var raisePolicyService = GetServiceInstance<IRaisePolicyService>();

            var submissionResult = raisePolicyService.PostPolicy(quoteReference);

            //Assert
            Assert.That(submissionResult, Is.Not.Null);
            Assert.That(submissionResult.SubmittedSuccessfully, Is.True);

            var policyWithRisksService = GetServiceInstance<IPolicyWithRisksService>();
            var policyWithRisks = policyWithRisksService.GetFrom(quoteReference);
            var risk = policyWithRisks.Risks.First().Risk;

            //assert status
            Assert.That(policyWithRisks.Policy.Status, Is.EqualTo(PolicyStatus.RaisedToPolicyAdminSystem));

            //assert audit written
            var auditRepo = GetServiceInstance<IRaisePolicySubmissionAuditDtoRepository>();
            var audits = auditRepo.GetAllAuditsForPolicy(policyWithRisks.Policy.Id);

            Assert.That(audits.Any(a => a.RaisePolicyAuditType == RaisePolicyAuditType.Submit), Is.True);

            //Asser that interaction is submitted
            var policyInteractionSvc = GetServiceInstance<IPolicyInteractionService>();
            var allInteractions = policyInteractionSvc.GetInteractions(PolicyInteractionsRequest.PolicyInteractionsByQuoteReference(quoteReference));
            var submittedInteraction = allInteractions.Interactions.FirstOrDefault(x => x.InteractionType == InteractionType.Submitted_To_Policy_Admin_System);

            Assert.That(submittedInteraction, Is.Not.Null);

            //assert uw complete
            Console.WriteLine(risk.InterviewId);

            var getInterviewService = GetServiceInstance<IGetUnderwritingInterview>();
            var interview = getInterviewService.GetInterview(risk.InterviewId, new UnderwritingBenefitsResponseChangeSubject());

            var currentUserProvider = GetServiceInstance<ICurrentUserProvider>();
            var currentUser = currentUserProvider.GetForApplication();

            Assert.That(interview.Completed, Is.True);
            Assert.That(interview.CompletedBy, Is.EqualTo(currentUser.UserName));

            
        }

        private string CreateCompletePolicyAndReturnQuoteReference()
        {
            //assumes all mandatory rating factors and personal details are filled out
            var quoteBuilder = new CreateQuoteBuilder()
                .WithJourneyType(PolicySource.CustomerPortalBuildMyOwn);

            var quote = CreateQuote(quoteBuilder.Build());
            var policyWithRisksService = GetServiceInstance<IPolicyWithRisksService>();
            var policyWithRisks = policyWithRisksService.GetFrom(quote.QuoteReference);
            var risk = policyWithRisks.Risks.First().Risk;

            var beneficiaries = GetServiceInstance<IBeneficiaryDetailsService>();
            beneficiaries.CreateOrUpdateBeneficiary(new RiskBeneficiaryDetailsParam
                {
                    Address = "123 ABC st",
                    BeneficiaryRelationshipId = 1,
                    DateOfBirth = DateTime.Now.AddYears(-30),
                    EmailAddress = "test.test@test.com",
                    FirstName = "Tester",
                    PhoneNumber = "0400000000",
                    RiskId = risk.Id,
                    Surname = "Test",
                    Suburb = "Test",
                    State = "VIC",
                    Share = 100,
                    Postcode = "3000",
                    Title = "Mr"
                }, risk.Id);

            var payments = GetServiceInstance<IPaymentOptionService>();
            payments.AssignCreditCardPayment(quote.QuoteReference, risk.Id, new CreditCardPaymentParam
            {
                CardNumber = "5454545454545454",
                CardType = CreditCardType.MasterCard,
                ExpiryMonth = "10",
                ExpiryYear = "18",
                NameOnCard = "Tester McTester"
            });

            //todo: complete underwriting, for now rely on feature toggle


            return quote.QuoteReference;
        }
    }
}
