namespace TAL.QuoteAndApply.Product.Definition
{
    public class ProductPlanConstants
    {
        public const string LifePlanCode = "DTH";
        public const string CriticalIllnessPlanCode = "TRS";
        public const string PermanentDisabilityPlanCode = "TPS";
        public const string IncomeProtectionPlanCode = "IP";
    }

    public class ProductRiderConstants
    {
        public const string CriticalIllnessRiderCode = "TRADTH";
        public const string PermanentDisabilityRiderCode = "TPDDTH";
    }

    public class ProductCoverConstants
    {
        public const string LifeAccidentCover = "DTHAC";
        public const string LifeIllnessCover = "DTHIC";
        public const string LifeAdventureSportsCover = "DTHASC";

        public const string CiSeriousIllnessCover = "TRSSIC";
        public const string CiCancerCover = "TRSCC";
        public const string CiSeriousInjuryCover = "TRSSIN";

        public const string TpdAccidentCover = "TPSAC";
        public const string TpdIllnessCover = "TPSIC";
        public const string TpdAdventureSportsCover = "TPSASC";

        public const string IpAccidentCover = "IPSAC";
        public const string IpIllnessCover = "IPSIC";
        public const string IpAdventureSportsCover = "IPSSC";
    }

    public class ProductRiderCoverConstants
    {
        public const string CiRiderSeriousIllnessCover = "TRADTHSIC";
        public const string CiRiderCancerCover = "TRADTHCC";
        public const string CiRiderSeriousInjuryCover = "TRADTHSIN";

        public const string TpdRiderAccidentCover = "TPDDTHAC";
        public const string TpdRiderIllnessCover = "TPDDTHIC";
        public const string TpdRiderAdventureSportsCover = "TPDDTHASC";

    }

    public class ProductOptionConstants
    {
        public const string LifePremiumRelief = "PR";
        public const string TpdPremiumRelief = "PR";
        public const string CiPremiumRelief = "PR";
        public const string IpDayOneAccident = "DOA";
        public const string IpIncreasingClaims = "IC";
        public const string TpdRiderDeathBuyBack = "TPDDTHDBB";
        public const string CiRiderDeathBuyBack = "TRADTHDBB";
    }

    public class ProductPlanVariableConstants
    {
        public const string LinkedToCpi = "linkedToCpi";
        public const string PremiumHoliday = "premiumHoliday";
        public const string PremiumType = "premiumType";
        public const string WaitingPeriod = "waitingPeriod";
        public const string BenefitPeriod = "benefitPeriod";
        public const string OccupationDefinition = "occupationDefinition";
    }

    public class ProductOccupationClassConstants
    {
        public const string AAA = "AAA";
        public const string AAPlus = "AA+";
        public const string AA = "AA";
        public const string A = "A";
        public const string BBB = "BBB";
        public const string BBPlus = "BB+";
        public const string BB = "BB";
        public const string B = "B";
        public const string SRA = "SRA";
    }
}