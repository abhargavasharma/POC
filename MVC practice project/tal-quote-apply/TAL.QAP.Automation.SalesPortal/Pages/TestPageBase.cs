using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using TAL.WebAutomation;

namespace TAL.QAP.Automation.SalesPortal.Pages
{
    public class TestPageBase : TestPage
    {
        public TestPageBase(IWebDriver webDriver) : base(webDriver)
        {
        }

        protected IWebElement FindElementByNgModel(string ngModel)
        {
            return FindElement(By.CssSelector(string.Format("input[ng-model=\"{0}\"]", ngModel)));
        }
    }
}
