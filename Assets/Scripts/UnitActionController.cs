using UnityEngine;


public enum ActionState
{
    Idle,
    MoveToHarvest,
    Harvesting,
    Depositing,
    Attacking,
    Dead
}

public class UnitActionController : MonoBehaviour
{

    private Unit unit;
    private ResourceTile harvestTile;
    private float harvestTimer;
    public Unit currentAttackTarget {get; private set;} = null;
    public Building currentBuildingTarget {get; private set;} = null;
    private float attackCooldown;




    public ActionState currentState {get; private set;} = ActionState.Idle;

    void Awake()
    {
        unit = GetComponent<Unit>();
    }


    void FixedUpdate()
    {
        switch (currentState)
        {
            case ActionState.MoveToHarvest:
                UpdateMovingToHarvest();
                break;
            case ActionState.Harvesting:
                UpdateHarvesting();
                break;
            case ActionState.Attacking:
                UpdateAttacking();
                break;
            case ActionState.Depositing:
                UpdateDepositing();
                break;
        }
    }

    public void ResetAction()
    {
        if(harvestTile != null)
        {
            harvestTile.RemoveWorker(unit);
        }


        harvestTile = null;
        harvestTimer = 0;

        currentAttackTarget = null;
        currentBuildingTarget = null;

        if(currentState != ActionState.Attacking)
        {
            attackCooldown = 0;
        }

        ChangeState(ActionState.Idle);
    }

    private void ChangeState(ActionState newState)
    {
        if(currentState == newState) return;

        currentState = newState;
        unit.UpdateAnimation();
        unit.OnActionStateChanged(newState);
    }

    /////////////////////
    // HARVEST FUNCTIONS
    /////////////////////
    public void StartHarvesting(ResourceTile tile)
    {
        if (currentState == ActionState.Harvesting) return;

        harvestTile = tile;
        if (!tile.RequestWork(unit)) return;

        unit.MoveTo(tile.GetWorkerSpot(unit));

        ChangeState(ActionState.MoveToHarvest);

        harvestTimer = 0f;

    }

    private void UpdateMovingToHarvest()
    {
        if (harvestTile == null)
        {
            ChangeState(ActionState.Idle);
            return;
        }

        float dist = Vector3.Distance(unit.transform.position, harvestTile.GetWorkerSpot(unit));

        if (dist <= 1.5f)
        {
            ChangeState(ActionState.Harvesting);
            harvestTimer = 0;
        }
    }

    private void UpdateHarvesting()
    {
        if (harvestTile == null)
        {
            ChangeState(ActionState.Idle);
            return;
        }

        if (unit.IsInventoryFull())
        {
            unit.MoveTo(unit.FindNearestTownHall().transform.position);
            ChangeState(ActionState.Depositing);
            return;
        }

        harvestTimer += Time.deltaTime;
        float timeToHarvest = harvestTile.baseResourceHarvestTime / unit.harvestingSpeed;

        if (harvestTimer >= timeToHarvest)
        {
            harvestTimer = 0;

            harvestTile.Harvest(unit);

            if (unit.IsInventoryFull())
            {
                unit.MoveTo(unit.FindNearestTownHall().transform.position);
                ChangeState(ActionState.Depositing);
            }
        }
    }

    private void UpdateDepositing()
    {
        GameObject hall = unit.FindNearestTownHall();

        float dist = Vector3.Distance(unit.transform.position, hall.transform.position);

        if (dist <= 5f)
        {
            unit.DepositResources(hall);

            // Resume harvesting if still assigned
            if (harvestTile != null)
            {
                StartHarvesting(harvestTile);
            }
            else
            {
                ChangeState(ActionState.Idle);
            }
        }
    }

    /////////////////////
    /////////////////////


    /////////////////////
    // Attack Functions
    /////////////////////


    private void UpdateAttacking()
    {
        

        if (currentAttackTarget != null)
        {
            if (currentAttackTarget.currentState == ActionState.Dead)
            {
                ChangeState(ActionState.Idle);
                return;
            }

            float dist = Vector3.Distance(unit.transform.position, currentAttackTarget.transform.position);

            if (dist > unit.attackRange)
            {
                unit.agent.SetDestination(currentAttackTarget.transform.position);
                return;
            }
            else
            {
                unit.agent.isStopped = true;
                attackCooldown -= Time.deltaTime;
                if (attackCooldown <= 0f)
                {
                    currentAttackTarget.Damage(unit.attackDamage);
                    attackCooldown = 1f / unit.attacksPerSecond;
                }
                return;
            }

        }

        if (currentBuildingTarget != null)
        {
            float dist = Vector3.Distance(
                unit.transform.position,
                currentBuildingTarget.GetComponent<Collider>().ClosestPoint(unit.transform.position)
            );

            if (dist > unit.attackRange)
            {
                unit.agent.isStopped = false;
                unit.agent.SetDestination(currentBuildingTarget.transform.position);
                return;
            }
            else
            {
                unit.agent.isStopped = true;

                attackCooldown -= Time.deltaTime;
                if (attackCooldown <= 0f)
                {
                    currentBuildingTarget.TakeDamage(unit.attackDamage);
                    attackCooldown = 1f / unit.attacksPerSecond;
                }
                return;
            }
        }

        ChangeState(ActionState.Idle);
    }


    public void StartAttack(Unit target)
    {
        if (target == null) return;

        //if already attacking and attacking the same unit, return. Prevents spam click
        if(currentState == ActionState.Attacking && currentAttackTarget == target) return;

        currentAttackTarget = target; 
        currentBuildingTarget = null;

        unit.agent.isStopped = false;

        ChangeState(ActionState.Attacking);
    }

    public void StartAttack(Building target)
    {
        if (target == null) return;

         //if already attacking and attacking the same building, return. Prevents spam click
        if(currentState == ActionState.Attacking && currentBuildingTarget == target) return;

        currentBuildingTarget = target;
        currentAttackTarget = null;

        unit.agent.isStopped = false;

        ChangeState(ActionState.Attacking);
    }

    /////////////////////
    /////////////////////
    
    /////////////////////
    // Dead Function
    /////////////////////
   
    public void Dead()
    {
        ResetAction();
        ChangeState(ActionState.Dead);
    }

}
