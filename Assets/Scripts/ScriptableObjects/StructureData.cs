using UnityEngine;


public enum BuildingType
{
    TownHall,
    UnitTraining,
    Farm
}


[CreateAssetMenu(fileName = "StructureData", menuName = "Scriptable Objects/StructureData")]
public class StructureData : ScriptableObject
{
    public string Name;
    public int MaxHealth;
    public ResourceCost[] cost;
    [SerializeField] public BuildingType buildingType;
}
