using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TAL.Performance.Infrastructure.Core;
using TAL.QuoteAndApply.SalesPortal.Web.IntegrationTests.Clients;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;
using TAL.QuoteAndApply.Web.Shared.HttpResults;
using Task = System.Threading.Tasks.Task;

namespace TAL.QuoteAndApply.SalesPortal.Web.IntegrationTests
{
    [TestFixture]
    public class LoginApiTests : BaseTestClass<LoginClient>
    {
        public LoginApiTests() : base(SimplePerformanceTool.CreateConsoleTimestamped())
        {
        }

        [Test]
        public async Task LoginAttempt_EmptyObjectPosted_RequiredFieldErrorsReturned_Async()
        {
            var loginRequest = new LoginRequest();

            var response = await Client.AttemptLoginAsync<Dictionary<string, IEnumerable<string>>>(loginRequest, false);

            Assert.That(response, Is.Not.Null);
            Assert.That(response.ContainsKey("loginRequest.userName"), Is.True);
            Assert.That(response.ContainsKey("loginRequest.password"), Is.True);
        }

        [Test]
        public async Task LoginAttempt_UseCredentialsWithNoPermissions_ErrorsReturned_Async()
        {
            var loginRequest = new LoginRequest
            {
                UserName = "TalConsmrNoPrmsns_QA",
                Password = "Transform2001!",
                UseWindowsAuth = false
            };

            var response = await Client.AttemptLoginAsync<Dictionary<string, IEnumerable<string>>>(loginRequest, false);

            Assert.That(response, Is.Not.Null);
            Assert.That(response.ContainsKey("loginRequest"), Is.True);
        }

        [Test]
        public async Task LoginAttempt_UseCredentialsWithPermissions_SuccessfulLogin_Async()
        {
            var loginRequest = new LoginRequest
            {
                UserName = "TalConsmrReadOnly_QA",
                Password = "Transform2001!",
                UseWindowsAuth = false
            };

            var response = await Client.AttemptLoginAsync<RedirectToResponse>(loginRequest, false);

            Assert.That(response, Is.Not.Null);
            Console.WriteLine(response.RedirectTo);
        }

        [Test]
        public async Task GetBrandsForUser_UseUnderwriterLogin_ReturnsMaverickYellowAndTalBrands_Async()
        {
            var loginRequest = new LoginRequest
            {
                UserName = "TalConsmrUndwrter_QA",
                Password = "Transform2001!",
                UseWindowsAuth = false
            };

            var response = await Client.AttemptLoginAsync<RedirectToResponse>(loginRequest, false);

            Assert.That(response, Is.Not.Null);
            Console.WriteLine(response.RedirectTo);

            var brands = await Client.GetBrandsAsync<BrandsLoginModel>();
            var brandList = brands.Brands.ToList();
            Assert.That(brands.Brands, Is.Not.Null);
            Assert.That(brands.Brands.Count(), Is.EqualTo(3));
            Assert.That(brandList[0], Is.EqualTo("TAL"));
            Assert.That(brandList[1], Is.EqualTo("YB"));
            Assert.That(brandList[2], Is.EqualTo("QA"));
            Assert.That(brands.IsUnderwriter, Is.EqualTo(true));
        }

        [Test]
        public async Task Brand_GetBrandsForUser_SaveMaverickBrand_GetSessionCookie_ReturnsMaverickBrandSetAsDefault_Async()
        {
            var loginRequest = new LoginRequest
            {
                UserName = "QantasTcReadOnly_QA",
                Password = "Transform2001!",
                UseWindowsAuth = false
            };

            var response = await Client.AttemptLoginAsync<RedirectToResponse>(loginRequest, false);

            Assert.That(response, Is.Not.Null);
            Console.WriteLine(response.RedirectTo);

            var brands = await Client.GetBrandsAsync<BrandsLoginModel>();

            Assert.That(brands, Is.Not.Null);
            Assert.That(brands.Brands.Count(), Is.EqualTo(1));
            Assert.That(brands.Brands.FirstOrDefault(), Is.EqualTo("QA"));
            Assert.That(brands.IsUnderwriter, Is.EqualTo(false));

            var selectedBrand = new SaveBrandRequest()
            {
                Brand = brands.Brands.FirstOrDefault()
            };

            await Client.SaveSelectedBrand(selectedBrand, false);

            var salesPortalSessionBrand = GetDefaultBrandInSession();

            Assert.That(salesPortalSessionBrand, Is.EqualTo(selectedBrand.Brand));
        }
    }
}
