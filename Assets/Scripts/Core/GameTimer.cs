using System;

public class GameTimer
{
    private TimeSpan _elapsed;
    private bool _isRunning;

    public bool IsRunning => _isRunning;
    public TimeSpan Elapsed => _elapsed;

    public void Start()
    {
        _elapsed = TimeSpan.Zero;
        _isRunning = true;
    }

    public void Stop()
    {
        _isRunning = false;
    }

    public void Update(float deltaTime)
    {
        if (_isRunning)
            _elapsed += TimeSpan.FromSeconds(deltaTime);
    }
}