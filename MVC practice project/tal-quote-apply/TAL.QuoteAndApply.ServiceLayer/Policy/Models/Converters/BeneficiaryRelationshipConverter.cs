using System.Collections.Generic;
using TAL.QuoteAndApply.Policy.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Models.Converters
{
    public interface IBeneficiaryRelationshipConverter
    {
        IReadOnlyList<BeneficiaryRelationship> From(IReadOnlyList<IBeneficiaryRelationshipDto> relationshipToTheInsuredDtos);
    }

    public class BeneficiaryRelationshipConverter : IBeneficiaryRelationshipConverter
    {
        public IReadOnlyList<BeneficiaryRelationship> From(IReadOnlyList<IBeneficiaryRelationshipDto> relationshipToTheInsuredDtos)
        {
            var relationships = new List<BeneficiaryRelationship>();
            foreach (var dto in relationshipToTheInsuredDtos)
            {
                relationships.Add(new BeneficiaryRelationship
                {
                    Id = dto.Id,
                    Code = dto.PASExportValue,
                    Description = dto.Description
                });                
            }
            return relationships.AsReadOnly();    
        }
    }
}
