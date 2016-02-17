﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour 
{

    public int version;
    private bool statusMenu = false;
    public GameObject accountManager;


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

        if(PlayerPrefs.GetString("OnlineStatus") != "Offline")
        {
            if (GameObject.Find("Account Manager") == null)
            {
                accountManager = Instantiate(accountManager);
                accountManager.name = "Account Manager";
                Debug.Log("Renamed Account Manager");
                DontDestroyOnLoad(accountManager);
            }
            else
            {
                accountManager = GameObject.Find("Account Manager");
            }
        }
	}
	
	IEnumerator Logout () 
	{
        accountManager.GetComponent<OnlineStatusCheck>().SetOffline();
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("Login");
    }

    void OnGUI()
    {
        var centeredStyle = GUI.skin.GetStyle("Label");
        centeredStyle.alignment = TextAnchor.UpperCenter;

        Rect statusRect = new Rect(5, 5, 250, 25);
        if(GUI.Button(statusRect, "Status Currently: " + PlayerPrefs.GetString("OnlineStatus"))) { statusMenu = !statusMenu; }
        if(statusMenu && PlayerPrefs.GetInt("Offline Mode") == 0)
        {
            statusRect.y += 30;
            if(GUI.Button(statusRect, "Online")) { accountManager.GetComponent<OnlineStatusCheck>().SetOnline(); }
            statusRect.y += 30;
            if(GUI.Button(statusRect, "AFK")) { accountManager.GetComponent<OnlineStatusCheck>().SetAway(); }
            statusRect.y += 30;
            if(GUI.Button(statusRect, "Busy")) { accountManager.GetComponent<OnlineStatusCheck>().SetBusy(); }
            statusRect.y += 30;
            if(GUI.Button(statusRect, "Appear Offline")) { accountManager.GetComponent<OnlineStatusCheck>().SetHidden(); }
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
        //if (GUILayout.Button("Play")) { SceneManager.LoadScene("CodeTestLab"); }
        if(GUILayout.Button("Setup Game"))
        {
            SceneManager.LoadScene("Lobby");
        }
        if (GUILayout.Button("Check for Update")) { GetComponent<Patcher>().Check(); GetComponent<MainMenu>().enabled = false; }
        if (GUILayout.Button("Logout"))
        {
            accountManager.GetComponent<OnlineStatusCheck>().SetOffline();
            SceneManager.LoadScene("Login");
        }
        //
        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }
}
