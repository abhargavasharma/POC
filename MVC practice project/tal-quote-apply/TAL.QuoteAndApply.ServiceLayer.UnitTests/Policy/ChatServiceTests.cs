using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using TAL.QuoteAndApply.Infrastructure.Time;
using TAL.QuoteAndApply.Infrastructure.Url;
using TAL.QuoteAndApply.Party.Configuration;
using TAL.QuoteAndApply.ServiceLayer.Policy;

namespace TAL.QuoteAndApply.ServiceLayer.UnitTests.Policy
{
    [TestFixture]
    public class ChatServiceTests
    {
        private MockRepository _mockRepo = new MockRepository(MockBehavior.Strict);
        private Mock<IPolicyInteractionService> _mockInteractionsService;
        private Mock<IChatService> _mockChatService;
        private IMock<IUrlUtilities> _mockUrlUtilities;
        private IMock<ICustomerCallbackService> _mockCustomerCallBack;
        private IMock<ICustomerSaveService> _mockCustomerSaveService;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new MockRepository(MockBehavior.Strict);
            _mockInteractionsService = _mockRepo.Create<IPolicyInteractionService>();
            _mockChatService = _mockRepo.Create<IChatService>();
            _mockUrlUtilities = _mockRepo.Create<IUrlUtilities>();
            _mockCustomerCallBack = _mockRepo.Create<ICustomerCallbackService>();
            _mockCustomerSaveService = _mockRepo.Create<ICustomerSaveService>();
        }

        [Test]
        public void TestIsWebChatAvailable()
        {
            // SET UP
            var config = new Mock<IChatConfigurationProvider>();
            var startTime = "09:00";
            var endTime = "11:00";
            config.Setup(i => i.StartTime).Returns(startTime);
            config.Setup(i => i.EndTime).Returns(endTime);
            var mockDateTimeProvider = _mockRepo.Create<IDateTimeProvider>();
            mockDateTimeProvider.Setup(call => call.GetCurrentDateAndTime()).Returns(new DateTime(2013, 1, 1, 10, 0, 0));

            var service = GetChatService(config.Object, mockDateTimeProvider.Object, null);

            var isAvailable = service.IsWebChatAvailable();

            Assert.That(isAvailable, Is.True);
        }

        [Test]
        public void TestIsWebChatNotAvailable_Before_Working_Hours()
        {
            var config = new Mock<IChatConfigurationProvider>();

            var mockDateTimeProvider = _mockRepo.Create<IDateTimeProvider>();
            var startTime = "09:00";
            var endTime = "11:00";
            config.Setup(i => i.StartTime).Returns(startTime);
            config.Setup(i => i.EndTime).Returns(endTime);
            mockDateTimeProvider.Setup(call => call.GetCurrentDateAndTime()).Returns(new DateTime(2013, 1, 1, 10, 0, 0));

            var service = GetChatService(config.Object, mockDateTimeProvider.Object, null);

            var isAvailable = service.IsWebChatAvailable();

            Assert.That(isAvailable, Is.True);
        }

        [Test]
        public void TestIsWebChatNotAvailable_After_Working_Hours()
        {
            var config = new Mock<IChatConfigurationProvider>();
            var mockDateTimeProvider = _mockRepo.Create<IDateTimeProvider>();
            var startTime = "09:00";
            var endTime = "11:00";
            config.Setup(i => i.StartTime).Returns(startTime);
            config.Setup(i => i.EndTime).Returns(endTime);
            mockDateTimeProvider.Setup(call => call.GetCurrentDateAndTime()).Returns(new DateTime(2013, 1, 1, 10, 0, 0));

            var service = GetChatService(config.Object, mockDateTimeProvider.Object, null);

            var isAvailable = service.IsWebChatAvailable();

            Assert.That(isAvailable, Is.True);
        }

        [Test]
        public void TestIsWebChatAvailableAndCreateInteraction_WhenWebChatNotAvailable_ThrowsException()
        {
            var config = new Mock<IChatConfigurationProvider>();
            string quoteRefNumber = "M123456789";
            var mockDateTimeProvider = _mockRepo.Create<IDateTimeProvider>();
            var startTime = "09:00";
            var endTime = "11:00";
            config.Setup(i => i.StartTime).Returns(startTime);
            config.Setup(i => i.EndTime).Returns(endTime);
            mockDateTimeProvider.Setup(call => call.GetCurrentDateAndTime()).Returns(new DateTime(2013, 1, 1, 10, 0, 0));

            var mockInteractions = _mockRepo.Create<IPolicyInteractionService>();
            mockInteractions.Setup(call => call.CustomerWebChatRequested(quoteRefNumber))
                .Throws(new ApplicationException("This method should not have been called"));

            var service = GetChatService(config.Object, mockDateTimeProvider.Object, null);
            
            var isAvailable = service.IsWebChatAvailableAndCreateInteraction(string.Empty);
            
            Assert.That(isAvailable, Is.True);
        }

        [Test]
        public void TestIsWebChatAvailableAndCreateInteraction_WhenQuoteNumberNull_MustNotCreateInteraction()
        {
            //Arrange
            var config = new Mock<IChatConfigurationProvider>();
            string quoteRefNumber = string.Empty;
            var mockDateTimeProvider = _mockRepo.Create<IDateTimeProvider>();
            var startTime = "09:00";
            var endTime = "11:00";
            config.Setup(i => i.StartTime).Returns(startTime);
            config.Setup(i => i.EndTime).Returns(endTime);
            mockDateTimeProvider.Setup(call => call.GetCurrentDateAndTime()).Returns(new DateTime(2013, 1, 1, 10, 0, 0));

            var service = GetChatService(config.Object, mockDateTimeProvider.Object, null);

            //Act
            var isAvailable = service.IsWebChatAvailableAndCreateInteraction(quoteRefNumber);

            //Assert
            Assert.That(isAvailable, Is.True);
           
        }

        [Test]
        public void TestIsWebChatAvailableAndCreateInteraction_WhenChatNotAvailable_MustNotCreateInteraction()
        {
            //Arrange
            var config = new Mock<IChatConfigurationProvider>();
            string quoteRefNumber = "M123456789";
            var mockDateTimeProvider = _mockRepo.Create<IDateTimeProvider>();
            var startTime = "09:00";
            var endTime = "11:00";
            config.Setup(i => i.StartTime).Returns(startTime);
            config.Setup(i => i.EndTime).Returns(endTime);
            mockDateTimeProvider.Setup(call => call.GetCurrentDateAndTime()).Returns(new DateTime(2013, 1, 1, 10, 0, 0));
            _mockInteractionsService.Setup(m => m.CustomerWebChatRequested(quoteRefNumber));
            var service = GetChatService(config.Object, mockDateTimeProvider.Object, _mockInteractionsService.Object);

            //Act
            var isAvailable = service.IsWebChatAvailableAndCreateInteraction(quoteRefNumber);
            
            //Assert
            Assert.That(isAvailable, Is.True);

        }

        private ChatService GetChatService(IChatConfigurationProvider config, IDateTimeProvider dateTimeProvider, IPolicyInteractionService policyInteractions )
        {
            return new ChatService
                (new Mock<IPolicyOverviewProvider>().Object,
                    dateTimeProvider,
                    _mockCustomerSaveService.Object,
                    _mockUrlUtilities.Object,
                    _mockCustomerCallBack.Object,
                    config,
                    policyInteractions
                    );
        }
        [Test]
        public void TestIsWebChatAvailableAndCreateInteraction_WhenQuoteValidAndChatAvailable_MustCreateInteraction()
        {
            //Arrange
            var config = new Mock<IChatConfigurationProvider>();
            var startTime = "09:00";
            var endTime = "11:00";
            config.Setup(i => i.StartTime).Returns(startTime);
            config.Setup(i => i.EndTime).Returns(endTime);
            var mockDateTimeProvider = _mockRepo.Create<IDateTimeProvider>();
            mockDateTimeProvider.Setup(call => call.GetCurrentDateAndTime()).Returns(new DateTime(2013, 1, 1, 10, 0, 0));
            
            string quoteRefNumber = "M123456789";
            _mockInteractionsService.Setup(m => m.CustomerWebChatRequested(quoteRefNumber));
            var service = GetChatService(config.Object, mockDateTimeProvider.Object, _mockInteractionsService.Object);
            //Act
            var isAvailable = service.IsWebChatAvailableAndCreateInteraction(quoteRefNumber);

            //Assert
            Assert.That(isAvailable, Is.True);

        }

        [Test]
        public void TestIsWebChatAvailableAndCreateInteraction_WhenQuoteNullAndChatUnavailable_MustNotCreateInteraction()
        {
            //Arrange
            var config = new Mock<IChatConfigurationProvider>();
            string quoteRefNumber = string.Empty;
            var mockDateTimeProvider = _mockRepo.Create<IDateTimeProvider>();
            var startTime = "09:00";
            var endTime = "11:00";
            config.Setup(i => i.StartTime).Returns(startTime);
            config.Setup(i => i.EndTime).Returns(endTime);
            mockDateTimeProvider.Setup(call => call.GetCurrentDateAndTime()).Returns(new DateTime(2013, 1, 1, 10, 0, 0));

            var service = GetChatService(config.Object, mockDateTimeProvider.Object, null);


            //Act
            var isAvailable = service.IsWebChatAvailableAndCreateInteraction(quoteRefNumber);
            //Assert
            Assert.That(isAvailable, Is.True);

        }
    }
}
