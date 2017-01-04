using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using TAL.QuoteAndApply.DataModel.Personal;
using TAL.QuoteAndApply.Party.Rules;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.ServiceLayer.Policy.RaisePolicy;
using TAL.QuoteAndApply.ServiceLayer.Policy.RaisePolicy.Models;

namespace TAL.QuoteAndApply.ServiceLayer.UnitTests.Policy.RaisePolicy
{
    [TestFixture]
    public class RaisePolicyValidationServiceTests
    {
        [Test]
        public void ValidateOwnerForInforce_EmptyOwner()
        {
            var owner = new RaisePolicyOwner();
            var service = new RaisePolicyValidationService(new PartyRuleFactory());

            var validationErrors = service.ValidateOwnerForInforce(owner);
            var errors = validationErrors.Where(rr => !rr.IsSatisfied).ToList();

            Assert.That(errors.Any(e => e.Key == "Owner.FirstName"));
            Assert.That(errors.Any(e => e.Key == "Owner.Surname"));
            Assert.That(errors.Any(e => e.Key == "Owner.Title"));

            Assert.That(errors.Any(e => e.Key == "Owner.MobileNumber"));
            Assert.That(errors.Any(e => e.Key == "Owner.EmailAddress"));

            Assert.That(errors.Any(e => e.Key == "Owner.State"));
            Assert.That(errors.Any(e => e.Key == "Owner.Postcode"));
            Assert.That(errors.Any(e => e.Key == "Owner.Address"));
        }

        [Test]
        public void ValidateOwnerForInforce_ValidOwner()
        {
            var owner = new RaisePolicyOwner
            {
                Title = Title.Dr,
                FirstName = "John",
                Surname = "Smith",

                Address = "Foo Street",
                Postcode = "1111",
                Suburb = "Suburd",
                State = State.ACT,

                MobileNumber = "0411112222",
                EmailAddress = "foo@bar.com"
            };

            var service = new RaisePolicyValidationService(new PartyRuleFactory());
            var validationErrors = service.ValidateOwnerForInforce(owner);

            var errors = validationErrors.Where(rr => !rr.IsSatisfied).ToList();

            Assert.That(errors.Count(), Is.EqualTo(0));
        }

        [Test]
        public void ValidateOwnerExternalCustomerRefForInforce_EmptyExternalCustRef()
        {
            var party = new RaisePolicyRisk();

            var service = new RaisePolicyValidationService(new PartyRuleFactory());
            var validationErrors = service.ValidateOwnerExternalCustomerRefForInforce(party.ExternalCustomerReference);

            var errors = validationErrors.Where(rr => !rr.IsSatisfied).ToList();

            Assert.That(errors.Any(e => e.Key == "Owner.ExternalCustomerReference"));
        }

        [Test]
        public void ValidateOwnerExternalCustomerRefForInforce_ValidExternalCustRef()
        {
            var owner = new RaisePolicyOwner
            {
                Title = Title.Dr,
                FirstName = "John",
                Surname = "Smith",

                Address = "Foo Street",
                Postcode = "1111",
                Suburb = "Suburd",
                State = State.ACT,

                MobileNumber = "0411112222",
                EmailAddress = "foo@bar.com",
                ExternalCustomerReference = "TEST122"
            };

            var service = new RaisePolicyValidationService(new PartyRuleFactory());
            var validationErrors = service.ValidateOwnerExternalCustomerRefForInforce(owner.ExternalCustomerReference);

            var errors = validationErrors.Where(rr => !rr.IsSatisfied).ToList();

            Assert.That(errors.Count(), Is.EqualTo(0));
        }

        [Test]
        public void ValidatePartyForInforce_ValidateEmptyRisk()
        {
            var party = new RaisePolicyRisk();

            var service = new RaisePolicyValidationService(new PartyRuleFactory());
            var validationErrors = service.ValidatePartyForInforce(party);

            var errors = validationErrors.Where(rr => !rr.IsSatisfied).ToList();

            Assert.That(errors.Any(e => e.Key == "Party.FirstName"));
            Assert.That(errors.Any(e => e.Key == "Party.Title"));
            Assert.That(errors.Any(e => e.Key == "Party.Postcode"));
            Assert.That(errors.Any(e => e.Key == "Party.Surname"));

            //we dont require address
            Assert.That(errors.All(e => e.Key != "Party.Address"));
        }

        [Test]
        public void ValidatePartyForInforce_ValidRisk()
        {
            var party = new RaisePolicyRisk
            {
                Title = Title.Mr,
                FirstName = "Test",
                Surname = "Testy",

                Postcode = "3000"
            };

            var service = new RaisePolicyValidationService(new PartyRuleFactory());
            var validationErrors = service.ValidatePartyForInforce(party);

            var errors = validationErrors.Where(rr => !rr.IsSatisfied).ToList();

            Assert.That(errors.Count, Is.EqualTo(0));
        }
    }
}
