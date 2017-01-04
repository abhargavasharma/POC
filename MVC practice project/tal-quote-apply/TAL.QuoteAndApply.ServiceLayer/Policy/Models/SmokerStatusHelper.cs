using System;
using System.IO;
using TAL.QuoteAndApply.Policy.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Models
{
    /*
        This class should manage the different ways that smoker status can be set via the web
        Either set with bool value to then get a smoker status enum
        Or set with an enum string value to get bool value of wheter they're a smoker or not
    */

    public class SmokerStatusHelper
    {
        public SmokerStatus Status { get; private set; }
        public bool IsSmoker { get; private set; }

        public SmokerStatusHelper(bool isSmoker)
        {
            IsSmoker = isSmoker;
            Status = isSmoker ? SmokerStatus.Yes : SmokerStatus.No;
        }

        public SmokerStatusHelper(string smokerStatusEnumNameOrYesNo)
        {
            if (smokerStatusEnumNameOrYesNo == null)
            {
                Status = SmokerStatus.Unknown;
            }
            else if (smokerStatusEnumNameOrYesNo.Equals("No", StringComparison.CurrentCultureIgnoreCase))
            {
                Status = SmokerStatus.No;
            }
            else if (smokerStatusEnumNameOrYesNo.Equals("Yes", StringComparison.CurrentCultureIgnoreCase))
            {
                Status = SmokerStatus.Yes;
            }
            else
            {
                Status = GetSmokerStatus(smokerStatusEnumNameOrYesNo);
            }

            SetIsSmokerBasedOnStatus();
        }

        private void SetIsSmokerBasedOnStatus()
        {
            switch (Status)
            {
                case SmokerStatus.Unknown:
                case SmokerStatus.No:
                    IsSmoker = false;
                    break;
                case SmokerStatus.Yes:
                    IsSmoker = true;
                    break;
                default:
                    throw new InvalidDataException("Un-accounted for Smoker Status");
            }
        }

        public SmokerStatus GetSmokerStatus(string smokerStatus)
        {
            SmokerStatus retVal;
            var convertSuccess = Enum.TryParse(smokerStatus, true, out retVal);
            if (!convertSuccess)
            {
                throw new ArgumentException("Invalid Smoker Status");
            }

            return retVal;
        }
    }
}