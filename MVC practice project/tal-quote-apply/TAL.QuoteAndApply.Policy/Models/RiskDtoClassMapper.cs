using TAL.QuoteAndApply.DataLayer.Service;

namespace TAL.QuoteAndApply.Policy.Models
{
    public sealed class RiskDtoClassMapper : DbItemClassMapper<RiskDto>
    {
        protected override void DefineMappings(ClassMapperWrapper<RiskDto> mapper)
        {
            mapper.MapTable("Risk");
            mapper.MapProperty(risk => risk.Gender, "GenderId");
            mapper.MapProperty(risk => risk.Residency, "ResidencyId");
            mapper.MapProperty(risk => risk.SmokerStatus, "SmokerStatusId");
            mapper.MapProperty(risk => risk.InterviewStatus, "InterviewStatusId");

            mapper.Ignore(risk => risk.OccupationCode);
            mapper.Ignore(risk => risk.OccupationTitle);
            mapper.Ignore(risk => risk.OccupationClass);
            mapper.Ignore(risk => risk.IndustryTitle);
            mapper.Ignore(risk => risk.IndustryCode);
            mapper.Ignore(risk => risk.IsTpdAny);
            mapper.Ignore(risk => risk.IsTpdOwn);
            mapper.Ignore(risk => risk.TpdLoading);
            mapper.Ignore(risk => risk.PasCode);
        }
    }
}