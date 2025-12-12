using UnityEngine;


//Script that goes on each of the panel objects 

public class UnitPanelScript : MonoBehaviour 
{

    public GameObject Unit;

    [SerializeField] private GameObject HPBar;
    [SerializeField] private GameObject CurrentHPIndicator;
    

    public float FakeHP;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHealthIndicator(FakeHP);
    }

    public void UpdateHealthIndicator(float percentHP) //Give this Method the PERCENTHP of given unit
    {
        if(percentHP >= 1)
        {
            HPBar.SetActive(false);
        }
        else
        {
            HPBar.SetActive(true);
            CurrentHPIndicator.transform.localScale = new Vector3(percentHP,1,1);
        }

        // IF YOU WANT HEALTHBAR TO ALWAYS BE ACTIVE
        //CurrentHPIndicator.transform.localScale = new Vector3(percentHP,1,1);

    }


}
