using UnityEngine;
using UnityEngine.AI;

public class UnitEnemyScript : Unit
{
    [Header("Enemy Variables")]
    public GameObject Zone;
    public NavMeshAgent NMA;

    public override void Awake()
    {
        SelectionManager.Instance.AvailableUnits.Add(this);
        agent = GetComponent<NavMeshAgent>();
        playerID = 1;
        currentHealth = unitData.maxHealth;
        Zone.transform.localScale = new Vector3(unitData.sightRange * 5, Zone.transform.localScale.y, unitData.sightRange * 5);
        NMA.speed = unitData.moveSpeed;
    }
    
    public override void Attack(Unit targetUnit = null, Building targetBuilding = null)
    {
        Debug.Log($"{name} is planning to attack.");

        if (targetUnit == null && targetBuilding == null) return;

        if (targetUnit != null)
        {
            Debug.Log($"{name} choose unit!");
        }
        else if (targetBuilding != null)
        {
            Debug.Log($"{name} choose building!");
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
}
