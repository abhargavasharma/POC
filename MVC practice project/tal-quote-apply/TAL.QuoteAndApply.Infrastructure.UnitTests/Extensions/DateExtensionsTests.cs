using System;
using NUnit.Framework;
using TAL.QuoteAndApply.Infrastructure.Extensions;

namespace TAL.QuoteAndApply.Infrastructure.UnitTests.Extensions
{
    [TestFixture]
    public class DateExtensionsTests
    {
        [Test]
        public void Age_TodayMinus30Years_AgeIs30()
        {
            var dob = DateTime.Today.AddYears(-30);

            var result = dob.Age();

            Assert.That(result, Is.EqualTo(30));
        }

        [Test]
        public void Age_YesterdayMinus30Years_AgeIs30()
        {
            var dob = DateTime.Today.AddYears(-30).AddDays(-1);

            var result = dob.Age();

            Assert.That(result, Is.EqualTo(30));
        }

        [Test]
        public void Age_TomorrowMinus30Years_AgeIs29()
        {
            var dob = DateTime.Today.AddYears(-30).AddDays(+1);

            var result = dob.Age();

            Assert.That(result, Is.EqualTo(29));
        }

        [Test]
        public void AgeNextBirthday_TodayMinus30Years_AgeIs31()
        {
            var dob = DateTime.Today.AddYears(-30);

            var result = dob.AgeNextBirthday();

            Assert.That(result, Is.EqualTo(31));
        }

        [Test]
        public void AgeNextBirthday_YesterdayMinus30Years_AgeIs31()
        {
            var dob = DateTime.Today.AddYears(-30).AddDays(-1);

            var result = dob.AgeNextBirthday();

            Assert.That(result, Is.EqualTo(31));
        }

        [Test]
        public void AgeNextBirthday_TomorrowMinus30Years_AgeIs30()
        {
            var dob = DateTime.Today.AddYears(-30).AddDays(+1);

            var result = dob.AgeNextBirthday();

            Assert.That(result, Is.EqualTo(30));
        }
    }
}
