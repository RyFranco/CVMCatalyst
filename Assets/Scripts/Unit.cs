
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public enum ActionState
{
    Idle,
    MoveToHarvest,
    Harvesting,
    Depositing,
    Attacking,
    Dead
}

[RequireComponent(typeof(NavMeshAgent))]



public class Unit : MonoBehaviour
{
    public NavMeshAgent agent;
    [SerializeField] private GameObject selectionSprite;
    public UnitData unitData;
    [SerializeField] public int currentHealth;

    bool isSelected = false;

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

    public bool IsInventoryFull() => currentInventory >= MaxInventory;

    protected UnitActionController actionController;
    public ActionState currentState => actionController.currentState;
    public Unit currentAttackTarget => actionController.currentAttackTarget;
    public Building currentBuildingTarget => actionController.currentBuildingTarget;

    public virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        actionController = GetComponent<UnitActionController>();
        currentHealth = unitData.maxHealth;
        agent.speed = unitData.moveSpeed;
        RegisterInSelectionManager();
    }

    protected virtual void RegisterInSelectionManager()
    {
        SelectionManager.Instance.AvailableUnits.Add(this);
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


    public void UpdateAnimation()
    {
        if(!animator) return;

        //animator.SetBool("IsMoving", state == ActionState.MoveToHarvest || state == ActionState.Depositing);
        animator.SetBool("IsHarvesting", currentState == ActionState.Harvesting);
        animator.SetBool("IsAttacking", currentState == ActionState.Attacking);
    }

    public void OnActionStateChanged(ActionState state)
    {
        UnitPanelScript?.UpdateBackground(state);
    }

    public void StartHarvesting(ResourceTile tile)
    {
        actionController.StartHarvesting(tile);
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
        isSelected = false;
        selectionSprite.SetActive(false);
    }



    public void AddResource(ResourceType type, int amount)
    {
        if (currentInventory + amount > MaxInventory) return;


        if (!carriedResources.ContainsKey(type))
            carriedResources[type] = 0;

        carriedResources[type] += amount;
        currentInventory += amount;


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



    public void DepositResources(GameObject townHall)
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
        actionController.ResetAction();
        agent.isStopped = false;
    }

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


    public virtual void Heal(int amount)
    {
        if (UnitPanelScript) UnitPanelScript.UpdateHealthIndicator(currentHealth / (float)unitData.maxHealth);
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
        if (!currentAttackTarget) LookForEnemy();

        currentHealth -= amount;
        if (UnitPanelScript) UnitPanelScript.UpdateHealthIndicator(currentHealth / (float)unitData.maxHealth);
        Debug.Log($"{name} took {amount} damage and has {currentHealth} remaining");
        if (currentHealth <= 0)
        {
            Debug.Log($"{name} should have died");
            currentHealth = 0;
            Die();
        }
    }

    void LookForEnemy()
    {

        Debug.Log("HIHIHI");
        //Finds all units, then removes units on same team
        List<GameObject> AllUnitsList = GameObject.FindGameObjectsWithTag("Unit").ToList();
        List<GameObject> EnemyUnits = new List<GameObject>();

        foreach (var UnitinList in AllUnitsList)
        {
            if (UnitinList.GetComponent<Unit>().playerID != playerID)
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

}
