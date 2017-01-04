using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.UserRoles.Configuration;
using TAL.QuoteAndApply.UserRoles.Services;

namespace TAL.QuoteAndApply.ServiceLayer.Referral
{
    public interface IGetUnderwritersService
    {
        List<string> GetAll();
    }

    public class GetUnderwritersService : IGetUnderwritersService
    {
        private readonly IUserPrincipalService _principalService;
        private readonly IUserRolesConfigurationProvider _userRolesConfigurationProvider;
        private readonly IBrandSettingsProvider _brandSettingsProvider;

        public GetUnderwritersService(IUserPrincipalService principalService, 
            IUserRolesConfigurationProvider userRolesConfigurationProvider, 
            IBrandSettingsProvider brandSettingsProvider)
        {
            _principalService = principalService;
            _userRolesConfigurationProvider = userRolesConfigurationProvider;
            _brandSettingsProvider = brandSettingsProvider;
        }

        public List<string> GetAll()
        {
            var result = _principalService.GetUsersInGroup(_userRolesConfigurationProvider.Domain, _brandSettingsProvider.GetUnderwriterGroup());
            return result.Select(x => x).ToList();
        }
    }
}
