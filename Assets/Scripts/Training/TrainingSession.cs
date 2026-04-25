using System.Collections.Generic;
using UnityEngine;

public enum TrainingMode { Training, Assessment }

public class TrainingSession
{
    public TrainingConfig Config { get; }
    public TrainingMode Mode { get; }
    public int CurrentStepIndex { get; private set; }
    public List<int> ErrorStepIds { get; } = new List<int>();
    public float StartTime { get; private set; }

    public TrainingStep CurrentStep =>
        CurrentStepIndex < Config.Steps.Count ? Config.Steps[CurrentStepIndex] : null;

    public bool IsComplete => CurrentStepIndex >= Config.Steps.Count;

    public TrainingSession(TrainingConfig config, TrainingMode mode)
    {
        Config = config;
        Mode = mode;
        StartTime = Time.time;
    }

    public void RecordError(int stepId)
    {
        if (!ErrorStepIds.Contains(stepId))
            ErrorStepIds.Add(stepId);
    }

    public void AdvanceStep()
    {
        if (!IsComplete) CurrentStepIndex++;
    }

    public int GetFinalScore()
        => ScoringEngine.Calculate(Config.Steps, ErrorStepIds);

    public int GetDurationSeconds()
        => (int)(Time.time - StartTime);
}
