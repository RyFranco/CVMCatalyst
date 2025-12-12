using Unity.VisualScripting;
using UnityEngine;

public class SpriteFaceCam : MonoBehaviour
{

    void LateUpdate()
    {
        transform.forward = Camera.main.transform.forward;
    }
}
