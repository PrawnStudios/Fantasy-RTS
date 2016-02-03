using UnityEngine;
using System.Collections;

public class Barracks : MonoBehaviour
{
    private Transform spawnWaypoint;

    void Start()
    {
        spawnWaypoint = transform.FindChild("UnitSpawnWaypoint");
    }

	public void CreateLegion (string legionName)
    {
        Debug.Log("Create Leion Called " + legionName);
        Instantiate(Resources.Load("Prefabs/Units/" + legionName), spawnWaypoint.position, transform.rotation);
    }
}
