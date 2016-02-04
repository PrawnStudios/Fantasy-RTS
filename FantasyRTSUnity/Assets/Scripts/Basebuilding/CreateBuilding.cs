using UnityEngine;
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
    public static bool isInRange = false;

    private Material previewOk;
    private Material previewBlocked;

    public int currentRotationDegree = 25;
    private bool togglePrevewRotation = false;

    public string currentBuilding;
    private string previewName;

    private Vector2 origin = new Vector2(0, 0);
    private int radius = 100;

    //LINE RENDERERERERERER
    public float ThetaScale = 0.01f;
    private int Size;
    private LineRenderer LineDrawer;
    private float Theta = 0f;

    void Start ()
    {        
        previewOk = Resources.Load("Materials/Buildings/BuildingPreview") as Material;
        previewBlocked = Resources.Load("Materials/Buildings/PreviewBlocked") as Material;
        LineDrawer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        KeyBindings(); // Check for Inputs    
        Prerequisites(); //Check Prerequisite "Functions".

        if(previewName == "Citadel")
        {
            GetComponent<LineRenderer>().enabled = true;
            Theta = 0f;
            Size = (int)((1f / ThetaScale) + 1f);
            LineDrawer.SetVertexCount(Size);
            LineDrawer.material = new Material(Shader.Find("Particles/Additive"));
            LineDrawer.SetColors(Color.red, Color.red);
            for (int i = 0; i < Size; i++)
            {
                Theta += (2.0f * Mathf.PI * ThetaScale);
                float x = radius * Mathf.Cos(Theta);
                float y = radius * Mathf.Sin(Theta);
                LineDrawer.SetPosition(i, new Vector3(x, tempBuilding.transform.position.y + 1, y));
            }
        }
        else
        {
            GetComponent<LineRenderer>().enabled = false;
        }
    }

    public void CheckSensors ()
    {
        if (isTriggered)
        {
            canBuild = false;
            tempBuildingGraphic.GetComponent<MeshRenderer>().material = previewBlocked;
            Debug.Log("Can't Build Here");
        }
        else if (Vector2.Distance(origin, new Vector2(tempBuilding.transform.position.x, tempBuilding.transform.position.z)) > radius)
        {
            canBuild = false;
            tempBuildingGraphic.GetComponent<MeshRenderer>().material = previewBlocked;                  
            return;
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
            tempBuilding.name = buildingName + "Preview";
            tempBuildingGraphic = tempBuilding.transform.FindChild("Graphic").gameObject;
            FoundationCheckers = tempBuilding.transform.FindChild("Foundation Check").GetComponentsInChildren<Transform>();
            previewName = buildingName;
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
            GameObject _building = Instantiate(Resources.Load("Prefabs/Buildings/" + previewName), tempBuilding.transform.position, tempBuilding.transform.rotation) as GameObject;
            _building.name = previewName;
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
        previewName = null;
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
