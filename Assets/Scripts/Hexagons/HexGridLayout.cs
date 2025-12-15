
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


    [Header("Tile Prefabs and Chances")]
    public List<TileSpawnChance> tileSpawnChances; 
    public GameObject defaultTilePrefab;

    [Header("Enemy Spawns")]
    public GameObject minion;
    public GameObject lesserMinion;
    public float minionSpawnChance;
    float lowerMinionSpawnChance;

    public List<Vector3> positions;
    Vector3 LMPosition;

    GameObject Minions;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lowerMinionSpawnChance = 1;
        surface = FindFirstObjectByType<NavMeshSurface>();
        LayOutGrid();
        BakeNavMesh();
        Minions = GameObject.Find("MinionEnemies");
        SpawnMinions();
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void LayOutGrid()
    {
        for (int y = 0; y < gridSize.y; y++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                
                GameObject prefabToUse = GetRandomTilePrefab() ;
                GameObject tile = Instantiate(prefabToUse,  GetPositionForHexFromCoordinate(new Vector2Int(x, y)), Quaternion.identity, transform);

                //if(x == gridSize.x - 1 && y == gridSize.y - 1) LMPosition = tile.transform.position + new Vector3(0,1f,0);

                if(y > 5)
                {
                    if (UnityEngine.Random.Range(0f, 100f) <= minionSpawnChance)
                    {
                        if(prefabToUse == defaultTilePrefab) //prevents spawning on resourceTile
                        {
                            Debug.Log("Minion Spawn Success with " + minionSpawnChance + "%");
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
            if(y > 5) lowerMinionSpawnChance += 5f;
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
