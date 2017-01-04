namespace TAL.QuoteAndApply.Web.Shared.Workflow
{
    public class StepWorkflowItem
    {
        public int Weight { get; private set; }
        public string DisplayName { get; private set; }

        public StepWorkflowItem(int weight, string displayName)
        {
            Weight = weight;
            DisplayName = displayName;
        }
    }

    public class StepWorkflowContext
    {
        public ApplicationStep ApplicationStep { get; private set; }
        public int StepIndex { get; private set; }
        public StepWorkflowItem[] WorkFlowItems { get; private set; }

        public string ProductName { get; set; }
        public string ProductCode { get; set; }

        public int TotalStepsInWorkFlow
        {
            get { return WorkFlowItems.Length; }
        }

        public bool ShowNavaigationMenu { get; private set; }
        public bool ShowStepCount { get; private set; }

        public StepWorkflowContext(ApplicationStep step, int index, StepWorkflowItem[] stepWeights, bool showNavigationMenu, bool showStepCount)
        {
            ApplicationStep = step;
            StepIndex = index;
            WorkFlowItems = stepWeights;
            ShowNavaigationMenu = showNavigationMenu;
            ShowStepCount = showStepCount;
        }

        public StepWorkflowContext(ApplicationStep step)
        {
            ApplicationStep = step;
            WorkFlowItems = new StepWorkflowItem[0];
        }
    }
}