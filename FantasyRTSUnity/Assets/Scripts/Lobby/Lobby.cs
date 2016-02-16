using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Lobby : MonoBehaviour 
{
    private Rect settingsRect;
    private Rect mapRect;
    private Rect playersRect;

    private GameObject map;

    void Start () 
	{
        map = Instantiate(Resources.Load("Terrains/Map1Prefab") as GameObject);        
    }
	
	void Update () 
	{
        float widthEigth = Screen.width / 8;

        settingsRect = new Rect(3, 0, widthEigth * 2, Screen.height);
        mapRect = new Rect(widthEigth * 2, 0, widthEigth * 5, Screen.height);
        playersRect = new Rect(widthEigth * 7, 0, widthEigth, Screen.height);
    }

    void OnGUI()
    {
        GUI.Box(settingsRect, "Settings");
        GUI.Box(mapRect, "Map");
        GUI.Box(playersRect, "Players");
    }
}
