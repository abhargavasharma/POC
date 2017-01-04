namespace TAL.QuoteAndApply.ServiceLayer.Product.Models
{
    public class ProductBrand
    {
        public ProductBrand(int id, string brandCode, int defaultDefaultOrganisationId)
        {
            Id = id;
            BrandCode = brandCode;
            DefaultOrganisationId = defaultDefaultOrganisationId;
        }

        public int Id { get; }
        public string BrandCode { get; }
        public int DefaultOrganisationId { get; }
    }
}