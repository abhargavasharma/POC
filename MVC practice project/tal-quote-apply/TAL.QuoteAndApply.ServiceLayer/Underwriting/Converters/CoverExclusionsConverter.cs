using System.Collections.Generic;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Underwriting.Models.Dto;

namespace TAL.QuoteAndApply.ServiceLayer.Underwriting.Converters
{
    public interface ICoverExclusionsConverter
    {
        IEnumerable<ICoverExclusion> From(ICover cover, IEnumerable<ReadOnlyExclusion> uwExclusions);
    }

    public class CoverExclusionsConverter: ICoverExclusionsConverter
    {
        public IEnumerable<ICoverExclusion> From(ICover cover, IEnumerable<ReadOnlyExclusion> uwExclusions)
        {
            var exclusions = new List<ICoverExclusion>();

            foreach (var uwEx in uwExclusions)
            {
                exclusions.Add(new CoverExclusionDto {CoverId = cover.Id, Text = uwEx.Text, Name = uwEx.Name});
            }

            return exclusions;
        }
    }
}