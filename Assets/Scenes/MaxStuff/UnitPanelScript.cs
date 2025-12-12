using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;


//Script that goes on each of the panel objects 

public class UnitPanelScript : MonoBehaviour 
{

    public GameObject Unit;

    public GameObject UnitImage;

    public float FakeHP; //Placeholder for hp DELETE LATER 

    [SerializeField] private GameObject HPBar;
    [SerializeField] private GameObject CurrentHPIndicator;
    

    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHealthIndicator(FakeHP); //Placeholder for hp DELETE LATER 
    }

    //When a player controlled unit takes damage, they should call this function on their given tile
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

        // IF YOU WANT HEALTHBAR TO ALWAYS BE ACTIVE REMOVE THE IF STATEMENT ABOVE AND ALLOW THIS BELOW
        //CurrentHPIndicator.transform.localScale = new Vector3(percentHP,1,1);

    }
    
    public void Selected()
    {
        /* Will run when portrait is clicked
        move camera to unit
        select unit
        play sound? */
        Debug.Log(Unit.name);
    }


}
