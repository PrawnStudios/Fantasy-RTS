using UnityEngine;
using System.Collections;
using UnityEditor;

public class Unit : MonoBehaviour 
{
    public bool selected = false;
    public bool platoonSelected = false;

    public Renderer rend;
    public Color defaultColor;

    void Start()
    {
        rend = transform.FindChild("Graphic").GetComponent<Renderer>();
        defaultColor = rend.material.color;
    }
	
	void Update () 
	{
        if (Input.GetMouseButton(0) && !platoonSelected)
        {
            Vector3 pos = Camera.main.WorldToScreenPoint(rend.bounds.center);
            pos.y = Target.InvertMouseY(pos.y);

            selected = Target.selection.Contains(pos);

            if (selected && KeyModifiers.Shift())
            {
                Selected();    
            }
            else
            {
                Deselected();
            }            
        }
    }

    public void Selected()
    {
        rend.material.color = Color.red;
    }

    public void Deselected()
    {
        rend.material.color = defaultColor;
    }
}