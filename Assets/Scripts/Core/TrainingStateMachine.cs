using System;

public enum TrainingState { Idle, Playing, Complete }

public class TrainingStateMachine
{
    public TrainingState CurrentState { get; private set; } = TrainingState.Idle;
    public event Action<TrainingState> OnStateChanged;

    public void Start()
    {
        if (CurrentState != TrainingState.Idle)
            throw new InvalidOperationException($"Cannot Start from state {CurrentState}");
        Transition(TrainingState.Playing);
    }

    public void Complete()
    {
        if (CurrentState != TrainingState.Playing) return;
        Transition(TrainingState.Complete);
    }

    public void Reset()
    {
        Transition(TrainingState.Idle);
    }

    private void Transition(TrainingState next)
    {
        CurrentState = next;
        OnStateChanged?.Invoke(next);
    }
}
