using System;
using System.Collections.Generic;

public static class ScoringEngine
{
    public static int Calculate(List<TrainingStep> steps, List<int> errorStepIds)
    {
        int totalWeight = 0;
        int deduction = 0;

        foreach (var step in steps)
        {
            totalWeight += step.ScoreWeight;
            if (errorStepIds.Contains(step.StepId))
                deduction += step.ScoreWeight;
        }

        int score = totalWeight - deduction;
        return Math.Max(0, score);
    }

    public static bool IsPassed(int score, int passScore) => score >= passScore;
}
