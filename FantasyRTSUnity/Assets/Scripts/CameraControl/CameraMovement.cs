using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour
{
    // Camera Movement Variables
    private float horiztonalPanSpeed = 25;
    private float verticalPanSpeed = 25;
    private float rotSpeed = 150;
    private float zoomSpeed = 25;
    private int zoomCapUpper = 150;
    private int zoomCapLower = 5;
    private int tiltCapLower = 0;
    private int tiltCapUpper = 60;
    private int heightSpeedModifer = 25;
    private bool heightChanged = false;
    private GameObject target;
    private Transform childCamera;

    void Start()
    {
        childCamera = transform.FindChild("Main Camera");
    }

    void OnEnable()
    {
        Target.OnTargetUpdate += UpdateTarget;
    }

    void OnDisable()
    {
        Target.OnTargetUpdate -= UpdateTarget;
    }

    void UpdateTarget(GameObject _target)
    {
        target = _target;
    }

    void Update ()
    {
        Keybindings();
	}

    void Rotate(Vector3 dir)
    {
        if (target == null)
        {
            transform.Rotate(dir * rotSpeed * Time.deltaTime);
        }
        else
        {
            transform.RotateAround(target.transform.position, dir, rotSpeed * Time.deltaTime);
        }
    }

    void Pan(Vector3 dir, float speed)
    {
        if (heightChanged)
        {
            float height = transform.position.y;
            height = (height * 0.01f) * heightSpeedModifer;
            speed = speed * height;
            heightChanged = false;
        }
        transform.Translate(dir * Time.deltaTime * speed);
        //Debug.Log("Speed = " + speed + ", Height = " + transform.position.y + ", Height to Speed Modifier = " + heightSpeedModifer + "%");
    }

    void Zoom(Vector3 dir)
    {
        float height = transform.position.y;
        float speed = zoomSpeed + 10 * ((height * 0.01f) * heightSpeedModifer);
        

        if (dir.y >= 0) // If Zooming Out
        {
            
            if (childCamera.transform.localEulerAngles.x < tiltCapUpper) //If tilted
            {
                float xx = childCamera.transform.localEulerAngles.x + rotSpeed * Time.deltaTime;
                xx = Mathf.Clamp(xx, tiltCapLower, tiltCapUpper);
                childCamera.transform.localEulerAngles = new Vector3(xx, childCamera.transform.localEulerAngles.y, childCamera.transform.localEulerAngles.z);
            }
            else
            {
                if (transform.position.y + dir.y * speed * Time.deltaTime <= zoomCapUpper)
                {
                    transform.position = transform.position + dir * speed * Time.deltaTime;
                }
                else
                {
                    transform.position = new Vector3(transform.position.x, zoomCapUpper, transform.position.z);
                }
            }
        }
        else
        {
            if (transform.position.y + dir.y * speed * Time.deltaTime > zoomCapLower)
            {
                transform.position = transform.position + dir * speed * Time.deltaTime;
            }
            else
            {                
                transform.position = new Vector3(transform.position.x, zoomCapLower, transform.position.z); // Moves Parent Position//Once fully zoomed start to tilt

                float xx = childCamera.transform.localEulerAngles.x - rotSpeed * Time.deltaTime;
                xx = Mathf.Clamp(xx, tiltCapLower, tiltCapUpper);
                childCamera.transform.localEulerAngles = new Vector3(xx, childCamera.transform.localEulerAngles.y, childCamera.transform.localEulerAngles.z);
            }
        }
        
    }

    void Keybindings()
    {
        if(Input.GetButton("Camera Pan Forward"))
        {
            if (Input.GetButton("Camera Pan Right") || Input.GetButton("Camera Pan Left"))
            {
                Pan(Vector3.forward, verticalPanSpeed * 0.5f);
            }
            else
            {
                Pan(Vector3.forward, verticalPanSpeed);
            }
        }

        if (Input.GetButton("Camera Pan Left"))
        {
            if (Input.GetButton("Camera Pan Forward") || Input.GetButton("Camera Pan Back"))
            {
                Pan(Vector3.left, horiztonalPanSpeed * 0.5f);
            }
            else
            {
                Pan(Vector3.left, horiztonalPanSpeed);
            }
        }

        if (Input.GetButton("Camera Pan Back"))
        {
            if (Input.GetButton("Camera Pan Left") || Input.GetButton("Camera Pan Right"))
            {
                Pan(Vector3.back, verticalPanSpeed * 0.5f);
            }
            else
            {
                Pan(Vector3.back, verticalPanSpeed);
            }
        }

        if (Input.GetButton("Camera Pan Right"))
        {
            if (Input.GetButton("Camera Pan Forward") || Input.GetButton("Camera Pan Back"))
            {
                Pan(Vector3.right, horiztonalPanSpeed * 0.5f);
            }
            else
            {
                Pan(Vector3.right, horiztonalPanSpeed);
            }
        }

        //ZOOM
        if(Input.GetButton("Camera Zoom In"))
        {
            Zoom(Camera.main.transform.forward);
        }
        if(Input.GetButton("Camera Zoom Out"))
        {
            Zoom(-Camera.main.transform.forward);
        }

        if(Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            Zoom(Camera.main.transform.forward);
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            Zoom(-Camera.main.transform.forward);
        }
        
        //ROTATION
        if(Input.GetButton("Camera Rotate Left"))
        {
            Rotate(Vector3.down);
        }

        if (Input.GetButton("Camera Rotate Right"))
        {
            Rotate(Vector3.up);
        }

    }
}
