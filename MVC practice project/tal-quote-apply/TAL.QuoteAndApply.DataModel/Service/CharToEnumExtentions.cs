using TAL.QuoteAndApply.DataModel.Personal;

namespace TAL.QuoteAndApply.DataModel.Service
{
    public static class CharToEnumExtentions
    {
        public static Gender MapToGender(this char gender)
        {
            if (gender == 'M' || gender == 'm')
                return Gender.Male;

            if (gender == 'F' || gender == 'f')
                return Gender.Female;

            return Gender.Unknown;
        }
    }
}