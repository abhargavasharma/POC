using TAL.QuoteAndApply.DataModel;

namespace TAL.QuoteAndApply.Policy.Models
{ 
    public class BeneficiaryRelationshipDto : DbItem, IBeneficiaryRelationshipDto
    {
        public string PASExportValue { get; set; }
        public string Description { get; set; }
    }
}
