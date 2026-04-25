using NUnit.Framework;
using System.Collections.Generic;

public class ScoringEngineTests
{
    [Test]
    public void Calculate_NoErrors_Returns100()
    {
        var steps = new List<TrainingStep>
        {
            new TrainingStep { StepId = 1, ScoreWeight = 40 },
            new TrainingStep { StepId = 2, ScoreWeight = 60 }
        };
        int score = ScoringEngine.Calculate(steps, errorStepIds: new List<int>());
        Assert.AreEqual(100, score);
    }

    [Test]
    public void Calculate_AllErrors_Returns0()
    {
        var steps = new List<TrainingStep>
        {
            new TrainingStep { StepId = 1, ScoreWeight = 40 },
            new TrainingStep { StepId = 2, ScoreWeight = 60 }
        };
        int score = ScoringEngine.Calculate(steps, errorStepIds: new List<int> { 1, 2 });
        Assert.AreEqual(0, score);
    }

    [Test]
    public void Calculate_OneError_DeductsWeight()
    {
        var steps = new List<TrainingStep>
        {
            new TrainingStep { StepId = 1, ScoreWeight = 30 },
            new TrainingStep { StepId = 2, ScoreWeight = 70 }
        };
        int score = ScoringEngine.Calculate(steps, errorStepIds: new List<int> { 1 });
        Assert.AreEqual(70, score);
    }

    [Test]
    public void Calculate_ScoreNeverBelowZero()
    {
        var steps = new List<TrainingStep>
        {
            new TrainingStep { StepId = 1, ScoreWeight = 60 },
            new TrainingStep { StepId = 2, ScoreWeight = 60 }
        };
        int score = ScoringEngine.Calculate(steps, errorStepIds: new List<int> { 1, 2 });
        Assert.AreEqual(0, score);
    }

    [Test]
    public void IsPassed_ScoreAbovePassScore_ReturnsTrue()
    {
        Assert.IsTrue(ScoringEngine.IsPassed(score: 60, passScore: 60));
        Assert.IsTrue(ScoringEngine.IsPassed(score: 90, passScore: 60));
        Assert.IsFalse(ScoringEngine.IsPassed(score: 59, passScore: 60));
    }
}
