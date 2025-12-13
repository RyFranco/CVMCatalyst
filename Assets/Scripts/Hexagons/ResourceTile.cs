using System.Collections.Generic;
using UnityEngine;

public class ResourceTile : MonoBehaviour
{

    [Header("Tile Settings")]
    [SerializeField] public int maxWorkers = 3;
    [SerializeField] public GameObject[] spots;

    private List<Unit> activeWorkers = new List<Unit>();
    private Queue<Unit> waitingWorkers = new Queue<Unit>();
    private Dictionary<Unit, int> workerSpots = new Dictionary<Unit, int>();
    public bool hasSpace => activeWorkers.Count < spots.Length;


    [Header("Resource Settings")]
    [SerializeField] private ResourceType resourceType;
    [SerializeField] public float baseResourceHarvestTime = 2f;
    [SerializeField] private int resourcePerCollect = 1;

    //returns true if they are allowed to work there, false is not. If full put in queue
    public bool RequestWork(Unit unit)
    {
        if (activeWorkers.Contains(unit))
        {
            return true;
        }

        if (waitingWorkers.Contains(unit))
        {
            return false;
        }

        if (hasSpace)
        {
            AddWorkerToSpot(unit);
            return true;
        }

        waitingWorkers.Enqueue(unit);
        return false;


    }

    private void AddWorkerToSpot(Unit unit)
    {
        int spotIndex = GetFreeSpotIndex();
        if (spotIndex == -1)
        {
            Debug.LogError("No free spot found but HasSpace was true :/");
            return;
        }

        activeWorkers.Add(unit);
        workerSpots[unit] = spotIndex;

        // Tell unit to walk to the spot
        unit.MoveTo(spots[spotIndex].transform.position);
    }

    //loop through all available spots 
    private int GetFreeSpotIndex()
    {
        for (int i = 0; i < spots.Length; i++)
        {
            if (!workerSpots.ContainsValue(i)) //is spot available
                return i;
        }
        return -1; // all spots are used
    }

     public void RemoveWorker(Unit unit)
    {
        if (activeWorkers.Remove(unit))
        {
            workerSpots.Remove(unit);
            TryAssignNextWorker();
        }
    }

     private void TryAssignNextWorker()
    {
        if (!hasSpace) return;
        if (waitingWorkers.Count == 0) return;

        Unit next = waitingWorkers.Dequeue();
        AddWorkerToSpot(next);
    }


    public void Harvest(Unit worker)
    {
        worker.AddResource(resourceType, 1);
    }

   public Vector3 GetWorkerSpot(Unit unit)
    {
        if (workerSpots.TryGetValue(unit, out int index))
            return spots[index].transform.position;

        return transform.position;
    }

}
