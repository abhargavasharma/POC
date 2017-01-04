using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Results;
using Autofac;
using NUnit.Framework;
using TAL.Performance.Infrastructure.Core;
using TAL.QuoteAndApply.Customer.Web.IntegrationTests.Clients;
using TAL.QuoteAndApply.Customer.Web.Models.Api;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.Party.Services;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.DataModel.Interactions;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using Task = System.Threading.Tasks.Task;

namespace TAL.QuoteAndApply.Customer.Web.IntegrationTests.Tests
{
    [TestFixture]
    public class SaveApiTests : BaseTestClass<SaveClient>
    {
        public SaveApiTests() : base(SimplePerformanceTool.CreateConsoleTimestamped())
        {
        }
        [Test]
        public async Task SaveCustomer_ClickSaveWithBlankFields_ReturnsResponseWithRequiredErrors_Async()
        {
            //Arrange
            var createResponse = CreateAndLoginAsDefaultPolicy();
            var saveCustomerRequest = new SaveCustomerRequest
            {
                FirstName = null,
                LastName = null,
                PhoneNumber = null,
                EmailAddress = null,
                ExpressConsent = true
            };

            //Act
            var response = await Client.SaveDetailsAsync<Dictionary<string, IEnumerable<string>>>
                (createResponse.Risk.Id, saveCustomerRequest, throwOnFailure: false);

            //Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(
                response.First(x => x.Key.Equals("saveCustomerRequest.firstName")).Value.First(),
                Is.EqualTo("First name is required"));
            Assert.That(
                response.First(x => x.Key.Equals("saveCustomerRequest.lastName")).Value.First(),
                Is.EqualTo("Last name is required"));
            Assert.That(
                response.First(x => x.Key.Equals("saveCustomerRequest.emailAddress")).Value.First(),
                Is.EqualTo("Email address is required"));
        }

        [Test]
        public async Task SaveCustomer_EnterCustomerDetailsAndClickSave_SavesCustomerDetailsInTheDB_MobilePhone_Async()
        {
            //Arrange
            var createResponse = CreateAndLoginAsDefaultPolicy();

            var saveCustomerRequest = new SaveCustomerRequest
            {
                FirstName = "Karish",
                LastName = "Test",
                PhoneNumber = "0400000000",
                EmailAddress = "sauman@gmail.com",
                ExpressConsent = true
            };

            //Act

            await Client.SaveDetailsAsync(createResponse.Risk.Id, saveCustomerRequest);

            //Assert
            var partyService = Container.Resolve<IPartyService>();
            var party = partyService.GetParty(createResponse.Party.Id);

            Assert.That(party != null);
            Assert.That(party.FirstName, Is.EqualTo(saveCustomerRequest.FirstName));
            Assert.That(party.Surname, Is.EqualTo(saveCustomerRequest.LastName));
            Assert.That(party.MobileNumber, Is.EqualTo(saveCustomerRequest.PhoneNumber));
            Assert.That(party.EmailAddress, Is.EqualTo(saveCustomerRequest.EmailAddress));

            var partyConsentService = Container.Resolve<IPartyConsentService>();
            var partyConsent = partyConsentService.GetPartyConsentByPartyId(createResponse.Party.Id);
            Assert.That(partyConsent != null);
            Assert.That(partyConsent.ExpressConsent, Is.EqualTo(saveCustomerRequest.ExpressConsent));
            Assert.That(partyConsent.ExpressConsentUpdatedTs, Is.LessThan(DateTime.Now));
            Assert.That(partyConsent.PartyId, Is.EqualTo(party.Id));

            var policyService = Container.Resolve<IPolicyService>();
            var policy = policyService.GetByQuoteReferenceNumber(createResponse.Policy.QuoteReference);

            Assert.That(policy, Is.Not.Null);
            Assert.That(policy.SaveStatus, Is.EqualTo(PolicySaveStatus.PersonalDetailsEntered));

            Assert.That(QuoteHasInteraction(QuoteSessionContext.QuoteSession.QuoteReference, InteractionType.Saved_By_Customer));
        }

        [Test]
        public async Task SaveCustomer_EnterCustomerDetailsAndClickSave_SavesCustomerDetailsInTheDB_HomePhone_Async()
        {
            //Arrange
            var createResponse = CreateAndLoginAsDefaultPolicy();

            var saveCustomerRequest = new SaveCustomerRequest
            {
                FirstName = "Karish",
                LastName = "Test",
                PhoneNumber = "0200000000",
                EmailAddress = "sauman@gmail.com",
                ExpressConsent = true
            };

            //Act

            await Client.SaveDetailsAsync(createResponse.Risk.Id, saveCustomerRequest);

            //Assert
            var partyService = Container.Resolve<IPartyService>();
            var party = partyService.GetParty(createResponse.Party.Id);

            Assert.That(party != null);
            Assert.That(party.FirstName, Is.EqualTo(saveCustomerRequest.FirstName));
            Assert.That(party.Surname, Is.EqualTo(saveCustomerRequest.LastName));
            Assert.That(party.HomeNumber, Is.EqualTo(saveCustomerRequest.PhoneNumber));
            Assert.That(party.EmailAddress, Is.EqualTo(saveCustomerRequest.EmailAddress));

            var partyConsentService = Container.Resolve<IPartyConsentService>();
            var partyConsent = partyConsentService.GetPartyConsentByPartyId(createResponse.Party.Id);
            Assert.That(partyConsent != null);
            Assert.That(partyConsent.ExpressConsent, Is.EqualTo(saveCustomerRequest.ExpressConsent));
            Assert.That(partyConsent.ExpressConsentUpdatedTs, Is.LessThan(DateTime.Now));
            Assert.That(partyConsent.PartyId, Is.EqualTo(party.Id));

            var policyService = Container.Resolve<IPolicyService>();
            var policy = policyService.GetByQuoteReferenceNumber(createResponse.Policy.QuoteReference);

            Assert.That(policy, Is.Not.Null);
            Assert.That(policy.SaveStatus, Is.EqualTo(PolicySaveStatus.PersonalDetailsEntered));

            Assert.That(QuoteHasInteraction(QuoteSessionContext.QuoteSession.QuoteReference, InteractionType.Saved_By_Customer));
        }

        [Test]
        public async Task SaveCustomer_SaveCustomerDetailsWithIncorrectPhoneNumberFormat_ShouldThrowPhoneNumberValidationError_Async()
        {
            //Arrange
            var createResponse = CreateAndLoginAsDefaultPolicy();
            var saveCustomerRequest = new SaveCustomerRequest
            {
                FirstName = "Butlerrrrrrrrrrrrrrrrrrrrr",
                LastName = "Testtttttttttttttttttttttt",
                PhoneNumber = "0111111111",
                EmailAddress = "step@gmail.commmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm"
            };

            //Act
            var response = await Client.SaveDetailsAsync<Dictionary<string, IEnumerable<string>>>
                (createResponse.Risk.Id, saveCustomerRequest, throwOnFailure: false);
            
            //Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.First(x => x.Key.Equals("saveCustomerRequest.phoneNumber")).Value.First(),
                Is.EqualTo("Phone number should start with 02, 03, 04, 07 or 08"));
            Assert.That(response.First(x => x.Key.Equals("saveCustomerRequest.firstName")).Value.First(),
                Is.EqualTo("First name cannot be longer than 22 characters."));
            Assert.That(response.First(x => x.Key.Equals("saveCustomerRequest.lastName")).Value.First(),
                Is.EqualTo("Last name cannot be longer than 22 characters."));
            Assert.That(response.First(x => x.Key.Equals("saveCustomerRequest.emailAddress")).Value.First(),
                Is.EqualTo("Email address cannot be longer than 80 characters."));

            Assert.That(QuoteHasInteraction(QuoteSessionContext.QuoteSession.QuoteReference, InteractionType.Saved_By_Customer), Is.False);
        }

        [Test]
        public async Task SaveCustomer_SaveCustomerDetailWithNumericFirstName_ShouldThrowFirstNameValidationError_Async()
        {
            //Arrange
            var createResponse = CreateAndLoginAsDefaultPolicy();
            var saveCustomerRequest = new SaveCustomerRequest
            {
                FirstName = "36565",
                LastName = "Test",
                PhoneNumber = "0400000000",
                EmailAddress = "joe@gmail.com"
            };

            //Act
            var response = await Client.SaveDetailsAsync<Dictionary<string, IEnumerable<string>>>
                (createResponse.Risk.Id, saveCustomerRequest, throwOnFailure: false);

            //Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(
                response.First(x => x.Key.Equals("saveCustomerRequest.firstName")).Value.First(),
                Is.EqualTo("Name must not contain numbers"));

            Assert.That(QuoteHasInteraction(QuoteSessionContext.QuoteSession.QuoteReference, InteractionType.Saved_By_Customer), Is.False);
        }

        [Test]
        public async Task SaveCustomer_SaveCustomerDetailWithExistingAccount_ShouldReturnCorrectExistingAccountFlag_Async()
        {
            var uniqueEmailAddress = Guid.NewGuid().ToString().Replace("-", "") + "@user.com";

            //Arrange
            var createResponse = CreateAndLoginAsDefaultPolicy();
            var saveCustomerRequest = new SaveCustomerRequest
            {
                FirstName = "Testy",
                LastName = "Test",
                PhoneNumber = "0400000000",
                EmailAddress = uniqueEmailAddress
            };

            //Act
            //First save, account doesn't exist
            var response = await Client.SaveDetailsAsync<SaveCustomerResponse>(createResponse.Risk.Id, saveCustomerRequest);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.AccountExists, Is.False);
            await Client.SavePasswordAsync(createResponse.Risk.Id, new CreateLoginRequest { Password = "AAbb44$$"});

            //Second save, account exists
            response = await Client.SaveDetailsAsync<SaveCustomerResponse>(createResponse.Risk.Id, saveCustomerRequest);
            Assert.That(response, Is.Not.Null);
            Assert.That(response.AccountExists, Is.True);

            Assert.That(QuoteHasInteraction(QuoteSessionContext.QuoteSession.QuoteReference, InteractionType.Saved_By_Customer));
        }

        [Test]
        public async Task SavePassword_PasswordIsNull_ShouldFailWithPasswordRequiredError_Async()
        {
            //Arrange
            var createResponse = CreateAndLoginAsDefaultPolicy();
            var createLoginRequest = new CreateLoginRequest
            {
                Password = null
            };

            //Act
            var response = await Client.SavePasswordAsync<Dictionary<string, IEnumerable<string>>>
                (createResponse.Risk.Id, createLoginRequest);

            //Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(
                response.First(x => x.Key.Equals("createLoginRequest.password")).Value.First(),
                Is.EqualTo("Password is required"));
        }

        [Test]
        public async Task SavePassword_PasswordNotComplexEnough_ShouldFailWithComplexityError_Async()
        {
            //Arrange
            var createResponse = CreateAndLoginAsDefaultPolicy();
            var createLoginRequest = new CreateLoginRequest
            {
                Password = "xxx"
            };

            //Act
            var response = await Client.SavePasswordAsync<Dictionary<string, IEnumerable<string>>>
                (createResponse.Risk.Id, createLoginRequest);

            //Assert
            Assert.That(response, Is.Not.Null);
            Assert.That(response.First(x => x.Key.Equals("createLoginRequest.password")).Value.First(),
                Is.EqualTo(
                    "Your password must be at least 6 characters long, and include upper and lowercase letters."));
        }

        [Test]
        public async Task SavePassword_PasswordValid_ShouldReturnSuccessAndSetCorrectStatusInDb_Async()
        {
            //Arrange
            var uniqueEmailAddress = Guid.NewGuid().ToString().Replace("-", "") + "@user.com";
            var saveCustomerRequest = new SaveCustomerRequest
            {
                FirstName = "Butler",
                LastName = "Test",
                PhoneNumber = "0400000000",
                EmailAddress = uniqueEmailAddress
            };

            var createLoginRequest = new CreateLoginRequest
            {
                Password = "AAaa11%%"
            };

            var createResponse = CreateAndLoginAsDefaultPolicy();

            //Act
            var saveCustomerResponse = await Client.SaveDetailsAsync<SaveCustomerResponse>(createResponse.Risk.Id, saveCustomerRequest);
            Assert.That(saveCustomerResponse, Is.Not.Null);
            Assert.That(saveCustomerResponse.AccountExists, Is.False);

            await Client.SavePasswordAsync(createResponse.Risk.Id, createLoginRequest);

            //Assert
            var policyService = Container.Resolve<IPolicyService>();
            var policy = policyService.GetByQuoteReferenceNumber(createResponse.Policy.QuoteReference);

            Assert.That(policy, Is.Not.Null);
            Assert.That(policy.SaveStatus, Is.EqualTo(PolicySaveStatus.CreatedLogin));

            Assert.That(QuoteHasInteraction(QuoteSessionContext.QuoteSession.QuoteReference, InteractionType.Saved_By_Customer));
        }

        [Test]
        public async Task GetCustomerDetails_EnterCustomerDetailsAndClickSave_FetchCustomerDetailsAgain_ReturnsCorrectDetails_Async()
        {
            //Arrange
            var createResponse = CreateAndLoginAsDefaultPolicy();

            var saveCustomerRequest = new SaveCustomerRequest
            {
                FirstName = "Karish",
                LastName = "Test",
                PhoneNumber = "0400000000",
                EmailAddress = "sauman@gmail.com"
            };

            //Act

            await Client.SaveDetailsAsync(createResponse.Risk.Id, saveCustomerRequest);
            var contactDetails = await Client.GetContactDetails<SaveCustomerParam>(createResponse.Risk.Id);

            //Assert
            Assert.That(contactDetails, Is.Not.Null);
            Assert.That(contactDetails.EmailAddress, Is.EqualTo(saveCustomerRequest.EmailAddress));
            Assert.That(contactDetails.FirstName, Is.EqualTo(saveCustomerRequest.FirstName));
            Assert.That(contactDetails.LastName, Is.EqualTo(saveCustomerRequest.LastName));
            Assert.That(contactDetails.PhoneNumber, Is.EqualTo(saveCustomerRequest.PhoneNumber));
        }
    }
}
