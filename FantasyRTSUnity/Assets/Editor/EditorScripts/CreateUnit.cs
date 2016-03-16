using UnityEngine;
using UnityEditor;
using System.Collections;

public class CreateUnit : EditorWindow
{
    UnitStats unitStats;
    public Object unitMeshObj;
    public Object unitMatObj;
    public GameObject customPlatoon;
    public GameObject customUnit;
    private Vector3 spacerX;
    private Vector3 spacerZ;
    private string unitName = "Enter unit name here.";
    private string[] options = new string[] { "Infantry", "Mounted", "Seige", "Beast", "Magic" };
    private string[] presets = new string[] {"None", "Test Legion Small","Test Legion Medium","Test Legion Large"};
    private int[][] unitForm = new int[2][];
    private float unitSpaceX;
    private float unitSpaceZ;
    public int presetSelect = 0;
    public int selected = 0;
    public int health;
    public int damage;
    public int armor;
    public int magicResistance;
    public int speed;
    public int morale;
    public int columns;
    public int rows;

    [MenuItem("Tools/Game Object Creation/Unit Creation Tool %u")]
    public static void showEditor()
    {
        //DisplayWizard("Unit Creation", typeof(CreateUnit), "Create Unit and close");
        EditorWindow.GetWindow<CreateUnit>(false, "Unit Creation");
    }

    void OnGUI()
    {

        GUILayout.Label("");
        unitName = EditorGUILayout.TextArea(unitName);
        unitMeshObj = EditorGUILayout.ObjectField("Unit Mesh:", unitMeshObj, typeof(Mesh), true);
        unitMatObj = EditorGUILayout.ObjectField("Unit Material:", unitMatObj, typeof(Material), true);

        GUILayout.Label("");
        selected = EditorGUILayout.Popup("Choose your unit type:", selected, options);
        columns = EditorGUILayout.IntSlider("Number Of Columns:", columns, 0, 10);
        rows = EditorGUILayout.IntSlider("Number Of Rows:", rows, 0, 10);
        unitSpaceZ = EditorGUILayout.Slider("Unit spacing Z:", unitSpaceZ,0, 20);
        unitSpaceX = EditorGUILayout.Slider("Unit spacing X:",unitSpaceX,0, 20);
        health = EditorGUILayout.IntSlider("Total Health:", health, 0, 500);
        damage = EditorGUILayout.IntSlider("Damage Per Second:", damage, 0, 500);
        armor = EditorGUILayout.IntSlider("Physical Armor:", armor, 0, 250);
        magicResistance = EditorGUILayout.IntSlider("Magic Resistance:", magicResistance, 0, 250);
        speed = EditorGUILayout.IntSlider("Movement Speed:", speed, 0, 500);
        morale = EditorGUILayout.IntSlider("Unit Morale", morale, 0, 100);

        if (health > 0 && damage > 0 && armor > 0 && speed > 0 && unitMatObj != null && unitMeshObj != null)
        {
            if (GUILayout.Button("Create your unit!"))
            {
                spacerX = new Vector3(0, 0, 0);
                spacerZ = new Vector3(0, 0, 0);
                CreatePlatoon();
                MakeFormation();
                Debug.Log("Unit Created");
            }
        }
    }

    void MakeFormation()
    {
        //give each dimension of the array a value of colums and rows
        unitForm[0] = new int[columns];
        unitForm[1] = new int[rows];
        for (int y = 0; y < columns; y++) //nested for loop so each y the x will move 4 times
        {
            CreateUnits();
            incrementZ();
            for (int x = 0; x < rows; x++)
            {
                CreateUnits();
                incrementX();
                unitForm[0] = new int[x * y]; //assign the first 2d array as a x * y
                unitForm[1] = new int[x * y]; //assign the second 2d array as x * y
                Debug.Log(unitForm);
            }
        }
    }

    void CreateUnits()
    {
        customUnit = new GameObject(unitName, typeof(MeshFilter), typeof(MeshRenderer), typeof(CapsuleCollider), typeof(UnitStats));
        customUnit.tag = "Unit";
        customUnit.transform.parent = customPlatoon.transform;
        customUnit.GetComponent<MeshFilter>().mesh = unitMeshObj as Mesh;
        customUnit.GetComponent<MeshRenderer>().material = unitMatObj as Material;
        customUnit.GetComponent<UnitStats>().health = health;
        customUnit.GetComponent<UnitStats>().damage = damage;
        customUnit.GetComponent<UnitStats>().armor = armor;
        customUnit.GetComponent<UnitStats>().magicResistance = magicResistance;
        customUnit.GetComponent<UnitStats>().speed = speed;
        customUnit.GetComponent<UnitStats>().morale = morale;
    }

    void incrementZ()
    {
        spacerX = Vector3.zero; //resets the the x line
        spacerZ += new Vector3(0, 0, unitSpaceZ);
        customUnit.transform.localPosition = (spacerZ);
    }

    void incrementX()
    {
        spacerX += new Vector3(unitSpaceX, 0, 0);
        customUnit.transform.localPosition = (spacerX);
        customUnit.transform.localPosition += (spacerZ);
    }

    void CreatePlatoon()
    {
        customPlatoon = new GameObject(unitName + " Platoon", typeof(BoxCollider),typeof(Platoon));
    }

    //TODO add race check box
    //TODO add presets such as legion small, large ect.
}