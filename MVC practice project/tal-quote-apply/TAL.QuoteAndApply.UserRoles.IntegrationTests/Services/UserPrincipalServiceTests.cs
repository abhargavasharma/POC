using System.Linq;
using NUnit.Framework;
using TAL.QuoteAndApply.UserRoles.Configuration;
using TAL.QuoteAndApply.UserRoles.Services;

namespace TAL.QuoteAndApply.UserRoles.IntegrationTests.Services
{
    [TestFixture]
    public class UserPrincipalServiceTests
    {
        private IUserPrincipalService _userPrincipalService;

        [TestFixtureSetUp]
        public void Setup()
        {
            _userPrincipalService = new UserPrincipalService();
        }

        [Test]
        public void AuthenticateCurrentWindowsUser_UserInAppropriateRoles_Success()
        {
            var result = _userPrincipalService.GetUsersInGroup("TOWER", ".uTalConsumerUnderwriter_QA").ToList();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.GreaterThan(0));
        }
    }
}