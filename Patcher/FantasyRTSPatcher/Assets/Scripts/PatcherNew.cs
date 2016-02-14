using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class PatcherNew : MonoBehaviour 
{

    private string FantasyRTSBuildsURL = "http://www.prawnstudios.com/rts/builds/updates/";
    private int fantasyRTSVersion = 1;

    private bool fantasyRTSInstalled;
    private string fantasyRTSLocation;

    public List<string> patchList;
    private List<string> downloadListNames = new List<string>(0);
    public List<string> downloadList = new List<string>(0);

    //Download Variables
    private bool downloadError = false;
    private string fileDownloadProccess;
    private int filesDownloaded = 0;

    private string displayMessage = "Prawn Studios Patcher";

    void Start () 
	{
        fantasyRTSInstalled = true;
        fantasyRTSLocation = Application.dataPath + "/../" + "/FantasyRTS/"; //Install Location of Fantasy RTS
        Debug.Log(fantasyRTSLocation);
    }

    IEnumerator UpdateFantasyRTS()
    {
        yield return BuildPatchList(fantasyRTSVersion, FantasyRTSBuildsURL); // Wait untill the patch list is built

        if (patchList.Count > 0) //If there are patches avalible to download
        {
            yield return BuildDownloadList(); // Wait untill the download list is built

            if (downloadList.Count > 0) //If there are files avalible to download
            {
                string tempLocation = Path.GetTempPath() + "PrawnStudios/Downloads/FantasyRTS/"; //Location to save the files whist we download them
                Directory.CreateDirectory(tempLocation); //Create the folder

                for (int i=0; i < downloadList.Count; i++)
                {
                    yield return DownloadFile(downloadList[i], downloadListNames[i], tempLocation); //Download each file and save them in the temp location                    
                }

                if(!downloadError)
                {
                    displayMessage = "File Download Complete, Prepairing to install files";
                    foreach (string dirPath in Directory.GetDirectories(tempLocation, "*", SearchOption.AllDirectories))
                    {
                        string dirName = dirPath.Replace(tempLocation, "");
                        Debug.Log("dirpath = " + dirName);
                        if (!Directory.Exists(fantasyRTSLocation + dirPath))
                        {
                            Debug.Log("Directory created in fantasy rts install location");
                            Directory.CreateDirectory(fantasyRTSLocation + dirName);
                        }
                        else
                        {
                            Debug.Log("Directory already found in fantasy rts install location");
                        }
                    }
                    int filesInstalled = 0;
                    foreach (string newPath in Directory.GetFiles(tempLocation, "*.*", SearchOption.AllDirectories))
                    {
                        File.Copy(newPath, newPath.Replace(tempLocation, fantasyRTSLocation), true);
                        filesInstalled++;
                        displayMessage = "Installed " + filesInstalled + "/" + downloadList.Count + " Files";
                    }
                    displayMessage = "Patch Successfully Installed.";
                    Directory.Delete(tempLocation, true);
                }
                else
                {
                    //TODO Only Create a list of the files that failed to download correctly then delete only them, then offer the user a chance to try and download those files again before deleting the patch.
                    displayMessage = "Patching Failed";
                    Directory.Delete(tempLocation, true); //Deete the corrupted patch.
                }
            }
        }
    }

    IEnumerator DownloadFantasyRTS()
    {
        if(!Directory.Exists(fantasyRTSLocation))
        {
            Directory.CreateDirectory(fantasyRTSLocation);
        }

        WWW patchWWW = new WWW(FantasyRTSBuildsURL + "/current_patch.txt");
        yield return patchWWW;
        string versionToDownload = patchWWW.text;

        string versionURL = FantasyRTSBuildsURL + versionToDownload + "/FullDownload.zip";
        string tempLocation = Path.GetTempPath() + "PrawnStudios/Downloads/FantasyRTS/"; //Location to save the files whist we download them
        Directory.CreateDirectory(tempLocation);
        yield return DownloadFile(versionURL, "FullDownload.zip", tempLocation);
        
        ZipUtil.Unzip(tempLocation + "FullDownload.zip", fantasyRTSLocation);
        Directory.Delete(tempLocation, true); //Delete the temp directory
        if (!downloadError)
        {
            displayMessage = "Fantasy RTS Successfully Downloaded";
        }
        else
        {
            displayMessage = "Download Failed";
        }
    }

    IEnumerator BuildPatchList(int version, string url)
    {
        int versionToCheck = version;
        string nextPatchText = "";

        while (nextPatchText != "null")
        {
            WWW nextPatchWWW = new WWW(url + versionToCheck + "/meta/nextPatch.txt");
            yield return nextPatchWWW;     // wait untill we have downloaded the nextPatch.txt file
            nextPatchText = nextPatchWWW.text;
            if (nextPatchText != "null")
            {
                patchList.Add(nextPatchText);
                versionToCheck++;
            }
        }
    }

    IEnumerator BuildDownloadList()
    {
        int versions = patchList.Count;
        for (int i = patchList.Count + 1; i > 0; i--)
        {
            WWW patchWWW = new WWW(FantasyRTSBuildsURL + i + "/meta/updatedfiles.txt");
            yield return patchWWW;     // wait untill we have downloaded the updatedfiles.txt file
            string patchinfo = patchWWW.text;
            string[] patchFiles = patchinfo.Split(','); //Create and array of the names of all the files this patch updates

            foreach(string file in patchFiles)
            {
                if (file != "")
                {
                    if (!downloadListNames.Contains(file)) //If this file is not already updated by a newer patch
                    {
                        downloadListNames.Add(file);
                        downloadList.Add(FantasyRTSBuildsURL + i + "/" + file); //Add the file to the download list
                    }
                }
            }
        }
    }

    IEnumerator DownloadFile(string file, string _fileName, string tempLocation)
    {
        string[] split = _fileName.Split('/');
        string fileName = "";
        string fileDirectory = "";
        for(int i=0; i < split.Length; i++)
        {
            if(i == split.Length - 1)
            {
                fileName = split[i];
            }
            else
            {
                fileDirectory += split[i];
                fileDirectory += "/";
            }
        }
        
        //Clean the file and folder names of space and new lines
            fileDirectory = fileDirectory.Replace("\n", "");
            fileDirectory = fileDirectory.Replace("\r", "");
            fileDirectory.Trim();
            fileName = fileName.Replace("\n", "");
            fileName = fileName.Replace("\r", "");
            fileName.Trim();

        string saveLoc = tempLocation;
        Debug.Log("File Directory = " + fileDirectory + "File Name = " + fileName);
        if(fileDirectory != "" && fileDirectory != null)
        {
            yield return Directory.CreateDirectory(tempLocation + fileDirectory); //Wait untill the folder is created.
            saveLoc = tempLocation + fileDirectory;
        }


        Debug.Log("Downloading File: " + _fileName);
        WWW download = new WWW(file);
        while (!download.isDone)
        {
            if (download.error != null && download.error != "") //If there was an error downloading the file
            {
                downloadError = true;
            }
            else
            {
                fileDownloadProccess = (download.progress * 100).ToString("##0.00") + "%";
            }
        }

        yield return download; //Wait untill the file is fully downloaded

        try
        {
            File.WriteAllBytes(saveLoc + fileName, download.bytes); //Save the file to the temp location
        }
        catch (IOException error) //If there was an error writing to the temp Location
        {
            downloadError = true;
            Debug.LogError(error.Message);
        }
        filesDownloaded++; //Add 1 to the number of files downloaded
        displayMessage = "Downloaded " + filesDownloaded + "/" + downloadList.Count + " Files. (Current file is " + fileDownloadProccess + " Downloaded)";
    }

    void OnGUI()
    {
        GUILayout.Label(displayMessage);

        if (fantasyRTSInstalled)
        {
            if (GUILayout.Button("Update Fantasy RTS"))
            {
                StartCoroutine( UpdateFantasyRTS() );
            }
        }
        else
        {
            if(GUILayout.Button("Download Fantasy RTS"))
            {
                StartCoroutine(DownloadFantasyRTS());
            }
        }
    }
}
