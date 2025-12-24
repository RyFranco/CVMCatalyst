using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitEnemyScript : Unit
{
    [Header("Enemy Variables")]
    public List<Unit> Intruders;
    public GameObject Zone;

    public override void Awake()
    {
        base.Awake();
        Zone.GetComponent<EnemyZoneScript>().Owner = gameObject;
        playerID = 1;
        Zone.transform.localScale = new Vector3(unitData.sightRange * 5, Zone.transform.localScale.y, unitData.sightRange * 5);
    }



    public override void Update()
    {
        base.Update();
        HandleAggro();

    }

    void HandleAggro()
    {
        //remove all dead or non-existing units
        Intruders.RemoveAll(units => units == null || units.currentState == ActionState.Dead);

        //if no enemies nearby stop attacking
        if (Intruders.Count == 0)
        {
            //stop attacking if no enemies around
            if (currentState == ActionState.Attacking) StopAllActions();
            return;
        }

        //if we are already attacking target
        if(currentState == ActionState.Attacking && currentAttackTarget != null) return;

        Unit nearestEnemy = null;
        float shortestDistance = float.MaxValue;
        foreach (Unit Enemy in Intruders)
        {
            float dist = Vector3.Distance(Enemy.transform.position, transform.position);
            if (dist < shortestDistance)
            {
                shortestDistance = Vector3.Distance(Enemy.transform.position, transform.position);
                nearestEnemy = Enemy;
            }

        }

        Attack(nearestEnemy);

    }

    public override void Die()
    {
        
       //change state to dead, prevents other action like attacking from being done post mortem 
        actionController.Dead();

        //Destroy the panel associated with unit
        Destroy(gameObject);

    }

    //overwritten to prevent parent function from running
    public override void Select() { }
    public override void Deselect() { }
    protected override void RegisterInSelectionManager() { }



}
