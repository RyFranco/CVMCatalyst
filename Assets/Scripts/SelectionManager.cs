using System.Collections.Generic;
using System.Diagnostics;
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

    public void Select(Unit unit)
    {
        SelectedUnits.Add(unit);
        unit.Select();
    }

    public void Deselect(Unit unit)
    {
        unit.Deselect();
        SelectedUnits.Remove(unit);
    }

    public void DeselectAll()
    {
        foreach(Unit unit in SelectedUnits)
        {
            unit.Deselect();
        }
        SelectedUnits.Clear();
    }

    public bool IsSelected(Unit unit)
    {
        return SelectedUnits.Contains(unit);
    }
}
