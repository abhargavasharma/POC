namespace TAL.QuoteAndApply.Customer.Web.Models.Api
{
    public class RiskResponse
    {
        //TODO: eventually could fill this out with names etc if we capture earlier in the customer journey and want to display for each risk

        public int Id { get; set; }

        public RiskResponse(int id)
        {
            Id = id;
        }
    }
}