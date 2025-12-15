using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SpecializeUnit : MonoBehaviour
{
    public float timeToTrain;
    public GameObject TrainToThisUnit;

    public GameObject trainingPositionNode;
    
    public bool isTraining;


    public void StartTraining(Unit unit)
    {
        if(isTraining) 
            return;

        unit.StopAllActions();
        unit.StartCoroutine(MoveToTrainingNode(unit));

    }

    public IEnumerator MoveToTrainingNode(Unit unit)
    {   
        Vector3 targetPos = trainingPositionNode.transform.position;
        unit.MoveTo(targetPos);

        while(Vector3.Distance(unit.transform.position, targetPos) > 1.5)
        {
            yield return null;
        }

        StartCoroutine(TrainingRoutine(unit));
        
    }

    public IEnumerator TrainingRoutine(Unit unit)
    {
        isTraining = true;

        unit.gameObject.SetActive(false);

        float timer = 0f;

        while(timer < timeToTrain)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        Instantiate(TrainToThisUnit, trainingPositionNode.transform.position, Quaternion.identity);

        SelectionManager.Instance.AvailableUnits.Remove(unit);
        SelectionManager.Instance.SelectedUnits.Remove(unit);
        Destroy(unit.gameObject);

        isTraining = false;
    }

}
