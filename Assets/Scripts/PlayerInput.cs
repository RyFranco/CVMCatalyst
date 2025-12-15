using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerInput : MonoBehaviour
{
    [SerializeField]
    private Camera cam;

    [SerializeField]
    private RectTransform selectionBox;

    [SerializeField]
    private LayerMask unitLayer;

    [SerializeField]
    private LayerMask floorLayer;

    private Vector2 startMousePosition;

    float mouseDownTime;
    float dragDelay = 0.1f;

    public Building selectedBuilding;

    private void Update()
    {
        HandleMovementInputs();
        HandleBuildingSelection();
        HandleSelectionInputs();



    }


    private void HandleBuildingSelection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
            {


                if (hit.transform.CompareTag("Building"))
                {
                    Debug.Log("Building Clicked");
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
                    if (selectedBuilding != null)
                    {
                        selectedBuilding.Deselect();
                        selectedBuilding = null;
                    }
                }

            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("Right Clicked Building");
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
            {
                if(hit.transform.gameObject.CompareTag("Tile")) return;
                if (hit.transform.GetComponentInParent<Building>().buildingData.buildingType == BuildingType.UnitTraining)
                {
                    foreach(Unit unit in SelectionManager.Instance.SelectedUnits)
                    {
                       hit.transform.GetComponentInParent<SpecializeUnit>().StartTraining(unit);
                    }
                   
                }
            }
        }
    }

    private void HandleMovementInputs()
    {
        if (Input.GetMouseButtonDown(1) && SelectionManager.Instance.SelectedUnits.Count > 0)
        {
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, floorLayer))
            {

                //Was resource tiles clicked?
                ResourceTile resourceTile = hit.collider.GetComponent<ResourceTile>();
                if (resourceTile)
                {
                    foreach (Unit unit in SelectionManager.Instance.SelectedUnits)
                    {
                        unit.StopAllActions();
                        unit.StartCoroutine(unit.TryStartHarvesting(resourceTile));
                    }
                    return;
                }

                // //Was enemy unit clicked?
                Unit targetUnit = hit.collider.GetComponent<Unit>();
                if (targetUnit != null)
                {
                    if (targetUnit.playerID != 0)
                    {
                        foreach (Unit unit in SelectionManager.Instance.SelectedUnits)
                        {
                            unit.StopAllActions();
                            unit.Attack(targetUnit);
                        }
                        return;
                    }

                }

                //Was enemy building clicked?
                Building targetBuilding = hit.collider.GetComponent<Building>();
                if (targetBuilding != null)
                {
                    if (targetBuilding.playerID != 0)
                    {
                        foreach (Unit unit in SelectionManager.Instance.SelectedUnits)
                            unit.Attack(null, targetBuilding);
                        return;

                    }
                }

                foreach (Unit unit in SelectionManager.Instance.SelectedUnits)
                {
                    unit.StopAllActions();
                    unit.MoveTo(hit.point);
                }
            }
        }
    }

    void HandleSelectionInputs()
    {
        if (Input.GetMouseButtonDown(0)) //when mouse is intially pressed
        {
            selectionBox.sizeDelta = Vector2.zero;
            selectionBox.gameObject.SetActive(true);
            startMousePosition = Input.mousePosition;
            mouseDownTime = Time.time;
        }
        else if (Input.GetMouseButton(0) && mouseDownTime + dragDelay < Time.time) // if mouse is still being held
        {
            ResizeSelectionBox();
        }
        else if (Input.GetMouseButtonUp(0))// if mouse is let go
        {
            selectionBox.sizeDelta = Vector2.zero;
            selectionBox.gameObject.SetActive(false);

            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, unitLayer)
                && hit.collider.TryGetComponent<Unit>(out Unit unit))
            {
                if(unit.playerID != 0 ) return;
                if(unit.playerID != 0 ) return;
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                {
                    if (SelectionManager.Instance.IsSelected(unit))
                    {
                        SelectionManager.Instance.Deselect(unit);
                    }
                    else
                    {
                        SelectionManager.Instance.Select(unit);
                    }

                }
                else
                {
                    SelectionManager.Instance.DeselectAll();
                    SelectionManager.Instance.Select(unit);
                }

            }
            else if (mouseDownTime + dragDelay > Time.time)
            {
                SelectionManager.Instance.DeselectAll();
            }

            mouseDownTime = 0;

        }
    }


    void ResizeSelectionBox()
    {
        float width = Input.mousePosition.x - startMousePosition.x;
        float height = Input.mousePosition.y - startMousePosition.y;

        selectionBox.anchoredPosition = startMousePosition + new Vector2(width / 2, height / 2);
        selectionBox.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));

        Bounds bounds = new Bounds(selectionBox.anchoredPosition, selectionBox.sizeDelta);

        for (int i = 0; i < SelectionManager.Instance.AvailableUnits.Count; i++)
        {
            if (UnitIsInSelectionBox(cam.WorldToScreenPoint(SelectionManager.Instance.AvailableUnits[i].transform.position), bounds))
            {
                SelectionManager.Instance.Select(SelectionManager.Instance.AvailableUnits[i]);

            }
            else
            {
                SelectionManager.Instance.Deselect(SelectionManager.Instance.AvailableUnits[i]);

            }
        }
    }

    bool UnitIsInSelectionBox(Vector2 Position, Bounds Bounds)
    {
        return Position.x > Bounds.min.x && Position.x < Bounds.max.x
        && Position.y > Bounds.min.y && Position.y < Bounds.max.y;
    }
}
