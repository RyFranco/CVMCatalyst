using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class BuildModeManager : MonoBehaviour
{
    public static BuildModeManager instance { get; private set; }
    public static event Action<bool> OnBuildModeChanged;
    bool buildModeActive = false;
    Hex lastHoveredHex;
    public GameObject buildMenuUi;
    public GameObject selectedBuilding;

    public List<GameObject> buildingList = new List<GameObject>();
    void Awake()
    {
        instance = this;
    }
    public void ToggleBuildMode()
    {
        buildModeActive = !buildModeActive;
        buildMenuUi.SetActive(!buildMenuUi.activeSelf);
        OnBuildModeChanged?.Invoke(buildModeActive);

        if (!buildModeActive)
        {
            selectedBuilding = null;
            ClearTileSelectOutline();
        }
    }

    public void SelectBuilding(int buildingID)
    {
        Debug.Log($"building id: {buildingList[buildingID]}");
        Debug.Log($"cost: {buildingList[buildingID].GetComponent<Building>().buildingData.cost}");
        if(ResourceManager.Instance.RemoveResource(ResourceType.Food, buildingList[buildingID].GetComponent<Building>().buildingData.cost))
        {
            selectedBuilding = Instantiate(buildingList[buildingID], lastHoveredHex.transform.position, quaternion.identity);
        }
            
    }
    
    void Update()
    {

        if (!buildModeActive) return;

        if (Input.GetMouseButtonDown(0) && selectedBuilding)
        {
            Debug.Log("BUILDING PLACED");
            ToggleBuildMode();
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.transform.CompareTag("Tile"))
            {
                if (hit.transform.GetComponent<Hex>() != lastHoveredHex)
                {
                    ClearTileSelectOutline();
                    hit.transform.gameObject.GetComponent<Hex>().ToggleSelectOutline(true);
                    lastHoveredHex = hit.transform.gameObject.GetComponent<Hex>();
                    if (selectedBuilding)
                        selectedBuilding.transform.position = lastHoveredHex.transform.position;
                }
               
            }
        }
    }

    void ClearTileSelectOutline()
    {
        if (lastHoveredHex)
        {
            lastHoveredHex.ToggleSelectOutline(false);
            lastHoveredHex = null;
        }
    }
    
    
}
