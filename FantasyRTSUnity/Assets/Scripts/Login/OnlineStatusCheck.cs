using UnityEngine;
using System.Collections;

public class OnlineStatusCheck : MonoBehaviour
{
    public static string onlineStatus = "";
    private string reportToURL = "http://prawnstudios.com/ingame/activitycheck.php";
    private int secondsIdle = 0;
    string lastStatus;
    //TODO 

    void Start ()
    {
        InvokeRepeating("AddSecond", 0, 1);
        InvokeRepeating("PingServer", 0, 60);
    }
	
	void Update ()
    {        
        if (Input.anyKey)
        {
            secondsIdle = 0;
            if(onlineStatus == "Afk")
            {
                onlineStatus = "Online";
            }
        }
        else if(secondsIdle >= 300) //If idle for 5 mins
        {
            if (onlineStatus != "Hidden" || onlineStatus != "Busy") //And you are not set to hidden or Busy
            {
                SetAway();
            }
        }
	}

    IEnumerator AddSecond()
    {
        secondsIdle++;
        yield return null;
    }

    public void SetAway()
    {
        onlineStatus = "Afk";
        StartCoroutine("PingServer");
    }

    public void SetBusy()
    {
        onlineStatus = "Busy";
        StartCoroutine("PingServer");
    }

    public void SetOnline()
    {
         onlineStatus = "Online";
        StartCoroutine("PingServer");
    }

    public void SetHidden()
    {
        onlineStatus = "Hidden";
        StartCoroutine("PingServer");
    }

    public void SetOffline()
    {
        onlineStatus = "Offline";
        StartCoroutine("PingServer");
    }

    IEnumerator PingServer()
    {
        WWWForm form = new WWWForm();
        form.AddField("Username", PlayerPrefs.GetString("Username")); //TODO Replace sending username with passing ID (recieve ID in login)
        form.AddField("Status", onlineStatus);

        yield return null;
    }
}

