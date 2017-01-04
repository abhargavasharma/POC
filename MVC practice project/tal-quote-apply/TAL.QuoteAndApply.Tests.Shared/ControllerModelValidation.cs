using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Http;
using System.Web.Mvc;

namespace TAL.QuoteAndApply.Tests.Shared
{
    /// <summary>
    /// Used to simulate model state validation in unit tests
    /// ASP.NET stupidly has different base controllers and model state dictionaries in MVC and Web api
    /// so need two variations
    /// </summary>
    public static class ControllerModelValidation
    {
        public static void ValidateModel(Controller controller, object model, string nameOfCaller = "")
        {
            var errors = new List<Tuple<string, string>>();
            ValidateModelInternal(errors, model, nameOfCaller);

            foreach (var err in errors)
            {
                controller.ModelState.AddModelError(err.Item1, err.Item2);
            }
        }

        public static void ValidateModel(ApiController controller, object model, string nameOfCaller = "")
        {
            var errors = new List<Tuple<string, string>>();
            ValidateModelInternal(errors, model, nameOfCaller);

            foreach (var err in errors)
            {
                controller.ModelState.AddModelError(err.Item1, err.Item2);
            }
        }

        private static void ValidateModelInternal(IList<Tuple<string, string>> errors, object model, string nameOfCaller = "")
        {
            if (model == null)
                return;

            var validationContext = new ValidationContext(model, null, null);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(model, validationContext, validationResults, true);
            foreach (var validationResult in validationResults)
            {
                if (nameOfCaller == "")
                {
                    errors.Add(new Tuple<string, string>(validationResult.MemberNames.First(),
                        validationResult.ErrorMessage));
                }
                else
                {
                    errors.Add(new Tuple<string, string>(nameOfCaller + "." + validationResult.MemberNames.First(),
                        validationResult.ErrorMessage));
                }

            }
            var type = model.GetType();
            var properties = type.GetProperties();

            foreach (var propertyInfo in properties)
            {
                if (propertyInfo.CanRead
                    && (propertyInfo.PropertyType.Name.IndexOf("ienumerable", StringComparison.OrdinalIgnoreCase) == -1)
                    &&
                    !(propertyInfo.PropertyType.IsPrimitive || propertyInfo.PropertyType == typeof(Decimal) ||
                      propertyInfo.PropertyType == typeof(String)))
                {
                    ValidateModelInternal(errors, propertyInfo.GetValue(model), propertyInfo.Name);
                }
            }
        }

    }
}
