using System;
using System.Net;
using System.Net.Http;
using Okta.Core;
using Okta.Core.Clients;
using Okta.Core.Models;
using TAL.QuoteAndApply.Infrastructure.Extensions;
using TAL.QuoteAndApply.Infrastructure.Logging;
using TAL.QuoteAndApply.UserRoles.Configuration;

namespace TAL.QuoteAndApply.UserRoles.Customer
{
    public class OktaCustomerAuthenticationService : ICustomerAuthenticationService
    {
        private readonly UsersClient _oktaUsersClient;
        private readonly AuthClient _oktaAuthClient;
        private readonly ILoggingService _loggingService;
        private readonly IOktaAuthenticationResultFactory _authenticationResultFactory;

        public OktaCustomerAuthenticationService(IOktaConfigurationProvider oktaConfigurationProvider,
            IOktaAuthenticationResultFactory authenticationResultFactory, ILoggingService loggingService)
        {
            _authenticationResultFactory = authenticationResultFactory;
            _loggingService = loggingService;

            var handler = CreateHttpClientHandler(oktaConfigurationProvider);

            var settings = new OktaSettings();

            settings.ApiToken = oktaConfigurationProvider.ApiToken;
            settings.BaseUri = new Uri(oktaConfigurationProvider.BaseUri);
            settings.CustomHttpHandler = handler;

            _oktaAuthClient = new AuthClient(settings);

            var client = new OktaClient(settings);
            _oktaUsersClient = client.GetUsersClient();
        }

        public bool AccountExists(string quoteReference)
        {
            //TODO: when doing retrieve can use get account to determine if user exists
            try
            {
                _oktaUsersClient.Get(GetOktaUsernameFromQuoteReference(quoteReference));
            }
            catch (OktaException e)
            {
                var status = _authenticationResultFactory.FromErrorCode(e.ErrorCode);
                if (status == CustomerResultStatus.UserNotFound)
                {
                    return false;
                }
                throw;
            }
            return true;
        }

        public CreateCustomerLoginResult CreateCustomerLogin(string quoteReference, string emailAddress, string password, string firstName, string lastName)
        {
            //Masked out user email, firstName and lastName details for US22309
            var userProfile = new User(GetOktaUsernameFromQuoteReference(quoteReference), "mmd@tal.com.au", "#####", "#####")
            {
                Credentials = new LoginCredentials
                {
                    Password =
                    {
                        Value = password
                    }
                }
            };

            try
            {
                var createdUser = _oktaUsersClient.Add(userProfile, true);
                return new CreateCustomerLoginResult { Status = CustomerResultStatus.Success, Id = createdUser.Id };
            }
            catch (OktaException ex)
            {
                _loggingService.Error(ex);
                return _authenticationResultFactory.From(ex);                
            }
        }

        public CustomerAuthenticateResult Authenticate(string quoteReference, string password)
        {
            try
            {
                var response = _oktaAuthClient.Authenticate(GetOktaUsernameFromQuoteReference(quoteReference), password);
                return _authenticationResultFactory.From(response.Status);
            }
            catch (OktaAuthenticationException ex)
            {
                _loggingService.Info("Auth failed for quote ref '{0}': Error message '{1}'", quoteReference, ex.Message);
                return _authenticationResultFactory.From(ex);
            }
        }

        private static HttpClientHandler CreateHttpClientHandler(IOktaConfigurationProvider oktaConfigurationProvider)
        {
            var handler = new HttpClientHandler();

            //Use proxy if we have a value for it
            var proxyUri = oktaConfigurationProvider.ProxyUri;
            if (!proxyUri.IsNullOrWhiteSpace())
            {
                handler.Proxy = new WebProxy(new Uri(proxyUri));
                handler.UseProxy = true;
            }

            return handler;
        }

        private static string GetOktaUsernameFromQuoteReference(string quoteReference)
        {
            return $"{quoteReference}@TAL.quote";
        }
    }
}

/*
TODO: Below here can be deleted after full implementation
public class SimpleUserLoginResponse
{
    public string Email { get; set; }
    public string UserName { get; set; }
    public bool IsApproved { get; set; }
    public bool IsLockedOut { get; set; }
}

public class OktaProviderClient
{
    public OktaSettings settings { get; set; }
    public OktaClient client { get; set; }
    public UsersClient users { get; set; }
    public GroupsClient groups { get; set; }
    public SessionsClient sessions { get; set; }
    public AppsClient apps { get; set; }
    public AuthClient authn { get; set; }
    public OrgFactorsClient factors { get; set; }

    private void Setup(string apiToken, Uri baseUri)
    {
        var handler = new HttpClientHandler()
        {
            Proxy = new WebProxy(new Uri("http://infraproxy.tower.lan:8080")),
            UseProxy = true
        };

        settings = new OktaSettings();
        settings.ApiToken = apiToken;
        settings.BaseUri = baseUri;
        settings.CustomHttpHandler = handler;

        client = new OktaClient(settings);
        users = client.GetUsersClient();

        groups = client.GetGroupsClient();
        sessions = client.GetSessionsClient();
        apps = client.GetAppsClient();
        authn = new AuthClient(settings);
        factors = new OrgFactorsClient(settings);
    }

    public OktaProviderClient()
    {
        var apiToken = "00_hgA8YKtCX8GxluUPO9hWAk7dXCg2SkOIHvzRwar";
        var baseUri = new System.Uri("https://quotepoc.okta.com");
        Setup(apiToken, baseUri);
    }

    public OktaProviderClient(string apiToken, Uri baseUri)
    {
        Setup(apiToken, baseUri);
    }

    public SimpleUserLoginResponse GetOktaMembershipUser(string username)
    {
        User oktaUser;
        try
        {
            oktaUser = users.Get(username);
        }
        catch (OktaException e)
        {
            // "Not found."
            if (e.ErrorCode == "E0000007")
            {
                return null;
            }
            throw e;
        }
        return OktaUserToOktaMembershipUser(oktaUser);
    }
    public SimpleUserLoginResponse GetOktaMembershipUser(User oktaUser, bool populateApps = true)
    {
        return OktaUserToOktaMembershipUser(oktaUser, populateApps);
    }
    public SimpleUserLoginResponse OktaUserToOktaMembershipUser(User oktaUser, bool populateApps = true)
    {
        bool isApproved = false;
        bool isLockedOut = false;
        var status = oktaUser.Status;
        if (status == "ACTIVE")
        {
            isApproved = true;
        }
        if (status == "PASSWORD_EXPIRED" | status == "LOCKED_OUT" | status == "RECOVERY")
        {
            isLockedOut = true;
        }

        return new SimpleUserLoginResponse()
        {
            Email = oktaUser.Profile.Email,
            UserName = oktaUser.Id,
            IsApproved = isApproved,
            IsLockedOut = isLockedOut
        };
    }
}
*/
