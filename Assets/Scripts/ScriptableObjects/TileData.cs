using UnityEngine;

public enum tileTypes {
    Plains,
    Forest,
    Mountain,
    Water

}

[CreateAssetMenu(fileName = "TileData", menuName = "Scriptable Objects/TileData")]
public class TileData : ScriptableObject
{
    public tileTypes tileTypes;
}
