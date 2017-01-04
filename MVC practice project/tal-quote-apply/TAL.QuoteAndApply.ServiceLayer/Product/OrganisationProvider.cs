using System;
using TAL.QuoteAndApply.Policy.Data;

namespace TAL.QuoteAndApply.ServiceLayer.Product
{
    public interface IOrganisationProvider
    {
        int GetDefaultOrganisationIdByBrandId(int brandId);
    }

    public class OrganisationProvider : IOrganisationProvider
    {
        private readonly IOrganisationDtoRepository _organisationDtoRepository;

        public OrganisationProvider(IOrganisationDtoRepository organisationDtoRepository)
        {
            _organisationDtoRepository = organisationDtoRepository;
        }

        public int GetDefaultOrganisationIdByBrandId(int brandId)
        {
            var organisation = _organisationDtoRepository.GetOrganisationByBrandId(brandId);
            if (organisation == null)
            {
                throw new ApplicationException($"No organisation associated with brand Id {brandId}");
            }
            return organisation.Id;
        }
    }
}
