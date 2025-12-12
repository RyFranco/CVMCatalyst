using System.Collections.Generic;
using UnityEngine.UI;

public class SelectionManager
{

    private static SelectionManager _instance;
    public HashSet<Unit> SelectedUnits = new HashSet<Unit>();
    public List<Unit> AvailableUnits = new List<Unit>();
    public static SelectionManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SelectionManager();
            }

            return _instance;
        }

        private set
        {
            _instance = value;
        }
    }

    private SelectionManager() { }

    public void Select(Unit Unit)
    {
        SelectedUnits.Add(Unit);
    }

    public void Deselect(Unit Unit)
    {
        SelectedUnits.Remove(Unit);
    }

    public void DeselectAll()
    {
        SelectedUnits.Clear();
    }
}
