using System;
using NUnit.Framework;
using TAL.QuoteAndApply.Infrastructure.Crypto;

namespace TAL.QuoteAndApply.Infrastructure.Tests
{
    [TestFixture]
    public class SecurityServiceTests
    {
        [Test]
        public void SecurityService_EncryptsAndDecrypt_IsSuccessful()
        {
            //Arrange
            var securityService = new SecurityService();
            const string originalString = "Never gonna give you up, never gonna let you down, Never gonna run around and desert you";

            //Act
            var encryptedString = securityService.Encrypt(originalString);
            var unEncryptedString = securityService.Decrypt(encryptedString);

            //Assert
            Assert.That(encryptedString, Is.Not.EqualTo(originalString));
            Assert.That(unEncryptedString, Is.EqualTo(originalString));
        }

        [Test, Ignore("Use this test if you want to encrypt a string")]
        public void Encrypt()
        {
            var securityService = new SecurityService();
            var encryptedString = securityService.Encrypt("Encrypt me baby, one more time");

            Console.WriteLine(encryptedString);
        }
    }
}
