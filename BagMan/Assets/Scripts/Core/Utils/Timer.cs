using System;
using UnityEngine;

public class Timer
{
    private float _duration = 0f;
    private float _spanTime = 0f;

    private bool _isRunning;
    private bool _isFinished;

    public float Duration => _duration;
    public float SpanTime => _spanTime;
    public bool IsRunning => _isRunning;
    public bool IsFinished => _isFinished;

    public event Action OnTimerComplete;

    public Timer(float duration)
    {
        _duration = duration;
        _spanTime = 0f;
        _isRunning = false;
    }

    public void Start()
    {
        _spanTime = 0f;
        _isRunning = true;
    }

    public void Stop()
    {
        _isRunning = false;
    }

    public void Update()
    {
        if (!IsRunning || IsFinished)
            return;
        _spanTime += Time.deltaTime;

        if (_spanTime >= _duration)
        {
            _isFinished = true;
            _isRunning = false;
            OnTimerComplete?.Invoke();
        }
    }

    public void Reset()
    {
        _spanTime = 0f;
        _isRunning = true;
    }

    public float GetRemainingTime()
    {
        return Math.Max(_duration - _spanTime, 0f);
    }


    public float GetProgress01()
    {
        // 0.0 to 1.0f
        return Math.Min(_spanTime / _duration, 1f);
    }
}

