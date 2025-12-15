using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.EventSystems;
using Mono.Cecil;
using Unity.VisualScripting;

public class UnitSelectionManager : MonoBehaviour
{

    public Building selectedBuilding;
    Camera cam;

    void Start()
    {
        cam = Camera.main;
    }


    void Update()
    {

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);


        //left click and select 1 unit
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            if (Physics.Raycast(ray, out RaycastHit hit))
            {


                if (hit.transform.CompareTag("Building"))
                {
                    //Debug.Log("Building Clicked");
                    if (selectedBuilding)
                    {
                        selectedBuilding.Deselect();
                        selectedBuilding = null;
                    }

                    selectedBuilding = hit.transform.GetComponent<Building>();
                    selectedBuilding.Select();

                }
                else
                {
                    if(selectedBuilding) selectedBuilding.Deselect();
                    selectedBuilding = null;
                }

            }
        }

        //Right Click and selected if at least one unit is selected, move them
        if (Input.GetMouseButtonDown(1))
        {
            if (Physics.Raycast(ray, out RaycastHit hit))
            {

              


            }
        }


    }



}
