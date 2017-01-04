using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;

namespace TAL.QuoteAndApply.UserRoles.Services
{
    public interface IUserPrincipalService
    {
        UserDetails GetDetailsForUser(string domain, string userName);
        bool ValidateCredentials(string domain, string userName, string password);
        IReadOnlyList<string> GetUsersInGroup(string domain, string groupName);
    }

    public class UserPrincipalService : IUserPrincipalService
    {
        public UserDetails GetDetailsForUser(string domain, string userName)
        {
            using (var ctx = new PrincipalContext(ContextType.Domain, domain))
            {
                var user = UserPrincipal.FindByIdentity(ctx, userName);
                if (user != null)
                {
                    var groups = user.GetGroups();

                    var agentDetails = new UserDetails()
                    {
                        Groups = groups.Select(g => g.Name).ToList(),
                        EmailAddress = user.EmailAddress,
                        GivenName = user.GivenName,
                        Name = userName,
                        Surname = user.Surname
                    };
                    return agentDetails;
                }

                return null;
            }
        }

        public bool ValidateCredentials(string domain, string userName, string password)
        {
            using (var ctx = new PrincipalContext(ContextType.Domain, domain))
            {
                return ctx.ValidateCredentials(userName, password);
            }
        }

        public IReadOnlyList<string> GetUsersInGroup(string domainName, string groupName)
        {
            using (var ctx = new PrincipalContext(ContextType.Domain, domainName))
            {
                var group = GroupPrincipal.FindByIdentity(ctx, groupName);

                if (group != null)
                {
                    var users = group.GetMembers();

                    return users.Select(u => u.Name).ToList();
                }
                return null;
            }
        }
    }
}