﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Friends : MonoBehaviour 
{
    private Rect friendsListPos = new Rect(Screen.width - 250, 25, 250, 400);
    private Rect friendsListViewPos = new Rect(-10, -50, 230, 350);
    private Vector2 scrollPos = Vector2.zero;
    private bool addFriend = false;
    private string friendName;

    private int friendListLength = 1;
    private string friendButtonText = "Show";
    private string addFriendUrl = "http://prawnstudios.com/ingame/addfriend.php";
    private string removeFriendUrl = "http://prawnstudios.com/ingame/removefriend.php";
    private string getFriendsLIstUrl = "http://prawnstudios.com/ingame/getfriendslist.php";

    private bool friendOptions = false;
    private Vector2 friendOptionsPos;

    private string[] friendsList = new string[0];
    private string[] friendsStatusList = new string[0];
    private string selectedFriend;

    void Start()
    {
        if (PlayerPrefs.GetInt("Offline Mode") != 1)
        {
            StartCoroutine("GetFriendList");
            InvokeRepeating("UpdateFriendsList", 5, 5);
        }
    }

    void UpdateFriendsList()
    {
        if (friendButtonText == "Hide" && friendsList.Length > 0)
        {
            StartCoroutine("GetFriendList");
        }
    }

    IEnumerator GetFriendList()
    {
        Debug.Log("Get Friends List Called");
        //friendsList = new string[0];
       // friendsStatusList = new string[0];

        WWWForm form = new WWWForm(); //this is what sends messages to our php script

        //the fields are the variables we are sending
        form.AddField("ID", PlayerPrefs.GetString("UserID"));

        WWW LoginAccountWWW = new WWW(getFriendsLIstUrl, form);
        yield return LoginAccountWWW;

        if (LoginAccountWWW.error != null)
        {
            Debug.LogError("Cannot connect to login server.");
        }
        else
        {
            string logText = LoginAccountWWW.text;
            Debug.Log(logText);
            if (logText == "User not found.")
            {
                Debug.Log("Could not find the logged in user");
            }
            else if(logText == "No Friends Added")
            {
                Debug.Log("No Friends Found");
            }
            else if(logText == "User already on your freinds list")
            {
                Debug.Log("User already Added");
            }
            else
            {
                string[] friendsAndStatus = logText.Split(':');
                bool friend = true;

                string friendString = "";
                string statusString = "";
                for (int i=0; i < friendsAndStatus.Length; i++)
                {
                    if(friend)
                    {
                        friendString += friendsAndStatus[i] + ":";
                        friend = false;
                    }
                    else
                    {
                        statusString += friendsAndStatus[i] + ":";
                        friend = true;
                    }
                }
                if (friendString.Length > 0) { friendString = friendString.Substring(0, friendString.Length - 1); }
                if (statusString.Length > 0) { statusString = statusString.Substring(0, statusString.Length - 1); }
                friendsList = new string[0];
                friendsList = friendString.Split(':');
                friendsStatusList = new string[0];
                friendsStatusList = statusString.Split(':');
                Debug.Log("Friend's list updated");
            }
        }
    }

    IEnumerator AddFriend()
    {
        Debug.Log("Add Friend Called");
        
        WWWForm form = new WWWForm(); //this is what sends messages to our php script

        //the fields are the variables we are sending
        form.AddField("FriendUsername", friendName);
        form.AddField("ID", PlayerPrefs.GetString("UserID"));

        WWW LoginAccountWWW = new WWW(addFriendUrl, form);
        yield return LoginAccountWWW;

        if (LoginAccountWWW.error != null)
        {
            Debug.LogError("Cannot connect to server.");
        }
        else
        {            
            string logText = LoginAccountWWW.text;
            if (logText == "Cannot find user")
            {
                Debug.Log("Could not find the user to add.");
            }
            else
            {
                Debug.Log("Added Friend");
                StartCoroutine("GetFriendList");
            }
        }
    }

    IEnumerator RemoveFriend()
    {
        Debug.Log("Add Friend Called");

        WWWForm form = new WWWForm(); //this is what sends messages to our php script

        //the fields are the variables we are sending
        form.AddField("FriendsUsername", selectedFriend);
        form.AddField("ID", PlayerPrefs.GetString("UserID"));

        WWW LoginAccountWWW = new WWW(removeFriendUrl, form);
        yield return LoginAccountWWW;

        if (LoginAccountWWW.error != null)
        {
            Debug.LogError("Cannot connect to server.");
        }
        else
        {
            string logText = LoginAccountWWW.text;
            if(logText == "Friend Removed")
            {
                Debug.Log("Removed Friend");
                StartCoroutine("GetFriendList");
            }
        }
    }

    void OnGUI()
    {
        if(PlayerPrefs.GetInt("Offline Mode") == 1)
        {
            GUI.Button(new Rect(friendsListPos.x, 0, friendsListPos.width, 25), "Can't Show Friends List in Offline Mode");
        }
        else
        {
        if (GUI.Button(new Rect(friendsListPos.x, 0, friendsListPos.width, 25), friendButtonText + " Friends List"))
        {
            if(friendButtonText == "Show")
            {
                friendButtonText = "Hide";
            }
            else
            {
                friendButtonText = "Show";
            }
        }

            if (friendButtonText == "Hide")
            {
                GUI.Box(friendsListPos, "");
                if (friendsList.Length == 0)
                {
                    GUI.Label(new Rect(Screen.width - 250, 35, 250, 350), "No Friends Added!");
                }
                else
                {
                    scrollPos = GUI.BeginScrollView(new Rect(Screen.width - 250, 35, 250, 350), scrollPos, new Rect(-10, 0, 230, friendsList.Length * 35));
                    //
                    int buttonY = 0;
                    int num = 0;
                    foreach (string friend in friendsList)
                    {

                        string displayStatus;
                        if (friendsStatusList[num] == "Appear Offline")
                        {
                            displayStatus = "Offline";
                        }
                        else
                        {
                            displayStatus = friendsStatusList[num];
                        }

                        if (GUI.Button(new Rect(0, buttonY, 220, 25), friend + " (" + displayStatus + ")"))
                        {
                            if (Event.current.button == 0) // If Left CLick
                            {
                                //TODO View Profile
                            }
                            else if (Event.current.button == 1) // If Right Click
                            {
                                if (selectedFriend == friend && friendOptions == true)
                                {
                                    friendOptions = false;
                                }
                                else
                                {
                                    friendOptions = true;
                                    friendOptionsPos = new Vector2(Screen.width - 400, buttonY + 35);
                                    selectedFriend = friend;
                                }
                            }
                            num++;
                        }
                        buttonY += 35;
                    }
                    //
                    GUI.EndScrollView();
                }
                if (GUI.Button(new Rect(friendsListPos.x + 5, 395, 240, 25), "Add Friend"))
                {
                    addFriend = !addFriend;
                    friendName = "Add Friend";
                }
                if (addFriend)
                {
                    friendName = GUI.TextArea(new Rect(friendsListPos.x, 425, 250, 25), friendName);
                    if (GUI.Button(new Rect(friendsListPos.x, 450, 250, 25), "Confirm"))
                    {
                        if (friendName != "" && friendName != "Add Friend")
                        {
                            StartCoroutine("AddFriend");
                            friendName = "Add Friend";
                            addFriend = false;
                        }
                    }
                }

                if (friendOptions)
                {
                    if (GUI.Button(new Rect(friendOptionsPos.x, friendOptionsPos.y, 150, 25), "Remove Friend"))
                    {
                        StartCoroutine("RemoveFriend");
                        friendOptions = false;
                        selectedFriend = null;
                        friendOptionsPos = Vector2.zero;
                    }
                }
            }
        }
    }
}
