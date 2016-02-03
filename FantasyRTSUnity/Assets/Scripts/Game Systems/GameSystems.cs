using UnityEngine;
using System;

public class GameSystems : MonoBehaviour 
{
    private int minutes;
    private int seconds;
    private int time;
    private string timeString;

    public delegate void TimeUpdate(int newTime, int newSeconds, int newMinutes, string timeString);
    public static event TimeUpdate OnTimeUpdated;


    void Start () 
	{
        InvokeRepeating("AddSecond", 0, 1);
	}
	
	void AddSecond () 
	{
        time += 1;
        UpdateTimer();

    }

    void UpdateTimer ()
    {
        minutes = Mathf.FloorToInt(time / 60F);
        seconds = Mathf.FloorToInt(time - minutes * 60);
        timeString = string.Format("{0:0}:{1:00}", minutes, seconds);

        if (OnTimeUpdated != null)
            OnTimeUpdated(time, seconds, minutes, timeString);
    }
}
