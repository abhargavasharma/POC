using System;
using TAL.QuoteAndApply.DataLayer.Service;
using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.DataModel.Policy;

namespace TAL.QuoteAndApply.Policy.Models
{
    public class PolicyAccessControlDto : DbItem
    {
        public int PolicyId { get; set; }
        public string LastTouchedByName { get; set; }
        public AccessControlType LastTouchedByType { get; set; }
        public DateTime LastTouchedTime { get; set; }
    }

    public sealed class PolicyAccessControlDtoClassMapper : DbItemClassMapper<PolicyAccessControlDto>
    {
        protected override void DefineMappings(ClassMapperWrapper<PolicyAccessControlDto> mapper)
        {
            mapper.MapTable("PolicyAccessControl");
            mapper.MapProperty(p => p.LastTouchedTime, "LastTouchedByTS");
            mapper.MapProperty(p => p.LastTouchedByType, "LastTouchedByTypeId");
        }
    }
}