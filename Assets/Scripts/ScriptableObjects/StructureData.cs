using UnityEngine;


public enum BuildingType
{
    TownHall,
    UnityTraining,
    Farm
}
[CreateAssetMenu(fileName = "StructureData", menuName = "Scriptable Objects/StructureData")]
public class StructureData : ScriptableObject
{
    public string Name;
    public int MaxHealth;
    public int cost;
    [SerializeField] public BuildingType buildingType;
}
