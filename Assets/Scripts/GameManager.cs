using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

public class GameManager : MonoBehaviour
{
    // Public attributes
    public GameObject agent;
    public GameObject finalGoal;
    public GameObject mapHolder;
    public GameObject collectibleHolder;
    public GameObject enemiesHolder;

    // Private attributes
    private int frameCounter;
    private Vector3 agentIntialPose;
    private Vector3 goalInitialPose;

    private MapDesigner mapDesigner;
    private WallManager wallManager;
    private EnemiesManager enemiesManager;
    private CollectiblesManager collectiblesManager;

    private MapDescription map;
    private List<MapParameters> mapParamDifficulty;

    // Start is called before the first frame update
    void Start()
    {
        // Time.timeScale = 3.0f;
        mapDesigner = new MapDesigner();
        wallManager = mapHolder.transform.Find("WallHolder").GetComponent<WallManager>();
        enemiesManager = enemiesHolder.GetComponent<EnemiesManager>();
        collectiblesManager = collectibleHolder.GetComponent<CollectiblesManager>();

        // Sorted difficulty: walls, collectibles, enemies
        mapParamDifficulty = new List<MapParameters>();
        mapParamDifficulty.Add(new MapParameters(0, 1, 0));
        mapParamDifficulty.Add(new MapParameters(0, 2, 0));
        mapParamDifficulty.Add(new MapParameters(0, 4, 1));
        mapParamDifficulty.Add(new MapParameters(2, 4, 1));
        mapParamDifficulty.Add(new MapParameters(4, 4, 2));
        mapParamDifficulty.Add(new MapParameters(6, 4, 2));
        mapParamDifficulty.Add(new MapParameters(10, 4, 2));
        mapParamDifficulty.Add(new MapParameters(10, 4, 4));
    }
    
    public void BuildMap(int difficulty)
    {
        MapDesigner.mapWidth = 70;
        MapDesigner.mapHeight = 70;

        // mapHolder.transform.Find("Ground").localScale = new Vector3(mapWidth, 1, mapHeight);
        // mapHolder.transform.Find("WallHolder").transform.Find("Border_1").localScale = new Vector3(1, 5, mapHeight);
        // mapHolder.transform.Find("WallHolder").transform.Find("Border_1").localPosition = new Vector3(mapWidth/2, 2, 0);

        // mapHolder.transform.Find("WallHolder").transform.Find("Border_2").localScale = new Vector3(1, 5, mapHeight);
        // mapHolder.transform.Find("WallHolder").transform.Find("Border_2").localPosition = new Vector3(mapWidth/2, 2, 0);
        
        // mapHolder.transform.Find("WallHolder").transform.Find("Border_3").localScale = new Vector3(1, 5, mapHeight);
        // mapHolder.transform.Find("WallHolder").transform.Find("Border_3").localPosition = new Vector3(mapWidth/2, 2, 0);

        // mapHolder.transform.Find("WallHolder").transform.Find("Border_4").localScale = new Vector3(1, 5, mapHeight);
        // mapHolder.transform.Find("WallHolder").transform.Find("Border_4").localPosition = new Vector3(mapWidth/2, 2, 0);

        MapParameters mapParam = mapParamDifficulty[Mathf.Min(difficulty, mapParamDifficulty.Count - 1)];
        map = mapDesigner.CreateMap(mapParam.numWalls, mapParam.numCollectibles, mapParam.numEnenmies);
        agent.transform.localPosition = new Vector3(map.startPos.x, 0.0f, map.startPos.z);
        finalGoal.transform.localPosition = new Vector3(map.finishPos.x, 0.0f, map.finishPos.z);
        wallManager.InstantiateWalls(map.walls);
        enemiesManager.InstantiateEnemies(map.enemiesPositions);
        collectiblesManager.InstantiateCollectibles(map.collectiblesPositions);
        
    }

    public void ResetMap()
    {
        agent.transform.localPosition = new Vector3(map.startPos.x, 0.0f, map.startPos.z);
        enemiesManager.ResetEnemyPositions();
        collectiblesManager.SetAllCollectiblesActive();
    }

    public void DestroyMap()
    {
        wallManager.DestroyRandomizedWalls();
        enemiesManager.DestroyRandomizedEnemies();
        collectiblesManager.DestroyRandomizedCollectibles();
    }
}

public class MapParameters
{
    public int numWalls;
    public int numCollectibles;
    public int numEnenmies;

    public MapParameters(int nWalls, int nColls, int nEnem)
    {
        numWalls = nWalls;
        numCollectibles = nColls;
        numEnenmies = nEnem;
    }
}