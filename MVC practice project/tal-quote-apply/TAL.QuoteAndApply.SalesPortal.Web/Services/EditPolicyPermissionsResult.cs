using TAL.QuoteAndApply.DataModel.User;

namespace TAL.QuoteAndApply.SalesPortal.Web.Services
{
    public class EditPolicyPermissionsResult
    {
        public bool ReadOnly { get; }
        public Role Role { get; }

        public EditPolicyPermissionsResult(bool readOnly, Role role)
        {
            ReadOnly = readOnly;
            Role = role;
        }
    }
}