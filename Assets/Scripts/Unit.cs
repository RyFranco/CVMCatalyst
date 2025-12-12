using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.BuiltIn.ShaderGraph;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public enum UnitState
{
    Idle,
    Gathering,
    Attacking,
    Dead
}

[RequireComponent(typeof(NavMeshAgent))]



public class Unit : MonoBehaviour
{
    private NavMeshAgent agent;
    [SerializeField]
    private SpriteRenderer selectionSprite;
    public UnitData unitData;
    int currentHealth;
    bool isSelected = false;
    GameObject selIndicator;

    public UnitState currentState { get; private set; } = UnitState.Idle;

    private ResourceTile currentTile;
    private Coroutine harvestingRoutine;

    private Coroutine attackRoutine;
    private Unit attackTarget;

    private ResourceTile lastHarvestTile;

    [Header("Inventory")]
    public int MaxInventory = 3;
    private int currentInventory = 0;

    private Dictionary<ResourceType, int> carriedResources = new Dictionary<ResourceType, int>();

    public int playerID;

    [Header("Harvest Progress UI")]
    public Canvas harvestBarCanvas;
    public Slider harvestBar;

    public UnitPanelScript UnitPanelScript  { get; set; } = null;

    public void Awake()
    {
        SelectionManager.Instance.AvailableUnits.Add(this);
        agent = GetComponent<NavMeshAgent>();
        currentHealth = unitData.maxHealth;
    }


    public void AddResource(ResourceType type, int amount)
    {
        if (currentInventory + amount > MaxInventory)
        {
            Debug.Log($"{name} inventory full! Can’t carry more.");
            return;
        }

        if (!carriedResources.ContainsKey(type))
            carriedResources[type] = 0;

        carriedResources[type] += amount;
        currentInventory += amount;

        Debug.Log($"{name} collected {amount} {type}. Now carrying {currentInventory}/{MaxInventory}");
    }

    public void Select()
    {
        isSelected = true;
        selIndicator.SetActive(true);

    }

    public void Deselect()
    {
        isSelected = false;
        selIndicator.SetActive(false);
    }

    public bool IsSelected()
    {
        return isSelected;
    }

    public bool isInventoryFull() => currentInventory >= MaxInventory;



    //HARVESTING
    public IEnumerator TryStartHarvesting(ResourceTile tile)
    {
        Debug.Log($"Trying start harvest at {tile}");
        if (!tile.TryAddWorker(this)){
            Debug.Log($"{name} cannot harvest — tile full!");
            yield break;
        }
        
        currentTile = tile;
        lastHarvestTile = tile;
        currentState = UnitState.Idle;

        var agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.isStopped = false;
        agent.SetDestination(tile.transform.position);
        while (Vector3.Distance(transform.position, tile.transform.position) > 1.5f)
            yield return null;

        harvestingRoutine = StartCoroutine(HarvestRoutine(tile));
    
        

    }

    private IEnumerator HarvestRoutine(ResourceTile tile)
    {
        currentState = UnitState.Gathering;
        var harvestTime = tile.baseResourceHarvestTime / unitData.harvestingSpeed;
        harvestBar.value = 0f;

        while (tile != null && !isInventoryFull() && currentState == UnitState.Gathering)
        {
            float elaspeTime = 0f;


            yield return new WaitForSeconds(tile.baseResourceHarvestTime / unitData.harvestingSpeed);
            tile.Harvest(this);
            if (isInventoryFull()) break;
        }

        StopAllActions();

        if (isInventoryFull())
        {
            GameObject townHall = FindNearestTownHall();
            StartCoroutine(DepositRoutine(townHall));
        }
    }

    private GameObject FindNearestTownHall()
    {
        GameObject[] allBuildings = GameObject.FindGameObjectsWithTag("Building");
        GameObject nearestTownHall = null;
        float minDist = float.MaxValue;

        foreach (var b in allBuildings)
        {
            Building building = b.GetComponent<Building>();
            if (building == null) continue;

            if (building.buildingData.buildingType != BuildingType.TownHall)
                continue;

            float dist = Vector3.Distance(transform.position, b.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearestTownHall = b;
            }
        }

        return nearestTownHall;
    }

    private IEnumerator DepositRoutine(GameObject townHall)
    {
        currentState = UnitState.Idle;

        var agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.SetDestination(townHall.transform.position);
        
        while(Vector3.Distance(transform.position, townHall.transform.position) > 5f) yield return null;

        DepositResources(townHall);

        // Resume harvesting
        if (lastHarvestTile != null)
        {
            agent.SetDestination(lastHarvestTile.transform.position);
            while (Vector3.Distance(transform.position, lastHarvestTile.transform.position) > 1.5f) yield return null;

            yield return new WaitForSeconds(0.2f);
            lastHarvestTile.RemoveWorker(this);

            if (lastHarvestTile.TryAddWorker(this))
            {
                harvestingRoutine = StartCoroutine(HarvestRoutine(lastHarvestTile));
            }
            else
            {
                Debug.LogWarning($"{name} couldn't re-register to {lastHarvestTile.name}");
            }
        }

    }

    private void DepositResources(GameObject townHall)
    {
        foreach (var kvp in carriedResources)
        {
            ResourceType type = kvp.Key;
            int amount = kvp.Value;
            ResourceManager.Instance.AddResource(type, amount);

        }
        
        carriedResources.Clear();
        currentInventory = 0;
    }

    public void StopAllActions()
    {
        if (harvestingRoutine != null)
        {
            StopCoroutine(harvestingRoutine);
            harvestingRoutine = null;
        }

        if (attackRoutine != null)
        {
            StopCoroutine(attackRoutine);
            attackRoutine = null;
        }

        attackTarget = null;
    
        if (currentTile != null)
        {
            currentTile.RemoveWorker(this);
            currentTile = null;
        }

        currentState = UnitState.Idle;
        var agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.isStopped = false;
    }

    public void Attack(Unit targetUnit = null, Building targetBuilding = null)
    {
        Debug.Log($"{name} is planning to attack.");
        StopAllActions();

        if (targetUnit == null && targetBuilding == null) return;

        currentState = UnitState.Attacking;

        if (targetUnit != null)
        {
            Debug.Log($"{name} choose unit!");
            attackRoutine = StartCoroutine(AttackRoutine_Unit(targetUnit));
        }
        else if (targetBuilding != null)
        {
            Debug.Log($"{name} choose building!");
            attackRoutine = StartCoroutine(AttackRoutine_Building(targetBuilding));   
        }
    }

    private IEnumerator AttackRoutine_Unit(Unit target)
    {
        var agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

        while (target != null && target.currentState != UnitState.Dead && currentState == UnitState.Attacking)
        {
            float dist = Vector3.Distance(transform.position, target.transform.position);

            if (dist <= unitData.attackRange)
            {
                agent.isStopped = true;
                target.Damage(unitData.attackDamage);
                yield return new WaitForSeconds(unitData.attacksPerSecond);
            }
            else
            {
                agent.isStopped = false;
                agent.SetDestination(target.transform.position);
                yield return null;
            }
        }

        currentState = UnitState.Idle;
        attackRoutine = null;
        agent.isStopped = false;
    }

    private IEnumerator AttackRoutine_Building(Building target)
    {
        var agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        Debug.Log($"{name} attacking building.");

        Collider buildingCollider = target.GetComponent<Collider>();

        while (target != null && currentState == UnitState.Attacking)
        {

            Vector3 closestPoint = buildingCollider.ClosestPoint(transform.position);
            float dist = Vector3.Distance(transform.position, closestPoint);

            if (dist <= unitData.attackRange)
            {
                Debug.Log($"Closed in!");
                agent.isStopped = true;
                target.TakeDamage(unitData.attackDamage);
                yield return new WaitForSeconds(unitData.attacksPerSecond);
            }
            else
            {
                agent.isStopped = false;
                agent.SetDestination(target.transform.position);
                yield return null;
            }
        }

        currentState = UnitState.Idle;
        attackRoutine = null;
        agent.isStopped = false;
    }

    public void Heal(int amount)
    {
        if ((amount + currentHealth) >= unitData.maxHealth)
        {
            currentHealth = unitData.maxHealth;
        }
        else
        {
            currentHealth += amount;
        }
    }

    public void Damage(int amount)
    {
        
        currentHealth -= amount;
        UnitPanelScript.UpdateHealthIndicator(currentHealth/unitData.maxHealth);
        Debug.Log($"{name} took {amount} damage and has {currentHealth} remaining");
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    private void Die()
    {
        currentState = UnitState.Dead;
        selIndicator.SetActive(false);
        Destroy(gameObject);
        Debug.Log($"{name} has died!");
    }
}
