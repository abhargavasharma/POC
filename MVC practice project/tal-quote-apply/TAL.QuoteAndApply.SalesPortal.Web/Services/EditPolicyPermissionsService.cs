using System;
using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;

namespace TAL.QuoteAndApply.SalesPortal.Web.Services
{
    public interface IEditPolicyPermissionsService
    {
        EditPolicyPermissionsResult GetPermissionsFor(PolicyStatus policyStatus, IEnumerable<Role> currentUserRoles);
    }

    public class EditPolicyPermissionsService : IEditPolicyPermissionsService
    {
        public EditPolicyPermissionsResult GetPermissionsFor(PolicyStatus policyStatus, IEnumerable<Role> currentUserRoles)
        {
            if (policyStatus == PolicyStatus.Incomplete)
            {
                var role = GetAppropriateRoleAgentTakesPrecedence(currentUserRoles);

                var readOnly = role != Role.Agent;

                return new EditPolicyPermissionsResult(readOnly, role);
            }

            if (policyStatus == PolicyStatus.ReferredToUnderwriter)
            {
                var role = GetAppropriateRoleUnderwriterTakesPrecedence(currentUserRoles);

                return new EditPolicyPermissionsResult(true, role);
            }

            return new EditPolicyPermissionsResult(true, GetAppropriateRoleAgentTakesPrecedence(currentUserRoles));
        }

        private Role GetAppropriateRoleAgentTakesPrecedence(IEnumerable<Role> roles)
        {
            var role = Role.ReadOnly;

            if (roles.Contains(Role.Agent))
            {
                role = Role.Agent;
            }
            else if (roles.Contains(Role.Underwriter))
            {
                role = Role.Underwriter;
            }

            return role;
        }

        private Role GetAppropriateRoleUnderwriterTakesPrecedence(IEnumerable<Role> roles)
        {
            var role = Role.ReadOnly;

            if (roles.Contains(Role.Underwriter))
            {
                role = Role.Underwriter;
            }
            else if (roles.Contains(Role.Agent))
            {
                role = Role.Agent;
            }

            return role;
        }
    }
}