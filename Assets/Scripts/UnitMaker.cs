using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class UnitMaker : MonoBehaviour
{
    public List<GameObject> unitPrefabs = new List<GameObject>();
    public int playerID = 0;

    public GameObject UIUnitPanel;
    public GameObject UnitUIObject;
    public UnitUIScript UnitUIScript;

    public GameObject spawnNode;

    public void Awake()
    {
        UnitUIObject = GameObject.Find("UnitSelectionCanvas");
        UnitUIScript = UnitUIObject.GetComponent<UnitUIScript>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void makeUnit(int id)
    {
            if (ResourceManager.Instance.RemoveResource(ResourceType.Food, 10))
            {
                GameObject unitObj = Instantiate(unitPrefabs[id], spawnNode.transform.position, quaternion.identity);
                Unit unit = unitObj.GetComponent<Unit>();
                if (unit != null)
                {
                    unit.playerOwnerID = playerID;

                    //Assign and creates an additional UI Panel

                    //CREATES AND ASSIGNS UI PANELS FOR NEW UNIT
                    GameObject UIGridParent =  UnitUIScript.Grid; //Grid - to let the uipane
                    GameObject newUIPanel = Instantiate(UIUnitPanel, UIGridParent.transform);
                    UnitPanelScript newUnitPanelScript = newUIPanel.GetComponent<UnitPanelScript>();

                    unit.UnitPanelScript = newUnitPanelScript; //Assigns the new unit to the new panel
                    newUnitPanelScript.MatchingUnit = unitObj; //Assigns the new panel to the new unit

                    newUnitPanelScript.UnitImage.GetComponent<Image>().sprite = unit.unitData.icon;

                    unit.UnitPanelScript = newUnitPanelScript;


                }
            }
            else
            {
                Debug.Log("Not enough food");
            }

    }

    public void makeUnit(GameObject UnitToMake, Vector3 spawnPoint)//Overload makeUnit for Specialization Buildings
    {
        Debug.Log("IM here");
        GameObject unitObj = Instantiate(UnitToMake, spawnPoint, quaternion.identity);
        Unit unit = unitObj.GetComponent<Unit>();
        if (unit != null)
        {
            unit.playerOwnerID = playerID;
            //Assign and creates an additional UI Panel
            //CREATES AND ASSIGNS UI PANELS FOR NEW UNIT
            GameObject UIGridParent =  UnitUIScript.Grid; //Grid - to let the uipane
            GameObject newUIPanel = Instantiate(UIUnitPanel,UIGridParent.transform);
            UnitPanelScript newUnitPanelScript = newUIPanel.GetComponent<UnitPanelScript>();
            unit.UnitPanelScript = newUnitPanelScript; //Assigns the new unit to the new panel
            newUnitPanelScript.MatchingUnit = unitObj; //Assigns the new panel to the new unit
            newUnitPanelScript.UnitImage.GetComponent<Image>().sprite = unit.unitData.icon;
            unit.UnitPanelScript = newUnitPanelScript;
        }
            
    }





}
