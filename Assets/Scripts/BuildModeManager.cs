using System;
using System.Collections.Generic;
using Mono.Cecil;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildModeManager : MonoBehaviour
{
    public static BuildModeManager instance { get; private set; }
    public static event Action<bool> OnBuildModeChanged;
    bool buildModeActive = false;
    Hex lastHoveredHex;
    public GameObject buildMenuUi;
    public GameObject selectedBuilding;

    private int currentBuildingID;

    public List<GameObject> buildingList = new List<GameObject>();
    public List<GameObject> buildingListTransparent = new List<GameObject>();
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

        //if(ResourceManager.Instance.RemoveResource(ResourceType.Food, buildingList[buildingID].GetComponent<Building>().buildingData.cost))
        if( buildingList[buildingID].GetComponent<Building>().buildingData.cost <= ResourceManager.Instance.food)
        {
            selectedBuilding = Instantiate(buildingListTransparent[buildingID], lastHoveredHex.transform.position, quaternion.identity);
            currentBuildingID = buildingID;
        }
            
    }
    
    void Update()
    {

        if (!buildModeActive) return;


        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity ,LayerMask.GetMask("Ground")))
        {


            if (hit.transform.CompareTag("Tile"))
            {
               
                if (Input.GetMouseButtonDown(0) && selectedBuilding)
                {
                    Debug.Log("BUILDING PLACED");
                    Debug.Log(hit.transform.gameObject);
                    Destroy(hit.transform.gameObject); // destroy clicked tile



                    Instantiate(buildingList[currentBuildingID], lastHoveredHex.transform.position, quaternion.identity);
                    Destroy(selectedBuilding);

                    ToggleBuildMode();

                }



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
