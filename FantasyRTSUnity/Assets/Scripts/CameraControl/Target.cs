using UnityEngine;
using System.Collections;

public class Target : MonoBehaviour
{

    public GameObject target;

    public delegate void TargetUpdate(GameObject target);
    public static event TargetUpdate OnTargetUpdate;
    public static bool updateTargets = true;

    void Update()
    {
        if (updateTargets)
        {
            if (Input.GetMouseButtonDown(0))
            {
                SelectTarget();
            }
        }
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

    public void BroadcastNewTarget ()
    {
        if (OnTargetUpdate != null)
            OnTargetUpdate(target);
    }
}