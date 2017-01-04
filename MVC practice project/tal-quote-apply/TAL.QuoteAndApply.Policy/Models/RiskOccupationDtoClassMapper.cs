using TAL.QuoteAndApply.DataLayer.Service;

namespace TAL.QuoteAndApply.Policy.Models
{
    public sealed class RiskOccupationDtoClassMapper : DbItemClassMapper<RiskOccupationDto>
    {
        protected override void DefineMappings(ClassMapperWrapper<RiskOccupationDto> mapper)
        {
            mapper.MapTable("RiskOccupation");

            mapper.MapProperty(o => o.OccupationCode, "OccupationAnswerId");
            mapper.MapProperty(o => o.IndustryCode, "IndustryAnswerId");
            mapper.MapProperty(o => o.TpdLoading, "TpdOccupationLoading");
        }
    }
}