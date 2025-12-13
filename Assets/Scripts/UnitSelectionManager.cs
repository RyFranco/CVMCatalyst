using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.EventSystems;
using Mono.Cecil;
using Unity.VisualScripting;

public class UnitSelectionManager : MonoBehaviour
{

    // public Building selectedBuilding;
    // Camera cam;
    // Vector2 startMousePos;
    // public RectTransform SelectionBox;
    // float dragDelay = 0.1f;
    // float mouseDownTime;
    // void Start()
    // {
    //     cam = Camera.main;
    // }


    // void Update()
    // {

    //     Ray ray = cam.ScreenPointToRay(Input.mousePosition);


    //     //left click and select 1 unit
    //     if (Input.GetMouseButtonDown(0))
    //     {
    //         if (EventSystem.current.IsPointerOverGameObject())
    //         {
    //             return;
    //         }

    //         if (Physics.Raycast(ray, out RaycastHit hit))
    //         {

    //             if (hit.transform.CompareTag("Unit"))
    //             {

    //                 Unit unit = hit.transform.GetComponent<Unit>();
    //                 if(unit.playerID == 0)
    //                 {
    //                     selectedUnits.Add(unit);
    //                     unit.Select();

    //                     if (selectedBuilding)
    //                     {
    //                         selectedBuilding.Deselect();
    //                         selectedBuilding = null;
    //                     }
    //                 }
                   

    //             }
    //             else if (hit.transform.CompareTag("Building"))
    //             {
    //                 Debug.Log("Building Clicked");
    //                 if (selectedBuilding)
    //                 {
    //                     selectedBuilding.Deselect();
    //                     selectedBuilding = null;
    //                 }

    //                 selectedBuilding = hit.transform.GetComponent<Building>();
    //                 selectedBuilding.Select();

    //             }
    //             else
    //             {
    //                 deselectAll();
    //                 if (selectedBuilding)
    //                 {
    //                     selectedBuilding.Deselect();
    //                     selectedBuilding = null;
    //                 }
    //             }

    //         }
    //     }

    //     //Right Click and selected if at least one unit is selected, move them
    //     if (Input.GetMouseButtonDown(1) && selectedUnits.Count > 0)
    //     {
    //         if (Physics.Raycast(ray, out RaycastHit hit))
    //         {

    //             //Was resource tiles clicked?
    //             ResourceTile resourceTile = hit.collider.GetComponent<ResourceTile>();
    //             if (resourceTile)
    //             {
    //                 foreach (Unit unit in selectedUnits)
    //                 {
    //                     unit.StopAllActions();
    //                     unit.StartCoroutine(unit.TryStartHarvesting(resourceTile));
    //                 }
    //                 return;
    //             }

    //             //Was enemy unit clicked?
    //             Unit targetUnit = hit.collider.GetComponent<Unit>();
    //             if (targetUnit != null)
    //             {
    //                 if (targetUnit.playerID != 0)
    //                 {
    //                     foreach (Unit unit in selectedUnits)
    //                     {
    //                         unit.StopAllActions();
    //                         unit.Attack(targetUnit);
    //                     }
    //                     return;
    //                 }

    //             }

    //             //Was enemy building clicked?
    //             Building targetBuilding = hit.collider.GetComponent<Building>();
    //             if (targetBuilding != null)
    //             {
    //                 if(targetBuilding.playerID != 0)
    //                 {
    //                     foreach (Unit unit in selectedUnits)
    //                         unit.Attack(null, targetBuilding);
    //                     return;
                        
    //                 }
    //             }

    //             //Else just move to mouse click position
    //             foreach (Unit unit in selectedUnits)
    //             {
    //                 unit.StopAllActions();
    //                 unit.GetComponent<UnitMovement>().MoveTo(hit.point);
    //             }

               
    //         }
    //     }

    //     HandleSelectionBox();

    // }

   


    // //Box Selection
    // void HandleSelectionBox()
    // {
    //     if (Input.GetMouseButtonDown(0)) //when mouse is intially pressed
    //     {
    //         SelectionBox.sizeDelta = Vector2.zero;
    //         SelectionBox.gameObject.SetActive(true);
    //         startMousePos = Input.mousePosition;
    //         mouseDownTime = Time.time;
    //     }
    //     else if (Input.GetMouseButton(0) && mouseDownTime + dragDelay < Time.time) // if mouse is still being held
    //     {
    //         ResizeSelectionBox();
    //     }
    //     else if (Input.GetMouseButtonUp(0))// if mouse is let go
    //     {
    //         SelectionBox.sizeDelta = Vector2.zero;
    //         SelectionBox.gameObject.SetActive(false);
    //         mouseDownTime = 0;
    //     }
    // }


    // void ResizeSelectionBox()
    // {
    //     float width = Input.mousePosition.x - startMousePos.x;
    //     float height = Input.mousePosition.y - startMousePos.y;

    //     SelectionBox.anchoredPosition = startMousePos + new Vector2(width / 2, height / 2);
    //     SelectionBox.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));

    //     Bounds bounds = new Bounds(SelectionBox.anchoredPosition, SelectionBox.sizeDelta);

    //     for (int i = 0; i < AvailableUnits.Count; i++)
    //     {
    //         if (UnitIsInSelectionBox(cam.WorldToScreenPoint(AvailableUnits[i].transform.position), bounds))
    //         {
    //             AvailableUnits[i].Select();
    //             selectedUnits.Add(AvailableUnits[i]);
    //         }
    //         else
    //         {
    //             AvailableUnits[i].Deselect();
                
    //         }
    //     }
    // }

    // bool UnitIsInSelectionBox(Vector2 Position, Bounds Bounds)
    // {
    //     return Position.x > Bounds.min.x && Position.x < Bounds.max.x
    //     && Position.y > Bounds.min.y && Position.y < Bounds.max.y;
    // }


}
