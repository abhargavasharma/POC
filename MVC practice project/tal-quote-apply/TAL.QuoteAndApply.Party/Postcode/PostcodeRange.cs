namespace TAL.QuoteAndApply.Party.Postcode
{
    public class PostcodeRange
    {
        public int Lower { get; }
        public int Upper { get; }

        public PostcodeRange(int lower, int upper)
        {
            Lower = lower;
            Upper = upper;
        }

        public bool ContainsPostcode(int postcode)
        {
            return postcode >= Lower && postcode <= Upper;
        }
    }
}