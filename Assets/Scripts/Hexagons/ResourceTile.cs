using System.Collections.Generic;
using UnityEngine;

public class ResourceTile : MonoBehaviour
{

    [Header("Resource Settings")]
    [SerializeField] public int maxWorkers = 3;
    protected List<Unit> activeWorkers = new List<Unit>();
    public bool HasSpace => activeWorkers.Count < maxWorkers;


    [Header("Tile Settings")]
    [SerializeField] private ResourceType resourceType;
    [SerializeField] public float baseResourceHarvestTime = 2f; 
    [SerializeField] private int resourcePerCollect = 1; 




    public virtual bool TryAddWorker(Unit unit)
    {

        if (HasSpace && !activeWorkers.Contains(unit))
        {
            activeWorkers.Add(unit);
            return true;
        }
        return false;
    }

    public virtual void RemoveWorker(Unit unit)
    {
        if (activeWorkers.Contains(unit))
        {
            activeWorkers.Remove(unit);
        }
    }

   public void Harvest(Unit worker)
    {
        worker.AddResource(resourceType, 1);
    }

 
}
