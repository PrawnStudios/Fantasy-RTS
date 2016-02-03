using UnityEngine;
using System.Collections;

public class TriggerDetectition : MonoBehaviour
{
	void OnTriggerEnter (Collider other)
    {
        if (other.transform.tag == "BuildingPreview")
        {
            CreateBuilding.isTriggered = true;
        }
	}
	
	void OnTriggerExit (Collider other)
    {
        if (other.transform.tag == "BuildingPreview")
        {
            CreateBuilding.isTriggered = false;
        }
    }
}