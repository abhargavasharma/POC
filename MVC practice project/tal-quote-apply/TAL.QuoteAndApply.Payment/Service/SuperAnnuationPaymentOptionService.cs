using TAL.QuoteAndApply.DataModel.Payment;
using TAL.QuoteAndApply.Payment.Data;
using TAL.QuoteAndApply.Payment.Models;
using TAL.QuoteAndApply.Payment.Service.TFN;

namespace TAL.QuoteAndApply.Payment.Service
{
    public interface ISuperAnnuationPaymentOptionService
    {
        ISuperAnnuationPayment AssignSuperAnnuationPayment(int policyId, string fundName, string fundProduct, string fundAbn,
            string fundUSI, string membershipNumber, string taxFileNumber);

        ISuperAnnuationPayment GetCurrentSuperAnnuationPayment(int policyId);
        void RemoveExistingPayment(int policyPaymentId);
    }

    public class SuperAnnuationPaymentOptionService : BasePaymentOptionService<SuperAnnuationPaymentDto>, ISuperAnnuationPaymentOptionService
    {
        private readonly ISuperAnnuationPaymentRepository _superAnnuationPaymentRepository;
        private readonly ITaxFileNumberEncyptionService _taxFileNumberEncyptionService;

        public ISuperAnnuationPayment AssignSuperAnnuationPayment(int policyId, string fundName, string fundProduct, string fundAbn,
            string fundUSI, string membershipNumber, string taxFileNumber)
        {
            var superPaymentDto = new SuperAnnuationPaymentDto()
            {
                FundName = fundName,
                FundABN = fundAbn,
                FundUSI = fundUSI,
                FundProduct = fundProduct,
                MembershipNumber = membershipNumber,
                TaxFileNumber = _taxFileNumberEncyptionService.Encrypt(taxFileNumber)
            };

            return AssignPayment(policyId, superPaymentDto, PaymentType.SuperAnnuation);
        }

        public ISuperAnnuationPayment GetCurrentSuperAnnuationPayment(int policyId)
        {
            var paymentDto = GetPolicyPaymentDto(policyId, PaymentType.SuperAnnuation);
            if (paymentDto == null)
                return null;

            return _superAnnuationPaymentRepository.GetForPolicyPaymentId(paymentDto.Id);
        }

        public SuperAnnuationPaymentOptionService(IPolicyPaymentRepository policyPaymentRepository,
            ISuperAnnuationPaymentRepository superAnnuationPaymentRepository,
            ITaxFileNumberEncyptionService taxFileNumberEncyptionService) : base(policyPaymentRepository)
        {
            _superAnnuationPaymentRepository = superAnnuationPaymentRepository;
            _taxFileNumberEncyptionService = taxFileNumberEncyptionService;
        }

        public override SuperAnnuationPaymentDto InsertIntoRepository(SuperAnnuationPaymentDto payment)
        {
            return _superAnnuationPaymentRepository.Insert(payment);
        }

        public override void RemoveExistingPayment(int policyPaymentId)
        {
            var superPayment = _superAnnuationPaymentRepository.GetForPolicyPaymentId(policyPaymentId);
            if (superPayment != null)
            {
                _superAnnuationPaymentRepository.Delete(superPayment);
            }
        }
    }
}
