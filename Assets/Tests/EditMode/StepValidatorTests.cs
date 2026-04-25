using NUnit.Framework;
using System.Collections.Generic;

public class StepValidatorTests
{
    [Test]
    public void Validate_CorrectClickAction_ReturnsTrue()
    {
        var step = new TrainingStep
        {
            TriggerAction = "click",
            TargetObject = "HydraulicLeg_L",
            TriggerParams = new Dictionary<string, object>()
        };
        bool result = StepValidator.Validate(step, interactedObjectName: "HydraulicLeg_L");
        Assert.IsTrue(result);
    }

    [Test]
    public void Validate_WrongObject_ReturnsFalse()
    {
        var step = new TrainingStep
        {
            TriggerAction = "click",
            TargetObject = "HydraulicLeg_L",
            TriggerParams = new Dictionary<string, object>()
        };
        bool result = StepValidator.Validate(step, interactedObjectName: "WrongPart");
        Assert.IsFalse(result);
    }

    [Test]
    public void Validate_NullObjectName_ReturnsFalse()
    {
        var step = new TrainingStep
        {
            TriggerAction = "click",
            TargetObject = "HydraulicLeg_L",
            TriggerParams = new Dictionary<string, object>()
        };
        bool result = StepValidator.Validate(step, interactedObjectName: null);
        Assert.IsFalse(result);
    }
}
