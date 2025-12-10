using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ResourceUI : MonoBehaviour
{
    public TMP_Text  foodText, woodText, stoneText, metalText ;
    void OnEnable()
    {
        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.onResourceChange += UpdateUI;
        }
    }


    void OnDisable()
    {
        ResourceManager.Instance.onResourceChange -= UpdateUI;
    }
    

    private void UpdateUI()
    {
        foodText.text = ResourceManager.Instance.food.ToString();
        woodText.text = ResourceManager.Instance.wood.ToString();
        stoneText.text = ResourceManager.Instance.stone.ToString();
        metalText.text = ResourceManager.Instance.metal.ToString();
    }
}
