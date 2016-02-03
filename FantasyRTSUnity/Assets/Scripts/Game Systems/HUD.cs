using UnityEngine;
using System.Collections;

public class HUD : MonoBehaviour 
{

    private string timeString;
    private GameObject target;

    private Rect targetRect = new Rect(0, 0, 200, 250);
    private Rect createRect = new Rect(0, 0, 200, 25);
    private Rect closeRect = new Rect(0, 0, 200, 25);

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

        if (target != null)
        {
            if (target.tag == "Building")
            {
                Vector3 screenPos = Camera.main.WorldToScreenPoint(target.transform.position);
                targetRect.height = 75;
                targetRect.x = screenPos.x;
                createRect.x = screenPos.x;
                closeRect.x = screenPos.x;
                targetRect.y = Screen.height - screenPos.y;
                createRect.y = Screen.height - screenPos.y + 25;
                closeRect.y = Screen.height - screenPos.y + 50;

                GUI.Box(targetRect, target.name);

                if (GUI.Button(createRect, "Create Legion"))
                {
                    target.GetComponent<Barracks>().CreateLegion("Test Legion");
                    GetComponent<Target>().target = null;
                    GetComponent<Target>().BroadcastNewTarget();
                    Target.updateTargets = true;
                }

                if (GUI.Button(closeRect, "Close"))
                {
                    GetComponent<Target>().target = null;
                    GetComponent<Target>().BroadcastNewTarget();
                    Target.updateTargets = true;
                }
            }
        }
    }
}
