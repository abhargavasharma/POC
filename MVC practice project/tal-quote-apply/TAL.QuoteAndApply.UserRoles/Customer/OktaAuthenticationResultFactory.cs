using System.Linq;
using Okta.Core;
using Okta.Core.Models;

namespace TAL.QuoteAndApply.UserRoles.Customer
{
    public interface IOktaAuthenticationResultFactory
    {
        CreateCustomerLoginResult From(OktaException exception);
        CustomerAuthenticateResult From(OktaAuthenticationException exception);
        CustomerAuthenticateResult From(string authStatus);
        CustomerResultStatus FromErrorCode(string errorCode);
    }

    public class OktaAuthenticationResultFactory : IOktaAuthenticationResultFactory
    {
        CreateCustomerLoginResult IOktaAuthenticationResultFactory.From(OktaException exception)
        {
            var status = CustomerResultStatus.Failure;

            switch (exception.ErrorCode)
            {
                case OktaErrorCodes.ApiValidationException:
                    if (exception.ErrorCauses.Any(c => c.ErrorSummary == "login: An object with this field already exists in the current organization"))
                    {
                        status = CustomerResultStatus.UserAlreadyExists;
                    }
                    break;
                default:
                    status = FromErrorCode(exception.ErrorCode);
                    break;
            }

            return new CreateCustomerLoginResult { Status = status };
        }

        public CustomerAuthenticateResult From(OktaAuthenticationException exception)
        {
            return new CustomerAuthenticateResult {Status = FromErrorCode(exception.ErrorCode)};
        }

        public CustomerAuthenticateResult From(string authStatus)
        {
            var customerResultStatus = authStatus == AuthStatus.Success
                ? CustomerResultStatus.Success
                : CustomerResultStatus.Failure;

            return new CustomerAuthenticateResult { Status = customerResultStatus };
        }

        public CustomerResultStatus FromErrorCode(string errorCode)
        {
            //TODO: When we do retrieve will look for more error codes here...

            switch (errorCode)
            {
                case OktaErrorCodes.ResourceNotFoundException:
                    return CustomerResultStatus.UserNotFound;
                default:
                    return CustomerResultStatus.Failure;
            }
        }
    }
}
