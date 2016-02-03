﻿using UnityEngine;
using System.Collections;

public class CreateBuilding : MonoBehaviour
{

    private GameObject tempBuilding;
    private GameObject tempBuildingGraphic;
    private Transform[] FoundationCheckers;

    private Vector3 instLocation;
    private Vector3 previousMousePos = Vector3.zero;

    private bool canBuild = false;
    private bool buildingEnabled = false;
    public static bool isTriggered = false;

    private Material previewOk;
    private Material previewBlocked;

    public int currentRotationDegree = 25;
    private bool togglePrevewRotation = false;

    public string currentBuilding;

    void Start ()
    {        
        previewOk = Resources.Load("Materials/Buildings/BuildingPreview") as Material;
        previewBlocked = Resources.Load("Materials/Buildings/PreviewBlocked") as Material;
    }

    void Update()
    {
        KeyBindings(); // Check for Inputs    
        Prerequisites(); //Check Prerequisite "Functions".   
    }

    public void CheckSensors ()
    {
        if (isTriggered)
        {
            canBuild = false;
            tempBuildingGraphic.GetComponent<MeshRenderer>().material = previewBlocked;
            Debug.Log("Can't Build Here");
        }
        else
        {
            canBuild = true;
            for (int i = 0; i < FoundationCheckers.Length; i++)
            {
                if (i <= 3) //If Ground Checkers
                {
                    RaycastHit hit;
                    Vector3 dir = new Vector3(0, -1, 0);

                    if (Physics.Raycast(FoundationCheckers[i].transform.position, dir, out hit, 2) && hit.transform.tag == "Ground")
                    {
                        Debug.DrawRay(FoundationCheckers[i].position, dir * 2, Color.green);
                    }
                    else
                    {
                        canBuild = false;
                        Debug.DrawRay(FoundationCheckers[i].position, dir * 2, Color.red);
                    }
                }
                else
                {
                    RaycastHit hit;
                    Vector3 dir = new Vector3(0, -1, 0);

                    if (Physics.Raycast(FoundationCheckers[i].transform.position, dir, out hit, 100))
                    {
                        if (hit.transform.tag == "Ground")
                        {
                            Debug.DrawRay(FoundationCheckers[i].position, dir * 100, Color.green);
                        }
                        else
                        {
                            canBuild = false;
                            Debug.DrawRay(FoundationCheckers[i].position, dir * 100, Color.red);
                        }
                    }
                    else
                    {
                        Debug.DrawRay(FoundationCheckers[i].position, dir * 100, Color.green);
                    }
                }
            }
            if(canBuild)
            {
                tempBuildingGraphic.GetComponent<MeshRenderer>().material = previewOk;
            }
            else
            {
                tempBuildingGraphic.GetComponent<MeshRenderer>().material = previewBlocked;
            }
        }
    }

    public void CreatePreview(string buildingName)
    {
        if (!buildingEnabled)
        {
            buildingEnabled = true;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1000))
            {
                if (hit.transform.tag != "Ground") { canBuild = false; } else { canBuild = true; }
            }

            tempBuilding = Instantiate(Resources.Load("Prefabs/Buildings/Previews/" + buildingName + "Preview"), hit.point, Quaternion.identity) as GameObject;
            tempBuildingGraphic = tempBuilding.transform.FindChild("Graphic").gameObject;
            FoundationCheckers = tempBuilding.transform.FindChild("Foundation Check").GetComponentsInChildren<Transform>();
        }
    }

    public void MovePreview ()
    {
        if (!togglePrevewRotation)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1000))
            {
                tempBuilding.transform.position = hit.point;
            }
        }
    }

    public void RotatePreview()
    {
        if (KeyModifiers.Shift())
        {
            Vector3 pos = Camera.main.WorldToScreenPoint(tempBuilding.transform.position);
            Vector3 dir = Input.mousePosition - pos;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            tempBuilding.transform.rotation = Quaternion.AngleAxis((int)angle, Vector3.down);
        }
        else
        {
            Vector3 pos = Camera.main.WorldToScreenPoint(tempBuilding.transform.position);
            Vector3 dir = Input.mousePosition - pos;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            tempBuilding.transform.rotation = Quaternion.AngleAxis((int)angle, Vector3.down);

            var vec = tempBuilding.transform.eulerAngles;
            vec.y = Mathf.Round(vec.y / currentRotationDegree) * currentRotationDegree;
            tempBuilding.transform.eulerAngles = vec;
        }
    }

    public void ConfirmPreview(string buildingName)
    {
        if (canBuild)
        {
            GameObject _building = Instantiate(Resources.Load("Prefabs/Buildings/" + buildingName), tempBuilding.transform.position, tempBuilding.transform.rotation) as GameObject;
            CancelPreview();
            Debug.Log(buildingName + " Has been Spawned at " + instLocation.ToString() + " at *CurrentGameTime*");
            //TODO Update a* graph with new building.
        }
    }

    public void CancelPreview()
    {
        Destroy(tempBuilding);
        buildingEnabled = false;
        canBuild = false;
        isTriggered = false;
        togglePrevewRotation = false;
    }

    void Prerequisites()
    {
        if (buildingEnabled)
        {
            CheckSensors();
            MovePreview();
        }

        if (togglePrevewRotation)
        {
            RotatePreview();
        }
    }

    void KeyBindings() // NO HARD CODED KEYBINDINGS
    {
        if (Input.GetButtonUp("Create Building Preview"))
        {
            CreatePreview(currentBuilding);
        }

        if (Input.GetButtonUp("Confirm Building Preview"))
        {
            ConfirmPreview(currentBuilding);
        }

        if (Input.GetButtonUp("Cancel Building Preview"))
        {
            CancelPreview();
        }

        if (Input.GetButtonUp("Toggle Preview Rotation"))
        {
            if (tempBuilding != null)
            {
                togglePrevewRotation = !togglePrevewRotation;
            }
        }
    }
}
