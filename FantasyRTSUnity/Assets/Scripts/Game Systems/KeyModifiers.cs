using UnityEngine;
using System.Collections;

public class KeyModifiers : MonoBehaviour
{
    public static bool Shift ()
    {
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) { return true; } else { return false; }
	}

    public static bool Ctrl()
    {
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) { return true; } else { return false; }
    }

    public static bool Alt()
    {
        if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) { return true; } else { return false; }
    }

    public static bool Custom1()
    {
        if (Input.GetKey("Custom Modifier 1")) { return true; } else { return false; }
    }

    public static bool Custom2()
    {
        if (Input.GetKey("Custom Modifier 2")) { return true; } else { return false; }
    }

    public static bool Custom3()
    {
        if (Input.GetKey("Custom Modifier 3")) { return true; } else { return false; }
    }

    public static bool Custom4()
    {
        if (Input.GetKey("Custom Modifier 4")) { return true; } else { return false; }
    }

    public static bool Custom5()
    {
        if (Input.GetKey("Custom Modifier 5")) { return true; } else { return false; }
    }
}