namespace TAL.QuoteAndApply.Underwriting.Models.Talus
{
    public class ValueContainer<T>
    {
        public readonly T Value;

        public ValueContainer(T value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return string.Format("{0}", Value);
        }
    }
}
