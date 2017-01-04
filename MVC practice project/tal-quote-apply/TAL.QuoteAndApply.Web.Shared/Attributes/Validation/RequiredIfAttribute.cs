using System.ComponentModel.DataAnnotations;

namespace TAL.QuoteAndApply.Web.Shared.Attributes.Validation
{
    public enum RequiredIfCheckType
    {
        Null,
        StringEmpty
    }

    public class RequiredIfAttribute : RequiredAttribute
    {
        public string PropertyName { get; }
        public object[] PropertyValues { get; }

        public RequiredIfAttribute(string propertyName, params object[] propertyValues)
        {
            PropertyValues = propertyValues;
            PropertyName = propertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var requiredIfProperty = validationContext.ObjectType.GetProperty(PropertyName);
            var requiredIfValue = requiredIfProperty.GetValue(validationContext.ObjectInstance, null);
            
            foreach (var propertyVal in PropertyValues)
            {
                if (propertyVal is RequiredIfCheckType)
                {
                    var checkType = (RequiredIfCheckType) propertyVal;
                    if (checkType == RequiredIfCheckType.Null)
                    {
                        if(requiredIfValue == null)
                            return base.IsValid(value, validationContext);
                    }
                    else if (checkType == RequiredIfCheckType.StringEmpty)
                    {
                        if (requiredIfValue != null && requiredIfValue.ToString() == string.Empty)
                            return base.IsValid(value, validationContext);
                    }
                }
                else
                {
                    if (requiredIfValue != null && propertyVal.ToString() == requiredIfValue.ToString())
                    {
                        return base.IsValid(value, validationContext);
                    }
                }
            }
            
            //not required so move on
            return ValidationResult.Success;
        }
        
    }
}