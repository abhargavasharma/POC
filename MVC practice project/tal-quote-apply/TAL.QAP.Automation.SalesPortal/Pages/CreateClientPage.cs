using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using TAL.WebAutomation;

namespace TAL.QAP.Automation.SalesPortal.Pages
{
    [TestPage(RelativeUrl = "/Client/Create")]
    public class CreateClientPage : TestPageBase
    {
        public CreateClientPage(IWebDriver webDriver) : base(webDriver)
        {
        }

        //private IWebElement DateOfBirthTextBox => FindElementByNgModel("ctrl.model.ratingFactors.dateOfBirth");
        private IWebElement DateOfBirthTextBox { get { return FindElement(By.Name("dateOfBirth")); } }

        private IWebElement AnnualIncomeTextBox { get { return FindElement(By.Name("infome")); } }

        public CreateClientPage WithAnnualIncome(int annualIncome)
        {
            AnnualIncomeTextBox.SendKeys(annualIncome.ToString());
            return this;
        }

        public CreateClientPage WithDateOfBirth(string dateOfBirth)
        {
            DateOfBirthTextBox.SendKeys(dateOfBirth);
            return this;
        }

    }
}
