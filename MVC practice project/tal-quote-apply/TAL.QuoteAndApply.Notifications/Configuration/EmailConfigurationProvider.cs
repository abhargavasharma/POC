using System;
using System.Configuration;

namespace TAL.QuoteAndApply.Notifications.Configuration
{
    public interface IEmailConfigurationProvider
    {
        string LiveCustomerPortalPdsUrl { get; }
        string SitecoreContentBaseUrl { get; }
        string DefaultSenderEmailAddress { get; }
        string Header_01 { get; }
        string Header_02 { get; }
        string Header_03 { get; }
        string Icon_australia { get; }
        string Icon_coins { get; }
        string Icon_grow { get; }
        string Icon_people { get; }
        string Icon_person { get; }
        string Icon_piggy { get; }
        string Icon_tick { get; }
        string Logo { get; }
        string Placeholder_ltall_mum { get; }
        string Placeholder_tall_01 { get; }
        string Placeholder_tall_02 { get; }
        string Placeholder_tall_mum { get; }
        string Placeholder_wide_01 { get; }
        string Placeholder_wide_02 { get; }
        string Placeholder_wide_03 { get; }
        string Placeholder_wide_ipad  { get; }
        string Placeholder_xtall_mum { get; }
        string Video_01 { get; }
        bool AllowExternalEmails { get; }
        string DefaultBccEmailAddress { get; }
        string NoReplyEmailAddress { get; }
        string Full_width_spacer { get; }
        string Tal_bicycle_man { get; }
    }

    public class EmailConfigurationProvider : IEmailConfigurationProvider
    {
        public string LiveCustomerPortalPdsUrl => ConfigurationManager.AppSettings["EmailConfiguration.LiveCustomerPortalPdsUrl"];
        public string DefaultSenderEmailAddress => ConfigurationManager.AppSettings["EmailConfiguration.DefaultSenderEmailAddress"];
        public string SitecoreContentBaseUrl => ConfigurationManager.AppSettings["EmailConfiguration.SitecoreContentBaseUrl"];

        public bool AllowExternalEmails
        {
            get
            {
                bool outValue;
                if (Boolean.TryParse(ConfigurationManager.AppSettings["EmailConfiguration.AllowExternalEmails"],
                    out outValue))
                {
                    return outValue;
                }
                return false;
            }
        }

        public string EmailImageDomain 
        {
            get { return ConfigurationManager.AppSettings["EmailConfiguration.EmailImageDomain"]; }
        }
        public string DefaultBccEmailAddress => "ConsumerCustomerCorr@tal.com.au";
        public string NoReplyEmailAddress => "no-reply @tal.com.au";
        public string Header_01  => $"{EmailImageDomain}/~/media/D9E79B30E3B94AFB8FBAB2EBD40BC7B9.ashx";
        public string Header_02  => $"{EmailImageDomain}/~/media/D03C70D817D344A6B8B42DB6C09DC2B3.ashx";
        public string Header_03  => $"{EmailImageDomain}/~/media/ED690280C456448F8D4F7DF29038AAA3.ashx";
        public string Icon_australia => $"{EmailImageDomain}/~/media/36D0DC14B99A4EFAA9BC8515823E23E3.ashx";
        public string Icon_coins => $"{EmailImageDomain}/~/media/161D42DECCB14B2690D56AC1FC0C11E1.ashx";
        public string Icon_grow  => $"{EmailImageDomain}/~/media/25F6F4F8A25F4B54A6541F7C719C8FF4.ashx";
        public string Icon_people  => $"{EmailImageDomain}/~/media/1CD177BD21E9430EB8FD4F7481BC3386.ashx";
        public string Icon_person => $"{EmailImageDomain}/~/media/88EADD601169448298DF28C3CD98E6D9.ashx";
        public string Icon_piggy => $"{EmailImageDomain}/~/media/A1E10BBCFC7941A7A8763A29CDEFD77D.ashx";
        public string Icon_tick  => $"{EmailImageDomain}/~/media/D8A023CFC6114797A48F74969F09ECE3.ashx";
        public string Logo => $"{EmailImageDomain}/~/media/1956CBF654CD4D96906B2000C8FD7892.ashx";
        public string Placeholder_ltall_mum => $"{EmailImageDomain}/~/media/E4FAD2C2E7C241C29464E290A51BE396.ashx";
        public string Placeholder_tall_01 => $"{EmailImageDomain}/~/media/49F37ACCEC544B7185779FAF573B4DCE.ashx";
        public string Placeholder_tall_02 => $"{EmailImageDomain}/~/media/0EA8C23BB90248B7BF88C37980E37B7B.ashx";
        public string Placeholder_tall_mum => $"{EmailImageDomain}/~/media/1A47135885134D549827A5A087B3FE97.ashx";
        public string Placeholder_wide_01 => $"{EmailImageDomain}/~/media/DEAEFDD778464B10A3BDA0F11768F1B5.ashx";
        public string Placeholder_wide_02 => $"{EmailImageDomain}/~/media/AF3E2F3B95A14E2A8942773599ACAB5D.ashx";
        public string Placeholder_wide_03 => $"{EmailImageDomain}/~/media/97B16E772EE545738ABD2E8E3EBB4293.ashx";
        public string Placeholder_wide_ipad => $"{EmailImageDomain}/~/media/F0726E6C54664B7D84399C74166A906C.ashx";
        public string Placeholder_xtall_mum => $"{EmailImageDomain}/~/media/98961FD23EDF4FDF9DFCCCF7CC9AEEFE.ashx";
        public string Video_01 => $"{EmailImageDomain}/~/media/5F59D802534C44A48A5F64A5B828A613.ashx";
        public string Full_width_spacer => $"{EmailImageDomain}/~/media/b3813155f7b2418c9cb4e59608db5023.ashx";
        public string Tal_bicycle_man => $"{EmailImageDomain}/~/media/02ef2647eb444f77b338517fbd18fd65.ashx";
    }
}
