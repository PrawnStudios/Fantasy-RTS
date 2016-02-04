﻿using UnityEngine;
using System;

public class GameSystems : MonoBehaviour 
{
    private int minutes;
    private int seconds;
    private int time;
    private string timeString;

    public static bool paused = false;

    public delegate void TimeUpdate(int newTime, int newSeconds, int newMinutes, string timeString);
    public static event TimeUpdate OnTimeUpdated;


    void Start () 
	{
        InvokeRepeating("AddSecond", 0, 1);
	}

    void Update()
    {
        Keybindings();
    }
	
	void AddSecond () 
	{
        time += 1;
        UpdateTimer();

    }

    void UpdateTimer ()
    {
        minutes = Mathf.FloorToInt(time / 60F);
        seconds = Mathf.FloorToInt(time - minutes * 60);
        timeString = string.Format("{0:0}:{1:00}", minutes, seconds);

        if (OnTimeUpdated != null)
            OnTimeUpdated(time, seconds, minutes, timeString);
    }

    public void Pause()
    {
        if(paused == false)
        {
            Time.timeScale = 0;
            AudioListener.volume = 0;
            paused = true;
        }
        else
        {
            Time.timeScale = 1;
            AudioListener.volume = 1;
            paused = false;
        }
    }

    void OnGUI()
    {
        var centerdLabel = GUI.skin.GetStyle("Label");
        centerdLabel.alignment = TextAnchor.UpperCenter;
        centerdLabel.fontStyle = FontStyle.Normal;

        if (paused)
        {
            GUI.Box(new Rect(0, 0, Screen.width, Screen.height), ""); //Darkens screen when paused

            GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
            GUILayout.BeginHorizontal();            
            GUILayout.FlexibleSpace();            
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            //
            if (GUILayout.Button("Resume Game"))
            {
                Pause();
            }

            if (GUILayout.Button("Main Menu"))
            {
                //TODO Go to Main Menu
            }

            if (GUILayout.Button("Report a Bug"))
            {
                //TODO Report Bug Function
            }

            if (GUILayout.Button("Quit"))
            {
                Application.Quit();
            }           
            //
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
            //
            GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            //
            GUILayout.Space(250);

            GUILayout.Label("Version: Pre-Alpha", centerdLabel);
            GUILayout.Space(10);
            centerdLabel.fontStyle = FontStyle.Bold;
            GUILayout.Label("Notes:", centerdLabel);
            centerdLabel.fontStyle = FontStyle.Normal;
            GUILayout.Label("Quit does not work in the editor. \n Main Menu does not work. \n Report Bug does not work.", centerdLabel);
            //
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }
    }

    void Keybindings()
    {
        if(Input.GetButtonDown("Pause"))
        {
            Pause();
        }
    }
}
