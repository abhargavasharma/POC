using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using NUnit.Framework;
using TAL.Performance.Infrastructure.Core;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.SalesPortal.Web.Extensions;
using TAL.QuoteAndApply.SalesPortal.Web.IntegrationTests.Clients;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using Task=System.Threading.Tasks.Task;

namespace TAL.QuoteAndApply.SalesPortal.Web.IntegrationTests
{
    public class BeneficiaryApiTests : BaseTestClass<BeneficiaryClient>
    {
        public BeneficiaryApiTests() : base(SimplePerformanceTool.CreateConsoleTimestamped())
        {
            
        }

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            
        }

        [Test]
        public async Task PostBeneficiary_BeneficiaryCreated_Async()
        {
            var policyWithRisks = await SetUpPolicyWithRisks();

            var risk = policyWithRisks.Risks.First();

            var beneficiaryRequest = SetupCleanBeneficiaryRequest(risk.Risk.Id);
            
            var result = await CreateCleanBeneficiary(policyWithRisks, beneficiaryRequest, risk.Risk.Id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(1));
            var returnedReferral = result.First();
            Assert.That(returnedReferral.EmailAddress, Is.EqualTo(beneficiaryRequest[0].EmailAddress));
            Assert.That(returnedReferral.Address, Is.EqualTo(beneficiaryRequest[0].Address));
            Assert.That(returnedReferral.BeneficiaryRelationshipId, Is.EqualTo(1));
            Assert.That(returnedReferral.DateOfBirth, Is.EqualTo(beneficiaryRequest[0].DateOfBirth));
            Assert.That(returnedReferral.FirstName, Is.EqualTo(beneficiaryRequest[0].FirstName));
            Assert.That(returnedReferral.Id, Is.Not.Null);
            Assert.That(returnedReferral.PhoneNumber, Is.EqualTo(beneficiaryRequest[0].PhoneNumber));
            Assert.That(returnedReferral.Postcode, Is.EqualTo(beneficiaryRequest[0].Postcode));
            Assert.That(returnedReferral.RiskId, Is.EqualTo(risk.Risk.Id));
            Assert.That(returnedReferral.Share, Is.EqualTo(beneficiaryRequest[0].Share));
            Assert.That(returnedReferral.State, Is.EqualTo(beneficiaryRequest[0].State));
            Assert.That(returnedReferral.Suburb, Is.EqualTo(beneficiaryRequest[0].Suburb));
            Assert.That(returnedReferral.Surname, Is.EqualTo(beneficiaryRequest[0].Surname));
            Assert.That(returnedReferral.Title, Is.EqualTo(beneficiaryRequest[0].Title));
            Assert.That(returnedReferral.HasEmptyFields, Is.EqualTo(false));
        }

        private List<BeneficiaryDetailsRequest> SetupCleanBeneficiaryRequest(int riskId)
        {
            return new[]
            {
                new BeneficiaryDetailsRequest(){
                    Address = "12 Happy St",
                    DateOfBirth = "12/12/1987",
                    FirstName = "Jim",
                    BeneficiaryRelationshipId = 1,
                    Share = "100",
                    Surname = "Test",
                    Suburb = "Sunnyville",
                    State = "VIC",
                    Postcode = "4444",
                    Title = "Mr",
                    PhoneNumber = "0400000000",
                    EmailAddress = "test@test.com",
                    RiskId = riskId
                }
            }.ToList();
        }

        private async Task<IEnumerable<BeneficiaryDetailsRequest>> CreateCleanBeneficiary(PolicyWithRisks policyWithRisks, List<BeneficiaryDetailsRequest> beneficiaryRequest, int riskId)
        {
            var response = await Client.CreateUpdateBeneficiariesAsync<IEnumerable<BeneficiaryDetailsRequest>>(policyWithRisks.Policy.QuoteReference, riskId, beneficiaryRequest,
                throwOnFailure: false);
            return response;
        }

        private async Task<PolicyWithRisks> SetUpPolicyWithRisks()
        {
            AddUnderwriterPermission();

            var policyWithRisks = await CreatePolicyFromRatingFactorsAsync(new RatingFactorsRequest
            {
                AustralianResident = true,
                DateOfBirth = DateTime.Now.AddYears(-35).ToFriendlyString(),
                Gender = 'M',
                Income = 100000,
                OccupationCode = "229",
                IndustryCode = "27l"
            });

            return policyWithRisks;
        }

        [Test]
        public async Task PostBeneficiary_BeneficiaryCreatedWithNoFields_TotalShareErrorMessageReturned_Async()
        {
            var policyWithRisks = await SetUpPolicyWithRisks();

            var risk = policyWithRisks.Risks.First();

            var beneficiaryRequest = new[]
            {
                new BeneficiaryDetailsRequest(){
                    Address = null,
                    DateOfBirth = null,
                    FirstName = null,
                    BeneficiaryRelationshipId = 1,
                    Surname = null,
                    Suburb = null,
                    State = null,
                    Postcode = "abc",
                    Title = null,
                    PhoneNumber = "abc",
                    EmailAddress = null,
                    RiskId = risk.Risk.Id
                }
            }.ToList();

            var response = await Client.CreateUpdateBeneficiariesAsync<Dictionary<string, IEnumerable<string>>>(policyWithRisks.Policy.QuoteReference, risk.Risk.Id, beneficiaryRequest,
                throwOnFailure: false);

            Assert.That(response, Is.Not.Null);
            Assert.That(response.Count, Is.EqualTo(3));
            Assert.That(response.ContainsKey("beneficiaryDetailsRequest[0].share"), Is.True);
            Assert.That(response.First(x => x.Key.Equals("beneficiaryDetailsRequest[0].share")).Value.First(), Is.EqualTo("Benefit must not be less than 1%"));
            var postcodeResponse =
                response.First(x => x.Key.Equals("beneficiaryDetailsRequest[0].postcode")).Value.ToList();
            Assert.Contains("Postcode must not contain any non numeric values", postcodeResponse);
            Assert.Contains("Post code must be 4 digits", postcodeResponse);
            Assert.Contains("Postcode is not valid", postcodeResponse);
            var phoneNumberResponse =
                response.First(x => x.Key.Equals("beneficiaryDetailsRequest[0].phoneNumber")).Value.ToList();
            Assert.That(phoneNumberResponse.Count, Is.EqualTo(4));
        }

        [Test]
        public async Task PostBeneficiary_BeneficiaryUpdateWithNoFields_RequiredErrorMessagesReturned_Async()
        {
            var policyWithRisks = await SetUpPolicyWithRisks();

            var risk = policyWithRisks.Risks.First();
            risk.Risk.LprBeneficiary = false;

            var riskService = Container.Resolve<IRiskService>();
            riskService.UpdateRisk(risk.Risk);

            var beneficiaryRequest = new[]
            {
                new BeneficiaryDetailsRequest(){
                    Address = "",
                    DateOfBirth = "12/12/1987",
                    FirstName = "",
                    BeneficiaryRelationshipId = 1,
                    Share = "100",
                    Surname = "",
                    Suburb = "",
                    State = "",
                    Postcode = "",
                    Title = "",
                    PhoneNumber = "",
                    EmailAddress = "",
                    RiskId = risk.Risk.Id
                }
            }.ToList();

            var response = await Client.CreateUpdateBeneficiariesAsync<IEnumerable<BeneficiaryDetailsRequest>>(policyWithRisks.Policy.QuoteReference, risk.Risk.Id, beneficiaryRequest,
                throwOnFailure: false);

            Assert.That(response, Is.Not.Null);
            Assert.That(response.Count(), Is.GreaterThan(0));

            var updateResponse = await Client.ValidateAsync<Dictionary<string, IEnumerable<string>>>(policyWithRisks.Policy.QuoteReference, risk.Risk.Id, throwOnFailure: false);

            Assert.That(updateResponse, Is.Not.Null);
            Assert.That(updateResponse.First(x => x.Key.Equals("beneficiaryDetailsRequest[0].address")).Value.First(), Is.EqualTo("Address is required"));
            Assert.That(updateResponse.First(x => x.Key.Equals("beneficiaryDetailsRequest[0].firstName")).Value.First(), Is.EqualTo("First name is required"));
            Assert.That(updateResponse.First(x => x.Key.Equals("beneficiaryDetailsRequest[0].surname")).Value.First(), Is.EqualTo("Surname is required"));
            Assert.That(updateResponse.First(x => x.Key.Equals("beneficiaryDetailsRequest[0].suburb")).Value.First(), Is.EqualTo("Suburb is required"));
            Assert.That(updateResponse.First(x => x.Key.Equals("beneficiaryDetailsRequest[0].state")).Value.First(), Is.EqualTo("State is required"));
            Assert.That(updateResponse.First(x => x.Key.Equals("beneficiaryDetailsRequest[0].postcode")).Value.First(), Is.EqualTo("Postcode is required"));
            Assert.That(updateResponse.First(x => x.Key.Equals("beneficiaryDetailsRequest[0].title")).Value.First(), Is.EqualTo("Title is required"));
            Assert.That(updateResponse.First(x => x.Key.Equals("beneficiaryDetailsRequest[0].phoneNumber")).Value.First(), Is.EqualTo("Phone number is required"));
        }

        [Test]
        public async Task PostBeneficiary_BeneficiaryUpdateWithFieldsWithCharMaxLengthsExceeded_MaxCharLengthErrorMessagesReturned_Async()
        {
            var policyWithRisks = await SetUpPolicyWithRisks();

            var risk = policyWithRisks.Risks.First();
            risk.Risk.LprBeneficiary = false;

            var riskService = Container.Resolve<IRiskService>();
            riskService.UpdateRisk(risk.Risk);

            var beneficiaryRequest = new[]
            {
                new BeneficiaryDetailsRequest(){
                    Address = "123 Test Streeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeettttttttttttt",
                    DateOfBirth = "12/12/1987",
                    FirstName = "Testfirstnaaaaaaaaaamee",
                    BeneficiaryRelationshipId = 1,
                    Share = "100",
                    Surname = "TestSurnaaaaaaaaaaaamee",
                    Suburb = "TestSuburbbbbbbbbbbbb",
                    State = "VIC",
                    Postcode = "1234",
                    Title = "Mr",
                    PhoneNumber = "022222222222",
                    EmailAddress = "test@test.commmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm",
                    RiskId = risk.Risk.Id
                }
            }.ToList();

            var updateResponse = await Client.CreateUpdateBeneficiariesAsync<Dictionary<string, IEnumerable<string>>>(policyWithRisks.Policy.QuoteReference, risk.Risk.Id, beneficiaryRequest,
                throwOnFailure: false);

            Assert.That(updateResponse, Is.Not.Null);
            Assert.That(updateResponse.First(x => x.Key.Equals("beneficiaryDetailsRequest[0].address")).Value.First(), Is.EqualTo("Address cannot be longer than 30 characters."));
            Assert.That(updateResponse.First(x => x.Key.Equals("beneficiaryDetailsRequest[0].firstName")).Value.First(), Is.EqualTo("First name cannot be longer than 22 characters."));
            Assert.That(updateResponse.First(x => x.Key.Equals("beneficiaryDetailsRequest[0].surname")).Value.First(), Is.EqualTo("Surname cannot be longer than 22 characters."));
            Assert.That(updateResponse.First(x => x.Key.Equals("beneficiaryDetailsRequest[0].suburb")).Value.First(), Is.EqualTo("Suburb cannot be longer than 20 characters."));
            Assert.That(updateResponse.First(x => x.Key.Equals("beneficiaryDetailsRequest[0].emailAddress")).Value.First(), Is.EqualTo("Email address cannot be longer than 80 characters."));
        }

        [Test]
        public async Task UpdateOptions_WithLprSetToTrue_GetOptionsReturnsLprAsTrue_Async()
        {
            var policyWithRisks = await SetUpPolicyWithRisks();

            var risk = policyWithRisks.Risks.First();

            var beneficiaryRequest = SetupCleanBeneficiaryRequest(risk.Risk.Id);
            
            await CreateCleanBeneficiary(policyWithRisks, beneficiaryRequest, risk.Risk.Id);

            var options = new BeneficiaryOptionsRequest()
            {
                NominateLpr = true
            };

            await Client.UpdateBeneficiaryOptionsAsync(policyWithRisks.Policy.QuoteReference, risk.Risk.Id, options, throwOnFailure: false);

            var optionsResponse = await Client.GetOptionsAsync(policyWithRisks.Policy.QuoteReference, risk.Risk.Id);

            Assert.That(optionsResponse, Is.Not.Null);
            Assert.That(optionsResponse.NominateLpr, Is.True);
        }

        [Test]
        public async Task PostBeneficiary_CreateMultipleBeneficiaries_GetBeneficiariesAsyncReturnsAll_Async()
        {
            var policyWithRisks = await SetUpPolicyWithRisks();

            var risk = policyWithRisks.Risks.First();

            var beneficiaryRequest = new[]
            {
                new BeneficiaryDetailsRequest(){
                    Address = "12 Happy St",
                    DateOfBirth = "12/12/1987",
                    FirstName = "Jim",
                    BeneficiaryRelationshipId = 1,
                    Share = "20",
                    Surname = "Test",
                    Suburb = "Sunnyville",
                    State = "VIC",
                    Postcode = "4444",
                    Title = "Mr",
                    PhoneNumber = "0400000000",
                    EmailAddress = "test@test.com.au",
                    RiskId = risk.Risk.Id
                },
                new BeneficiaryDetailsRequest(){
                    Address = "12 Happy St",
                    DateOfBirth = "12/12/1987",
                    FirstName = "Jimmy",
                    BeneficiaryRelationshipId = 2,
                    Share = "20",
                    Surname = "Test",
                    Suburb = "Sunnyville",
                    State = "VIC",
                    Postcode = "4444",
                    Title = "Mr",
                    PhoneNumber = "0400000000",
                    EmailAddress = "test@tal.com.au",
                    RiskId = risk.Risk.Id
                }
            }.ToList();

            var update = await Client.CreateUpdateBeneficiariesAsync<IEnumerable<BeneficiaryDetailsRequest>>(policyWithRisks.Policy.QuoteReference, risk.Risk.Id, beneficiaryRequest);

            var beneficiaries = await Client.GetBeneficiariesAsync(policyWithRisks.Policy.QuoteReference, risk.Risk.Id);

            Assert.That(beneficiaries, Is.Not.Null);
            Assert.That(beneficiaries.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task DeleteBeneficiary_BeneficiaryCreatedAndSuccessfullyDeleted_Async()
        {
            var policyWithRisks = await SetUpPolicyWithRisks();

            var riskId = policyWithRisks.Risks.First().Risk.Id;

            var beneficiaryRequest = SetupCleanBeneficiaryRequest(riskId);

            var result = await CreateCleanBeneficiary(policyWithRisks, beneficiaryRequest, riskId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(1));

            await Client.DeleteBeneficiaryAsync(policyWithRisks.Policy.QuoteReference, riskId, result.First().Id);

            var beneficiaries = await Client.GetBeneficiariesAsync(policyWithRisks.Policy.QuoteReference, riskId);

            Assert.That(beneficiaries, Is.Not.Null);
            Assert.That(beneficiaries.Count(), Is.EqualTo(0));
        }
    }
}