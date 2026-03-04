using UnityEngine;

public class TimerDriver : MonoBehaviour
{
    private GameTimer _timer;

    public void Initialize(GameTimer timer)
    {
        _timer = timer;
    }

    private void Update()
    {
        if (_timer != null && _timer.IsRunning)
            _timer.Update(Time.deltaTime);
    }
}