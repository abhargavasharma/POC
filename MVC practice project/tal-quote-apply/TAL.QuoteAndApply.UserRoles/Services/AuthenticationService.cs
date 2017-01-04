using System;
using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.UserRoles.Configuration;
using TAL.QuoteAndApply.UserRoles.Models;

namespace TAL.QuoteAndApply.UserRoles.Services
{
    public interface IAuthenticationService
    {
        AuthenticationResult AuthenticateCurrentWindowsUser(string userName);
        AuthenticationResult AuthenticateUser(string userName, string password);
        IEnumerable<string> GetBrandsForCurrentUser(string userName);
        IEnumerable<Role> GetRolesForCurrentUser(string userName);
    }

    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRolesConfigurationProvider _userRolesConfigurationProvider;
        private readonly IUserPrincipalService _userPrincipalService;
        private readonly IBrandSettingsProvider _brandSettingsProvider;

        public AuthenticationService(IUserRolesConfigurationProvider userRolesConfigurationProvider, 
            IUserPrincipalService userPrincipalService, 
            IBrandSettingsProvider brandSettingsProvider)
        {
            _userRolesConfigurationProvider = userRolesConfigurationProvider;
            _userPrincipalService = userPrincipalService;
            _brandSettingsProvider = brandSettingsProvider;
        }

        public AuthenticationResult AuthenticateCurrentWindowsUser(string userName)
        {
            return GetUserRolesAndAuthenticate(userName);
        }

        public AuthenticationResult AuthenticateUser(string userName, string password)
        {
            if (!_userPrincipalService.ValidateCredentials(_userRolesConfigurationProvider.Domain, userName, password))
            {
                return AuthenticationResult.Failure(_userPrincipalService.GetDetailsForUser(_userRolesConfigurationProvider.Domain, userName), 
                    AuthenticationFailureReason.InvalidCredentials);
            }

            return GetUserRolesAndAuthenticate(userName);
        }

        public IEnumerable<string> GetBrandsForCurrentUser(string userName)
        {
            var user = _userPrincipalService.GetDetailsForUser(_userRolesConfigurationProvider.Domain, userName);
            var returnBrandList = (from brand in _brandSettingsProvider.GetAllBrands()
                                   where user.Groups.Contains(brand.RoleSettings.AgentGroup) 
                                    || user.Groups.Contains(brand.RoleSettings.UnderwritingGroup) 
                                    || user.Groups.Contains(brand.RoleSettings.ReadOnlyGroup)
                                   select brand.BrandKey).ToList();
            
            return returnBrandList;
        }

        public IEnumerable<Role> GetRolesForCurrentUser(string userName)
        {
            var user = _userPrincipalService.GetDetailsForUser(_userRolesConfigurationProvider.Domain, userName);
            return ConvertGroupsToRoles(user.Groups);
        }

        private AuthenticationResult GetUserRolesAndAuthenticate(string userName)
        {
            var user = _userPrincipalService.GetDetailsForUser(_userRolesConfigurationProvider.Domain, userName);
            var roles = ConvertGroupsToRoles(user.Groups);

            if (UserHasApporiateRoles(roles))
            {
                return AuthenticationResult.Success(user, roles);
            }

            return AuthenticationResult.Failure(user, AuthenticationFailureReason.NoRoles);
        }

        private bool UserHasApporiateRoles(IEnumerable<Role> roles)
        {
            return roles.Any();
        }

        private IEnumerable<Role> ConvertGroupsToRoles(IEnumerable<string> groups)
        {
            var roles = new List<Role>();
            foreach (var brand in _brandSettingsProvider.GetAllBrands())
            {
                if (!roles.Contains(Role.ReadOnly) && groups.Contains(brand.RoleSettings.ReadOnlyGroup, StringComparer.InvariantCultureIgnoreCase))
                    roles.Add(Role.ReadOnly);

                if (!roles.Contains(Role.Agent) && groups.Contains(brand.RoleSettings.AgentGroup, StringComparer.InvariantCultureIgnoreCase))
                    roles.Add(Role.Agent);

                if (!roles.Contains(Role.Underwriter) && groups.Contains(brand.RoleSettings.UnderwritingGroup, StringComparer.InvariantCultureIgnoreCase))
                    roles.Add(Role.Underwriter);
            }

            return roles;
        }
    }
}
