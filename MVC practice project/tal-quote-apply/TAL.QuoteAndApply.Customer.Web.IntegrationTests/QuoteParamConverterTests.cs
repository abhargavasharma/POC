using System;
using System.Linq;
using Autofac;
using NUnit.Framework;
using TAL.QuoteAndApply.Customer.Web.Converters;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.Customer.Web.IntegrationTests.Tests;
using TAL.QuoteAndApply.Customer.Web.IntegrationTests.Clients;
using TAL.Performance.Infrastructure.Core;
using TAL.QuoteAndApply.DataModel.Interactions;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.ServiceLayer.Validation;

namespace TAL.QuoteAndApply.Customer.Web.IntegrationTests
{
    [TestFixture]
    public class QuoteParamConverterTests : BaseTestClass<PolicyClient>
    {
        public QuoteParamConverterTests() : base(SimplePerformanceTool.CreateConsoleTimestamped())
        {

        }

        //TODO: these test don't seem to actually test the QuoteParamConverter! They are really only testing the Service Layer Create Quote service???

        [Test]
        public void FromBasicInfoViewModel_WhenIsAustralianResidentNullAndNotEnforcingResidencyRuleForTalCustomer_ShouldCreateQuote()
        {
            //Arrange
            var createQuoteService = Container.Resolve<ICreateQuoteService>();

            //Act
            var response = createQuoteService.CreateQuote(
            new CreateQuoteParam(new RatingFactorsParam('M', DateTime.Now.AddYears(-30), null,
            new SmokerStatusHelper(true), "229", "27l", 600000),
            new PersonalInformationParam("Mr", "Tim", "Test", "0400000000", "0800000000", "VIC",  "Victor@tal.com.au", null, "Kensington", "12 Happy St", "1234"), false, ServiceLayer.Models.PolicySource.CustomerPortalBuildMyOwn, "TAL"));

            //Assert
            Assert.That(response.QuoteReference, Is.Not.Null);
            Assert.IsFalse(response.HasErrors);

        }

        [Test]
        public void FromBasicInfoViewModel_WhenIsAustralianResidentNullAndNotEnforcingResidencyRuleForExternalCustomer_ShouldCreateQuote()
        {
            //Arrange
            var createQuoteService = Container.Resolve<ICreateQuoteService>();

            //Act
            var response = createQuoteService.CreateQuote(
            new CreateQuoteParam(new RatingFactorsParam('M', DateTime.Now.AddYears(-30), null,
            new SmokerStatusHelper(true), "229", "27l", 600000),
            new PersonalInformationParam("Mr", "Tim", "Test", "0400000000", "0800000000", "VIC", "Victor@tal.com.au", null, "Kensington", "12 Happy St", "1234", "123"), false, ServiceLayer.Models.PolicySource.CustomerPortalBuildMyOwn, "TAL"));

            //Assert
            Assert.That(response.QuoteReference, Is.Not.Null);
            Assert.IsFalse(response.HasErrors);

        }

        [Test]
        public void FromBasicInfoViewModel_WhenIsAustralianResidentNullAndEnforcingResidencyRule_ShouldThrowResidencyValidatonError()
        {
            //Arrange
            var createQuoteService = Container.Resolve<ICreateQuoteService>();

            //Act
            var response = createQuoteService.CreateQuote(
            new CreateQuoteParam(new RatingFactorsParam('M', DateTime.Now.AddYears(-30), null,
            new SmokerStatusHelper(true), "229", "27l", 600000),
            new PersonalInformationParam("Mr", "Tim", "Test", "0400000000", "0800000000", "VIC", "Victor@tal.com.au", null, "Kensington", "12 Happy St", "1234"), true, ServiceLayer.Models.PolicySource.CustomerPortalBuildMyOwn, "TAL"));

            //Assert
            Assert.That(response.QuoteReference, Is.Null);
            Assert.IsTrue(response.Errors[0].Key.Equals(ValidationKey.AustralianResidency));
        }

        [Test]
        public void FromBasicInfoViewModel_WhenIsAustralianResidentNullAndEnforcingResidencyRuleForExternalCustomer_ShouldThrowResidencyValidatonError()
        {
            //Arrange
            var createQuoteService = Container.Resolve<ICreateQuoteService>();

            //Act
            var response = createQuoteService.CreateQuote(
            new CreateQuoteParam(new RatingFactorsParam('M', DateTime.Now.AddYears(-30), null,
            new SmokerStatusHelper(true), "229", "27l", 600000),
            new PersonalInformationParam("Mr", "Tim", "Test", "0400000000", "0800000000", "VIC", "Victor@tal.com.au", null, "Kensington", "12 Happy St", "1234", "123"), true, ServiceLayer.Models.PolicySource.CustomerPortalBuildMyOwn, "TAL"));

            //Assert
            Assert.That(response.QuoteReference, Is.Null);
            Assert.IsTrue(response.Errors[0].Key.Equals(ValidationKey.AustralianResidency));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void FromBasicInfoViewModel_WhenIsAustralianResidentTrue_ShouldCreateSuccessfulQuoteAndUpdateUREResidencyQuestionAsYes(bool validateResidency)
        {
            //Arrange
            var createQuoteService = Container.Resolve<ICreateQuoteService>();
        
            //Act
            var response = createQuoteService.CreateQuote(
            new CreateQuoteParam(new RatingFactorsParam('M', DateTime.Now.AddYears(-30), true,
            new SmokerStatusHelper(true), "229", "27l", 600000),
            new PersonalInformationParam("Mr", "Tim", "Test", "0400000000", "0800000000", "VIC", "Victor@tal.com.au", null, "Kensington", "12 Happy St", "1234"), validateResidency, ServiceLayer.Models.PolicySource.CustomerPortalBuildMyOwn, "TAL"));

            //Assert
            Assert.That(response.QuoteReference, Is.Not.Null);
            Assert.IsFalse(response.HasErrors);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void FromBasicInfoViewModel_WhenIsAustralianResidentTrueForExternalCustomer_ShouldCreateSuccessfulQuoteAndUpdateUREResidencyQuestionAsYes(bool validateResidency)
        {
            //Arrange
            var createQuoteService = Container.Resolve<ICreateQuoteService>();

            //Act
            var response = createQuoteService.CreateQuote(
            new CreateQuoteParam(new RatingFactorsParam('M', DateTime.Now.AddYears(-30), true,
            new SmokerStatusHelper(true), "229", "27l", 600000),
            new PersonalInformationParam("Mr", "Tim", "Test", "0400000000", "0800000000", "VIC", "Victor@tal.com.au", null, "Kensington", "12 Happy St", "1234", "123"), validateResidency, ServiceLayer.Models.PolicySource.CustomerPortalBuildMyOwn, "TAL"));

            //Assert
            Assert.That(response.QuoteReference, Is.Not.Null);
            Assert.IsFalse(response.HasErrors);
        }
        [Test]
        public void FromBasicInfoViewModel_WhenIsAustralianResidentFalseAndEnforcingResidencyRyle_ShouldThrowResidencyValidationError()
        {
            //Arrange
            var createQuoteService = Container.Resolve<ICreateQuoteService>();

            //Act
            var response = createQuoteService.CreateQuote(
            new CreateQuoteParam(new RatingFactorsParam('M', DateTime.Now.AddYears(-30), false,
            new SmokerStatusHelper(true), "229", "27l", 600000),
            new PersonalInformationParam("Mr", "Tim", "Test", "0400000000", "0800000000",  "VIC", "Victor@tal.com.au", null, "Kensington", "12 Happy St", "1234"), true, ServiceLayer.Models.PolicySource.CustomerPortalBuildMyOwn, "TAL"));

            //Assert
            Assert.That(response.QuoteReference, Is.Null);
            Assert.IsTrue(response.HasErrors);
            Assert.IsTrue(response.Errors[0].Key.Equals(ValidationKey.AustralianResidency));
        }

        [Test]
        public void FromBasicInfoViewModel_WhenIsAustralianResidentFalseAndEnforcingResidencyRyleForExternalCustomer_ShouldThrowResidencyValidationError()
        {
            //Arrange
            var createQuoteService = Container.Resolve<ICreateQuoteService>();

            //Act
            var response = createQuoteService.CreateQuote(
            new CreateQuoteParam(new RatingFactorsParam('M', DateTime.Now.AddYears(-30), false,
            new SmokerStatusHelper(true), "229", "27l", 600000),
            new PersonalInformationParam("Mr", "Tim", "Test", "0400000000", "0800000000", "VIC", "Victor@tal.com.au", null, "Kensington", "12 Happy St", "1234", "123"), true, ServiceLayer.Models.PolicySource.CustomerPortalBuildMyOwn, "TAL"));

            //Assert
            Assert.That(response.QuoteReference, Is.Null);
            Assert.IsTrue(response.HasErrors);
            Assert.IsTrue(response.Errors[0].Key.Equals(ValidationKey.AustralianResidency));
        }
        [Test]
        public void FromBasicInfoViewModel_WhenIsAustralianResidentFalseAndNotEnforcingResidencyRule_ShouldCreateSuccessfulQuoteAndUpdateUREQuestionAnswerAsNo()
        {
            //Arrange
            var createQuoteService = Container.Resolve<ICreateQuoteService>();

            //Act
            var response = createQuoteService.CreateQuote(
            new CreateQuoteParam(new RatingFactorsParam('M', DateTime.Now.AddYears(-30), false,
            new SmokerStatusHelper(true), "229", "27l", 600000),
            new PersonalInformationParam("Mr", "Tim", "Test", "0400000000", "0800000000","VIC", "Victor@tal.com.au", null, "Kensington", "12 Happy St", "1234"), false, ServiceLayer.Models.PolicySource.CustomerPortalBuildMyOwn, "TAL"));

            //Assert
            Assert.That(response.QuoteReference, Is.Not.Null);
            Assert.IsFalse(response.HasErrors);
        }

        [Test]
        public void FromBasicInfoViewModel_WhenIsAustralianResidentFalseAndNotEnforcingResidencyRuleForExternalCustomer_ShouldCreateSuccessfulQuoteAndUpdateUREQuestionAnswerAsNo()
        {
            //Arrange
            var createQuoteService = Container.Resolve<ICreateQuoteService>();

            //Act
            var response = createQuoteService.CreateQuote(
            new CreateQuoteParam(new RatingFactorsParam('M', DateTime.Now.AddYears(-30), false,
            new SmokerStatusHelper(true), "229", "27l", 600000),
            new PersonalInformationParam("Mr", "Tim", "Test", "0400000000", "0800000000", "VIC", "Victor@tal.com.au", null, "Kensington", "12 Happy St", "1234", "123"), false, ServiceLayer.Models.PolicySource.CustomerPortalBuildMyOwn, "TAL"));

            //Assert
            Assert.That(response.QuoteReference, Is.Not.Null);
            Assert.IsFalse(response.HasErrors);
        }
        [Test]
        public void FromBasicInfoViewModel_WhenPolicySourceSetToCustomerPortalBuildMyOwn_ShouldCreateQuoteWithBuildMyOwnSourceAndCustomerInteraction()
        {
            //Arrange
            var createQuoteService = Container.Resolve<ICreateQuoteService>();

            //Act
            var response = createQuoteService.CreateQuote(
            new CreateQuoteParam(new RatingFactorsParam('M', DateTime.Now.AddYears(-30), null,
            new SmokerStatusHelper(true), "229", "27l", 600000),
            new PersonalInformationParam("Mr", "Tim", "Test", "0400000000", "0800000000", "VIC", "Victor@tal.com.au", null, "Kensington", "12 Happy St", "1234"), false, ServiceLayer.Models.PolicySource.CustomerPortalBuildMyOwn, "TAL"));

            //Assert
            Assert.That(response.QuoteReference, Is.Not.Null);
            Assert.IsFalse(response.HasErrors);

            var policyService = Container.Resolve<IPolicyService>();
            var policy = policyService.GetByQuoteReferenceNumber(response.QuoteReference);

            Assert.That(policy.Source, Is.EqualTo(PolicySource.CustomerPortalBuildMyOwn));

            var policyInteractionService = Container.Resolve<IPolicyInteractionService>();
            var policyInteractions = policyInteractionService.GetInteractions(
                    PolicyInteractionsRequest.PolicyInteractionsByQuoteReference(response.QuoteReference)).Interactions.ToList();
            var customerPortalInteraction =
                policyInteractions.FirstOrDefault(x => x.InteractionType == InteractionType.Created_By_Customer);
            Assert.That(customerPortalInteraction, Is.Not.Null);
        }

        [Test]
        public void FromBasicInfoViewModel_WhenPolicySourceSetToCustomerPortalBuildMyOwnForExternalCustomer_ShouldCreateQuoteWithBuildMyOwnSourceAndCustomerInteraction()
        {
            //Arrange
            var createQuoteService = Container.Resolve<ICreateQuoteService>();

            //Act
            var response = createQuoteService.CreateQuote(
            new CreateQuoteParam(new RatingFactorsParam('M', DateTime.Now.AddYears(-30), null,
            new SmokerStatusHelper(true), "229", "27l", 600000),
            new PersonalInformationParam("Mr", "Tim", "Test", "0400000000", "0800000000", "VIC", "Victor@tal.com.au", null, "Kensington", "12 Happy St", "1234", "123"), false, ServiceLayer.Models.PolicySource.CustomerPortalBuildMyOwn, "TAL"));

            //Assert
            Assert.That(response.QuoteReference, Is.Not.Null);
            Assert.IsFalse(response.HasErrors);

            var policyService = Container.Resolve<IPolicyService>();
            var policy = policyService.GetByQuoteReferenceNumber(response.QuoteReference);

            Assert.That(policy.Source, Is.EqualTo(PolicySource.CustomerPortalBuildMyOwn));

            var policyInteractionService = Container.Resolve<IPolicyInteractionService>();
            var policyInteractions = policyInteractionService.GetInteractions(
                    PolicyInteractionsRequest.PolicyInteractionsByQuoteReference(response.QuoteReference)).Interactions.ToList();
            var customerPortalInteraction =
                policyInteractions.FirstOrDefault(x => x.InteractionType == InteractionType.Created_By_Customer);
            Assert.That(customerPortalInteraction, Is.Not.Null);
        }

        [Test]
        public void FromBasicInfoViewModel_WhenPolicySourceSetToCustomerPortalHelpMeChoose_ShouldCreateQuoteWithHelpMeChooseSourceAndCustomerInteraction()
        {
            //Arrange
            var createQuoteService = Container.Resolve<ICreateQuoteService>();

            //Act
            var response = createQuoteService.CreateQuote(
            new CreateQuoteParam(new RatingFactorsParam('M', DateTime.Now.AddYears(-30), null,
            new SmokerStatusHelper(true), "229", "27l", 600000),
            new PersonalInformationParam("Mr", "Tim", "Test", "0400000000", "0800000000", "VIC", "Victor@tal.com.au", null, "Kensington", "12 Happy St", "1234"), false, ServiceLayer.Models.PolicySource.CustomerPortalHelpMeChoose, "TAL"));

            //Assert
            Assert.That(response.QuoteReference, Is.Not.Null);
            Assert.IsFalse(response.HasErrors);

            var policyService = Container.Resolve<IPolicyService>();
            var policy = policyService.GetByQuoteReferenceNumber(response.QuoteReference);

            Assert.That(policy.Source, Is.EqualTo(PolicySource.CustomerPortalHelpMeChoose));

            var policyInteractionService = Container.Resolve<IPolicyInteractionService>();
            var policyInteractions = policyInteractionService.GetInteractions(
                    PolicyInteractionsRequest.PolicyInteractionsByQuoteReference(response.QuoteReference)).Interactions.ToList();
            var customerPortalInteraction =
                policyInteractions.FirstOrDefault(x => x.InteractionType == InteractionType.Created_By_Customer);
            Assert.That(customerPortalInteraction, Is.Not.Null);
        }

        [Test]
        public void FromBasicInfoViewModel_WhenPolicySourceSetToCustomerPortalHelpMeChooseForExternalCustomer_ShouldCreateQuoteWithHelpMeChooseSourceAndCustomerInteraction()
        {
            //Arrange
            var createQuoteService = Container.Resolve<ICreateQuoteService>();

            //Act
            var response = createQuoteService.CreateQuote(
            new CreateQuoteParam(new RatingFactorsParam('M', DateTime.Now.AddYears(-30), null,
            new SmokerStatusHelper(true), "229", "27l", 600000),
            new PersonalInformationParam("Mr", "Tim", "Test", "0400000000", "0800000000", "VIC", "Victor@tal.com.au", null, "Kensington", "12 Happy St", "1234", "123"), false, ServiceLayer.Models.PolicySource.CustomerPortalHelpMeChoose, "TAL"));

            //Assert
            Assert.That(response.QuoteReference, Is.Not.Null);
            Assert.IsFalse(response.HasErrors);

            var policyService = Container.Resolve<IPolicyService>();
            var policy = policyService.GetByQuoteReferenceNumber(response.QuoteReference);

            Assert.That(policy.Source, Is.EqualTo(PolicySource.CustomerPortalHelpMeChoose));

            var policyInteractionService = Container.Resolve<IPolicyInteractionService>();
            var policyInteractions = policyInteractionService.GetInteractions(
                    PolicyInteractionsRequest.PolicyInteractionsByQuoteReference(response.QuoteReference)).Interactions.ToList();
            var customerPortalInteraction =
                policyInteractions.FirstOrDefault(x => x.InteractionType == InteractionType.Created_By_Customer);
            Assert.That(customerPortalInteraction, Is.Not.Null);
        }
    }
}
