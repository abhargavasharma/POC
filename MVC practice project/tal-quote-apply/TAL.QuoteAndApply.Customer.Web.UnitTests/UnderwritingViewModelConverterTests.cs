using NUnit.Framework;
using TAL.QuoteAndApply.Customer.Web.Converters;
using TAL.QuoteAndApply.ServiceLayer.Underwriting.Models;

namespace TAL.QuoteAndApply.Customer.Web.UnitTests
{
    [TestFixture]
    public class UnderwritingViewModelConverterTests
    {

        [Test]
        public void From_AnswerResponse_SetsAttributesCorrectly()
        {
            //Arrange
            var converter = new UnderwritingViewModelConverter();
            var answer = new UnderwritingAnswer("answer id", "answer text");

            //Act
            var model = converter.From(answer);

            //Assert
            Assert.That(model.Id, Is.EqualTo("answer id"));
            Assert.That(model.Text, Is.EqualTo("answer text"));
            Assert.That(model.IsSelected, Is.False);
        }

    }

}
