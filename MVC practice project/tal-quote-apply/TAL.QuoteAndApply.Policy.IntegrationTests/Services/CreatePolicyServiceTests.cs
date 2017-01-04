using System;
using Moq;
using NUnit.Framework;
using TAL.QuoteAndApply.DataLayer.Service;
using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.Policy.Configuration;
using TAL.QuoteAndApply.Policy.Data;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.Tests.Shared.Mocks;

namespace TAL.QuoteAndApply.Policy.IntegrationTests.Services
{
    public class CreatePolicyServiceTests
    {

        private Mock<IQuoteReferenceGenerationService> _mockQuoteGenerationService;

        [SetUp]
        public void Setup()
        {
            _mockQuoteGenerationService = new Mock<IQuoteReferenceGenerationService>();
        }

        [Test]
        public void CreatePolicy_NormalCreatePolicy_CreatesPolicyWithNoException()
        {
            //Arrange

            //Act
            var policy = PolicyCreator.CreatePolicy();

            //Assert
            Assert.That(policy, Is.Not.Null);
            Assert.That(policy.QuoteReference, Is.Not.Empty);
        }


        [Test]
        [ExpectedException(typeof(ApplicationException))]
        public void CreatePolicy_QuoteReferenceDuplicateAfterMaxRetries_ExceptionThrown()
        {
            //Arrange
            _mockQuoteGenerationService.Setup(s => s.RandomQuoteReference()).Returns("Q1234567");

            //Act
            PolicyCreator.CreatePolicy(_mockQuoteGenerationService.Object);
            PolicyCreator.CreatePolicy(_mockQuoteGenerationService.Object); //Creating second policy with same quote reference will raise exception
        }

    }
}
