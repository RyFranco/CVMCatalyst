using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEditor;
using UnityEditor.Rendering.BuiltIn.ShaderGraph;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public enum UnitState
{
    Idle,
    Gathering,
    Attacking,
    MovingToAction,
    Dead
}

[RequireComponent(typeof(NavMeshAgent))]



public class Unit : MonoBehaviour
{
    protected NavMeshAgent agent;
    [SerializeField]
    private GameObject selectionSprite;
    public UnitData unitData;
    [SerializeField] private protected int currentHealth;

    bool isSelected = false;
    public UnitState currentState = UnitState.Idle;
    private ResourceTile currentTile;
    private Coroutine harvestingRoutine;

    [SerializeField] protected Coroutine attackRoutine;
    [SerializeField] protected Unit attackTarget;

    private ResourceTile lastHarvestTile;

    [Header("Inventory")]
    public int MaxInventory = 3;
    private int currentInventory = 0;
    public Dictionary<ResourceType, int> carriedResources = new Dictionary<ResourceType, int>();
    public int playerID;

    [Header("Harvest Progress UI")]
    [HideInInspector] public Canvas harvestBarCanvas;
    [HideInInspector] public Slider harvestBar;
    [HideInInspector] public UnitPanelScript UnitPanelScript;
    [HideInInspector] public Animator animator;

    public virtual void Awake()
    {
        SelectionManager.Instance.AvailableUnits.Add(this);
        agent = GetComponent<NavMeshAgent>();
        currentHealth = unitData.maxHealth;
        agent.speed = unitData.moveSpeed;
        Debug.Log(selectionSprite);
    }

    public virtual void Update()
    {
        if(agent.velocity.x > .5f)
        {
            Quaternion newRotation = transform.rotation;
            newRotation.y = 0;
            transform.rotation = newRotation;
        }
        else if (agent.velocity.x < -.5f)
        {
            Quaternion newRotation = transform.rotation;
            newRotation.y = 180;
            transform.rotation = newRotation;
        }
        else
        {
            if (attackTarget)
            {
                float Direction = attackTarget.transform.position.x - gameObject.transform.position.x;
                if(Direction < 0) Direction = -1; else Direction = 1;

                Quaternion newRotation = transform.rotation;
                newRotation.y = 180 * Direction;
                transform.rotation = newRotation;
            }
        }

        

        if(animator) animator.SetFloat("Speed", math.abs(agent.velocity.magnitude));
        if(UnitPanelScript) UnitPanelScript.UpdateBackground(currentState);

        switch (currentState)//Sets Animation based on Unit State
        {
            case UnitState.Idle:
                    animator.SetBool("IsHarvesting", false);
                    animator.SetBool("IsAttacking",false);
                    
                break;
            case UnitState.Gathering:
                    animator.SetBool("IsHarvesting", true);
                    animator.SetBool("IsAttacking",false);
                break;
            case UnitState.Attacking:
                    animator.SetBool("IsHarvesting", false);
                    animator.SetBool("IsAttacking", true);
                break;
            case UnitState.MovingToAction:
                    animator.SetBool("IsHarvesting", false);
                    animator.SetBool("IsAttacking", false);
                break;
            default:
                Debug.Log("INVALID UNIT STATE");
                break;

        }

    }

    public void MoveTo(Vector3 pos)
    {
        agent.SetDestination(pos);
        
    }

    public virtual void Select()
    {
        isSelected = true;
        selectionSprite.SetActive(true);

    }

    public virtual void Deselect()
    {
        Debug.Log(name + " Deselected");
        isSelected = false;
        selectionSprite.SetActive(false);
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


    public bool isInventoryFull() => currentInventory >= MaxInventory;



    //HARVESTING
    public IEnumerator TryStartHarvesting(ResourceTile tile)
    {
        //if alreadying harvesting leave
        if (currentState == UnitState.Gathering)
            yield break;


        Debug.Log($"Trying start harvest at {tile}");

        bool assigned = tile.RequestWork(this);

        if (!assigned)
        {
            Debug.Log($"{name} waiting for a free spot");
            yield break;
        }

        //We have a spot and now assign it
        currentTile = tile;
        lastHarvestTile = tile;
        currentState = UnitState.MovingToAction;


        Vector3 target = tile.GetWorkerSpot(this);
        agent.isStopped = false;
        agent.SetDestination(target);

        while (Vector3.Distance(transform.position, target) > 1.5f)
            yield return null;


        currentState = UnitState.Gathering;
        harvestingRoutine = StartCoroutine(HarvestRoutine(tile));



    }

    private IEnumerator HarvestRoutine(ResourceTile tile)
    {
        currentState = UnitState.Gathering;
        //harvestBar.value = 0f; // STILL NEEDED TO IMPLEMENT BAR


        while (tile != null && !isInventoryFull() && currentState == UnitState.Gathering)
        {

            float harvestTime = tile.baseResourceHarvestTime / unitData.harvestingSpeed;
            Debug.Log(harvestTime);
            float timer = 0f;


            while (timer < harvestTime)
            {
                if (currentState != UnitState.Gathering || tile == null)
                    yield break;

                timer += Time.deltaTime;
                //harvestBar.value = timer / harvestTime;

                yield return null;
            }


            tile.Harvest(this);

            if (isInventoryFull()) break;
        }

        StopAllActions();
        tile.RemoveWorker(this);

        if (isInventoryFull())
        {
            StartCoroutine(DepositRoutine(FindNearestTownHall()));
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

    private protected IEnumerator DepositRoutine(GameObject townHall)
    {
        currentState = UnitState.MovingToAction;

        //move to townhall
        agent.isStopped = false;
        agent.SetDestination(townHall.transform.position);

        //wait till near twon hall
        while (Vector3.Distance(transform.position, townHall.transform.position) > 5f) yield return null;

        //deposit resources
        DepositResources(townHall);

        // Resume harvesting
        if (lastHarvestTile != null)
        {
            bool assigned = lastHarvestTile.RequestWork(this);

            if (!assigned)
            {
                Debug.Log($"{name} waiting for open spot at {lastHarvestTile.name}");
                yield break;
            }

            //worker assigned immediately, go to harvesting spot
            StartCoroutine(TryStartHarvesting(lastHarvestTile));
        }

    }

    private protected void DepositResources(GameObject townHall)
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
        agent.isStopped = false;
    }

    public virtual void Attack(Unit targetUnit = null, Building targetBuilding = null)
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

    protected IEnumerator AttackRoutine_Unit(Unit target)
    {
        while (target != null && target.currentState != UnitState.Dead && currentState == UnitState.Attacking)
        {
            float dist = Vector3.Distance(transform.position, target.transform.position);

            if (dist <= unitData.attackRange)
            {
                agent.isStopped = true;
                target.Damage(unitData.attackDamage);
                yield return new WaitForSeconds(1/unitData.attacksPerSecond);
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

    private protected IEnumerator AttackRoutine_Building(Building target)
    {
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
                yield return new WaitForSeconds(1/unitData.attacksPerSecond);
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

    public virtual void Heal(int amount)
    {
        if(UnitPanelScript) UnitPanelScript.UpdateHealthIndicator(currentHealth/(float)unitData.maxHealth);
        if ((amount + currentHealth) >= unitData.maxHealth)
        {
            currentHealth = unitData.maxHealth;
        }
        else
        {
            currentHealth += amount;
            
        }
    }

    public virtual void Damage(int amount)
    {
        if (!attackTarget && currentState == UnitState.Idle) LookForEnemy();

        currentHealth -= amount;
        if(UnitPanelScript) UnitPanelScript.UpdateHealthIndicator(currentHealth/(float)unitData.maxHealth);
        Debug.Log($"{name} took {amount} damage and has {currentHealth} remaining");
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    void LookForEnemy()
    {
        //Finds all units, then removes units on same team
        List<GameObject> AllUnitsList = GameObject.FindGameObjectsWithTag("Unit").ToList();
        List<GameObject> EnemyUnits = new List<GameObject>();

        foreach (var UnitinList in AllUnitsList)
        {
            if(UnitinList.GetComponent<Unit>().playerID != playerID)
            {
                EnemyUnits.Add(UnitinList);
            }
        }

        //finds the nearest enemy
        GameObject nearestEnemy = null;
        float NearestEnemyDistance = 9999;

        foreach(GameObject Enemy in EnemyUnits)
        {
            float currentDistance = Vector3.Distance(transform.position, Enemy.transform.position);
            if (currentDistance < NearestEnemyDistance && currentDistance <= unitData.sightRange)
            {
                NearestEnemyDistance = currentDistance;
                nearestEnemy = Enemy;
            }
        }

        if (nearestEnemy)
        {
            StopAllActions();
            Attack(nearestEnemy.GetComponent<Unit>());
        }
        
    }

    void Die()
    {
        if (SelectionManager.Instance != null)
        {
            SelectionManager.Instance.AvailableUnits.Remove(this);
            SelectionManager.Instance.SelectedUnits.Remove(this);
        }

        currentState = UnitState.Dead;

        //foreach()

        //List <GameObject> EnemyList = GameObject.FindGameObjectsWithTag("Unit");
        RemovePanel();
        Destroy(gameObject);
        
        Debug.Log($"{name} has died!");
    }

    public void RemovePanel()
    {
        Destroy(UnitPanelScript.gameObject);
    }

}
