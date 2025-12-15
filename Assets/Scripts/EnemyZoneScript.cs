using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyZoneScript : MonoBehaviour
{
    public GameObject Owner;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Unit" && other.gameObject.GetComponent<Unit>()) //checks if collision is a valid unit SHOULD only get creatures
        {
            if(other.gameObject.GetComponent<Unit>().playerID == 0){
                Debug.Log(other.gameObject.name + " YOU'RE TOO CLOSE");
                Owner.GetComponent<UnitEnemyScript>().Intruders.Add(other.gameObject.GetComponent<Unit>());
            }
        }    
    }

    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Unit" && other.gameObject.GetComponent<Unit>()) //checks if collision is a valid unit SHOULD only get creatures
        {
            if(other.gameObject.GetComponent<Unit>().playerID == 0){
                Debug.Log(other.gameObject.name + " YOU'RE SAFE FOR NOW");
                Owner.GetComponent<UnitEnemyScript>().Intruders.Remove(other.gameObject.GetComponent<Unit>());
            }
        }   
    }

}
