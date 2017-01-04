using System.Collections.Generic;
using TAL.QuoteAndApply.DataModel.User;

namespace TAL.QuoteAndApply.ServiceLayer.User
{
    public interface IApplicationCurrentUser
    {
        string CurrentUser { get; }
        IEnumerable<Role> CurrentUserRoles { get; }
        string CurrentUserEmailAddress { get; }
        string CurrentUserGivenName { get; }
        string CurrentUserSurname { get; }
    }
}