using System;
using TAL.QuoteAndApply.DataModel.Interactions;
using TAL.QuoteAndApply.DataModel.Personal;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.DataModel.Product;

namespace TAL.QuoteAndApply.DataModel.Service
{
    public static class EnumToStringExtensions
    {
        public static string ToFriendlyString(this State state)
        {
            return state.ToString();
        }

        public static string ToFriendlyString(this Gender gender)
        {
            var c = ToFriendlyChar(gender);

            if (c == null)
                return null;

            return c.ToString();
        }

        public static char? ToFriendlyChar(this Gender gender)
        {
            switch (gender)
            {
                case Gender.Male:
                    return 'M';
                case Gender.Female:
                    return 'F';
                default:
                    return null;
            }
        }

        public static string ToShortString(this PremiumFrequency premiumFrequency)
        {
            switch (premiumFrequency)
            {
                case PremiumFrequency.Monthly:
                    return "M";
                case PremiumFrequency.Quarterly:
                    return "Q";
                case PremiumFrequency.HalfYearly:
                    return "HY";
                case PremiumFrequency.Yearly:
                    return "A";
                default:
                    return null;
            }
        }

        public static string ToFriendlyString(this PremiumFrequency premiumFrequency)
        {
            switch (premiumFrequency)
            {
                case PremiumFrequency.Monthly:
                    return "Monthly";
                case PremiumFrequency.Quarterly:
                    return "Quarterly";
                case PremiumFrequency.HalfYearly:
                    return "Half Yearly";
                case PremiumFrequency.Yearly:
                    return "Yearly";
                default:
                    return null;
            }
        }
    }


    public static class StringToEnumExtenstions
    {
        public static State MapToState(this string state)
        {
            if (state == null)
                return State.Unknown;

            State retVal;
            Enum.TryParse(state, true, out retVal);

            return retVal;
        }

        public static PremiumFrequency MapToPremiumFrequency(this string premiumFrequency)
        {
            if (premiumFrequency == null)
                return PremiumFrequency.Unknown;

            PremiumFrequency retVal;
            Enum.TryParse(premiumFrequency.Replace(" ", ""), true, out retVal);

            return retVal;
        }

        public static PolicyProgress MapToProgress(this string progress)
        {
            if (progress == null)
                return PolicyProgress.Unknown;

            PolicyProgress retVal;
            Enum.TryParse(progress, true, out retVal);

            return retVal;
        }

        public static PolicyOwnerType MapToOwnerType(this string ownerType)
        {
            if (ownerType == null)
                return PolicyOwnerType.Ordinary;

            PolicyOwnerType retVal;
            Enum.TryParse(ownerType, true, out retVal);

            return retVal;
        }

        public static PremiumType MapToPremiumType(this string premiumType)
        {
            if (premiumType == null)
                return PremiumType.Unknown;

            PremiumType retVal;
            Enum.TryParse(premiumType, true, out retVal);

            return retVal;
        }

        public static OccupationDefinition MapToOccupationDefinition(this string occupationDefinition)
        {
            if (occupationDefinition == null)
                return OccupationDefinition.Unknown;

            OccupationDefinition retVal;
            Enum.TryParse(occupationDefinition, true, out retVal);

            return retVal;
        }

        public static Title MapToTitle(this string title)
        {
            if (title == null)
                return Title.Unknown;

            if (CaseInsenstiveMatch(title, "dr"))
                return Title.Dr;

            if (CaseInsenstiveMatch(title, "mr"))
                return Title.Mr;

            if (CaseInsenstiveMatch(title, "mrs"))
                return Title.Mrs;

            if (CaseInsenstiveMatch(title, "miss"))
                return Title.Miss;

            if (CaseInsenstiveMatch(title, "ms"))
                return Title.Ms;

            return Title.Unknown;
        }

        public static Gender MapToGender(this string gender)
        {
            if (gender == "M" || gender == "m")
                return Gender.Male;

            if (gender == "F" || gender == "f")
                return Gender.Female;

            return Gender.Unknown;
        }

        private static bool CaseInsenstiveMatch(string arg1, string arg2)
        {
            return arg1.Equals(arg2, StringComparison.OrdinalIgnoreCase);
        }

        public static string ToFriendlyString(this InteractionType interactionType)
        {
            return interactionType.ToString();
        }
    }

}