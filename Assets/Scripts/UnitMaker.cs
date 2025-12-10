using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class UnitMaker : MonoBehaviour
{
    public List<GameObject> unitPrefabs = new List<GameObject>();
    public int playerID = 0;
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

                }
            }
            else
            {
                Debug.Log("Not enough food");
            }
            
        }

    }
}
