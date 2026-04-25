using NUnit.Framework;
using System;

public class TrainingStateMachineTests
{
    [Test]
    public void InitialState_IsIdle()
    {
        var sm = new TrainingStateMachine();
        Assert.AreEqual(TrainingState.Idle, sm.CurrentState);
    }

    [Test]
    public void Start_FromIdle_TransitionsToPlaying()
    {
        var sm = new TrainingStateMachine();
        sm.Start();
        Assert.AreEqual(TrainingState.Playing, sm.CurrentState);
    }

    [Test]
    public void Complete_FromPlaying_TransitionsToComplete()
    {
        var sm = new TrainingStateMachine();
        sm.Start();
        sm.Complete();
        Assert.AreEqual(TrainingState.Complete, sm.CurrentState);
    }

    [Test]
    public void Reset_FromComplete_TransitionsToIdle()
    {
        var sm = new TrainingStateMachine();
        sm.Start();
        sm.Complete();
        sm.Reset();
        Assert.AreEqual(TrainingState.Idle, sm.CurrentState);
    }

    [Test]
    public void Start_WhenAlreadyPlaying_ThrowsInvalidOperation()
    {
        var sm = new TrainingStateMachine();
        sm.Start();
        Assert.Throws<InvalidOperationException>(() => sm.Start());
    }

    [Test]
    public void OnStateChanged_EventFiredOnTransition()
    {
        var sm = new TrainingStateMachine();
        TrainingState? capturedState = null;
        sm.OnStateChanged += s => capturedState = s;
        sm.Start();
        Assert.AreEqual(TrainingState.Playing, capturedState);
    }
}
