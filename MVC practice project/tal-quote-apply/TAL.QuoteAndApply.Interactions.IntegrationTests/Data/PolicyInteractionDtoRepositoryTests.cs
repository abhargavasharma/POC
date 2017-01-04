using NUnit.Framework;
using System;
using TAL.QuoteAndApply.DataLayer.Encrpytion;
using TAL.QuoteAndApply.DataLayer.Factory;
using TAL.QuoteAndApply.DataLayer.Repository;
using TAL.QuoteAndApply.DataLayer.Service;
using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.DataModel.Interactions;
using TAL.QuoteAndApply.Interactions.Configuration;
using TAL.QuoteAndApply.Interactions.Data;
using TAL.QuoteAndApply.Interactions.Models;
using TAL.QuoteAndApply.Tests.Shared.Helpers;
using TAL.QuoteAndApply.Tests.Shared.Mocks;

namespace TAL.QuoteAndApply.Interactions.IntegrationTests.Data
{
    [TestFixture]
    public class PolicyInteractionDtoRepositoryTests
    {
        [Test]
        public void Insert_Get_Update()
        {
            var interactionsConfig = new InteractionsConfigurationProvider();
            DbItemClassMapper<DbItem>.RegisterClassMaps();
            var policyInteractionRepo = new PolicyInteractionDtoRepository(interactionsConfig, new MockCurrentUserProvider(), new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()));

            #region Create Policy Interaction Tests

            var policy = PolicyHelper.CreatePolicy();

            //Arrange
            var createPolicyInteraction = new PolicyInteractionDto()
            {
                PolicyId = policy.Id,
                InteractionType = InteractionType.Quote_Accessed
            };

            //Act
            var insertedPolicyInteraction = policyInteractionRepo.InsertInteraction(createPolicyInteraction);

            //Assert
            Assert.That(insertedPolicyInteraction.Id, Is.GreaterThan(0));

            #endregion

            #region Get Policy Interaction Tests

            //Act
            var getInteraction = policyInteractionRepo.GetInteraction(insertedPolicyInteraction.Id);
            
            //Assert
            Assert.That(getInteraction, Is.Not.Null);
            Assert.That(getInteraction.PolicyId, Is.EqualTo(policy.Id));
            Assert.That(getInteraction.InteractionType, Is.EqualTo(InteractionType.Quote_Accessed));

            #endregion

            #region Update Policy Interaction Tests

            //Arrange
            bool updateResult = true;

            //Act
            try
            {
                policyInteractionRepo.UpdateInteraction(getInteraction);
            }
            catch (Exception ex)
            {
                updateResult = false;
            }

            //Assert
            Assert.That(updateResult, Is.True);

            #endregion

        }
    }
}
