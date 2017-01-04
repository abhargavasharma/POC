using System.Collections.Specialized;
using System.Web.Mvc;
using NUnit.Framework;
using TAL.QuoteAndApply.Customer.Web.Models.Binders;
using TAL.QuoteAndApply.Tests.Shared.Mocks;

namespace TAL.QuoteAndApply.Customer.Web.UnitTests.Models.Binders
{
    [TestFixture]
    public class JsonModelBinderTests
    {
        [Test]
        public void BindModel_WhenNoDataIsPostedForModel_NullReturned()
        {
            var modelName = "TEST";

            var formCollection = new NameValueCollection();

            var valueProvider = new NameValueCollectionValueProvider(formCollection, null);

            var bindingContext = GetModelBindingContext(modelName, valueProvider);

            var modelBinder = new JsonModelBinder();
            var result = modelBinder.BindModel(null, bindingContext);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void BindModel_WhenNullIsPostedForModel_NullReturned()
        {
            var modelName = "TEST";

            var formCollection = new NameValueCollection
            {
                { modelName, null },
            };

            var valueProvider = new NameValueCollectionValueProvider(formCollection, null);

            var bindingContext = GetModelBindingContext(modelName, valueProvider);

            var modelBinder = new JsonModelBinder();
            var result = modelBinder.BindModel(null, bindingContext);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void BindModel_WhenBlankStringIsPostedForModel_NullReturned()
        {
            var modelName = "TEST";

            var formCollection = new NameValueCollection
            {
                { modelName, "" },
            };

            var valueProvider = new NameValueCollectionValueProvider(formCollection, null);

            var bindingContext = GetModelBindingContext(modelName, valueProvider);

            var modelBinder = new JsonModelBinder();
            var result = modelBinder.BindModel(null, bindingContext);

            Assert.That(result, Is.Null);
        }

       
        [Test]
        public void BindModel_WhenInvalidJsonIsPostedForModel_NullReturned()
        {
            var modelName = "TEST";

            var formCollection = new NameValueCollection
            {
                { modelName, "{asdsa=asdsadsa}" },
            };

            var valueProvider = new NameValueCollectionValueProvider(formCollection, null);

            var bindingContext = GetModelBindingContext(modelName, valueProvider);

            var modelBinder = new JsonModelBinder(new MockLoggingService());
            var result = modelBinder.BindModel(null, bindingContext);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void BindModel_WhenBlankJsonIsPostedForModel_EmptyFakeClassReturned()
        {
            var modelName = "TEST";

            var formCollection = new NameValueCollection
            {
                { modelName, "{}" },
            };

            var valueProvider = new NameValueCollectionValueProvider(formCollection, null);

            var bindingContext = GetModelBindingContext(modelName, valueProvider);

            var modelBinder = new JsonModelBinder();
            var result = modelBinder.BindModel(null, bindingContext);

            Assert.That(result, Is.TypeOf<FakeClass>());

            var typedResult = result as FakeClass;
            Assert.That(typedResult.TestProperty, Is.Null);
        }

        [Test]
        public void BindModel_WhenValidJsonIsPostedForModel_FakeClassReturned()
        {
            var modelName = "TEST";

            var formCollection = new NameValueCollection
            {
                { modelName, "{TestProperty:'Test'}" },
            };

            var valueProvider = new NameValueCollectionValueProvider(formCollection, null);

            var bindingContext = GetModelBindingContext(modelName, valueProvider);

            var modelBinder = new JsonModelBinder();
            var result = modelBinder.BindModel(null, bindingContext);

            Assert.That(result, Is.TypeOf<FakeClass>());

            var typedResult = result as FakeClass;
            Assert.That(typedResult.TestProperty, Is.EqualTo("Test"));
        }

        private ModelBindingContext GetModelBindingContext(string modelName, NameValueCollectionValueProvider valueProviderDictionary)
        {
            var metadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(FakeClass));
            var bindingContext = new ModelBindingContext
            {
                ModelName = modelName,
                ModelMetadata = metadata,
                ValueProvider = valueProviderDictionary
            };

            return bindingContext;
        }

        public class FakeClass
        {
            public string TestProperty { get; set; }
        }
    }
}
