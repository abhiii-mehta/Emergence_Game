using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static bool IsRewinding { get; private set; }
    public static bool IsFastForwarding { get; private set; }
    public static float TimeScale { get; private set; } = 1f;

    void Update()
    {
        if (!IsRewinding)
            Time.timeScale = TimeScale;
    }

    public void StartRewind()
    {
        IsRewinding = true;
        IsFastForwarding = false;
        Time.timeScale = 1f;
    }

    public void StopRewind()
    {
        IsRewinding = false;
        TimeScale = 1f;
    }

    public void SetSlow(float factor)
    {
        IsRewinding = false;
        IsFastForwarding = false;
        TimeScale = factor;
    }

    public void SetFast(float factor)
    {
        IsRewinding = false;
        IsFastForwarding = true;
        TimeScale = factor;
    }

    public void SetNormal()
    {
        IsRewinding = false;
        IsFastForwarding = false;
        TimeScale = 1f;
    }

}
