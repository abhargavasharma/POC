using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.DataLayer.Factory;
using TAL.QuoteAndApply.DataLayer.Repository;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.Infrastructure;
using TAL.QuoteAndApply.Policy.Configuration;
using TAL.QuoteAndApply.Policy.Models;

namespace TAL.QuoteAndApply.Policy.Data
{
    public interface IBeneficiaryDtoRepository
    {
        BeneficiaryDto InsertBeneficiary(BeneficiaryDto beneficiary);
        BeneficiaryDto GetBeneficiary(int id);
        void UpdateBeneficiary(BeneficiaryDto beneficiary);
        bool DeleteBeneficiary(BeneficiaryDto beneficiary);
        ICollection<BeneficiaryDto> GetBeneficiariesForRisk(int riskId);
    }

    public class BeneficiaryDtoRepository : BaseRepository<BeneficiaryDto>, IBeneficiaryDtoRepository
    {
        public BeneficiaryDtoRepository(IPolicyConfigurationProvider setting, ICurrentUserProvider currentUserProvider,
            IDataLayerExceptionFactory dataLayerExceptionFactory, IDbItemEncryptionService dbItemEncryptionService)
            : base(setting.ConnectionString, currentUserProvider, dataLayerExceptionFactory, dbItemEncryptionService)
        {
        }

        public BeneficiaryDto GetBeneficiary(int id)
        {
            return Get(id);
        }

        public BeneficiaryDto InsertBeneficiary(BeneficiaryDto beneficiary)
        {
            return Insert(beneficiary);
        }

        public void UpdateBeneficiary(BeneficiaryDto beneficiary)
        {
            Update(beneficiary);
        }

        public bool DeleteBeneficiary(BeneficiaryDto beneficiary)
        {
            return Delete(beneficiary);
        }

        public ICollection<BeneficiaryDto> GetBeneficiariesForRisk(int riskId)
        {
            return Where(b => b.RiskId, Op.Eq, riskId).ToArray();
        }
    }
}
