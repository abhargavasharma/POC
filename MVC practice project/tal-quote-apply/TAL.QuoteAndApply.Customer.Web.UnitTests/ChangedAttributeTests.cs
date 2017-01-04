using System;
using NUnit.Framework;
using TAL.QuoteAndApply.Customer.Web.Models.Api.Underwriting;

namespace TAL.QuoteAndApply.Customer.Web.UnitTests
{
    [TestFixture]
    public class ChangedAttributeTests
    {
        [Test]
        public void Add_ObjectAttribtes_SetsCorrectDictionayValues()
        {
            var obj = new {StringAttribute = "I'm a string", BoolAttribute = true, IntAttribute = 47};

            var changedAttributes = new ChangedAttributes();
            changedAttributes.Add(() => obj.StringAttribute);
            changedAttributes.Add(() => obj.BoolAttribute);
            changedAttributes.Add(() => obj.IntAttribute);

            Assert.That(changedAttributes["StringAttribute"], Is.EqualTo("I'm a string"));
            Assert.That(changedAttributes["BoolAttribute"], Is.EqualTo(true));
            Assert.That(changedAttributes["IntAttribute"], Is.EqualTo(47));
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void Add_DuplilcateObjectAttributeName_ThrowsException()
        {
            var obj = new { StringAttribute = "I'm a string" };

            var changedAttributes = new ChangedAttributes();
            changedAttributes.Add(() => obj.StringAttribute);
            changedAttributes.Add(() => obj.StringAttribute);
        }
    }
}
