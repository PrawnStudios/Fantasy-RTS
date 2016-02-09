using UnityEngine;
using System.Collections;

public class GameStart : MonoBehaviour
{
    Vector2 origin;
    public GameObject citidel;
    public int radius = 100;

    void Start ()
    {
        //origin = new Vector2(Camera.main.transform.position.x, Camera.main.transform.position.z);
        origin = new Vector2(0, 0);
        GetComponent<CreateBuilding>().CreatePreview("Citadel");
        citidel = GameObject.Find("CitadelPreview");
	}
}