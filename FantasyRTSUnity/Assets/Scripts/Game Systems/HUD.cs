using UnityEngine;
using System.Collections;

public class HUD : MonoBehaviour 
{

    private string timeString;
    private GameObject target;

    void OnEnable()
    {
        GameSystems.OnTimeUpdated += UpdateGameTime;
        Target.OnTargetUpdate += UpdateTarget;
    }

    void OnDisable()
    {
        GameSystems.OnTimeUpdated -= UpdateGameTime;
        Target.OnTargetUpdate -= UpdateTarget;
    }

    void UpdateGameTime(int time, int seconds, int minutes, string newTimeString)
    {
        timeString = newTimeString;
    }

    void UpdateTarget(GameObject _target)
    {
        target = _target;
    }


    void OnGUI () 
	{
        GUILayout.Label(timeString);
    }
}
