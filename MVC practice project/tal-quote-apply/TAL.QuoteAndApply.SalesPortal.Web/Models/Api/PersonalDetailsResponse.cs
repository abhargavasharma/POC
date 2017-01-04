using System;
using System.ComponentModel.DataAnnotations;


namespace TAL.QuoteAndApply.SalesPortal.Web.Models.Api
{
    public class PersonalDetailsResponse
    {
        [Required(ErrorMessage = "The first name field is required.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "The last name field is required.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "The date of birth field is required.")]
        public DateTime DateOfBirth { get; set; }
        
        public int? Age { get; set; }
    }
}