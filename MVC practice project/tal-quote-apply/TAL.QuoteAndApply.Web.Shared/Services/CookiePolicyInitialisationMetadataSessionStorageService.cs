using System;
using TAL.QuoteAndApply.Infrastructure.Crypto;
using TAL.QuoteAndApply.Infrastructure.Extensions;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.Web.Shared.Cookie;

namespace TAL.QuoteAndApply.Web.Shared.Services
{
    public class CookiePolicyInitialisationMetadataSessionStorageService : IPolicyInitialisationMetadataSessionStorageService
    {
        private readonly ICookieService _cookieService;
        private readonly ISecurityService _securityService;

        public CookiePolicyInitialisationMetadataSessionStorageService(ICookieService cookieService, ISecurityService securityService)
        {
            _cookieService = cookieService;
            _securityService = securityService;
        }

        private const string CookieName = "PolicyInitialisation";

        public void SaveMetadata(PolicyInitialisationMetadata policyInitialisationMetadata)
        {
            var cookieContents = _securityService.Encrypt(policyInitialisationMetadata.ToJson());

            _cookieService.SetCookie(CookieName, cookieContents, DateTime.Now.AddMinutes(120));
        }

        public PolicyInitialisationMetadata GetMetaData()
        {
            var cookieContents = _cookieService.GetCookieValue(CookieName);

            if (cookieContents == null)
                return null;

            var jsonObj = _securityService.Decrypt(cookieContents);

            return jsonObj.FromJson<PolicyInitialisationMetadata>();
        }
    }
}