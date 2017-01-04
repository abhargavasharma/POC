using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.Infrastructure.Time;
using TAL.QuoteAndApply.Policy.Data;
using TAL.QuoteAndApply.Policy.Models;

namespace TAL.QuoteAndApply.Policy.Service.AccessControl
{
    public interface IPolicyAccessControlCreationService
    {
        void InsertNewAccessControl(int policyId);
    }

    public class PolicyAccessControlCreationService : IPolicyAccessControlCreationService
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IPolicyAccessControlDtoRepository _policyAccessControlDtoRepository;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IAccessControlTypeProvider _accessControlTypeProvider;

        public PolicyAccessControlCreationService(IDateTimeProvider dateTimeProvider, IPolicyAccessControlDtoRepository policyAccessControlDtoRepository, ICurrentUserProvider currentUserProvider, IAccessControlTypeProvider accessControlTypeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
            _policyAccessControlDtoRepository = policyAccessControlDtoRepository;
            _currentUserProvider = currentUserProvider;
            _accessControlTypeProvider = accessControlTypeProvider;
        }

        public void InsertNewAccessControl(int policyId)
        {
            var currentUser = _currentUserProvider.GetForApplication();

            var dto = new PolicyAccessControlDto()
            {
                PolicyId = policyId,
                LastTouchedByName = currentUser.UserName,
                LastTouchedByType = _accessControlTypeProvider.GetFor(currentUser.Roles),
                LastTouchedTime = _dateTimeProvider.GetCurrentDateAndTime()
            };
            _policyAccessControlDtoRepository.InsertAccessControl(dto);
        }
    }
}