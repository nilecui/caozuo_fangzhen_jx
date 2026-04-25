public static class StepValidator
{
    public static bool Validate(TrainingStep step, string interactedObjectName)
    {
        if (string.IsNullOrEmpty(interactedObjectName)) return false;
        return interactedObjectName == step.TargetObject;
    }
}
