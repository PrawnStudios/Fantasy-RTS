using UnityEngine;
using System.Collections.Generic;

public class Target : MonoBehaviour
{

    public GameObject target;
    public static List<GameObject> targets;
    public GameObject[] selectedTargets;

    public delegate void TargetUpdate(GameObject target);
    public static event TargetUpdate OnTargetUpdate;
    public static bool updateTargets = true;

    //New Method Variables
    private Vector3 clickPos = -Vector3.one;
    public static Rect selection = new Rect(0, 0, 0, 0);


    void Update()
    {
        TargetSelect();
        //selectedTargets = targets.ToArray();
    }

    void TargetSelect()
    {
        if(Input.GetMouseButtonDown(0))
        {
            clickPos = Input.mousePosition;
            //targets = new List<GameObject>(0); //Removes all existing targets.
        }
        if(Input.GetMouseButton(0))
        {
            if (Vector2.Distance(clickPos, Input.mousePosition) > 10)
            {
                selection = new Rect(clickPos.x, InvertMouseY(clickPos.y), Input.mousePosition.x - clickPos.x, InvertMouseY(Input.mousePosition.y) - InvertMouseY(clickPos.y));
                if (selection.width < 0)
                {
                    selection.x += selection.width;
                    selection.width = -selection.width;
                }
                if (selection.height < 0)
                {
                    selection.y += selection.height;
                    selection.height = -selection.height;
                }
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            clickPos = -Vector3.one;
        }
    }


    public static float InvertMouseY(float y)
    {
        return Screen.height - y;
    }

    public static void AddTarget(GameObject target)
    {
        targets.Add(target);
    }

    public static void RemoveTarget(GameObject target)
    {
        targets.Remove(target);
    }

    void SelectTarget()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.tag == "Building")
            {
                target = hit.transform.gameObject;                

                switch (hit.transform.name)
                {
                    case "Barracks":
                        target.GetComponent<Barracks>().selected = true;
                        updateTargets = false;
                        break;
                    case "Citadel":
                        break;
                }
            }
            else if (hit.transform.tag == "Unit")
            {
                target = hit.transform.gameObject;
            }
            else
            {
                target = null;
            }
        }

        BroadcastNewTarget();
    }

    void OnGUI()
    {
        if (Input.GetMouseButton(0) && Vector2.Distance(clickPos, Input.mousePosition) > 10)
        {
            GUI.Box(selection, "");
        }
    }

    public void BroadcastNewTarget ()
    {
        if (OnTargetUpdate != null)
            OnTargetUpdate(target);
    }
}