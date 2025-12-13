using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    
    public float panSpeed = 20f;
    public float dragSpeed = 2f;

    [Header("Zoom Settings")]
    public float zoomSpeed;
    public float minY;
    public float maxY;

    private float zoomRatio = 1.0f; //Help keep middle mouse movement synced

    [Header("Border(Pixels)")]
    public float edgeBorder = 10f; // measured in pixels

    Vector3 lastMousePos;

    void Start()
    {
        zoomSpeed *= 1000;
    }

    void Update()
    {
        Vector3 pos = transform.position;

        /////////////
        // Edge movement
        /////////////

        // if (!Input.GetMouseButton(0))
        // {
        // //Move right
        //     if (Input.mousePosition.x >= Screen.width - edgeBorder)
        //     {
        //         pos.x += panSpeed * Time.deltaTime;
        //     }

        //     //Move left
        //     if (Input.mousePosition.x <= edgeBorder)
        //     {
        //         pos.x -= panSpeed * Time.deltaTime;
        //     }

        //     if (Input.mousePosition.y >= Screen.height - edgeBorder)
        //     {
        //         pos.z += panSpeed * Time.deltaTime;
        //     }

        //     if (Input.mousePosition.y <=  edgeBorder)
        //     {
        //         pos.z -= panSpeed * Time.deltaTime;
        //     }

        // }


        ///////////////
        //Drag movement
        ///////////////

        if (Input.GetMouseButton(2))
        {
            if (Input.GetMouseButtonDown(2))
            {
                lastMousePos = Input.mousePosition;
            }

            Vector3 mouseDif = Input.mousePosition - lastMousePos;

            pos.x -= mouseDif.x * dragSpeed * zoomRatio * Time.deltaTime;
            pos.z -= mouseDif.y * dragSpeed * zoomRatio *Time.deltaTime;

            lastMousePos = Input.mousePosition;
        }

        ///////////////
        //Zoom movement
        ///////////////

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {

            // Move along local forward direction for smooth zoom
            Vector3 direction = transform.forward * scroll * zoomSpeed * Time.deltaTime;
            Vector3 newPos = pos + direction;

            if (newPos.y < minY)
            {
                newPos.y = minY;
                newPos.x = pos.x;
                newPos.z = pos.z;
            }
            else if (newPos.y > maxY)
            {
                newPos.y = maxY;
                newPos.x = pos.x;
                newPos.z = pos.z;
            }
            
            pos = newPos;
        }
        
        zoomRatio = Mathf.InverseLerp(minY, maxY, pos.y); // 0 = minY (zoomed in), 1 = maxY (zoomed out)
        zoomRatio = Mathf.Lerp(0.5f, 2f, zoomRatio); // scale drag speed to something usable

    
        transform.position = pos;
    }
}
