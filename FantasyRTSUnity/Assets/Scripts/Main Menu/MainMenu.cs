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
        var centeredStyle = GUI.skin.GetStyle("Label");
        centeredStyle.alignment = TextAnchor.UpperCenter;

        GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.BeginVertical();
        GUILayout.FlexibleSpace();
        //
        if (PlayerPrefs.GetString("Username") == "Offline Mode")
        {
            GUILayout.Label("Currently in Offline Mode", centeredStyle);
        }
        else

        {
            GUILayout.Label("Currently Logged in as: " + PlayerPrefs.GetString("Username"), centeredStyle);
        }
        if (GUILayout.Button("Play")) { SceneManager.LoadScene("CodeTestLab"); }
        if (GUILayout.Button("Check for Update")) { GetComponent<Patcher>().Check(); GetComponent<MainMenu>().enabled = false; }
        if (GUILayout.Button("Logout")) { SceneManager.LoadScene("Login"); }
        //
        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }
}
