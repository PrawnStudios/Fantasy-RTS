using UnityEngine;
using System;
using System.Collections;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

public class GameSystems : MonoBehaviour 
{
    private int minutes;
    private int seconds;
    private int time;
    private string timeString;

    public static bool paused = false;

    public delegate void TimeUpdate(int newTime, int newSeconds, int newMinutes, string timeString);
    public static event TimeUpdate OnTimeUpdated;

    private bool bugReportUI = false;
    public string reportTitle = "Bug Report Title";
    public string reportBody = "Please Describe the bug in as much detail as possible";
    private string defaultReportTitle = "Bug Report Title";
    private string defaultReportBody = "Please Describe the bug in as much detail as possible";

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
            Time.timeScale = 0; //This stops the game playing in the background. for multiplayer do not use this
            AudioListener.volume = 0;
            paused = true;
        }
        else
        {
            Time.timeScale = 1; //This stops the game playing in the background. for multiplayer do not use this
            AudioListener.volume = 1;
            paused = false;
        }
    }

    void SendBugReport(string _title, string _body)
    {
        MailMessage mail = new MailMessage();
        mail.From = new MailAddress("prawnstudiosbugreports@gmail.com");
        mail.To.Add("prawnstudios@gmail.com");
        mail.Subject = _title;
        mail.Body = _body;

        SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
        smtpServer.Port = 587;
        smtpServer.Credentials = new System.Net.NetworkCredential("prawnstudiosbugreports@gmail.com", "Alphaprawn1") as ICredentialsByHost;
        smtpServer.EnableSsl = true;
        ServicePointManager.ServerCertificateValidationCallback =
            delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
            { return true; };
        smtpServer.Send(mail);
        Debug.Log("Bug Report Successful");
        bugReportUI = false;
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
                //SendBugReport();
                bugReportUI = !bugReportUI;
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
            //
            if (bugReportUI)
            {
                GUILayout.Space(50);
                reportTitle = GUILayout.TextField(reportTitle, 25);
                reportBody = GUILayout.TextArea(reportBody, 500);                
                if (GUILayout.Button("Send Bug Report"))
                {
                    SendBugReport(reportTitle, reportBody);
                }
            }
            else
            {
                GUILayout.Label("Version: Pre-Alpha", centerdLabel);
                GUILayout.Space(10);
                centerdLabel.fontStyle = FontStyle.Bold;
                GUILayout.Label("Notes:", centerdLabel);
                centerdLabel.fontStyle = FontStyle.Normal;
                GUILayout.Label("Quit does not work in the editor. \n Main Menu does not work.", centerdLabel);
            }
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
