using System.ComponentModel.DataAnnotations;

namespace TAL.QuoteAndApply.SalesPortal.Web.Models.Api
{
    public class ClientRequest
    {
        public PersonalDetailsRequest PersonalDetails { get; set; }

        [Required]
        public RatingFactorsRequest RatingFactors { get; set; }

        public bool ValidateResidency { get; set; }

        public ClientRequest()
        {
            PersonalDetails = new PersonalDetailsRequest();
            RatingFactors = new RatingFactorsRequest();
            ValidateResidency = true;
        }
    }
}