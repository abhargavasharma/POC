using System;
using NUnit.Framework;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;

namespace TAL.QuoteAndApply.ServiceLayer.UnitTests.Models
{
    [TestFixture]
    public class SmokerStatusHelperTests
    {
        [Test]
        public void Created_WithFalseIsSmoker_SetsCorrectAttributes()
        {
            var smokerStatusHelper = new SmokerStatusHelper(false);
            Assert.That(smokerStatusHelper.IsSmoker, Is.False);
            Assert.That(smokerStatusHelper.Status, Is.EqualTo(SmokerStatus.No));
        }

        [Test]
        public void Created_WithTrueIsSmoker_SetsCorrectAttributes()
        {
            var smokerStatusHelper = new SmokerStatusHelper(true);
            Assert.That(smokerStatusHelper.IsSmoker, Is.True);
            Assert.That(smokerStatusHelper.Status, Is.EqualTo(SmokerStatus.Yes));
        }


        [TestCase("Yes", true, SmokerStatus.Yes)]        
        [TestCase("yes", true, SmokerStatus.Yes)]        
        [TestCase("No", false, SmokerStatus.No)]        
        [TestCase("no", false, SmokerStatus.No)]        
        [TestCase("Unknown", false, SmokerStatus.Unknown)]        
        [TestCase(null, false, SmokerStatus.Unknown)]        
        public void Created_WithSmokerStatusString_SetsCorrectAttributes(string smokerEnumString, bool expectedIsSmoker, SmokerStatus expectedSmokerStatus)
        {
            var smokerStatusHelper = new SmokerStatusHelper(smokerEnumString);
            Assert.That(smokerStatusHelper.IsSmoker, Is.EqualTo(expectedIsSmoker));
            Assert.That(smokerStatusHelper.Status, Is.EqualTo(expectedSmokerStatus));
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void Created_WithRubbishStatusString_ThrowsException()
        {
            var smokerStatusHelper = new SmokerStatusHelper("I'm totally not a smoker status enum value");
        }

    }
}
