using UnityEngine;
using System.Collections;

public class OnlineStatusCheck : MonoBehaviour
{
    public string onlineStatus = "Online";
    private string reportToURL = "http://prawnstudios.com/ingame/activitycheck.php";
    private int secondsIdle = 0;
    string lastStatus;
    private int offlineMode;
    //TODO 

    void Start ()
    {
        offlineMode = PlayerPrefs.GetInt("Offline Mode");
        if (offlineMode != 1)
        {
            InvokeRepeating("AddSecond", 1, 1);
            InvokeRepeating("StatusTimer", 0, 5);
        }
    }

    void StatusTimer()
    {
        StartCoroutine("PingServer");
    }
	
	void Update ()
    {
        if (offlineMode != 1)
        {
            if (Input.anyKey)
            {
                secondsIdle = 0;
                if (onlineStatus == "Afk")
                {
                    onlineStatus = "Online";
                }
            }
            else if (secondsIdle >= 300) //If idle for 5 mins
            {
                if (onlineStatus != "Hidden" || onlineStatus != "Busy") //And you are not set to hidden or Busy
                {
                    SetAway();
                }
            }
        }
	}

    public void AddSecond()
    {
        secondsIdle++;
    }

    public void SetAway()
    {
        onlineStatus = "Afk";
        StartCoroutine("PingServer");
        Debug.Log("Set Away Called");
        PlayerPrefs.SetString("OnlineStatus", onlineStatus);
    }

    public void SetBusy()
    {
        onlineStatus = "Busy";
        StartCoroutine("PingServer");
        Debug.Log("Set Busy Called");
        PlayerPrefs.SetString("OnlineStatus", onlineStatus);
    }

    public void SetOnline()
    {
        onlineStatus = "Online";
        StartCoroutine("PingServer");
        Debug.Log("Set Online Called");
        PlayerPrefs.SetString("OnlineStatus", onlineStatus);
    }

    public void SetHidden()
    {
        onlineStatus = "Appear Offline";
        StartCoroutine("PingServer");
        Debug.Log("Set Hidden Called");
        PlayerPrefs.SetString("OnlineStatus", onlineStatus);
    }

    public void SetOffline()
    {
        onlineStatus = "Offline";
        StartCoroutine("PingServer");
        Debug.Log("Set Offline Called");
        PlayerPrefs.SetString("OnlineStatus", onlineStatus);
    }

    public IEnumerator PingServer()
    {

        Debug.Log("Pinged Server with Status: " + onlineStatus);
        WWWForm form = new WWWForm();
        form.AddField("ID", PlayerPrefs.GetString("UserID"));
        form.AddField("Status", onlineStatus);

        //conect to our url, and put in our form
        WWW LoginAccountWWW = new WWW(reportToURL, form);
        yield return LoginAccountWWW;

        if (LoginAccountWWW.error != null)
        {
            Debug.LogError("Cannot connect to status update server.");
        }
        else
        {
            string logText = LoginAccountWWW.text;
            Debug.Log(logText);
        }
    }
}

