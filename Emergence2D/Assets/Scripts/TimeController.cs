using UnityEngine;

public class TimeController : MonoBehaviour
{
    private TimeManager tm;

    void Start()
    {
        tm = FindObjectOfType<TimeManager>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) tm.StartRewind();
        if (Input.GetKeyUp(KeyCode.R)) tm.StopRewind();

        if (Input.GetKeyDown(KeyCode.Q)) tm.SetSlow(0.3f);
        if (Input.GetKeyUp(KeyCode.Q)) tm.SetNormal();

        if (Input.GetKeyDown(KeyCode.F)) tm.SetFast(2.0f);
        if (Input.GetKeyUp(KeyCode.F)) tm.SetNormal();
    }

}