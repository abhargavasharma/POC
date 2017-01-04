using System.Collections.Generic;

namespace TAL.QuoteAndApply.ServiceLayer.Product.Rules
{
    public class AvailableFeature
    {
        public AvailableFeature(string code, bool isAvailable, IEnumerable<string> reasonIfUnavailable)
        {
            Code = code;
            IsAvailable = isAvailable;
            ReasonIfUnavailable = reasonIfUnavailable;
            ChildAvailableFeatures = new List<AvailableFeature>();
        }

        public static AvailableFeature Available(string code)
        {
            return new AvailableFeature(code, true, new string[0]);
        }

        public static AvailableFeature Unavailable(string code, params string[] reasons)
        {
            return new AvailableFeature(code, false, reasons);
        }

        public void AddSubAvailableFeature(AvailableFeature availableFeature)
        {
            ChildAvailableFeatures.Add(availableFeature);
        }

        public string Code { get; private set; }
        public bool IsAvailable { get; private set; }
        public IEnumerable<string> ReasonIfUnavailable { get; private set; }
        public IList<AvailableFeature> ChildAvailableFeatures { get; }
    }
}