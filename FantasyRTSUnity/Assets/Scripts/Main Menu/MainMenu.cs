using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour 
{

    public int version;
    private bool statusMenu = false;


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

        Rect statusRect = new Rect(5, 5, 250, 25);
        if(GUI.Button(statusRect, "Status Currently: " + PlayerPrefs.GetString("OnlineStatus"))) { statusMenu = !statusMenu; }
        if(statusMenu)
        {
            statusRect.y += 30;
            if(GUI.Button(statusRect, "Online")) { GetComponent<OnlineStatusCheck>().SetOnline(); }
            statusRect.y += 30;
            if(GUI.Button(statusRect, "AFK")) { GetComponent<OnlineStatusCheck>().SetAway(); }
            statusRect.y += 30;
            if(GUI.Button(statusRect, "Busy")) { GetComponent<OnlineStatusCheck>().SetBusy(); }
            statusRect.y += 30;
            if(GUI.Button(statusRect, "Appear Offline")) { GetComponent<OnlineStatusCheck>().SetHidden(); }
        }

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
        //TODO Add dropdown list for online status (Online, AFK, Busy, Hidden)
        if (GUILayout.Button("Play")) { SceneManager.LoadScene("CodeTestLab"); }
        if (GUILayout.Button("Check for Update")) { GetComponent<Patcher>().Check(); GetComponent<MainMenu>().enabled = false; }
        if (GUILayout.Button("Logout"))
        {
            SceneManager.LoadScene("Login");
            GetComponent<OnlineStatusCheck>().SetOffline();
        }
        //
        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }
}
