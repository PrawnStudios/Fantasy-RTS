using UnityEngine;
using System.Collections.Generic;

public class Platoon : MonoBehaviour 
{
    public List<GameObject> units;
    public bool selected = false;

    void Start () 
	{
        UpdateUnitList();
    }
	
	void Update () 
	{
        if (Input.GetMouseButton(0))
        {
            Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
            pos.y = Target.InvertMouseY(pos.y);
            selected = Target.selection.Contains(pos);

            if (selected && !KeyModifiers.Shift())
            {
                foreach (GameObject unit in units)
                {
                    unit.GetComponent<Unit>().Selected();
                    unit.GetComponent<Unit>().platoonSelected = true;
                }
            }
            else
            {
                foreach (GameObject unit in units)
                {
                    unit.GetComponent<Unit>().Deselected();
                    unit.GetComponent<Unit>().platoonSelected = false;
                }
            }
        }
	}

    public void UpdateUnitList()
    {
        Transform[] children = gameObject.GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            if (child.transform.tag == "Unit")
            {
                units.Add(child.gameObject);
            }
        }
    }
}
