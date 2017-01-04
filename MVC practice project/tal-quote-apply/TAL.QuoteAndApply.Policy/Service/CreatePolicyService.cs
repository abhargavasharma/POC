using System;
using System.Collections.Generic;
using TAL.QuoteAndApply.DataLayer.Exceptions;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.Infrastructure.Logging;
using TAL.QuoteAndApply.Policy.Data;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Service.AccessControl;
using TAL.QuoteAndApply.Product.Definition;

namespace TAL.QuoteAndApply.Policy.Service
{
    public interface ICreatePolicyService
    {
        PolicyDto CreatePolicy(PolicyDefaults policyDefaults, PolicySource source);
    }

    public class CreatePolicyService: ICreatePolicyService
    {
        private readonly IPolicyDtoRepository _policyDtoRepository;
        private readonly IPolicyAccessControlCreationService _policyAccessControlService;
        private readonly IQuoteReferenceGenerationService _quoteReferenceGenerationService;
        private readonly ILoggingService _loggingService;

        public CreatePolicyService(IPolicyDtoRepository policyDtoRepository,
            IQuoteReferenceGenerationService quoteReferenceGenerationService, 
            ILoggingService loggingService,
            IPolicyAccessControlCreationService policyAccessControlService)
        {
            _policyDtoRepository = policyDtoRepository;
            _quoteReferenceGenerationService = quoteReferenceGenerationService;
            _loggingService = loggingService;
            _policyAccessControlService = policyAccessControlService;
        }

        public PolicyDto CreatePolicy(PolicyDefaults policyDefaults, PolicySource source)
        {
            const int totalRetryAttempts = 5;

            //todo: having some issues with quote reference numbers during Integration Tests
            //log them all so we can spit them out in the error message
            var attemptedQuoteReferenceNumbers = new List<string>();

            var policy = new PolicyDto();
            var insertAttempt = 0;
            while (insertAttempt < totalRetryAttempts)
            {
                try
                {
                    policy.QuoteReference = _quoteReferenceGenerationService.RandomQuoteReference();
                    policy.DeclarationAgree = false;
                    policy.PremiumFrequency = policyDefaults.PremiumFrequency;
                    policy.Status = PolicyStatus.Incomplete;
                    policy.Progress = PolicyProgress.Unknown;
                    policy.Source = source;
                    policy.BrandId = policyDefaults.BrandId;
                    policy.OrganisationId = policyDefaults.OrganisationId;

                    attemptedQuoteReferenceNumbers.Add(policy.QuoteReference);

                    var insertedPolicy = _policyDtoRepository.InsertPolicy(policy);
                    _policyAccessControlService.InsertNewAccessControl(insertedPolicy.Id);

                    insertedPolicy.BrandKey = policyDefaults.BrandKey;

                    return insertedPolicy;
                }
                catch (DataLayerUniqueKeyContraintException ex)
                {
                    _loggingService.Error(string.Format("Error creating policy with quote reference: {0}", policy.QuoteReference), ex);
                    insertAttempt++;
                }
            }

            throw new ApplicationException(string.Format("Could not Create Policy after {0} attempts. These quote reference numbers were attempted: {1} ", totalRetryAttempts, string.Join(",", attemptedQuoteReferenceNumbers)));

        }

    }

}
