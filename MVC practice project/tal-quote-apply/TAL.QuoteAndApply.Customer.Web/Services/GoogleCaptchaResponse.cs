using System.Collections.Generic;
using System.Text;

namespace TAL.QuoteAndApply.Customer.Web.Services
{
    public class GoogleCaptchaResponse
    {
        public GoogleCaptchaResponse()
        {
            this.ErrorCodes = new List<string>();
        }

        public bool Success { get; set; }
        public string Hostname { get; set; }
        public string Challenge_TS { get; set; }
        public List<string> ErrorCodes { get; set; }

        public override string ToString()
        {
            if (Success)
                return $"Captcha successfull. TS: {Challenge_TS}";

            var sb = new StringBuilder("Captcha Failed.\n");
            sb.AppendLine($"\t TS: {Challenge_TS}");
            foreach (var errorCode in ErrorCodes)
            {
                sb.AppendLine($"\t Error: {errorCode}");
            }

            return sb.ToString();
        }
    }
}