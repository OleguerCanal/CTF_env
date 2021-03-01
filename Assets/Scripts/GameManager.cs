using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

public class GameManager : MonoBehaviour
{
    // Public attributes
    public GameObject agent;
    public GameObject finalGoal;
    public GameObject wallsHolder;
    public GameObject collectibleHolder;
    public GameObject enemiesHolder;
    public int maxFrames = 2000;  // Time limit in seconds

    // Private attributes
    private int frameCounter;
    private Vector3 agentIntialPose;
    private Vector3 goalInitialPose;

    // Rewards
    private float finishReward;
    private float collectableReward;
    private float deathByEnemyReward;
    private float notFinishedReward;

    private WallManager wallManager;
    private EnemiesManager enemiesManager;
    private CollectiblesManager collectiblesManager;
    
    // Start is called before the first frame update
    void Start()
    {
        finishReward = Academy.Instance.EnvironmentParameters.GetWithDefault("finishReward", 1.0f);
        collectableReward = Academy.Instance.EnvironmentParameters.GetWithDefault("collectableReward", 0.5f);
        deathByEnemyReward = Academy.Instance.EnvironmentParameters.GetWithDefault("deathByEnemyReward", -2.0f);
        notFinishedReward = Academy.Instance.EnvironmentParameters.GetWithDefault("notFinishedReward", -1.0f);
        // Time.timeScale = 3.0f;

        wallManager = wallsHolder.GetComponent<WallManager>();
        enemiesManager = enemiesHolder.GetComponent<EnemiesManager>();
        collectiblesManager = collectibleHolder.GetComponent<CollectiblesManager>();
    }
    
    public void BuildMap()
    {
        int numWalls = 10;
        int numCollectibles = 5;
        int numEnenmies = 3;
        MapDescription map = MapDesigner.CreateMap(numWalls, numCollectibles, numEnenmies);
        agent.transform.localPosition = new Vector3(map.startPos.x, 0.0f, map.startPos.z);
        finalGoal.transform.localPosition = new Vector3(map.startPos.x, 0.0f, map.startPos.z);
        wallManager.InstantiateWalls(map.walls);
        enemiesManager.InstantiateEnemies(map.enemiesPositions);
        collectiblesManager.InstantiateCollectibles(map.collectiblesPositions);
    }

    public void ResetMap()
    {
        enemiesManager.ResetEnemyPositions();
        CollectiblesManager.SetAllCollectiblesActive();
    }

    public void DestroyMap()
    {
        wallManager.DestroyRandomizedWalls();
        enemiesManager.DestroyRandomizedEnemies();
    }

    public void ResetGame()
    {
        frameCounter = 0;
    }

    public void EnteredBase(GameObject agent) {
        agent.GetComponent<CTFAgent>().SetReward(finishReward);  // Give reward
        agent.GetComponent<CTFAgent>().OnEpisodeEnd();  // Finish episode
    }

    public void CollectedCollectible(GameObject agent) {
        agent.GetComponent<CTFAgent>().SetReward(collectableReward);  // Give reward
    }

    public void DeathByEnemy(GameObject agent) {
        agent.GetComponent<CTFAgent>().SetReward(deathByEnemyReward);  // Give reward
        agent.GetComponent<CTFAgent>().OnEpisodeEnd();  // Finish episode
    }

    void Update()
    {
        if (frameCounter >= maxFrames)
        {
            agent.GetComponent<CTFAgent>().SetReward(notFinishedReward);  // Give reward
            agent.GetComponent<CTFAgent>().OnEpisodeEnd();
        }
        frameCounter += 1;
    }
}
