// Converted from UnityScript to C# at http://www.M2H.nl/files/js_to_c.php - by Mike Hergaarden
// Do test the code! You usually need to change a few small bits.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Patcher : MonoBehaviour //FOR THE PATCHER UNITY PROJECT
{               //to make the ToList function work

    Font font;
    string patcherVersion = "Patcher Version 1.0";
    Texture2D backgroundImage;
    GUISkin myskin;
    private string updateMsg = "";
    private bool showUpdateButton = false;
    private string originalMsg = "";
    private string savePath = "";
    private WWW download;
    private bool errorOccured = false;
    private bool updateDone = false;
    private bool updating = false;
    private string version = "";

    //Will Added
    //string[] downloadList;
    public List<string> downloadList;

    void Start()
    {
        StartCoroutine("checkVersion");
    }
    IEnumerator checkVersion()
    {    //for the patcher. Check the version first. 
        updateMsg = "\n\n\nChecking the Current Version";
        string buildVersion = Application.dataPath;
        if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor)
        {
            buildVersion += "/../../"; //finds the exectuable
        }
        else if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {
            buildVersion += "/../"; //finds the exectuable
        }
        buildVersion += "version.txt";//have this file next to app/exe


        version = File.ReadAllText(buildVersion);
        version = version.Replace(".", "");
        version = version.Trim();

        updateMsg = "\n\n\nChecking For Updates..."; //GUI update for user
        yield return new WaitForSeconds(1.0f);                        //make it visible
        string url = "http://www.prawnstudios.com/rts/Builds/Live/version.txt"; //txt file with version # in it
        updateMsg = "\n\n\nEstablishing Connection...";//GUI update for user
        WWW patchwww = new WWW(url); //create new web connection to the build number site
        yield return patchwww;     // wait for download to finish
        Debug.Log("Finished Establishing Connection");
        var convertedText = patchwww.text;
        convertedText = convertedText.Replace(".", "");
        convertedText = convertedText.Replace("\n", "");
        convertedText = convertedText.Replace("\r", "");
        convertedText = convertedText.Trim();

        float availablePatch = float.Parse(convertedText);
        if (float.Parse(version) >= availablePatch)
        {    //check version
            updateMsg = "\nCurrently update to date.\n\nWould you like to launch Fantasy RTS?";
            updateDone = true;
            Debug.Log("Already Up to date");
        }
        else
        {
            StartCoroutine("applyUpdate");            
        }
    }
    //----------------------------------------------------------------
    IEnumerator applyUpdate()
    {
        Debug.Log("Downloading Update");
        updateMsg = "\nIdentifying Platform";
        //Find the place to save all of the data
        savePath = Application.dataPath;
        if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor)
        {
            updateMsg = "\n\n\nIdentifying updates for the Mac platform\n\nEstablishing Connection...";
            savePath += "/../../Contents/";
        }
        else if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {
            updateMsg = "\n\n\nIdentifying updates for the Windows platform\n\nEstablishing Connection...";
            savePath += "/../FantasyRTS_Data/";
        }
        //open and read the patch file contents
        string currPatch = "http://www.prawnstudios.com/rts/Builds/Live/patch.txt";

        WWW patchlist = new WWW(currPatch);
        yield return patchlist;

        var fileList = patchlist.text;
        string[] fileListArray = fileList.Split("\n"[0]);    //every single line in patch.txt seperated into an array
        Debug.Log(fileListArray);
        bool fileSaveSuccess = true;

        //generate the list of files to download (removing duplicates)
        //string[] downloadList; //---------------------------------------------------------------------------------------------------------------------------------------------------------------- FIXME_VAR_TYPE downloadList = Array();
        float convertedString;
        bool addFiles = false;
        bool found = false;

        Debug.Log("FileListArray = " + fileListArray.Length);
        for (int j = 0; j < fileListArray.Length; j++)
        { //Look through list and build files to download list

            Debug.Log(fileListArray[j].Trim());

            if (fileListArray[j].Trim() == "patch")
            {
                convertedString = float.Parse(fileListArray[(j + 1)].Replace(".", "").Trim());
                if (convertedString > float.Parse(version))
                {
                    addFiles = true;
                    Debug.Log("Add Files = true");
                    j = j + 2;
                }
            }

            if (addFiles == true)
            {
                Debug.Log("Download List Length = " + downloadList.Count);
                for (int k = 0; k < downloadList.Count; k++)
                {
                    if (downloadList[k] == fileListArray[j].Trim())
                    {
                        found = true;
                        break;
                    }
                }
                if (found == false)
                    downloadList.Add(fileListArray[j].Trim());
            }
            found = false;
        }
        Debug.Log("Download List Length = " + downloadList.Count);
        for (int i = 0; i < downloadList.Count; i++) //<-----START DOWNLOADING INDIVIDUAL FILES
        {
            updateMsg = "\nFile " + (i + 1) + " of " + downloadList.Count + "\n\nDownloading " + downloadList[i];
            string fileName = downloadList[i].ToString().Trim();

            updateMsg = "\n\nDownloading File " + (i + 1) + " of " + downloadList.Count + "\n";
            originalMsg = "\n\nDownloading File " + (i + 1) + " of " + downloadList.Count + "\n";
            yield return downloadFile(fileName);
            if (errorOccured == true)
            {
                break;
            }
        }
        if (errorOccured == false)
        {
            string versionURL = "http://www.prawnstudios.com/rts/Builds/Live/version.txt";
            WWW versionText = new WWW(versionURL);
            yield return versionText;
            File.WriteAllBytes(savePath + "/../version.txt", versionText.bytes);
            updateMsg = "\n\nSuccessfully Updated. \n\nWould you like to relaunch Fantasy RTS?";
            updateDone = true;
        }
    }
    //----------------------------------------------------------------
    IEnumerator downloadFile(string file)
    {

        Debug.Log("Downloading File: " + file);
        string rawDataFolder = "http://www.prawnstudios.com/rts/Builds/Live/RawFiles/";
        string url = (rawDataFolder + file).ToString();

        download = new WWW(url);     //download file from platforms raw folder
        while (!download.isDone)
        {
            if (download.error != null && download.error != "") //TODO fix this block. currently giving error
            {
                errorOccured = true;
            }
            else
            {
                updateMsg = originalMsg + "\n\n" + file + "\nDownload Progress: " + (download.progress * 100).ToString("##0.00f") + "%";
            }
        }
        yield return download;                                 // wait for download to finish

        try
        {
            updateMsg += "...saving...";
            File.WriteAllBytes(savePath + file, download.bytes);
            updateMsg += "success!";
        }
        catch (IOException error)//TODO fix this block. currently fiving error 
        {
            updateMsg = "Update Failed with error message:\n\n" + error.ToString();
            errorOccured = true;
        }
    }
    void openFantasyRTS()
    {
        string fileToOpen = Application.dataPath;
        if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor)
        {
            fileToOpen += "/../../FantasyRTS.app";
        }
        else if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {
            fileToOpen += "/../FantasyRTS.exe";
        }
        Application.OpenURL(fileToOpen);
        Application.Quit();
    }
    void OnGUI()
    {
        GUI.skin = myskin;
        GUI.skin.font = font;
        GUI.Label(new Rect(Screen.width - 130, Screen.height - 80, 100, 80), "<size=20>" + patcherVersion + "</size>");
        if (backgroundImage)
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), backgroundImage);
        GUI.Box(new Rect(Screen.width * 0.5f - 250, Screen.height * 0.5f - 250, 500, 300), "<size=25>" + updateMsg + "</size>");

        if (updateDone == true)
        {
            if (GUI.Button(new Rect(Screen.width * 0.5f - 100, Screen.height * 0.5f + 82, 200, 100), "Yes"))
            {
                updateDone = false;
                openFantasyRTS();
            }
            if (GUI.Button(new Rect(Screen.width * 0.5f - 100, Screen.height * 0.5f + 192, 200, 100), "No"))
            {
                Application.Quit();
            }
        }
        if (errorOccured == true)
        {
            if (GUI.Button(new Rect(Screen.width * 0.5f - 100, Screen.height * 0.5f + 192, 200, 100), "Close"))
            {
                Application.Quit();
            }
        }
    }
}