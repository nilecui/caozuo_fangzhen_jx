using System;
using UnityEngine;

public class TrainingTimer : MonoBehaviour
{
    public bool IsRunning { get; private set; }
    public float ElapsedSeconds { get; private set; }
    public event Action OnTimeout;

    private float _limitSeconds;

    public void StartTimer(float limitSeconds = 0f)
    {
        ElapsedSeconds = 0f;
        _limitSeconds = limitSeconds;
        IsRunning = true;
    }

    public void StopTimer()
    {
        IsRunning = false;
    }

    void Update()
    {
        if (!IsRunning) return;
        ElapsedSeconds += Time.deltaTime;
        if (_limitSeconds > 0f && ElapsedSeconds >= _limitSeconds)
        {
            IsRunning = false;
            OnTimeout?.Invoke();
        }
    }

    public float GetElapsed() => ElapsedSeconds;
}
