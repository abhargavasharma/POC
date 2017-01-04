using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.DataModel.Policy;

namespace TAL.QuoteAndApply.Policy.Models
{
    public class PolicyOwnerDto : DbItem
    {
        public int PartyId { get; set; }
        public int PolicyId { get; set; }
        public PolicyOwnerType OwnerType { get; set; }
        /// <summary>
        /// Holds fund name of the SMSF if the owner type is SMSF
        /// </summary>
        public string FundName { get; set; }
    }
}
