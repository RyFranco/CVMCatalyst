using UnityEngine;

public class Building : MonoBehaviour
{
    public StructureData buildingData;

    int currentHealth;
    public bool isSelected {get; private set;} = false;
    public GameObject UICanvas;
    public int playerID;

    void Start()
    {
        currentHealth = buildingData.MaxHealth;
    }


    public void Select()
    {
        isSelected = true;
        UICanvas.SetActive(true);    
    }

    public void Deselect()
    {
        isSelected = false;
        UICanvas.SetActive(false);
    }


  
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log($"{name} took {amount} damage. Remaining: {currentHealth}");

        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        Debug.Log($"{name} destroyed!");
        Destroy(gameObject);
    }
}
