namespace TAL.QuoteAndApply.UserRoles.Customer
{
    public interface ICustomerAuthenticationService
    {
        bool AccountExists(string quoteReference);
        CreateCustomerLoginResult CreateCustomerLogin(string quoteReference, string emailAddress, string password, string firstName,
            string lastName);

        CustomerAuthenticateResult Authenticate(string quoteReference, string password);
    }
}
