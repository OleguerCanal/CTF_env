using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

public class GameManager : MonoBehaviour
{
    // Public attributes
    public GameObject finalGoal;
    public GameObject agent;
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
    
    // Start is called before the first frame update
    void Start()
    {
        // Academy.Instance.EnvironmentParameters.GetWithDefault("my_environment_parameter", 0.0f);
        agentIntialPose = agent.transform.position;
        goalInitialPose = finalGoal.transform.position;

        finishReward = Academy.Instance.EnvironmentParameters.GetWithDefault("finishReward", 1.0f);
        collectableReward = Academy.Instance.EnvironmentParameters.GetWithDefault("collectableReward", 0.5f);
        deathByEnemyReward = Academy.Instance.EnvironmentParameters.GetWithDefault("deathByEnemyReward", -2.0f);
        notFinishedReward = Academy.Instance.EnvironmentParameters.GetWithDefault("notFinishedReward", -1.0f);
        // Time.timeScale = 3.0f;
    }

    public void ResetGame() {
        int targetRadius = 1;
        int playerRadius = 1;
        
        collectibleHolder.GetComponent<CollectiblesManager>().DestroyRandomizedCollectibles();
        collectibleHolder.GetComponent<CollectiblesManager>().InstantiateCollectibles(0, 40);
        collectibleHolder.GetComponent<CollectiblesManager>().SetAllCollectiblesActive();
        enemiesHolder.GetComponent<EnemiesManager>().ResetPositions();
        
        Vector3 goalPoseNoise = new Vector3(Random.value * targetRadius - targetRadius/2,
                                            0.0f,
                                            Random.value * targetRadius - targetRadius/2);
        Vector3 agentPoseNoise = new Vector3(Random.value * playerRadius - playerRadius/2,
                                            0.0f,
                                            Random.value * playerRadius - playerRadius/2);
                                            
        finalGoal.transform.localPosition = goalInitialPose + goalPoseNoise;
        agent.transform.localPosition = agentIntialPose + agentPoseNoise;

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
