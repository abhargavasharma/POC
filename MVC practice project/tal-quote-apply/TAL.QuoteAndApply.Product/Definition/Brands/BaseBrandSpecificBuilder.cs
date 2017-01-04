using System;
using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.DataModel.Payment;
using TAL.QuoteAndApply.Product.Contracts;
using TAL.QuoteAndApply.Product.Models.Definition;

namespace TAL.QuoteAndApply.Product.Definition.Brands
{
    public abstract class BaseBrandSpecificBuilder : IBrandSpecificBuilder
    {
        protected readonly IProductBrandSettingsProvider ProductBrandSettingsProvider;
        protected readonly string BrandKey;

        protected BaseBrandSpecificBuilder(IProductBrandSettingsProvider productBrandSettingsProvider, string brandKey)
        {
            ProductBrandSettingsProvider = productBrandSettingsProvider;
            BrandKey = brandKey;
        }

        public ProductDefinition ApplyBrandSpecifics(ProductDefinition baseProductDefinition)
        {
            var productDefinition = ApplyCustomSettings(baseProductDefinition);

            baseProductDefinition.PaymentOptions = GetAvailablePaymentTypes().ToList();
            // do settings stuff here

            return productDefinition;
        }

        protected abstract ProductDefinition ApplyCustomSettings(ProductDefinition baseProductDefinition);

        private IEnumerable<IPaymentDefinition> GetAvailablePaymentTypes()
        {
            var paymentTypes = ProductBrandSettingsProvider
                .GetSetting(BrandKey, ProductBrandSettingConstants.SettingKey_AvailablePaymentTypes, "CreditCard,DirectDebit")
                .Split(',');

            if (paymentTypes.Contains(ProductBrandSettingConstants.PaymentMethodCreditCard))
            {
                var cardTypes = ProductBrandSettingsProvider.GetSetting(BrandKey, ProductBrandSettingConstants.SettingKey_AvailableCreditCardTypes,
                    "Visa,MasterCard").Split(',').Select(ct =>
                    {
                        CreditCardType outResult;
                        return Enum.TryParse(ct, out outResult) ? outResult : CreditCardType.Unknown;
                    }).Where(ct => ct != CreditCardType.Unknown);
                yield return new CreditCardPaymentDefinition { AvailableCreditCardTypes = cardTypes.ToList()};
            }
            if (paymentTypes.Contains(ProductBrandSettingConstants.PaymentMethodDirectDebit))
            {
                yield return new DirectDebitPaymentDefinition();
            }
            if (paymentTypes.Contains(ProductBrandSettingConstants.PaymentMethodSMSF))
            {
                yield return new SelfManagementSuperFundPaymentDefinition();
            }
            if (paymentTypes.Contains(ProductBrandSettingConstants.PaymentMethodSuper))
            {
                yield return new SuperannuationPaymentDefinition();
            }
        }
    }
}