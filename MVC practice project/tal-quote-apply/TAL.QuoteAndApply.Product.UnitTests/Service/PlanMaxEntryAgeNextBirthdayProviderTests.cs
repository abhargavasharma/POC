using System.Collections.Generic;
using NUnit.Framework;
using TAL.QuoteAndApply.Product.Models.Definition;
using TAL.QuoteAndApply.Product.Service;

namespace TAL.QuoteAndApply.Product.UnitTests.Service
{
    [TestFixture]
    public class PlanMaxEntryAgeNextBirthdayProviderTests
    {
        [Test]
        public void GetMaxAgeFrom_MaximumEntryAgeNextBirthdayForOccupationClassIsNull_MaximumEntryAgeNextBirthdayReturned()
        {
            var occupationClass = "AAA";
            var planDefinition = new PlanDefinition
            {
                MaximumEntryAgeNextBirthdayForOccupationClass = null,
                MaximumEntryAgeNextBirthday = 100
            };

            var provider = new PlanMaxEntryAgeNextBirthdayProvider();
            var maxAge = provider.GetMaxAgeFrom(planDefinition, occupationClass);

            Assert.That(maxAge, Is.EqualTo(planDefinition.MaximumEntryAgeNextBirthday));
        }

        [Test]
        public void GetMaxAgeFrom_NoMaxAgeForOccupationClass_MaximumEntryAgeNextBirthdayReturned()
        {
            var occupationClass = "AAA";
            var planDefinition = new PlanDefinition
            {
                MaximumEntryAgeNextBirthdayForOccupationClass = new List<MaximumEntryAgeNextBirthdayForOccupationClassDefinition>
                {
                    new MaximumEntryAgeNextBirthdayForOccupationClassDefinition(100, "BBB")
                },
                MaximumEntryAgeNextBirthday = 100
            };

            var provider = new PlanMaxEntryAgeNextBirthdayProvider();
            var maxAge = provider.GetMaxAgeFrom(planDefinition, occupationClass);

            Assert.That(maxAge, Is.EqualTo(planDefinition.MaximumEntryAgeNextBirthday));
        }

        [Test]
        public void GetMaxAgeFrom_MaxAgeForOccupationClassFound_MaxAgeForOccupationClassReturned()
        {
            var occupationClass = "AAA";
            var maxAgeForOccClass = 200;

            var planDefinition = new PlanDefinition
            {
                MaximumEntryAgeNextBirthdayForOccupationClass = new List<MaximumEntryAgeNextBirthdayForOccupationClassDefinition>
                {
                    new MaximumEntryAgeNextBirthdayForOccupationClassDefinition(maxAgeForOccClass, occupationClass)
                },
                MaximumEntryAgeNextBirthday = 100
            };

            var provider = new PlanMaxEntryAgeNextBirthdayProvider();
            var maxAge = provider.GetMaxAgeFrom(planDefinition, occupationClass);

            Assert.That(maxAge, Is.EqualTo(maxAgeForOccClass));
        }

        [Test]
        public void GetMaxAgeFrom_MultipleMaxAgeForOccupationClassFound_FirstMaxAgeForOccupationClassReturned()
        {
            var occupationClass = "AAA";
            var maxAgeForOccClass = 200;

            var planDefinition = new PlanDefinition
            {
                MaximumEntryAgeNextBirthdayForOccupationClass = new List<MaximumEntryAgeNextBirthdayForOccupationClassDefinition>
                {
                    new MaximumEntryAgeNextBirthdayForOccupationClassDefinition(maxAgeForOccClass, occupationClass),
                    new MaximumEntryAgeNextBirthdayForOccupationClassDefinition(18, occupationClass),
                },
                MaximumEntryAgeNextBirthday = 100
            };

            var provider = new PlanMaxEntryAgeNextBirthdayProvider();
            var maxAge = provider.GetMaxAgeFrom(planDefinition, occupationClass);

            Assert.That(maxAge, Is.EqualTo(maxAgeForOccClass));
        }
    }
}
