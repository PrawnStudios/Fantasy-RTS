using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour
{
    
    //TODO Redo the height speed modifier and all calculation related to it
    //TODO Look at optimizing and shortening the zoom function and hotkeys
    
    // Camera Movement Variables
    public  float panSpeedGround = 25; //The speed the camera will move at when it is locked to the ground.
    public float panSpeed; //The dynamic pan speed.

    private int heightSpeedModifer = 5; //The % speed increase the camera sound get for every 1% higher it is above the ground   
    public int currentHeightPercentage; //This is the current % of the total height the camera can be at.  dynamicCapLower = 0% and zoomCapUpper = 100%


    private float rotSpeed = 150; //The speed the camera rotates at (Applies for horizonatal and "Tilt" rotation.)
    private float zoomSpeed = 50; //The speed the camera zoom's in and out at.
    private int zoomCapUpper = 150; //The maximum height the camera can go. (This is a static value and does not increase based on the height of the terrain below)
    private int zoomCapLower = 5; //This is the mimumum height the camera can be above the terrain. (This is the static value)
    private float dynamicCapLower; //This is the the minimum y coorinate that the camera can be at. it is calculated as (terrain height directly below the camera + the zoom cap lower)
    private int tiltCapLower = 0; //This is the minimum nalge the camera can tilt to when fully zoomed in. (0 = looking directly ahead)
    private int tiltCapUpper = 60; // This is the defualt angle the camera is pointed at during gameplay.

    //private int heightSpeedModifer = 90; //(WIP) - This is a % modifier that increase the speed of the camera panning and zooming based on the height of the camera. (this is used to make the camera speed scale with it ammount it can see)

    private bool heightChanged = false; //Bool to check if the cameras height has changed since the last frame. 

    private GameObject target; //This is the target that is currently selected by the user. This is updated with a subscription to Target.onTargetUpdate()
    private Transform childCamera; //Cache of the Camera that is the child of the camera manager.

    private float terrainheigh; //Updates every frame to the height of the terrain directly below the camera
    private float distanceFromGround; //Updates every frame to the distance between the camera and the terrain directly below.
    private bool lockToGround = false; //A bool used to check if the camera's height should follow the height changes of the terrain directly below

    void Start()
    {
        childCamera = Camera.main.transform; //Cache the transform of the main camera.
    }

    void OnEnable()
    {
        Target.OnTargetUpdate += UpdateTarget; //When this object activates this subscribes it to recieve updates about when the camera target changes.
    }

    void OnDisable()
    {
        Target.OnTargetUpdate -= UpdateTarget; //When this object deactivates this unsubscribes it from recieving updates about when the camera target changes. (unsubscribe is needed to avoid memory leak)
    }

    void UpdateTarget(GameObject _target)
    {
        target = _target; //Caches the new target when the camera changes its target (From the Target script).
    }

    void Update ()
    {
        Keybindings(); //Checks to see which inputs are being called.
        HeightCheck(); //Calculates the dynamic cap lower and the height of the terrain directly below the camera.
    }

    void HeightCheck()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit)) //Raycast directly down
        {
            distanceFromGround = hit.distance; //Cache the distance between the camera and the terrain below
            terrainheigh = transform.position.y - distanceFromGround; // Calculate the height of the terrain below from
            dynamicCapLower = terrainheigh + zoomCapLower; //Calcuates and Updates the lowest y coordinate the camera can go based on the height of the terrain below.
        }        
    }

    void Rotate(Vector3 dir) //Roate the camera by passing it a direction as a Vector3
    {
        if (target == null) //If the camera has no target
        {
            transform.Rotate(dir * rotSpeed * Time.deltaTime);//Rotates the camera arround it current position
        }
        else //If the camera has a target
        {
            transform.RotateAround(target.transform.position, dir, rotSpeed * Time.deltaTime); //Rotates the camera arround the target of the camera
        }
    }

    void Pan(Vector3 dir, float speed)
    {
        if (transform.position.y <= dynamicCapLower || lockToGround) //if the camera height is <= to the height of the terrain or if the camera is locked to the terrain
        {
            transform.position = new Vector3(transform.position.x, dynamicCapLower, transform.position.z); //Sets the height of the camera to the height of the terrain         
        }

        if (lockToGround)
        {
            currentHeightPercentage = (int)((distanceFromGround - zoomCapLower) / ((zoomCapUpper - zoomCapLower) - terrainheigh) * 100); //If we are locked to the ground, calcualte our height% using this method.
        }
        else
        {
            currentHeightPercentage = (int)((transform.position.y - zoomCapLower) / ((zoomCapUpper - zoomCapLower)) * 100); //If we are not locked to the ground, calcualte our height% using this method.
        }


        panSpeed = panSpeedGround + (((panSpeedGround / 100) * currentHeightPercentage) * heightSpeedModifer); //Adjust the pan speed to increase based on the height of the camera

        transform.Translate(dir * Time.deltaTime * panSpeed); //Pans the camers in the direction we passed the function and with the speed the function calcualtes based on the height of the camera
    }

    void Zoom(Vector3 dir)
    {
        float speed = zoomSpeed + 10 * ((transform.position.y * 0.01f) * 90); //TODO Rework to use the heigh modifer the pan system uses.
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
                lockToGround = false;
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
            
            if (transform.position.y + dir.y * speed * Time.deltaTime > dynamicCapLower)
            {
                transform.position = transform.position + dir * speed * Time.deltaTime;               
            }
            else
            {
                transform.position = new Vector3(transform.position.x, dynamicCapLower, transform.position.z); // Moves Position if not fully zoomed and once fully zoomed start to tilt
                lockToGround = true;

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
                Pan(Vector3.forward, panSpeedGround * 0.5f);
            }
            else
            {
                Pan(Vector3.forward, panSpeedGround);
            }
        }

        if (Input.GetButton("Camera Pan Left"))
        {
            if (Input.GetButton("Camera Pan Forward") || Input.GetButton("Camera Pan Back"))
            {
                Pan(Vector3.left, panSpeedGround * 0.5f);
            }
            else
            {
                Pan(Vector3.left, panSpeedGround);
            }
        }

        if (Input.GetButton("Camera Pan Back"))
        {
            if (Input.GetButton("Camera Pan Left") || Input.GetButton("Camera Pan Right"))
            {
                Pan(Vector3.back, panSpeedGround * 0.5f);
            }
            else
            {
                Pan(Vector3.back, panSpeedGround);
            }
        }

        if (Input.GetButton("Camera Pan Right"))
        {
            if (Input.GetButton("Camera Pan Forward") || Input.GetButton("Camera Pan Back"))
            {
                Pan(Vector3.right, panSpeedGround * 0.5f);
            }
            else
            {
                Pan(Vector3.right, panSpeedGround);
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
