namespace TAL.QuoteAndApply.ServiceLayer.Validation
{
    public class ValidationError
    {
        public string Code { get; set; }
        public ValidationKey Key { get; set; }
        public string Message { get; set; }
        public ValidationType Type { get; set; }

        public string Location { get; set; }

        public ValidationError(string code, ValidationKey key, string message, ValidationType type, string location)
        {
            Code = code;
            Key = key;
            Message = message;
            Type = type;
            Location = location;
        }
    }
}