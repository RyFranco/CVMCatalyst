using System.Collections.Generic;
using System.Linq;
using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;

public class UnitUIScript : MonoBehaviour
{

    public bool MenuOpen;

    public GameObject Grid;

    [SerializeField] private TextMeshProUGUI FirstTextButton;
    [SerializeField] private GameObject MenuObject;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("p"))
        {
            List<GameObject> AllUnitsList = GameObject.FindGameObjectsWithTag("Unit").ToList();
            foreach( GameObject Unit in AllUnitsList)
            {
                Unit.GetComponent<Unit>().Damage(1);
            } 
        }
        if (Input.GetKeyDown("o"))
        {
            List<GameObject> AllUnitsList = GameObject.FindGameObjectsWithTag("Unit").ToList();
            foreach( GameObject Unit in AllUnitsList)
            {
                Unit.GetComponent<Unit>().Heal(1);
            } 
        }
    }

    public void MenuToggle()
    {
        if (MenuOpen)
        {
            MenuObject.transform.localPosition = new Vector2(0,-800);
            MenuOpen = false;
            FirstTextButton.text = "OPEN";
        }
        else
        {
            MenuObject.transform.localPosition = new Vector2(0,-500);
            MenuOpen = true;
            FirstTextButton.text = "CLOSE";
        }
    }



}
