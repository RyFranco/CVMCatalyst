using UnityEngine;

public enum UnitFaction
{
    Creature,
    Minion
}



[CreateAssetMenu(fileName = "UnitData", menuName = "Scriptable Objects/UnitData")]
public class UnitData : ScriptableObject
{

    [Header("General")]
    [SerializeField] public string unitName;
    [TextArea] [SerializeField] public string description;
    [SerializeField] public Sprite icon;
    [SerializeField] public UnitFaction faction;

    [Header("Stats")]
    [SerializeField] public int maxHealth = 100;
    [SerializeField] public int maxEnergy = 50;
    [SerializeField] public float moveSpeed = 5f;
    [SerializeField] public float sightRange = 10f;

    [Header("Combat")]
    [SerializeField] public float attackRange = 1.5f;
    [SerializeField] public int attackDamage = 10;
    [SerializeField] public float attacksPerSecond = 1f;

    [Header("Gathering")]
    [SerializeField] public float harvestingSpeed = 1f;



}
