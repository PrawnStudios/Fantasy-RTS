using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour 
{

    public int version;

	void Start () 
	{
	    if(PlayerPrefs.HasKey("Version"))
        {
            version = PlayerPrefs.GetInt("Version");
        }
        else
        {
            version = 1; //TODO HAVE THIS READ FROM FILE. DO NOT LEAVE HARD CODED
        }
	}
	
	void Update () 
	{
	
	}

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.BeginVertical();
        GUILayout.FlexibleSpace();
        //
        if (GUILayout.Button("Play")) { SceneManager.LoadScene("CodeTestLab"); }
        if (GUILayout.Button("Check for Update")) {  }
        if (GUILayout.Button("Quit")) { Application.Quit(); }
        //
        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }
}
