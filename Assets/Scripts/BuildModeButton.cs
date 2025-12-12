using UnityEngine;

public class BuildModeButton : MonoBehaviour
{
    public void OnClick()
    {
        BuildModeManager.instance.ToggleBuildMode();
    }

    public void buildingSelected(int id)
    {
        BuildModeManager.instance.SelectBuilding(id);
    }
        
        
}
