using System;
using System.Web;
using System.Web.Helpers;
using Moq;
using NUnit.Framework;
using TAL.QuoteAndApply.Infrastructure.Crypto;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.Web.Shared.Cookie;
using TAL.QuoteAndApply.Web.Shared.Session;

namespace TAL.QuoteAndApply.Customer.Web.UnitTests
{
    [TestFixture]
    public class QuoteSessionContextTests
    {
        private Mock<ISecurityService> _mockSecurityService;
        private Mock<ICookieService> _mockCookieService;

        [SetUp]
        public void Setup()
        {
            _mockSecurityService = new Mock<ISecurityService>(MockBehavior.Strict);
            _mockCookieService = new Mock<ICookieService>(MockBehavior.Strict);
        }

        [Test]
        public void QuoteSession_WithCookieWithNoSessionData_ReturnsObjectWithCorrectQuoteReferenceAndNoSessionObject()
        {
            //This is to verify that previous cookies held in session won't break
            var service = new QuoteSessionContext(_mockSecurityService.Object, _mockCookieService.Object);
            const string quoteReference = "M123456789";
            const string cookieName = "testCookieName";

            _mockCookieService.Setup(call => call.GetCookieValue(It.IsAny<string>())).Returns(string.Concat(cookieName, "|", quoteReference));
            _mockSecurityService.Setup(call => call.Decrypt(It.IsAny<string>()))
                .Returns(string.Concat(cookieName, "|", quoteReference));

            var sessionObj = service.QuoteSession;

            Assert.That(sessionObj.QuoteReference, Is.Not.Null);
            Assert.That(sessionObj.QuoteReference, Is.EqualTo(quoteReference));
            Assert.That(sessionObj.SessionData, Is.Null);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void QuoteSession_WithCookieWithCallBackSet_ReturnsObjectWithCorrectCallBackSet(bool callBackRequested)
        {
            //This is for the new session data added with the inclusion of the flag set for the callback
            var service = new QuoteSessionContext(_mockSecurityService.Object, _mockCookieService.Object);
            const string quoteReference = "M123456789";
            const string cookieName = "testCookieName";
            var sessionDataObj = new SessionData()
            {
                CallBackRequested = callBackRequested
            };

            _mockCookieService.Setup(call => call.GetCookieValue(It.IsAny<string>())).Returns(string.Concat(cookieName, "|", quoteReference, "|", Json.Encode(sessionDataObj)));
            _mockSecurityService.Setup(call => call.Decrypt(It.IsAny<string>()))
                .Returns(string.Concat(cookieName, "|", quoteReference, "|", Json.Encode(sessionDataObj)));

            var sessionObj = service.QuoteSession;

            Assert.That(sessionObj.QuoteReference, Is.Not.Null);
            Assert.That(sessionObj.QuoteReference, Is.EqualTo(quoteReference));
            Assert.That(sessionObj.SessionData, Is.Not.Null);
            Assert.That(sessionObj.SessionData.CallBackRequested, Is.Not.Null);
            Assert.That(sessionObj.SessionData.CallBackRequested, Is.EqualTo(callBackRequested));
        }
        
        [Test]
        public void QuoteSession_WithCookieWithBadSessionData_SuccessfullyCatchesExceptionClearsCookieAndReturnsNull()
        {
            //This is for the new session data added with the inclusion of the flag set for the callback
            var service = new QuoteSessionContext(_mockSecurityService.Object, _mockCookieService.Object);
            const string quoteReference = "M123456789";
            const string cookieName = "testCookieName";
            var sessionDataObj = new QuoteSession();

            _mockCookieService.Setup(call => call.GetCookieValue(It.IsAny<string>())).Returns(string.Concat(cookieName, "|", quoteReference, "|", Json.Encode(sessionDataObj)));
            _mockSecurityService.Setup(call => call.Decrypt(It.IsAny<string>()))
                .Returns(string.Concat(cookieName, "|", quoteReference, "|", Json.Encode(sessionDataObj).Substring(1, Json.Encode(sessionDataObj).Length-1))); //Screw up QuoteSession json object
            _mockCookieService.Setup(call => call.ClearCookie(It.IsAny<string>()));

            var sessionObj = service.QuoteSession;

            Assert.That(sessionObj, Is.Null);
        }
    }
}
