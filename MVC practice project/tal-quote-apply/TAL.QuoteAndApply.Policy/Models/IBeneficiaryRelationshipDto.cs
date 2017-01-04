namespace TAL.QuoteAndApply.Policy.Models
{
    public interface IBeneficiaryRelationshipDto
    {
        int Id { get; set; }
        string PASExportValue { get; set; }
        string Description { get; set; }        
    }
}