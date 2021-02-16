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
    public float timeLimit = 4.0f;  // Time limit in seconds

    // Private attributes
    private float episodeBeginTime;
    private Vector3 agentIntialPose;
    private Vector3 goalInitialPose;

    private float finish_reward = 1.0f;  // Default value
    private float collectable_reward = 1.0f;  // Default value

    
    // Start is called before the first frame update
    void Start()
    {
        // Academy.Instance.EnvironmentParameters.GetWithDefault("my_environment_parameter", 0.0f);
        agentIntialPose = agent.transform.position;
        goalInitialPose = finalGoal.transform.position;

        finish_reward = Academy.Instance.EnvironmentParameters.GetWithDefault("finish_reward", finish_reward);
        collectable_reward = Academy.Instance.EnvironmentParameters.GetWithDefault("collectable_reward", collectable_reward);
    }

    public void ResetGame() {
        int target_radius = 2;
        int player_radius = 2;

        Vector3 goalPoseNoise = new Vector3(Random.value * target_radius - target_radius/2,
                                            0.0f,
                                            Random.value * target_radius - target_radius/2);
        Vector3 agentPoseNoise = new Vector3(Random.value * player_radius - player_radius/2,
                                            0.0f,
                                            Random.value * player_radius - player_radius/2);
                                            
        finalGoal.transform.localPosition = goalInitialPose + goalPoseNoise;
        agent.transform.localPosition = agentIntialPose + agentPoseNoise;

        collectibleHolder.GetComponent<CollectiblesManager>().SetAllCollectiblesActive();
        episodeBeginTime = Time.time;
    }

    public void EnteredBase(GameObject agent) {
        agent.GetComponent<CTFAgent>().SetReward(1.0f);  // Give reward
        agent.GetComponent<CTFAgent>().EndEpisode();  // Finish episode
    }

    public void CollectedCollectible(GameObject agent) {
        agent.GetComponent<CTFAgent>().SetReward(1.0f);  // Give reward
    }

    void Update()
    {
        if (Time.time - episodeBeginTime > timeLimit)
        {
            agent.GetComponent<CTFAgent>().EndEpisode();
        }
    }
}
