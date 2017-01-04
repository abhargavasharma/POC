using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.DataModel.User;

namespace TAL.QuoteAndApply.Policy.Service.AccessControl
{
    public interface IAccessControlTypeProvider
    {
        AccessControlType GetFor(IEnumerable<Role> roles);
    }

    public class AccessControlTypeProvider : IAccessControlTypeProvider
    {
        public AccessControlType GetFor(IEnumerable<Role> roles)
        {
            if (roles == null || !roles.Any())
            {
                return AccessControlType.Customer;
            }
            if (roles.Contains(Role.Underwriter))
            {
                return AccessControlType.Underwriter;
            }
            return AccessControlType.Agent;
        }
    }
}