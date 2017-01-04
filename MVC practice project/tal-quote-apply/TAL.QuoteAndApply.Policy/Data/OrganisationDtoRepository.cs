using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAL.QuoteAndApply.DataLayer.Factory;
using TAL.QuoteAndApply.DataLayer.Repository;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.Policy.Configuration;
using TAL.QuoteAndApply.Policy.Models;

namespace TAL.QuoteAndApply.Policy.Data
{
    public interface IOrganisationDtoRepository
    {
        OrganisationDto GetOrganisation(int id);
        OrganisationDto GetOrganisationByBrandId(int brandId);
    }

    public class OrganisationDtoRepository : BaseRepository<OrganisationDto>, IOrganisationDtoRepository
    {
        public OrganisationDtoRepository(IPolicyConfigurationProvider settings, ICurrentUserProvider currentUserProvider,
            IDataLayerExceptionFactory dataLayerExceptionFactory, IDbItemEncryptionService dbItemEncryptionService)
            : base(settings.ConnectionString, currentUserProvider, dataLayerExceptionFactory, dbItemEncryptionService)
        {
        }

        public OrganisationDto GetOrganisation(int id)
        {
            return Get(id);
        }

        public OrganisationDto GetOrganisationByBrandId(int brandId)
        {
            var organisations = Where(b => b.BrandId, Op.Eq, brandId);

            return organisations.FirstOrDefault();
        }
    }
}
