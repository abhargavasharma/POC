using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TAL.Performance.Infrastructure.Core;
using TAL.Performance.Infrastructure.Core.Data;
using TAL.Performance.Infrastructure.Core.WebServers;
using TAL.QuoteAndApply.PerformanceEmulator.Emulators;
using Task = System.Threading.Tasks.Task;

namespace TAL.QuoteAndApply.PerformanceEmulator.Tests
{
    [TestFixture]
    public class CustomerPortalEmulatorTestRunner
    {
        [Test]
        public void RunCustomerPortalEmulator()
        {
            var baseUrl = ConfigurationManager.AppSettings["PerfTest.CustomerUrl"];

            Cookie cookie = null;

            var setCookieCallback = new Action<string>(cookieString =>
            {
                var tokens = cookieString.Split(';');
                var name = "MnJAMeVjXZFVdHmT";
                var value = tokens[0].Replace($"{name}=", "");

                cookie = new Cookie(name, value, "/", baseUrl.Replace("http://", ".").Replace("https://", "."));

            });

            var webServer = new ExternalWebServer(baseUrl, handler =>
            {
                if (cookie != null)
                {
                    handler.CookieContainer = new CookieContainer();
                    handler.CookieContainer.Add(cookie);
                }
            });



            var rnd = new Random();
            var occupations = new[]
            {
                new[] { "ana", "analy", "analyst" }, // AAA
                new[] { "acc", "accou", "account" }, // AA
                new[] { "ab", "abat", "abattoir" } // BBB
            };

            var gender = rnd.NextDouble() < 0.5 ? "Male" : "Female";
            var nameGenerator = NameGenerator.Default;
            var firstName = nameGenerator.GetName(gender == "Female" ? NameGenerator.NameType.FemaleFirstName : NameGenerator.NameType.MaleFirstName);
            var address = nameGenerator.GetAddress();

            var data = new ScenarioData(
                firstName: firstName,
                lastName: "TAL TEST",
                title: "Mr" + (gender == "Female" ? "s" : ""),
                annualIncome: (long)rnd.Next(70000, 300000),
                dateOfBirth: DateTime.Today.AddDays(-rnd.Next((int)(25.1 * 365), (int)(55.1 * 365))).ToString("dd/MM/yyyy"),
                gender: gender,
                smoker: rnd.NextDouble() < 0.5,
                search: occupations[rnd.Next(0, occupations.Length - 1)],
                streetAddress: address.StreetAddress,
                postCode: address.PostCode.ToString(),
                state: address.State.ToString(),
                town: address.Town
                );

            var emulator = new CustomerPortalEmulator(webServer, SimplePerformanceTool.CreateConsoleTimestamped());

            try
            {
                Task.Run(() => emulator.Vanilla(data, setCookieCallback)).Wait();
            }
            catch (AggregateException ex)
            {
                foreach (var e in ex.Flatten().InnerExceptions)
                {
                    Console.WriteLine(e.Message);
                }

                throw;
            }
        }

        private const string Male = "Male";
        private const string Female = "Female";

        [Test, Category("PerformanceIgnore")]
        [TestCase(70000,  25.1, Male,   false, "ana", "analy", "analyst" )]
        [TestCase(70000,  25.1, Female, false, "ana", "analy", "analyst" )]
        [TestCase(300000, 25.1, Male,   false, "ana", "analy", "analyst" )]
        [TestCase(300000, 25.1, Female, false, "ana", "analy", "analyst" )]
        [TestCase(70000,  55.1, Male,   false, "ana", "analy", "analyst" )]
        [TestCase(70000,  55.1, Female, false, "ana", "analy", "analyst" )]
        [TestCase(300000, 55.1, Male,   false, "ana", "analy", "analyst" )]
        [TestCase(300000, 55.1, Female, false, "ana", "analy", "analyst" )]

        [TestCase(70000,  25.1, Male,   false, "acc", "accou", "account" )]
        [TestCase(70000,  25.1, Female, false, "acc", "accou", "account" )]
        [TestCase(300000, 25.1, Male,   false, "acc", "accou", "account" )]
        [TestCase(300000, 25.1, Female, false, "acc", "accou", "account" )]
        [TestCase(70000,  55.1, Male,   false, "acc", "accou", "account" )]
        [TestCase(70000,  55.1, Female, false, "acc", "accou", "account" )]
        [TestCase(300000, 55.1, Male,   false, "acc", "accou", "account" )]
        [TestCase(300000, 55.1, Female, false, "acc", "accou", "account" )]

        [TestCase(70000,  25.1, Male,   false, "ab", "abat", "abattoir" )]
        [TestCase(70000,  25.1, Female, false, "ab", "abat", "abattoir" )]
        [TestCase(300000, 25.1, Male,   false, "ab", "abat", "abattoir" )]
        [TestCase(300000, 25.1, Female, false, "ab", "abat", "abattoir" )]
        [TestCase(70000,  55.1, Male,   false, "ab", "abat", "abattoir" )]
        [TestCase(70000,  55.1, Female, false, "ab", "abat", "abattoir" )]
        [TestCase(300000, 55.1, Male,   false, "ab", "abat", "abattoir" )]
        [TestCase(300000, 55.1, Female, false, "ab", "abat", "abattoir" )]

        [TestCase(70000,  25.1, Male,   true, "ana", "analy", "analyst" )]
        [TestCase(70000,  25.1, Female, true, "ana", "analy", "analyst" )]
        [TestCase(300000, 25.1, Male,   true, "ana", "analy", "analyst" )]
        [TestCase(300000, 25.1, Female, true, "ana", "analy", "analyst" )]
        [TestCase(70000,  55.1, Male,   true, "ana", "analy", "analyst" )]
        [TestCase(70000,  55.1, Female, true, "ana", "analy", "analyst" )]
        [TestCase(300000, 55.1, Male,   true, "ana", "analy", "analyst" )]
        [TestCase(300000, 55.1, Female, true, "ana", "analy", "analyst" )]
                                        
        [TestCase(70000,  25.1, Male,   true, "acc", "accou", "account" )]
        [TestCase(70000,  25.1, Female, true, "acc", "accou", "account" )]
        [TestCase(300000, 25.1, Male,   true, "acc", "accou", "account" )]
        [TestCase(300000, 25.1, Female, true, "acc", "accou", "account" )]
        [TestCase(70000,  55.1, Male,   true, "acc", "accou", "account" )]
        [TestCase(70000,  55.1, Female, true, "acc", "accou", "account" )]
        [TestCase(300000, 55.1, Male,   true, "acc", "accou", "account" )]
        [TestCase(300000, 55.1, Female, true, "acc", "accou", "account" )]
                                        
        [TestCase(70000,  25.1, Male,   true, "ab", "abat", "abattoir" )]
        [TestCase(70000,  25.1, Female, true, "ab", "abat", "abattoir" )]
        [TestCase(300000, 25.1, Male,   true, "ab", "abat", "abattoir" )]
        [TestCase(300000, 25.1, Female, true, "ab", "abat", "abattoir" )]
        [TestCase(70000,  55.1, Male,   true, "ab", "abat", "abattoir" )]
        [TestCase(70000,  55.1, Female, true, "ab", "abat", "abattoir" )]
        [TestCase(300000, 55.1, Male,   true, "ab", "abat", "abattoir" )]
        [TestCase(300000, 55.1, Female, true, "ab", "abat", "abattoir" )]
        public void RunCustomerPortalEmulator_Scenario(long annualIncome, double age, string gender, bool smoker, string search1, string search2, string search3)
        {
            var baseUrl = ConfigurationManager.AppSettings["PerfTest.CustomerUrl"];

            Cookie cookie = null;

            var setCookieCallback = new Action<string>(cookieString =>
            {
                var tokens = cookieString.Split(';');
                var name = "MnJAMeVjXZFVdHmT";
                var value = tokens[0].Replace($"{name}=", "");

                cookie = new Cookie(name, value, "/", baseUrl.Replace("http://", "."));

            });

            var webServer = new ExternalWebServer(baseUrl, handler =>
            {
                if (cookie != null)
                {
                    handler.CookieContainer = new CookieContainer();
                    handler.CookieContainer.Add(cookie);
                }
            });

            var nameGenerator = NameGenerator.Default;
            var firstName = nameGenerator.GetName(gender == "Female" ? NameGenerator.NameType.FemaleFirstName : NameGenerator.NameType.MaleFirstName);
            var address = nameGenerator.GetAddress();

            var data = new ScenarioData(
                firstName: firstName,
                lastName: "TAL TEST",
                title: "Mr" + gender == "Female" ? "s" : "",
                annualIncome: annualIncome,
                dateOfBirth: DateTime.Today.AddDays(-age * 365).ToString("dd/MM/yyyy"),
                gender: gender,
                smoker: smoker,
                search: new[] { search1, search2, search3 },
                streetAddress: address.StreetAddress,
                postCode: address.PostCode.ToString(),
                state: address.State.ToString(),
                town: address.Town
                );

            var emulator = new CustomerPortalEmulator(webServer, SimplePerformanceTool.CreateConsoleTimestamped());

            try
            {
                Task.Run(() => emulator.Vanilla(data, setCookieCallback)).Wait();
            }
            catch (AggregateException ex)
            {
                foreach (var e in ex.Flatten().InnerExceptions)
                {
                    Console.WriteLine(e.Message);
                }

                throw;
            }
        }
    }
}
