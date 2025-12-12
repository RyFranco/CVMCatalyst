using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class UnitMaker : MonoBehaviour
{
    public List<GameObject> unitPrefabs = new List<GameObject>();
    public int playerID = 0;

    [SerializeField] GameObject UIUnitPanel;


    GameObject UnitUIObject;
    UnitUIScript UnitUIScript;

    public void Awake()
    {
        UnitUIObject = GameObject.Find("UnitSelectionCanvas");
        UnitUIScript = UnitUIObject.GetComponent<UnitUIScript>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void makeUnit(int id)
    {
        
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (ResourceManager.Instance.RemoveResource(ResourceType.Food, 10))
            {
                GameObject unitObj = Instantiate(unitPrefabs[id], hit.point, quaternion.identity);
                Unit unit = unitObj.GetComponent<Unit>();
                if (unit != null)
                {
                    unit.playerID = playerID;

                    //Assign and creates an additional UI Panel

                    //CREATES AND ASSIGNS UI PANELS FOR NEW UNIT
                    GameObject UIGridParent =  UnitUIScript.Grid; //Grid - to let the uipane
                    GameObject newUIPanel = Instantiate(UIUnitPanel,UIGridParent.transform);
                    UnitPanelScript newUnitPanelScript = newUIPanel.GetComponent<UnitPanelScript>();

                    unit.UnitPanelScript = newUnitPanelScript; //Assigns the new unit to the new panel
                    newUnitPanelScript.Unit = unitObj; //Assigns the new panel to the new unit

                    newUnitPanelScript.UnitImage.GetComponent<Image>().sprite = unit.unitData.icon;

                    unit.UnitPanelScript = newUnitPanelScript;


                }
            }
            else
            {
                Debug.Log("Not enough food");
            }
            
        }

    }



}
