using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using TAL.WebAutomation;

namespace TAL.QAP.Automation.SalesPortal.Pages
{
    [TestPage(RelativeUrl = "/")]
    public class HomePage : TestPage
    {
        public HomePage(IWebDriver webDriver) : base(webDriver)
        {

        }

        private IWebElement AnnualIncomeTextBox {get {return FindElement(By.Name("income")); } } 

        public HomePage WithAnnualIncome(int annualIncome)
        {
            AnnualIncomeTextBox.SendKeys(annualIncome.ToString());
            return this;
        }
    }
}
