using System.Collections.Generic;
using System.Linq;
using System.Monads;
using TAL.QuoteAndApply.Product.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Models.Mappers
{
    public interface ISelectedProductPlanOptionsConverter
    {
        SelectedProductPlanOptions From(AvailabilityPlanStateParam selectPlanStateParam);
    }

    public class SelectedProductPlanOptionsConverter : ISelectedProductPlanOptionsConverter
    {
        public SelectedProductPlanOptions From(AvailabilityPlanStateParam selectPlanStateParam)
        {
            return new SelectedProductPlanOptions(selectPlanStateParam.PlanCode,
                selectPlanStateParam.BrandKey,
                selectPlanStateParam.SelectedPlanCodes.With<IEnumerable<string>, List<string>>(sr => sr.ToList()), 
                selectPlanStateParam.SelectedCoverCodes.With<IEnumerable<string>, List<string>>(sr => sr.ToList()),
                selectPlanStateParam.SelectedRiderCodes.With<IEnumerable<string>, List<string>>(sr => sr.ToList()),
                selectPlanStateParam.SelectedRiderCoverCodes.With<IEnumerable<string>, 
                List<string>>(sr => sr.ToList()),
                (int)selectPlanStateParam.Age+1,
                selectPlanStateParam.OccupationClass,
                selectPlanStateParam.WaitingPeriod,
                selectPlanStateParam.LinkedToCpi);
        }
    }
}
