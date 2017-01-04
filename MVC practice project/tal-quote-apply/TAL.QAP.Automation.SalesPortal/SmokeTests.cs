using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TAL.QAP.Automation.SalesPortal.Pages;
using TAL.WebAutomation;

namespace TAL.QAP.Automation.SalesPortal
{
    [TestFixture]
    public class SmokeTests : BrowserTestFixture
    {
        [BrowserTest, Test]
        public void HomePage_SmokeTest(PageBrowser browser)
        {
            this.SetUp(browser, "SalesPortal");
            var page = browser.GoTo<HomePage>();

            Assert.IsNotNull(page);
        }

        [BrowserTest, Test]
        public void CreateClientPage_SmokeTest(PageBrowser browser)
        {
            this.SetUp(browser, "SalesPortal");
            var page = browser.GoTo<CreateClientPage>();

            page.WithAnnualIncome(50000);
            page.WithDateOfBirth("01/01/1985");

            Assert.IsNotNull(page);
        }
    }
}
