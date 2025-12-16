using System;
using Unity.Mathematics;
using UnityEngine;


public enum ResourceType
{
    Food,
    Wood,
    Stone,
    Metal
}

[System.Serializable]
public struct ResourceCost
{
    public ResourceType resource;
    public int amount;
}

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }
    public event Action onResourceChange;
    public int food;
    public int wood;
    public int stone;
    public int metal;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

    }
    public void AddResource(ResourceType type, int amount)
    {
        switch (type)
        {
            case ResourceType.Food: food += amount; break;
            case ResourceType.Wood: wood += amount; break;
            case ResourceType.Stone: stone += amount; break;
            case ResourceType.Metal: metal += amount; break;
        }

        onResourceChange?.Invoke();
    }

    public bool RemoveResource(ResourceType type, int amount)
    {
        bool isEnough = false;
        switch (type)
        {
            case ResourceType.Food: if (food >= amount) { food -= amount; isEnough = true; } break;
            case ResourceType.Wood: if (wood >= amount) { wood -= amount; isEnough = true; } break;
            case ResourceType.Stone: if (stone >= amount) { stone -= amount; isEnough = true; } break;
            case ResourceType.Metal: if (metal >= amount) { metal -= amount; isEnough = true; } break;
        }

        if (isEnough) onResourceChange?.Invoke();

        return isEnough;
    }

    public bool RemoveResources(ResourceCost[] costs)
    {
        foreach(var cost in costs)
        {
            if (!hasEnoughResources(costs)){
                return false;
            }
        }

        foreach(var cost in costs)
        {
            RemoveResource(cost.resource, cost.amount);
        }

        onResourceChange?.Invoke();

        return true;
    }

    public bool hasEnoughResources(ResourceCost[] costs)
    {
        foreach (var cost in costs)
        {
            switch (cost.resource)
            {
                case ResourceType.Food:
                    if(food < cost.amount) return false;
                    break;
                case ResourceType.Wood:
                    if(wood < cost.amount) return false;
                    break;
                case ResourceType.Stone:
                    if(stone < cost.amount) return false;
                    break;
                case ResourceType.Metal:
                    if(metal < cost.amount) return false;
                    break;
            }
        }
        return true;
    }
    

}
