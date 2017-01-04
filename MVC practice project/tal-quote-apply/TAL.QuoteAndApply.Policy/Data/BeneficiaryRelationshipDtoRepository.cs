using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.DataLayer.Repository;
using TAL.QuoteAndApply.DataLayer.Factory;
using TAL.QuoteAndApply.DataLayer.Repository.PredicateLogic;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.Infrastructure;
using TAL.QuoteAndApply.Policy.Configuration;

namespace TAL.QuoteAndApply.Policy.Data
{
    public interface IBeneficiaryRelationshipDtoRepository
    {
        ICollection<BeneficiaryRelationshipDto> GetBeneficiaryRelationships();
        string GetPasExportBeneficiaryRelationship(int? relationshipId);
    }

    public class BeneficiaryRelationshipDtoRepository : BaseRepository<BeneficiaryRelationshipDto>, IBeneficiaryRelationshipDtoRepository
    {
        public BeneficiaryRelationshipDtoRepository(IPolicyConfigurationProvider setting, ICurrentUserProvider currentUserProvider,
            IDataLayerExceptionFactory dataLayerExceptionFactory, IDbItemEncryptionService dbItemEncryptionService)
            : base(setting.ConnectionString, currentUserProvider, dataLayerExceptionFactory, dbItemEncryptionService)
        {
        }

        public ICollection<BeneficiaryRelationshipDto> GetBeneficiaryRelationships()
        {
            var beneficiaryRelationships = GetAll();            
            return beneficiaryRelationships.ToArray();
        }

        public string GetPasExportBeneficiaryRelationship(int? relationshipId)
        {
            if (relationshipId != null)
            {
                return Get(relationshipId.Value).PASExportValue;
            }
            return null;
        }
    }
}
