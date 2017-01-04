using System;
using System.Collections.Generic;
using TAL.QuoteAndApply.Underwriting.Configuration;

namespace TAL.QuoteAndApply.ServiceLayer.Underwriting
{
    /// <summary>
    /// THIS IS A DIRTY BIG HACK TO SUPPORT ARMSTRONG AND TAL CONSUMER UW TEMPLATES
    /// BACK THIS OUT WHEN WE CAN RETIRE ARMSTRONG
    /// </summary>
    public class QuestionIdsGetter
    {
        private readonly IUnderwritingConfigurationProvider _underwritingConfigurationProvider;

        public QuestionIdsGetter(IUnderwritingConfigurationProvider underwritingConfigurationProvider)
        {
            _underwritingConfigurationProvider = underwritingConfigurationProvider;
        }

        public string DisclosureQuestion => "Duty of Disclosure Statement?e3";

        public string EmploymentQuestion
        {
            get
            {
                if (IsArmstrongTemplate())
                    return "Employment?2m";

                return "Employment?186";
            }
        }

        public string ResidencyQuestion
        {
            get
            {
                if (IsArmstrongTemplate())
                    return "Personal Details?2";

                return "Personal Details?e7";
            }
        }

        public string SmokerQuestion
        {
            get
            {
                if (IsArmstrongTemplate())
                    return "Personal Details?4";

                return "Personal Details?e8";
            }
        }

        public string DateOfBirthQuestion
        {
            get
            {
                if (IsArmstrongTemplate())
                    return "Personal Details?0";

                return "Personal Details?wb";
            }
        }

        public string GenderQuestion
        {
            get
            {
                if (IsArmstrongTemplate())
                    return "Health and Lifestyle?1";

                return "Personal Details?Gender";
            }
        }

        public string AnnualIncomeQuestion
        {
            get
            {
                if (IsArmstrongTemplate())
                    return null;

                return "Employment?g3";
            }
        }

        private bool IsArmstrongTemplate()
        {
            if(!string.IsNullOrEmpty(_underwritingConfigurationProvider.TemplateName))
                return _underwritingConfigurationProvider.TemplateName.Equals("Armstrong", StringComparison.InvariantCultureIgnoreCase);

            return false;
        }
    }


    /// <summary>
    /// To be replaced with Tags Class
    /// </summary>
    public static class QuestionIds
    {
        private static readonly QuestionIdsGetter _questionIdsGetter = new QuestionIdsGetter(new UnderwritingConfigurationProvider());

        public static string DisclosureQuestion => _questionIdsGetter.DisclosureQuestion;

        public static string EmploymentQuestion
        {
            get { return _questionIdsGetter.EmploymentQuestion; }
        }

        public static string ResidencyQuestion
        {
            get { return _questionIdsGetter.ResidencyQuestion; }
        }

        public static string SmokerQuestion
        {
            get { return _questionIdsGetter.SmokerQuestion; }
        }

        public static string DateOfBirthQuestion
        {
            get { return _questionIdsGetter.DateOfBirthQuestion; }
        }

        public static string GenderQuestion
        {
            get { return _questionIdsGetter.GenderQuestion; }
        }
        public static string AnnualIncomeQuestion
        {
            get { return _questionIdsGetter.AnnualIncomeQuestion; }
        }

        public static List<string> ReplayQuestions = new List<string>
        {
            AnnualIncomeQuestion,
            GenderQuestion,
            SmokerQuestion,
            DateOfBirthQuestion
        };
    }

    public class QuestionTagConstants
    {
        public const string IndustryQuestionTag = "GLOBAL_INDUSTRY_QUESTION";
        public const string OccupationQuestionTag = "GLOBAL_OCCUPATION_QUESTION";
        public const string AnnualIncomeQuestionTag = "DIGITAL_ANNUAL_INCOME";
        public const string GenderQuestionTag = "DIGITAL_GENDER";
        public const string DateOfBirthQuestionTag = "DIGITAL_DATE_OF_BIRTH";
        public const string SmokerQuestionTag = "DIGITAL_SMOKER";
        public const string ResidencyQuestionTag = "DIGITAL_AUSTRALIAN_RESIDENT";
        public const string ChoicePoint = "DIGITAL_CHOICE_POINT";
    }

    
}