using UnityEngine;
using System.Collections;

public class OnlineStatusCheck : MonoBehaviour
{
    public string onlineStatus = "Online";
    private string reportToURL = "http://prawnstudios.com/ingame/activitycheck.php";
    private int secondsIdle = 0;
    string lastStatus;
    //TODO 

    void Start ()
    {
        InvokeRepeating("AddSecond", 1, 1);
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

    public void AddSecond()
    {
        secondsIdle++;
    }

    public void SetAway()
    {
        onlineStatus = "Afk";
        StartCoroutine("PingServer");
        Debug.Log("Set Away Called");
    }

    public void SetBusy()
    {
        onlineStatus = "Busy";
        StartCoroutine("PingServer");
        Debug.Log("Set Busy Called");
    }

    public void SetOnline()
    {
        onlineStatus = "Online";
        StartCoroutine("PingServer");
        Debug.Log("Set Online Called");
    }

    public void SetHidden()
    {
        onlineStatus = "Hidden";
        StartCoroutine("PingServer");
        Debug.Log("Set Hidden Called");
    }

    public void SetOffline()
    {
        onlineStatus = "Offline";
        StartCoroutine("PingServer");
        Debug.Log("Set Offline Called");
    }

    public void PingServer()
    {

        Debug.Log("Pinged Server with Status: " + onlineStatus);
        WWWForm form = new WWWForm();
        form.AddField("ID", PlayerPrefs.GetString("UserID"));
        form.AddField("Status", onlineStatus);     

        //conect to our url, and put in our form
        WWW LoginAccountWWW = new WWW(reportToURL, form);
        //make sure we get the returning information before we continue. 
    }
}

