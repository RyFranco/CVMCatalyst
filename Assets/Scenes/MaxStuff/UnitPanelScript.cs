using Microsoft.Unity.VisualStudio.Editor;
using Unity.VisualScripting;
using UnityEngine;

//Script that goes on each of the panel objects 

public class UnitPanelScript : MonoBehaviour 
{

    public GameObject MatchingUnit;

    public GameObject UnitImage;
    public UnityEngine.UI.Image IconBackground;

    [SerializeField] private GameObject HPBar;
    [SerializeField] private GameObject CurrentHPIndicator;
    
    float DoubleClickTime = 0.25f;

    float DoubleClickTimer;
    
    Color IdleColor =       new Color(1f,   200/255f,   130f/255f   );
    Color ActiveColor =    new Color(0f,   130f/255f,  1f          );
    Color CombatColor =     new Color(1f,   90f/255f,   50f/255f    );


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {   
        if(DoubleClickTimer > 0) DoubleClickTimer -= Time.deltaTime;
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
    
    public void Clicked()//Gets called when their panel is clicked with split functions for single/doubleclick
    {
        if(DoubleClickTimer <= 0) //Single Click
        {
            DoubleClickTimer = DoubleClickTime;
            SelectionManager.Instance.Select(MatchingUnit.GetComponent<Unit>());
            HighlightPanel();

        }
        else if(DoubleClickTimer> 0) //Double Click
        {
            GameObject.Find("Main Camera").GetComponent<CameraMovement>().FocusOnObject(MatchingUnit.gameObject);//Focus the camera on object
            DoubleClickTimer = 0;
        }
    }

    public void UpdateBackground(UnitState UnitState)
    {
        switch (UnitState)
        {
            case UnitState.Idle:
                IconBackground.color = IdleColor;
                break;
            case UnitState.Gathering:
                IconBackground.color = ActiveColor;
                break;

            case UnitState.MovingToAction:
                IconBackground.color = ActiveColor;
                break;
            case UnitState.Attacking:
                IconBackground.color = CombatColor;
                break;

        }
            
    }

    void HighlightPanel()
    {
        
    }

    void DeHighlightPanel()
    {
        
    }


}
