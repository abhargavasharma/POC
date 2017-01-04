using System.Collections.Generic;
using NUnit.Framework;
using TAL.QuoteAndApply.Product.Definition;
using TAL.QuoteAndApply.ServiceLayer.Policy.Plan;
using TAL.QuoteAndApply.ServiceLayer.Product;

namespace TAL.QuoteAndApply.ServiceLayer.UnitTests.Policy.Plan
{
    [TestFixture]
    public class PlanVariableResponseConverterTests
    {

        [Test]
        public void WaitingPeriodFrom_VariablesContainValue_ReturnsWaitingPeriodSelectedValue()
        {
            //Arrange
            var conterter = new PlanVariableResponseConverter();
            var variables = new List<VariableResponse>
            {
                new VariableResponse {Code = ProductPlanVariableConstants.WaitingPeriod, SelectedValue = (long)1}
            };

            //Act
            var selectedWaitingPeriod = conterter.WaitingPeriodFrom(variables);

            //Assert
            Assert.That(selectedWaitingPeriod, Is.Not.Null);
            Assert.That(selectedWaitingPeriod, Is.EqualTo(1));
        }

        [Test]
        public void WaitingPeriodFrom_VariablesDontContainValue_ReturnsNull()
        {
            //Arrange
            var conterter = new PlanVariableResponseConverter();
            var variables = new List<VariableResponse>();

            //Act
            var selectedWaitingPeriod = conterter.WaitingPeriodFrom(variables);

            //Assert
            Assert.That(selectedWaitingPeriod, Is.Null);
        }

        [Test]
        public void CpiFrom_VariablesContainValue_ReturnsSelectedValue()
        {
            //Arrange
            var conterter = new PlanVariableResponseConverter();
            var variables = new List<VariableResponse>
            {
                new VariableResponse {Code = ProductPlanVariableConstants.LinkedToCpi, SelectedValue = true}
            };

            //Act
            var selectedValue = conterter.CpiFrom(variables);

            //Assert
            Assert.That(selectedValue, Is.Not.Null);
            Assert.That(selectedValue, Is.EqualTo(true));
        }

        [Test]
        public void CpiFrom_VariablesDontContainValue_ReturnsNull()
        {
            //Arrange
            var conterter = new PlanVariableResponseConverter();
            var variables = new List<VariableResponse>();

            //Act
            var selectedValue = conterter.CpiFrom(variables);

            //Assert
            Assert.That(selectedValue, Is.Null);
        }

        [Test]
        public void BenefitPeriodFrom_VariablesContainValue_ReturnsSelectedValue()
        {
            //Arrange
            var conterter = new PlanVariableResponseConverter();
            var variables = new List<VariableResponse>
            {
                new VariableResponse {Code = ProductPlanVariableConstants.BenefitPeriod, SelectedValue = (long)5}
            };

            //Act
            var selectedValue = conterter.BenefitPeriodFrom(variables);

            //Assert
            Assert.That(selectedValue, Is.Not.Null);
            Assert.That(selectedValue, Is.EqualTo(5));
        }

        [Test]
        public void BenefitPeriodFrom_VariablesDontContainValue_ReturnsNull()
        {
            //Arrange
            var conterter = new PlanVariableResponseConverter();
            var variables = new List<VariableResponse>();

            //Act
            var selectedValue = conterter.BenefitPeriodFrom(variables);

            //Assert
            Assert.That(selectedValue, Is.Null);
        }

    }
}
