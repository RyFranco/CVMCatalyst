using Unity.Mathematics;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public bool MousePan;
    public float panSpeed = 20f;
    public float dragSpeed = 2f;

    [Header("Zoom Settings")]
    public float zoomSpeed;
    public float MinZoom = -18f;
    public float MaxZoom = 18f;

    private float zoomRatio = 1.0f; //Help keep middle mouse movement synced

    [Header("Border(Pixels)")]
    public float edgeBorder = 15f; // measured in pixels

    Vector3 lastMousePos;

    public GameObject CameraHolder;

    public GameObject FollowTarget;


    float ZoomNumber = 0f;

    Vector3 DefaultCameraFollowOffset = new Vector3(0,15,-25);

    Vector3 CameraFollowOffset;

    void Start()
    {
        CameraFollowOffset = DefaultCameraFollowOffset;
    }

    void Update()
    {
        HandleCameraMovement();
        
        //Zooms by moving camera further in/out of screen
        ZoomNumber += Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        ZoomNumber = math.clamp(ZoomNumber, MinZoom, MaxZoom);
        transform.localPosition = new Vector3(0,0,ZoomNumber);

        if(FollowTarget != null) //Makes camera follow unit if selected
        {
            CameraHolder.transform.position = FollowTarget.transform.position + CameraFollowOffset + new Vector3(0,1f,0);
        }

    }

    void HandleCameraMovement()
    {
        Vector3 pos = CameraHolder.transform.position;

        /////////////
        // Edge movement
        /////////////
        
        if (!Input.GetMouseButton(0))
        {
            //Move right
            if ((Input.mousePosition.x >= Screen.width - edgeBorder && MousePan) || Input.GetKey("d"))
            {
                pos.x += panSpeed * Time.deltaTime;
                FollowTarget = null;
            }

            //Move left
            if ((Input.mousePosition.x <= edgeBorder && MousePan) || Input.GetKey("a"))
            {
                pos.x -= panSpeed * Time.deltaTime;
                FollowTarget = null;
            }

            //move up
            if ((Input.mousePosition.y >= Screen.height - edgeBorder && MousePan) || Input.GetKey("w"))
            {
                pos.z += panSpeed * Time.deltaTime;
                FollowTarget = null;
            }
            
            //move down
            if ((Input.mousePosition.y <=  edgeBorder && MousePan) || Input.GetKey("s"))
            {
                pos.z -= panSpeed * Time.deltaTime;
                FollowTarget = null;
            }

        }

        ///////////////
        //Drag movement
        ///////////////
        if (Input.GetMouseButton(2))
        {
            FollowTarget = null;
            if (Input.GetMouseButtonDown(2))
            {
                lastMousePos = Input.mousePosition;
            }

            Vector3 mouseDif = Input.mousePosition - lastMousePos;

            pos.x -= mouseDif.x * dragSpeed * zoomRatio * Time.deltaTime;
            pos.z -= mouseDif.y * dragSpeed * zoomRatio *Time.deltaTime;

            lastMousePos = Input.mousePosition;
        }

        CameraHolder.transform.position = pos;
    }

    public void FocusOnObject(GameObject Subject)
    {
        FollowTarget = Subject;
        
    }
}
