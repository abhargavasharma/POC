using TAL.QuoteAndApply.DataModel.User;

namespace TAL.QuoteAndApply.ServiceLayer.User
{
    public class CurrentUserProvider : ICurrentUserProvider
    {
        private readonly IApplicationCurrentUser _applicationCurrentUser;

        public CurrentUserProvider(IApplicationCurrentUser applicationCurrentUser)
        {
            _applicationCurrentUser = applicationCurrentUser;
        }

        public ICurrentUser GetForApplication()
        {
            return new CurrentUser(_applicationCurrentUser.CurrentUser, _applicationCurrentUser.CurrentUserRoles, 
                _applicationCurrentUser.CurrentUserEmailAddress, _applicationCurrentUser.CurrentUserGivenName, _applicationCurrentUser.CurrentUserSurname);
        }
    }
}