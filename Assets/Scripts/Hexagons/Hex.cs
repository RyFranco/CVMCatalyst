using UnityEngine;

public class Hex : MonoBehaviour
{
    private GameObject selectOutline;
    private GameObject gridOutline;

    void Start()
    {
        selectOutline = transform.GetChild(0).gameObject;
        gridOutline = transform.GetChild(1).gameObject;
    }

    void OnEnable()
    {
        BuildModeManager.OnBuildModeChanged += ToggleOutline;
    }

    void OnDisable()
    {
        BuildModeManager.OnBuildModeChanged -= ToggleOutline;
    }


    void ToggleOutline(bool isActive)
    {
        gridOutline.SetActive(isActive);
    }
    
    public void ToggleSelectOutline(bool isActive)
    {
        selectOutline.SetActive(isActive); 
    }
}
