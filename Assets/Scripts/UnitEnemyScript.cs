using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitEnemyScript : Unit
{
    [Header("Enemy Variables")]
    public List <Unit> Intruders;
    public GameObject Zone;
    public float AttackTimer;

    public override void Awake()
    {
        Zone.GetComponent<EnemyZoneScript>().Owner = gameObject;
        agent = GetComponent<NavMeshAgent>();
        playerID = 1;
        currentHealth = unitData.maxHealth;
        Zone.transform.localScale = new Vector3(unitData.sightRange * 5, Zone.transform.localScale.y, unitData.sightRange * 5);
        agent.speed = unitData.moveSpeed;
    }
    
    public override void Update()
    {

        if(AttackTimer > 0)
        {
            AttackTimer -= Time.deltaTime * unitData.attacksPerSecond;
        }

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

        HandleAggro();

    }

    void HandleAggro()
    {
        if(Intruders.Count > 0)
        {
            Unit nearestEnemy = null;
            float shortestDistance = 9999;
            foreach (Unit Enemy in Intruders)
            {
                if(Enemy == null){
                    Intruders.Remove(Enemy);
                    return;
                }

                if(Vector3.Distance(Enemy.gameObject.transform.position, gameObject.transform.position) < shortestDistance)
                {
                    nearestEnemy = Enemy;
                }

            }
            attackTarget = nearestEnemy;
        }
        else
        {
            attackTarget = null;
            currentState = UnitState.Idle;
        }

        if (attackTarget)//If there is a valid target
        {
            if(Vector3.Distance(transform.position, attackTarget.transform.position) <= unitData.attackRange -.25f) //if valid target is in range of attack
            {   
                agent.isStopped = true;
                if(AttackTimer <= 0)//If attack is ready
                {

                    AttackTimer = 1;
                    Debug.Log("TAKE THIS! ");
                    attackTarget.Damage(unitData.attackDamage);
                }
            }
            else
            {
                //moveCloser
                agent.isStopped = false;
                agent.SetDestination(attackTarget.transform.position);
            }
            Debug.Log(agent.isStopped);
        }



    }

    public void Attack(Unit targetUnit = null)
    {
        Debug.Log($"{name} is planning to attack.");
        currentState = UnitState.Attacking;
        if (targetUnit != null)
        {
            Debug.Log($"{name} choose unit!");
            StopCoroutine(AttackRoutine_Unit(targetUnit));
            attackRoutine = StartCoroutine(AttackRoutine_Unit(targetUnit));
        }

    }

    public override void Damage(int amount)
    {

        currentHealth -= amount;
        Debug.Log($"{name} took {amount} damage and has {currentHealth} remaining");
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    void Die()
    {
        // if (SelectionManager.Instance != null)
        // {
        //     SelectionManager.Instance.AvailableUnits.Remove(this);
        //     SelectionManager.Instance.SelectedUnits.Remove(this);
        // }

        // currentState = UnitState.Dead;
        // Destroy(UnitPanelScript.gameObject);

        Destroy(gameObject);  
        Debug.Log($"{name} has died!");
    }

    /*
    public void Heal(int amount)
    {
        if(UnitPanelScript) UnitPanelScript.UpdateHealthIndicator( ( (float)currentHealth) / ( (float)unitData.maxHealth) );
        if ((amount + currentHealth) >= unitData.maxHealth)
        {
            currentHealth = unitData.maxHealth;
        }
        else
        {
            currentHealth += amount;
            
        }
    }

    

    

    */

    //overwritten to prevent being selected
    public override void Select(){}
    public override void Deselect(){}

}
