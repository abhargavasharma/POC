using System.Collections.Generic;

namespace TAL.QuoteAndApply.DataModel.User
{
    public interface ICurrentUser
    {
        string UserName { get; }
        IEnumerable<Role> Roles { get; }
        string EmailAddress { get; }
        string GivenName { get; }
        string Surname { get; }
    }
}
