using NUnit.Framework;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.SalesPortal.Web.Services;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;

namespace TAL.QuoteAndApply.SalesPortal.Web.UnitTests.Services
{
    [TestFixture]
    public class EditPolicyPermissionsServiceTests
    {
        [TestCase(PolicyStatus.RaisedToPolicyAdminSystem, new [] { Role.Agent }, Role.Agent)]
        [TestCase(PolicyStatus.RaisedToPolicyAdminSystem, new[] { Role.Underwriter }, Role.Underwriter)]
        [TestCase(PolicyStatus.RaisedToPolicyAdminSystem, new[] { Role.ReadOnly }, Role.ReadOnly)]
        [TestCase(PolicyStatus.RaisedToPolicyAdminSystem, new[] { Role.Agent, Role.Underwriter }, Role.Agent)]
        [TestCase(PolicyStatus.RaisedToPolicyAdminSystem, new[] { Role.Agent, Role.ReadOnly }, Role.Agent)]
        [TestCase(PolicyStatus.RaisedToPolicyAdminSystem, new[] { Role.Underwriter, Role.ReadOnly }, Role.Underwriter)]
        [TestCase(PolicyStatus.RaisedToPolicyAdminSystem, new[] { Role.Agent, Role.Underwriter, Role.ReadOnly }, Role.Agent)]

        [TestCase(PolicyStatus.ReadyForInforce, new[] { Role.Agent }, Role.Agent)]
        [TestCase(PolicyStatus.ReadyForInforce, new[] { Role.Underwriter }, Role.Underwriter)]
        [TestCase(PolicyStatus.ReadyForInforce, new[] { Role.ReadOnly }, Role.ReadOnly)]
        [TestCase(PolicyStatus.ReadyForInforce, new[] { Role.Agent, Role.Underwriter }, Role.Agent)]
        [TestCase(PolicyStatus.ReadyForInforce, new[] { Role.Agent, Role.ReadOnly }, Role.Agent)]
        [TestCase(PolicyStatus.ReadyForInforce, new[] { Role.Underwriter, Role.ReadOnly }, Role.Underwriter)]
        [TestCase(PolicyStatus.ReadyForInforce, new[] { Role.Agent, Role.Underwriter, Role.ReadOnly }, Role.Agent)]

        [TestCase(PolicyStatus.FailedDuringPolicyAdminSystemLoad, new[] { Role.Agent }, Role.Agent)]
        [TestCase(PolicyStatus.FailedDuringPolicyAdminSystemLoad, new[] { Role.Underwriter }, Role.Underwriter)]
        [TestCase(PolicyStatus.FailedDuringPolicyAdminSystemLoad, new[] { Role.ReadOnly }, Role.ReadOnly)]
        [TestCase(PolicyStatus.FailedDuringPolicyAdminSystemLoad, new[] { Role.Agent, Role.Underwriter }, Role.Agent)]
        [TestCase(PolicyStatus.FailedDuringPolicyAdminSystemLoad, new[] { Role.Agent, Role.ReadOnly }, Role.Agent)]
        [TestCase(PolicyStatus.FailedDuringPolicyAdminSystemLoad, new[] { Role.Underwriter, Role.ReadOnly }, Role.Underwriter)]
        [TestCase(PolicyStatus.FailedDuringPolicyAdminSystemLoad, new[] { Role.Agent, Role.Underwriter, Role.ReadOnly }, Role.Agent)]

        [TestCase(PolicyStatus.FailedToSendToPolicyAdminSystem, new[] { Role.Agent }, Role.Agent)]
        [TestCase(PolicyStatus.FailedToSendToPolicyAdminSystem, new[] { Role.Underwriter }, Role.Underwriter)]
        [TestCase(PolicyStatus.FailedToSendToPolicyAdminSystem, new[] { Role.ReadOnly }, Role.ReadOnly)]
        [TestCase(PolicyStatus.FailedToSendToPolicyAdminSystem, new[] { Role.Agent, Role.Underwriter }, Role.Agent)]
        [TestCase(PolicyStatus.FailedToSendToPolicyAdminSystem, new[] { Role.Agent, Role.ReadOnly }, Role.Agent)]
        [TestCase(PolicyStatus.FailedToSendToPolicyAdminSystem, new[] { Role.Underwriter, Role.ReadOnly }, Role.Underwriter)]
        [TestCase(PolicyStatus.FailedToSendToPolicyAdminSystem, new[] { Role.Agent, Role.Underwriter, Role.ReadOnly }, Role.Agent)]

        [TestCase(PolicyStatus.Inforce, new[] { Role.Agent }, Role.Agent)]
        [TestCase(PolicyStatus.Inforce, new[] { Role.Underwriter }, Role.Underwriter)]
        [TestCase(PolicyStatus.Inforce, new[] { Role.ReadOnly }, Role.ReadOnly)]
        [TestCase(PolicyStatus.Inforce, new[] { Role.Agent, Role.Underwriter }, Role.Agent)]
        [TestCase(PolicyStatus.Inforce, new[] { Role.Agent, Role.ReadOnly }, Role.Agent)]
        [TestCase(PolicyStatus.Inforce, new[] { Role.Underwriter, Role.ReadOnly }, Role.Underwriter)]
        [TestCase(PolicyStatus.Inforce, new[] { Role.Agent, Role.Underwriter, Role.ReadOnly }, Role.Agent)]
        public void GetPermissionsFor_PolicyIsInCompletedStatus_ReadOnlyReturnedWithAppropriateUser(PolicyStatus currentPolicyStatus, Role[] roles, Role expectedRole)
        {
            var svc = new EditPolicyPermissionsService();
            var result = svc.GetPermissionsFor(currentPolicyStatus, roles);

            Assert.That(result.ReadOnly, Is.True);
            Assert.That(result.Role, Is.EqualTo(expectedRole));
        }

        [TestCase(new Role[] { Role.Agent })]
        [TestCase(new Role[] { Role.Agent, Role.Underwriter })]
        [TestCase(new Role[] { Role.Agent, Role.ReadOnly })]
        [TestCase(new Role[] { Role.Agent, Role.Underwriter, Role.ReadOnly })]
        public void GetPermissionsFor_PolicyIsIncomplete_HasAgentRole_ReadOnlyIsFalseAndAgentReturned(Role[] roles)
        {            
            var svc = new EditPolicyPermissionsService();
            var result = svc.GetPermissionsFor(PolicyStatus.Incomplete, roles);

            Assert.That(result.ReadOnly, Is.False);
            Assert.That(result.Role, Is.EqualTo(Role.Agent));
        }

        [TestCase(new Role[] { Role.Underwriter }, Role.Underwriter)]
        [TestCase(new Role[] { Role.ReadOnly }, Role.ReadOnly)]
        [TestCase(new Role[] { Role.Underwriter, Role.ReadOnly }, Role.Underwriter)]
        public void GetPermissionsFor_PolicyIsIncomplete_DoesNotHaveAgentRole_ReadOnlyAndAppropriateRoleReturned(Role[] roles, Role expectedRole)
        {           
            var svc = new EditPolicyPermissionsService();
            var result = svc.GetPermissionsFor(PolicyStatus.Incomplete, roles);

            Assert.That(result.ReadOnly, Is.True);
            Assert.That(result.Role, Is.EqualTo(expectedRole));
        }

        [TestCase(new Role[] { Role.Agent }, Role.Agent)]
        [TestCase(new Role[] { Role.Underwriter }, Role.Underwriter)]
        [TestCase(new Role[] { Role.ReadOnly }, Role.ReadOnly)]
        [TestCase(new Role[] { Role.Agent, Role.ReadOnly }, Role.Agent)]
        [TestCase(new Role[] { Role.Agent, Role.Underwriter }, Role.Underwriter)]
        [TestCase(new Role[] { Role.Underwriter, Role.ReadOnly }, Role.Underwriter)]
        [TestCase(new Role[] { Role.Agent, Role.Underwriter, Role.ReadOnly }, Role.Underwriter)]
        public void GetPermissionsFor_PolicyIsReferredToUnderwriter_ReadOnlyAndAppropriateRoleReturned(Role[] roles, Role expectedRole)
        {           
            var svc = new EditPolicyPermissionsService();
            var result = svc.GetPermissionsFor(PolicyStatus.ReferredToUnderwriter, roles);

            Assert.That(result.ReadOnly, Is.True);
            Assert.That(result.Role, Is.EqualTo(expectedRole));
        }
    }
}
