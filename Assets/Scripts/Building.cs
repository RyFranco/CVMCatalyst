using UnityEngine;

public class Building : MonoBehaviour
{
    public StructureData buildingData;

    int currentHealth;
    bool isSelected = false;
    GameObject selIndicator;
    public int playerID;

    //public UnitSelectionManager USM;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        selIndicator = transform.GetChild(0).gameObject;
        //Camera.main.GetComponent<UnitSelectionManager>().AvailableUnits.Add(this);

        currentHealth = buildingData.MaxHealth;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Select()
    {
        isSelected = true;
        selIndicator.SetActive(true);
        Debug.Log("Building Selected");
        
    }

    public void Deselect()
    {
        isSelected = false;
        selIndicator.SetActive(false);
        Debug.Log("Building Deselected");
    }

    public bool IsSelected()
    {
        return isSelected;
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
