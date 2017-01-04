using System.Collections.Generic;

namespace TAL.QuoteAndApply.Notifications.Models
{
    public class EmailQuoteViewModel
    {
        public string QuoteReferenceNumber { get; set; }
        public string FirstName { get; set; }
        public string EmailAddress { get; set; }
        public ImagePaths ImgPaths { get; set; }
        public string RetrieveUrl { get; set; }
        public string PdsUrl { get; set; }
        public string ViewEmailInBrowserUrl { get; set; }
        public string UserEmailAddress { get; set; }
        public bool AllowEmail { get; set; }
    }

    public class EmailQuoteSavedViewModel
    {
        public string AgentEmailAddress { get; set; }
        public string FullAgentName { get; set; }
        public EmailQuoteViewModel BaseEmailModel { get; set; }
        public List<EmailQuoteSavedPlanSummary> PlanSummaries { get; set; }
        public string PremiumTotal { get; set; }
        public string PremiumFrequency { get; set; }
        public string Brandkey { get; set; }
    }

    public class EmailQuoteSavedPlanSummary
    {
        public string Name { get; set; }
        public string CoverAmount { get; set; }
        public string ParentName { get; set; }
        public string Premium { get; set; }
        public bool IsRider { get; set; }
        public List<EmailQuoteSavedCoverSummary> CoverSummaries { get; set; }
    }

    public class EmailQuoteSavedCoverSummary
    {
        public string Name { get; set; }
    }

    public class ImagePaths
    {
        public string Header_01 { get; set; }
        public string Header_02 { get; set; }
        public string Header_03 { get; set; }
        public string Logo { get; set; }
        public string Icon_Tick { get; set; }
        public string Placeholder_tall_mum { get; set; }
        public string Icon_people { get; set; }
        public string Icon_australia { get; set; }
        public string Icon_coins { get; set; }
        public string Tal_bicycle_man { get; set; }
        public string Full_width_spacer { get; set; }
    }
}
