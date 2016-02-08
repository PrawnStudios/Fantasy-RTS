// Converted from UnityScript to C# at http://www.M2H.nl/files/js_to_c.php - by Mike Hergaarden
// Do test the code! You usually need to change a few small bits.

using UnityEngine;
using System.Collections;

public class Patcher : MonoBehaviour
{
    public string buildVersion = "1.0";
    private bool updateDone = false;
    private bool downloadingFiles = false;
    private string savePath;
    private string originalMsg;
    private WWW download;
    private string patch;
    private string rawDataFolder;
    private string rawExc;

    //Will Added
    private string updateMsg;
    bool updating = false;
    bool showUpdateButton = false;
    bool showGUI = false;

    string url = "http://www.prawnstudios.com/rts/Builds/PreAlpha3/version.txt";

    public void Check()
    {
        StartCoroutine("CheckForUpdates");
    }
    void Update()
    {
        if (downloadingFiles)
        {
            string updateMsg = originalMsg + "\n Download Progress: " + (download.progress * 100).ToString("#00.00f") + "%\n";
        }
    }
    //-------------------------FOR PATCHING -----------------------------------------------------
    IEnumerator CheckForUpdates()
    {    //for the patcher. Check the version first. 
        buildVersion = "1.0";
        updating = true;
        showUpdateButton = false;
        updateMsg = "\n\n\nChecking For Updates..."; //GUI update for user
        yield return new WaitForSeconds(1.0f);                        //make it visible
        updateMsg = "\n\n\nEstablishing Connection...";//GUI update for user
        WWW patchwww = new WWW(url); //create new web connection to the build number site
        yield return patchwww;     // wait for download to finish
        var updateVersion = (patchwww.text).Trim();
        Debug.Log(updateVersion);

        if (updateVersion == buildVersion) //check version
        {    
            updateMsg = "\nCurrently update to date.";
            yield return new WaitForSeconds(1.0f);
            updating = false;
            showGUI = true;
            Debug.Log("No Update Avalible");
        }
        else
        {
            patch = patchwww.text;
            updateMsg = "Update Available.\n\n Current Version: " + buildVersion + "\n Patch Version: " + patchwww.text + "\n\n Would you like to download updates?\n\nThis will close this program and \n will launch the patching program.";
            showUpdateButton = true;
            Debug.Log("Update Avalible");
        }
    }
    //----------------------------------------------------------------
    void applyUpdate()
    {
        updateMsg = "\nIdentifying Platform";
        if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor)
        {
            Application.OpenURL(Application.dataPath + "/../../FantasyRTSUnityPatcher.app");
        }
        else if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {
            Application.OpenURL(Application.dataPath + "/../Patcher.exe");
        }
        Application.Quit();
    }

    void OnGUI()
    {
        if (updating)
        {
            GUI.Box(new Rect(Screen.width * 0.5f - 250, Screen.height * 0.5f - 250, 500, 300), "<size=25>" + updateMsg + "</size>");
            if (showUpdateButton)
            {
                if (GUI.Button(new Rect(Screen.width * 0.5f - 100, Screen.height * 0.5f + 82, 200, 100), "Update"))
                {
                    showUpdateButton = false;
                    applyUpdate();
                }

                if (GUI.Button(new Rect(Screen.width * 0.5f - 100, Screen.height * 0.5f + 192, 200, 100), "Cancel"))
                {
                    updating = false;
                    showGUI = true;
                    GetComponent<MainMenu>().enabled = true;
                }
            }

            if (updateDone == true)
            {
                if (GUI.Button(new Rect(Screen.width * 0.5f - 100, Screen.height * 0.5f + 192, 200, 100), "Ok"))
                {
                    updating = false;
                    showGUI = true;
                    GetComponent<MainMenu>().enabled = true;
                }
            }
        }
    }
}