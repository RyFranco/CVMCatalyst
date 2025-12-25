
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


[RequireComponent(typeof(NavMeshAgent))]



public class Unit : MonoBehaviour
{
    public UnitData unitData;

    [Header("General")]
    public string unitName { get; private set; }
    public string description { get; private set; }
    public Sprite unitPortrait { get; private set; }
    public UnitFaction faction { get; private set; }
    public int playerOwnerID = 0;

    [SerializeField] GameObject selectionIndicatorSprite; //Sprite that appears underneath unit to represent it being highlighted
    public Animator animator;
    public UnitPanelScript UnitPanelScript;
    bool isSelected = false;



    [Header("Unit Pathing")]
    public NavMeshAgent agent;

    [Header("Stats")]
    public int maxHealth { get; private set; }
    public int maxEnergy { get; private set; }
    public float moveSpeed { get; private set; }
    public float fogSightRange { get; private set; }


    [Header("Combat")]
    public float attackRange { get; private set; }
     public int attackDamage { get; private set; }
    public float attacksPerSecond { get; private set; }
    public int currentHealth { get; private set; }
    public int currentEnergy { get; private set; }


    [Header("Gathering")]
    public float harvestingSpeed { get; private set; }
    public int maxInventory { get; private set; }
    public int currentInventory { get; private set; }
    public ResourceTile currentHarvestTile;
    public Dictionary<ResourceType, int> carriedResources = new Dictionary<ResourceType, int>();
    public bool IsInventoryFull() => currentInventory >= maxInventory;




    //Action controller
    protected UnitActionController actionController;
    public ActionState currentState => actionController.currentState;
    public Unit currentAttackTarget => actionController.currentAttackTarget;
    public Building currentBuildingTarget => actionController.currentBuildingTarget;



    #region On Start / Update

    public virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        actionController = GetComponent<UnitActionController>();
        RegisterInSelectionManager();
        CopyFromUnitData();
    }

    //place unit in list in RegisterManager 
    protected virtual void RegisterInSelectionManager()
    {
        SelectionManager.Instance.AvailableUnits.Add(this);
    }

    //copies data from unitData to local variables
    //needed since stats can change
    public void CopyFromUnitData()
    {
        if (unitData == null)
        {
            Debug.LogError($"{name} has no UnitData assigned!");
            return;
        }

        // General
        unitName = unitData.unitName;
        description = unitData.description;
        unitPortrait = unitData.icon;
        faction = unitData.faction;

        // Stats
        maxHealth = unitData.maxHealth;
        maxEnergy = unitData.maxEnergy;
        moveSpeed = unitData.moveSpeed;
        agent.speed = unitData.moveSpeed;
        fogSightRange = unitData.sightRange;

        // Combat
        attackRange = unitData.attackRange;
        attackDamage = unitData.attackDamage;
        attacksPerSecond = unitData.attacksPerSecond;

        // Gathering
        harvestingSpeed = unitData.harvestingSpeed;
        maxInventory = unitData.maxInventory;


        currentHealth = maxHealth;
        currentEnergy = 0;
    }



    public virtual void Update()
    {
        if (agent.velocity.x > .5f)
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
            if (currentAttackTarget)
            {
                float Direction = currentAttackTarget.transform.position.x - gameObject.transform.position.x;
                if (Direction < 0) Direction = -1; else Direction = 1;

                Quaternion newRotation = transform.rotation;
                newRotation.y = 180 * Direction;
                transform.rotation = newRotation;
            }
        }



        if (animator) animator.SetFloat("Speed", math.abs(agent.velocity.magnitude));
    }

    #endregion

    #region Animation / State change

    //change animation based on unit's action
    public void UpdateAnimation()
    {
        if (!animator) return;

        //animator.SetBool("IsMoving", state == ActionState.MoveToHarvest || state == ActionState.Depositing);
        animator.SetBool("IsHarvesting", currentState == ActionState.Harvesting);
        animator.SetBool("IsAttacking", currentState == ActionState.Attacking);
    }

    public void OnActionStateChanged(ActionState state)
    {
        UnitPanelScript?.UpdateBackground(state);
    }

    public void StopAllActions()
    {
        actionController.ResetAction();
        agent.isStopped = false;
    }
    #endregion

    #region basic functionality
    public virtual void Heal(int amount)
    {
        if (UnitPanelScript) UnitPanelScript.UpdateHealthIndicator(currentHealth / (float)maxHealth);
        if ((amount + currentHealth) >= maxHealth)
        {
            currentHealth = maxHealth;
        }
        else
        {
            currentHealth += amount;

        }
    }

    public virtual void Damage(int amount)
    {
        if (!currentAttackTarget) LookForEnemy();

        currentHealth -= amount;
        if (UnitPanelScript) UnitPanelScript.UpdateHealthIndicator(currentHealth / (float)maxHealth);
        Debug.Log($"{name} took {amount} damage and has {currentHealth} remaining");
        if (currentHealth <= 0)
        {
            Debug.Log($"{name} should have died");
            currentHealth = 0;
            Die();
        }
    }


    public virtual void Die()
    {
        //remove from unit pool in selectionManager
        SelectionManager.Instance.AvailableUnits.Remove(this);
        SelectionManager.Instance.SelectedUnits.Remove(this);

        //change state to dead, prevents other action like attacking from being done post mortem 
        actionController.Dead();

        //Destroy the panel associated with unit
        if (UnitPanelScript.gameObject)
        {
            Destroy(UnitPanelScript.gameObject);
        }
        Destroy(gameObject);

    }

    public void MoveTo(Vector3 pos)
    {
        agent.SetDestination(pos);
    }

    public virtual void Select()
    {
        isSelected = true;
        selectionIndicatorSprite.SetActive(true);

    }

    public virtual void Deselect()
    {
        isSelected = false;
        selectionIndicatorSprite.SetActive(false);
    }
    #endregion

    #region Harvesting Logic



    public void StartHarvesting(ResourceTile tile)
    {
        actionController.StartHarvesting(tile);
    }


    public GameObject FindNearestTownHall()
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

    //add resource to unit's inventory
    public void AddResourceToUnitInventory(ResourceType type, int amount)
    {
        //stop if no inventory space
        if (currentInventory + amount > maxInventory) return;

        //if not carrying that material, add key
        if (!carriedResources.ContainsKey(type))
            carriedResources[type] = 0;

        carriedResources[type] += amount;
        currentInventory += amount;


    }

    public void DepositResources(GameObject townHall)
    {
        foreach (var kvp in carriedResources)
        {
            ResourceType type = kvp.Key;
            int amount = kvp.Value;
            ResourceManager.Instance.AddResource(type, amount);

        }

        //reset inventory
        carriedResources.Clear();
        currentInventory = 0;
    }

    #endregion

    #region Attacking Logic
    public void Attack(Unit targetUnit = null, Building targetBuilding = null)
    {
        StopAllActions();

        if (targetUnit != null)
        {
            actionController.StartAttack(targetUnit);
            return;
        }

        if (targetBuilding != null)
        {
            actionController.StartAttack(targetBuilding);
            return;
        }
    }
    void LookForEnemy()
    {

        //Finds all units, then removes units on same team
        List<GameObject> AllUnitsList = GameObject.FindGameObjectsWithTag("Unit").ToList();
        List<GameObject> EnemyUnits = new List<GameObject>();

        foreach (var UnitinList in AllUnitsList)
        {
            if (UnitinList.GetComponent<Unit>().playerOwnerID != playerOwnerID)
            {
                EnemyUnits.Add(UnitinList);
            }
        }

        //finds the nearest enemy
        GameObject nearestEnemy = null;
        float NearestEnemyDistance = 9999;

        foreach (GameObject Enemy in EnemyUnits)
        {
            float currentDistance = Vector3.Distance(transform.position, Enemy.transform.position);
            if (currentDistance < NearestEnemyDistance && currentDistance <= fogSightRange)
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

    #endregion


}
