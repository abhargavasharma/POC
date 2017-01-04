
using System;
using System.Configuration;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NUnit.Framework;
using TAL.QuoteAndApply.Infrastructure.Http;
using TAL.QuoteAndApply.Policy.Configuration;
using TAL.QuoteAndApply.Policy.Service.RaisePolicy;
using TAL.QuoteAndApply.Tests.Shared.Mocks;

namespace TAL.QuoteAndApply.Policy.IntegrationTests.Services.RaisePolicy
{
    public class RaisePolicyServiceTests
    {
        private IHttpClientService _clientService;
        private IRaisePolicyConfigurationProvider _configuration;
        private IHttpRaisePolicyService _httpRaisePolicyService;
        private IHttpResponseMessageSerializer _httpResponseMessageSerializer;
        private IIHttpRequestMessageSerializer _httpRequestMessageSerializer;
        private IMimeTypeProvider _mimeTypeProvider;

        [SetUp]
        public void Setup()
        {
            _httpRequestMessageSerializer = new HttpRequestMessageSerializer();
            _httpResponseMessageSerializer = new HttpResponseMessageSerializer();
            _mimeTypeProvider = new MimeTypeProvider();
            _clientService = new HttpClientService(_httpResponseMessageSerializer, _mimeTypeProvider, _httpRequestMessageSerializer);
            _configuration = new RaisePolicyConfigurationProvider();

            _httpRaisePolicyService = new HttpRaisePolicyService(_clientService, _configuration, new MockLoggingService());
        }

        [Test]
        public void CreatePolicy_FromLocalXmlFile_CreatesPolicyWithNoException()
        {
            //Arrange
            var serializer = new XmlSerializer(typeof(PolicyNewBusinessOrderProcess_Type));
            var reader = new StreamReader(ConfigurationManager.AppSettings["RaisePolicy.SampleXml"]);
            var policyNewBusinessOrderProcess = (PolicyNewBusinessOrderProcess_Type)serializer.Deserialize(reader);
            reader.Close();
            policyNewBusinessOrderProcess.MessageHeader.MessageId = Guid.NewGuid().ToString();

            //Act
            var policy = _httpRaisePolicyService.Submit("M123456789", policyNewBusinessOrderProcess);

            //Assert
            Assert.That(policy, Is.Not.Null);
        }
    }
}
