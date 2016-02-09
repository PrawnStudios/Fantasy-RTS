using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour
{

    //TODO Redo the height speed modifier and all calculation related to it
    //TODO Look at optimizing and shortening the zoom function and hotkeys

    // Panning Variables
    private float panSpeedGround = 25; //The speed the camera will move at when it is locked to the ground.
    private float panSpeed; //The dynamic pan speed.

    //Height Variables
    private int heightSpeedModifer = 5; //The % speed increase the camera sound get for every 1% higher it is above the ground   
    public int currentHeightPercentage; //This is the current % of the total height the camera can be at.  dynamicCapLower = 0% and zoomCapUpper = 100%

    //Zooming Variables
    public float zoomSpeed = 200; //The speed the camera zoom's in and out at.
    private float tiltSpeed = 75; //The speed the camera tilt's in and out at.
    public float tilt = 60;

    //Zooming Caps
    private int zoomCapUpper = 150; //The maximum height the camera can go. (This is a static value and does not increase based on the height of the terrain below)
    private int zoomCapLower = 5; //This is the mimumum height the camera can be above the terrain. (This is the static value)
    private float dynamicCapLower; //This is the the minimum y coorinate that the camera can be at. it is calculated as (terrain height directly below the camera + the zoom cap lower)
    private int tiltCapLower = 0; //This is the minimum nalge the camera can tilt to when fully zoomed in. (0 = looking directly ahead)
    private int tiltCapUpper = 60; // This is the defualt angle the camera is pointed at during gameplay.

    //Rotation Variables
    private float rotSpeed = 150; //The speed the camera rotates at (Applies for horizonatal and "Tilt" rotation.)
    

    private bool heightChanged = false; //Bool to check if the cameras height has changed since the last frame. 

    private GameObject target; //This is the target that is currently selected by the user. This is updated with a subscription to Target.onTargetUpdate()
    private Transform childCamera; //Cache of the Camera that is the child of the camera manager.

    private float terrainheigh; //Updates every frame to the height of the terrain directly below the camera
    private float distanceFromGround; //Updates every frame to the distance between the camera and the terrain directly below.
    public bool lockToGround = false; //A bool used to check if the camera's height should follow the height changes of the terrain directly below

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

            if (lockToGround)
            {
                currentHeightPercentage = (int)((distanceFromGround - zoomCapLower) / ((zoomCapUpper - zoomCapLower) - terrainheigh) * 100); //If we are locked to the ground, calcualte our height% using this method.
            }
            else
            {
                currentHeightPercentage = (int)((transform.position.y - zoomCapLower) / ((zoomCapUpper - zoomCapLower)) * 100); //If we are not locked to the ground, calcualte our height% using this method.
            }
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

        panSpeed = panSpeedGround + (((panSpeedGround / 100) * currentHeightPercentage) * heightSpeedModifer); //Adjust the pan speed to increase based on the height of the camera

        transform.Translate(dir * Time.deltaTime * panSpeed); //Pans the camers in the direction we passed the function and with the speed the function calcualtes based on the height of the camera
    }

    void Zoom(Vector3 dir)
    {
        float zoomSpeedDynamic = zoomSpeed + (((zoomSpeed / 100) * currentHeightPercentage) * heightSpeedModifer);
        if (dir.y >= 0) // If Zooming Out
        {
            if (tilt < tiltCapUpper) //If tilted
            {
                Tilt();
            }
            else
            {
                lockToGround = false;
                if (transform.position.y + dir.y * zoomSpeedDynamic * Time.deltaTime <= zoomCapUpper)
                {
                    transform.position = transform.position + dir * zoomSpeedDynamic * Time.deltaTime;
                }
                else
                {
                    transform.position = new Vector3(transform.position.x, zoomCapUpper, transform.position.z);
                }
            }
        }
        else // If zooming in
        {

            if (transform.position.y + dir.y * zoomSpeedDynamic * Time.deltaTime > dynamicCapLower)
            {
                transform.position = transform.position + dir * zoomSpeedDynamic * Time.deltaTime;
            }
            else
            {
                transform.position = new Vector3(transform.position.x, dynamicCapLower, transform.position.z); // Moves Position if not fully zoomed and once fully zoomed start to tilt
                Tilt();
            }
        }
    }

    void Tilt()
    {
        tilt = Camera.main.transform.eulerAngles.x - (Input.GetAxis("Mouse ScrollWheel") * tiltSpeed * Time.deltaTime) * tiltSpeed;
        tilt = Mathf.Clamp(tilt, tiltCapLower, tiltCapUpper);
        Camera.main.transform.eulerAngles = new Vector3(tilt, 0, 0);
        lockToGround = true;
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

       /* //ZOOM
        if(Input.GetButton("Camera Zoom In"))
        {
            Zoom(Camera.main.transform.forward);
        }
        if(Input.GetButton("Camera Zoom Out") || Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            Zoom(-Camera.main.transform.forward);
        }*/

        if(Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            Zoom(Camera.main.transform.forward);
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            Zoom(-Camera.main.transform.forward);
        }

        //ROTATION
        if (Input.GetButton("Camera Rotate Left"))
        {
            Rotate(Vector3.down);
        }

        if (Input.GetButton("Camera Rotate Right"))
        {
            Rotate(Vector3.up);
        }

    }
}
