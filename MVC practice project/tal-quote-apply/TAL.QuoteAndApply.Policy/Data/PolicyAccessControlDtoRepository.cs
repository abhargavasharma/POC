using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAL.QuoteAndApply.DataLayer.Factory;
using TAL.QuoteAndApply.DataLayer.Repository;
using TAL.QuoteAndApply.DataLayer.Repository.PredicateLogic;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.Infrastructure.Cache;
using TAL.QuoteAndApply.Policy.Configuration;
using TAL.QuoteAndApply.Policy.Models;

namespace TAL.QuoteAndApply.Policy.Data
{
    public interface IPolicyAccessControlDtoRepository
    {
        PolicyAccessControlDto GetAccessControlForPolicy(int policyId);
        void InsertAccessControl(PolicyAccessControlDto accesssControlDto);
        void UpdateAccessControl(PolicyAccessControlDto accesssControlDto);
    }

    public class PolicyAccessControlDtoRepository : BaseRepository<PolicyAccessControlDto>, IPolicyAccessControlDtoRepository
    {
        private readonly ICachingWrapper _cachingWrapper;
        private readonly IPolicyConfigurationProvider _config;
        private readonly IEndRequestOperationCollection _endRequestOperationCollection;

        public PolicyAccessControlDtoRepository(IPolicyConfigurationProvider config,
            ICurrentUserProvider currentUserProvider,
            IDataLayerExceptionFactory dataLayerExceptionFactory, IDbItemEncryptionService dbItemEncryptionService,
            ICachingWrapper cachingWrapper, IEndRequestOperationCollection endRequestOperationCollection)
            : base(config.ConnectionString, currentUserProvider, dataLayerExceptionFactory, dbItemEncryptionService)
        {
            _cachingWrapper = cachingWrapper;
            _endRequestOperationCollection = endRequestOperationCollection;
            _config = config;
        }

        public PolicyAccessControlDto GetAccessControlForPolicy(int policyId)
        {
            var q = DbItemPredicate<PolicyAccessControlDto>.Where(p => p.PolicyId, Op.Eq, policyId);
            return _cachingWrapper.GetOrAddCacheItemRequestScope(GetType(), $"PolicyId-{policyId}", () => Query(q).FirstOrDefault());
        }

        public void UpdateAccessControl(PolicyAccessControlDto accesssControlDto)
        {
            if (!_config.AccessControlSessionStorage)
            {
                Update(accesssControlDto);
                var updatedDto = Get(accesssControlDto.Id);
                accesssControlDto.RV = updatedDto.RV;
            }
            else
            {
                _endRequestOperationCollection.AddOrUpdateAction("PolicyAccessControlDtoRepository.Update", () => Update(accesssControlDto));
            }
            
            _cachingWrapper.UpdateOrAddCacheItemRequestScope(GetType(), $"PolicyId-{accesssControlDto.PolicyId}", accesssControlDto);
        }

        public void InsertAccessControl(PolicyAccessControlDto accesssControlDto)
        {
            var dtoInserted = Insert(accesssControlDto);
            _cachingWrapper.UpdateOrAddCacheItemRequestScope(GetType(), $"PolicyId-{accesssControlDto.PolicyId}", dtoInserted);
        }

        public new bool Delete(PolicyAccessControlDto accesssControlDto)
        {
            var result = base.Delete(accesssControlDto);
            if (result)
            {
                _cachingWrapper.RemoveItem(GetType(), $"PolicyId-{accesssControlDto.PolicyId}");
            }
            return result;
        }
    }
}
