using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TAL.QuoteAndApply.Customer.Web.Models.Api
{
    public class CalculatorResults
    {
        [Required]
        public string  Results { get; set; }
        [Required]
        public string  Assumptions { get; set; }

        public bool UseResultConfirmationRequired { get; set; }

    }
}