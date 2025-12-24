
using UnityEngine;
using Unity.AI.Navigation;
using Unity.Mathematics;
using System.Collections.Generic;
using Unity.VisualScripting;
using System;
using UnityEngine.Tilemaps;

public class HexGridLayout : MonoBehaviour
{
    [Header("Grid Settings")]
    public Vector2Int gridSize;
    public GameObject hex;
    private NavMeshSurface surface;
    public int MinionBuildingInteral = 5;

    [Header("Tile Prefabs and Chances")]
    public List<TileSpawnChance> tileSpawnChances; 
    public GameObject defaultTilePrefab;
    public GameObject minionBuilding;

    [Header("Enemy Spawns")]
    public GameObject minion;
    public float minionSpawnChance;
    float lowerMinionSpawnChance;


    [SerializeField] private List<Vector3> positions;
    [SerializeField] private List<Vector2> BuildingPositions;

    GameObject Minions;//Empty Gameobject to hold spawned minions

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lowerMinionSpawnChance = 1;
        Minions = GameObject.Find("MinionEnemies");
        surface = FindFirstObjectByType<NavMeshSurface>();

        LayOutGrid();
        BakeNavMesh();
        SpawnMinions();
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void LayOutGrid()
    {

        FindBuildingSpawnpoints();

        for (int y = 0; y < gridSize.y; y++)//building matrix for hextiles
        {
            for (int x = 0; x < gridSize.x; x++)
            {

                if (BuildingPositions.Contains(new Vector2(x,y)))//checks if current positions is part of the building list
                {
                    GameObject prefabToUse = minionBuilding; //creates minion building 
                    Instantiate(prefabToUse, GetPositionForHexFromCoordinate(new Vector2Int(x, y)), Quaternion.identity, transform);
                }   
                else
                {
                    GameObject prefabToUse = GetRandomTilePrefab();//gets a random tile from weighted list
                    GameObject tile = Instantiate(prefabToUse,  GetPositionForHexFromCoordinate(new Vector2Int(x, y)), Quaternion.identity, transform);
                    if(y > 5)
                    {
                        if (UnityEngine.Random.Range(0f, 100f) <= minionSpawnChance)
                        {
                            if(prefabToUse == defaultTilePrefab) //prevents spawning on resourceTile
                            {
                                //Debug.Log("Minion Spawn Success with " + minionSpawnChance + "%");
                                minionSpawnChance = lowerMinionSpawnChance;
                                positions.Add(tile.transform.localPosition + gameObject.transform.position);
                            } 
                        }
                        else
                        {
                            minionSpawnChance += 5f;
                        }
                        
                    }
                }
                
                
            }
            if(y > 5) lowerMinionSpawnChance += 5f;
        }
    }

    private void FindBuildingSpawnpoints() //Fills list with positions for Minion Buildings
    {
        for (int i = gridSize.y; i > 0; i--)
        {
            if(i % MinionBuildingInteral == 0)
            {
                BuildingPositions.Add(new Vector2( UnityEngine.Random.Range(0,gridSize.x -1), i - 1));
            }
        }

    }

    private void SpawnMinions()
    {

        foreach(Vector3 spawnPosition in positions)
        {
            GameObject newSpawn = Instantiate(minion, spawnPosition + new Vector3(0,1,0), transform.rotation);
            newSpawn.transform.parent = Minions.transform;

        }

    }

    private GameObject GetRandomTilePrefab()
    {
        float roll = UnityEngine.Random.value; 
        float cumulative = 0f;

        foreach (var chance in tileSpawnChances)
        {
            cumulative += chance.spawnChance;
            if (roll <= cumulative)
                return chance.tilePrefab;
        }

        return defaultTilePrefab;
    }

    private Vector3 GetPositionForHexFromCoordinate(Vector2Int coordinate)
    {
        int column = coordinate.x;
        //int row = coordinate.y;

        float width;
        float height;
        float xPosition;
        float yPosition;
        bool shouldOffset;
        float horizontalDistance;
        float verticalDistance;
        float offset;


        shouldOffset = (column % 2) == 0;
        width = 10f;
        height = Mathf.Sqrt(3f) * 5f;
        horizontalDistance = width * (3f / 4f);
        verticalDistance = height;

        offset = shouldOffset ? height / 2 : 0;

        xPosition = column * horizontalDistance;
        yPosition = (coordinate.y * verticalDistance) - offset;


        return new Vector3(xPosition, 0, -yPosition);
    }

    void BakeNavMesh()
    {
        surface.BuildNavMesh();
    }

}


[System.Serializable]
public class TileSpawnChance
{
    public GameObject tilePrefab;
    [Range(0f, 1f)] public float spawnChance;  
}
