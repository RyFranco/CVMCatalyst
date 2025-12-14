using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;


//Script that goes on each of the panel objects 

public class UnitPanelScript : MonoBehaviour 
{

    public GameObject Unit;

    public GameObject UnitImage;

    [SerializeField] private GameObject HPBar;
    [SerializeField] private GameObject CurrentHPIndicator;
    

    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {   

    }

    //When a player controlled unit takes damage, they should call this function on their given tile
    public void UpdateHealthIndicator(float percentHP) //Give this Method the PERCENTHP of given unit
    {
        if(percentHP >= 1)
        {
            CurrentHPIndicator.transform.localScale = new Vector3(percentHP,1,1);
            HPBar.SetActive(false);
        }
        else
        {
            HPBar.SetActive(true);
            CurrentHPIndicator.transform.localScale = new Vector3(percentHP,1,1);
        }
        

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
