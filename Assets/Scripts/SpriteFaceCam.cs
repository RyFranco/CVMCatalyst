using System;
using Unity.VisualScripting;
using UnityEngine;

public class SpriteFaceCam : MonoBehaviour
{

    private Transform cameraTransform;
    public float baseDistance = 5f;
    public float scaleMultiplier = 1f;

    public float minDistance = 10f;  
    public float maxDistance = 50f; 
    public float minScale = 0.01f;
    public float maxScale = 0.04f;




    void Start()
    {
        cameraTransform = Camera.main.transform;

    }

    void LateUpdate()
    {
        transform.forward = Camera.main.transform.forward;

        float dist = Vector3.Distance(cameraTransform.position, transform.position);
        float ratio = Mathf.InverseLerp(minDistance, maxDistance, dist);

        float scale = Mathf.Lerp(minScale, maxScale, ratio);

        transform.localScale = Vector3.one * scale;

    }



}
