using TAL.QuoteAndApply.DataModel;

namespace TAL.QuoteAndApply.Policy.Models
{
    public class OrganisationDto : DbItem
    {
        public string OrganisationKey { get; set; }
        public string Description { get; set; }
        public int BrandId { get; set; }
    }
}