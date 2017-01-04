namespace TAL.QuoteAndApply.Underwriting.Models.Talus
{
    public class Tags
    {
        //Question specific tags
        public const string AnnualIncomePermissionTag = "DIGITAL_ANNUAL_INCOME_PERMISSION";
        public const string AnnualIncomeTag = "DIGITAL_ANNUAL_INCOME";
        public const string AustralianResidentTag = "DIGITAL_AUSTRALIAN_RESIDENT";
        public const string DateOfBirthTag = "DIGITAL_DATE_OF_BIRTH";
        public const string GenderTag = "DIGITAL_GENDER";
        public const string SmokerTag = "DIGITAL_SMOKER";
        public const string HeightTag = "DIGITAL_HEIGHT";
        public const string WeightTag = "DIGITAL_WEIGHT";
        public const string SmokerHeavyTag = "DIGITAL_HEAVY_SMOKER";
        public const string OccupationTag = "DIGITAL_OCCUPATION";
        public const string OccupationOtherTag = "DIGITAL_OCCUPATION_OTHER";
        public const string ShowQuestionHelpText = "DIGITAL_SHOW_QUESTION_HELPTEXT";

        //Generic tags
        public const string Exclude = "DIGITAL_EXCLUDE";
        public const string NotListed = "DIGITAL_NOT_LISTED";
        public const string SeperateNoAnswer = "DIGITAL_SEPARATE_NO_ANSWER";
        public const string AnswerYes = "DIGITAL_ANSWER_YES";
        public const string AnswerNo = "DIGITAL_ANSWER_NO";
        public const string ShowAnswerHelpText = "DIGITAL_SHOW_ANSWER_HELPTEXT";
        public const string AnswerIsContextual = "DIGITAL_ANSWER_IS_CONTEXT";
        public const string AnswerStartPoint = "DIGITAL_ANSWER_START";
    }

    public class Variables
    {
        public const string IsLimitCoverDueToUnderwriting = "IsLimitCoverDueToUnderwriting";
        public const string OccupationClass = "OccupationClass";
    }
}
