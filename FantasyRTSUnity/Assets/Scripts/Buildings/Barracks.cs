using UnityEngine;
using System.Collections;

public class Barracks : MonoBehaviour
{
    private Transform spawnWaypoint;
    private GameObject cam;

    private Rect targetRect = new Rect(0, 0, 200, 250);
    private int windowWidth = 200;
    private int buttonHeight = 25;

    public bool selected = false;

    private int level = 2;

    private string[] slot;
    private int[] time;

    private bool showUnitTypeList;
    private bool showInfantryList;
    private bool showMountedList;
    private bool showSiegeList;

    private Rect UnitTypeBox;
    private Rect infantryListBox;
    private Rect mounedtListBox;
    private Rect seigeListBox;

    private int unitTypeButtonNo;

    private string[] infantry = new string[5] { "Test Legion Small", "Test Legion Medium", "Test Legion Large", "Archers", "Swordsman"};
    private string[] mounted = new string[2] { "Mounted Archers", "Mounted Swordsman" };
    private string[] siege = new string[3] { "Catapult", "Battering Ram", "Big Fish" };

    private int[] infantryTimes = new int[5] { 30, 45, 60, 30, 45 };
    private int[] mountedTimes = new int[2] { 45, 38 };
    private int[] siegeTimes = new int[3] { 60, 45, 120 };


    void Start()
    {
        spawnWaypoint = transform.FindChild("UnitSpawnWaypoint");
        cam = GameObject.Find("CameraController");

        slot = new string[level];// { "Test Legion Small", "Test Legion Medium", "Test Legion Large", "Test Legion Medium" };
        time = new int[level];// { 5, 10, 15, 10};

        InvokeRepeating("Timer", 0, 1);
    }

	public void CreateLegion (string legionName)
    {
        Debug.Log("Create Leion Called " + legionName);
        Instantiate(Resources.Load("Prefabs/Units/" + legionName), spawnWaypoint.position, transform.rotation);
    }

    private void Timer()
    {
        for(int i=0; i < slot.Length; i++)
        {
            if (time[i] > 0)
            {
                time[i] -= 1;
            }
            else if (slot[i] != null)
            {
                CreateLegion(slot[i]);
                slot[i] = null;
            }
        }
    }

    void OnGUI()
    {
        if (selected && !GameSystems.paused)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
            
            targetRect.x = screenPos.x - windowWidth * 0.5f;
            targetRect.y = Screen.height - screenPos.y;
            targetRect.width = windowWidth;
            targetRect.height = 25 + (buttonHeight + 5) * level - 5;

            GUI.Box(targetRect, "Barracks (Level " + level + ")"); //Create the outline box for the GUI

            GUI.color = Color.red;
            if (GUI.Button(new Rect(targetRect.x + windowWidth - 20, targetRect.y, 20, 20), "X")) //Create the close button
            {
                cam.GetComponent<Target>().target = null;
                selected = false;
                cam.GetComponent<Target>().BroadcastNewTarget();
                Target.updateTargets = true;

                showUnitTypeList = false;
                showInfantryList = false;
                showMountedList = false;
                showSiegeList = false;
            }
            GUI.color = Color.white;

            //Create Unit Area
            float _buttonY = targetRect.y + buttonHeight;

            for (int i=0; i < level; i++)
            {
                if (slot[i] == null) //If a Unit is not currently being trained in this slot
                {
                    UnitTypeBox = new Rect(targetRect.x + windowWidth, _buttonY , windowWidth, buttonHeight);
                    if (GUI.Button(new Rect(targetRect.x, _buttonY, windowWidth, buttonHeight), "Recruit a Unit")) //Display the Recruit a Unit Button
                    {
                        if (showUnitTypeList == false && unitTypeButtonNo == i)
                        {
                            showUnitTypeList = true;
                            unitTypeButtonNo = i;
                        }
                        else if(unitTypeButtonNo == i)
                        {
                            showUnitTypeList = false;
                            showInfantryList = false;
                            showMountedList = false;
                            showSiegeList = false;
                        }
                        else
                        {
                            showUnitTypeList = true;
                            unitTypeButtonNo = i;
                        }
                    }
                }
                else
                {
                    var centeredStyle = GUI.skin.GetStyle("Box");
                    centeredStyle.alignment = TextAnchor.UpperCenter;

                    GUI.Box(new Rect(targetRect.x, _buttonY, windowWidth, buttonHeight), slot[i] + " - " + time[i], centeredStyle); //TODO Instead of displaying time as a countdown, display the time and the current game time + time to build. Resulting in displaying the time it will be completed.
                }

                _buttonY += buttonHeight;
                if (i < level - 1) _buttonY += 5;
                    
            }


            //
            if(showUnitTypeList)
            {
                //var xx = targetRect.x + windowWidth;
                float xx = 0;
                if(screenPos.x <= Screen.width * 0.5f)
                {
                    xx = targetRect.x + windowWidth;
                }
                else
                {
                    xx = targetRect.x - windowWidth;
                }

                var yy = targetRect.y + (buttonHeight +  (5 + buttonHeight) * unitTypeButtonNo); //5 + buttonheight * number
                var buttonW = windowWidth / 3;

                GUI.Box(new Rect(xx, yy, windowWidth-1, buttonHeight), "");

                if (GUI.Button(new Rect(xx, yy, buttonW, buttonHeight), "Infantry"))
                {
                    if (showInfantryList == false)
                    {
                        showInfantryList = true;
                        showMountedList = false;
                        showSiegeList = false;
                    }
                    else
                    {
                        showInfantryList = false;
                    }                
                }

                //xx = targetRect.x + windowWidth + (windowWidth / 3);

                if (screenPos.x <= Screen.width * 0.5f)
                {
                    xx = targetRect.x + windowWidth + (windowWidth / 3);
                }
                else
                {
                    xx = targetRect.x - windowWidth + (windowWidth / 3);
                }
                if (GUI.Button(new Rect(xx, yy, buttonW, buttonHeight), "Mounted"))
                {
                    if (showMountedList == false)
                    {
                        showInfantryList = false;
                        showMountedList = true;
                        showSiegeList = false;
                    }
                    else
                    {
                        showMountedList = false;
                    }
                }

                //xx = targetRect.x + windowWidth + (windowWidth / 3) * 2;
                if (screenPos.x <= Screen.width * 0.5f)
                {
                    xx = targetRect.x + windowWidth + (windowWidth / 3) * 2;
                }
                else
                {
                    xx = targetRect.x - windowWidth + (windowWidth / 3) * 2;
                }
                if (GUI.Button(new Rect(xx, yy, buttonW, buttonHeight), "Siege"))
                {
                    if (showSiegeList == false)
                    {
                        showInfantryList = false;
                        showMountedList = false;
                        showSiegeList = true;
                    }
                    else
                    {
                        showSiegeList = false;
                    }
                }
            }

            if(showInfantryList)
            {
                var yy = targetRect.y + (buttonHeight + (5 + buttonHeight) * unitTypeButtonNo); //5 + buttonheight * number
                var hh = buttonHeight + (buttonHeight * infantry.Length);
                float xx = 0;
                if (screenPos.x <= Screen.width * 0.5f)
                {
                    xx = targetRect.x + windowWidth;
                }
                else
                {
                    xx = targetRect.x - windowWidth;
                }
                GUI.Box(new Rect(xx, yy + buttonHeight, windowWidth, hh), "Infantry");

                for(int i=0; i < infantry.Length; i++)
                {
                    if(GUI.Button(new Rect(xx, yy + buttonHeight * 2 + (buttonHeight * i), windowWidth, buttonHeight), infantry[i]))
                    {
                        slot[unitTypeButtonNo] = infantry[i];
                        time[unitTypeButtonNo] = infantryTimes[i];

                        showUnitTypeList = false;
                        showInfantryList = false;
                        showMountedList = false;
                        showSiegeList = false;
                    }
                }
            }

            if (showMountedList)
            {
                var yy = targetRect.y + (buttonHeight + (5 + buttonHeight) * unitTypeButtonNo); //5 + buttonheight * number
                var hh = buttonHeight + (buttonHeight * mounted.Length);
                float xx = 0;
                if (screenPos.x <= Screen.width * 0.5f)
                {
                    xx = targetRect.x + windowWidth;
                }
                else
                {
                    xx = targetRect.x - windowWidth;
                }
                GUI.Box(new Rect(xx, yy + buttonHeight, windowWidth, hh), "Mounted");

                for (int i = 0; i < mounted.Length; i++)
                {
                    if (GUI.Button(new Rect(xx, yy + buttonHeight * 2 + (buttonHeight * i), windowWidth, buttonHeight), mounted[i]))
                    {
                        slot[unitTypeButtonNo] = mounted[i];
                        time[unitTypeButtonNo] = mountedTimes[i];

                        showUnitTypeList = false;
                        showInfantryList = false;
                        showMountedList = false;
                        showSiegeList = false;
                    }
                }
            }

            if (showSiegeList)
            {
                var yy = targetRect.y + (buttonHeight + (5 + buttonHeight) * unitTypeButtonNo); //5 + buttonheight * number
                var hh = buttonHeight + (buttonHeight * siege.Length);
                float xx = 0;
                if (screenPos.x <= Screen.width * 0.5f)
                {
                    xx = targetRect.x + windowWidth;
                }
                else
                {
                    xx = targetRect.x - windowWidth;
                }
                GUI.Box(new Rect(xx, yy + buttonHeight, windowWidth, hh), "Siege");

                for (int i = 0; i < siege.Length; i++)
                {
                    if (GUI.Button(new Rect(xx, yy + buttonHeight * 2 + (buttonHeight * i), windowWidth, buttonHeight), siege[i]))
                    {
                        slot[unitTypeButtonNo] = siege[i];
                        time[unitTypeButtonNo] = siegeTimes[i];

                        showUnitTypeList = false;
                        showInfantryList = false;
                        showMountedList = false;
                        showSiegeList = false;
                    }
                }
            }
            //

        }
    }
}