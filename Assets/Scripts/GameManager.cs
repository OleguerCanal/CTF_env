using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Public attributes
    public GameObject greenFlag;
    public GameObject blueAgent;
    public GameObject blueBase;
    public float timeLimit = 4.0f;  // Time limit in seconds

    // Private attributes
    private FlagController greenFlagController;
    private float episodeBeginTime;
    
    // Start is called before the first frame update
    void Start()
    {
        greenFlagController = greenFlag.GetComponent<FlagController>();
    }

    public void ResetGame() {
        int target_radius = 20;
        int base_radius = 20;
        int player_radius = 20;

        // Move the target to a new spot
        Vector3 green_flag_pos = new Vector3(Random.value * target_radius - target_radius/2,
                                            1.0f,
                                            Random.value * target_radius - target_radius/2);
        Vector3 blue_base_pos = new Vector3(Random.value * base_radius - base_radius/2,
                                            0.0f,
                                            Random.value * base_radius - base_radius/2);
        Vector3 blue_agent_pos = new Vector3(Random.value * player_radius - player_radius/2,
                                            1.0f,
                                            Random.value * player_radius - player_radius/2);
                                            
        greenFlagController.StopFollowing();
        blueAgent.GetComponent<CTFAgent>().bringingFlag = false;
        greenFlagController.Reset(green_flag_pos);
        blueBase.transform.localPosition = blue_base_pos;
        blueAgent.transform.localPosition = blue_agent_pos;
        episodeBeginTime = Time.time;
    }

    // agent found flag
    public void FlagFound(GameObject agent) {
        // TODO: check stuff
        Debug.Log("got flag!");
        agent.GetComponent<CTFAgent>().SetReward(0.5f);
        agent.GetComponent<CTFAgent>().bringingFlag = true;
        greenFlagController.StartFollowing(agent);
        agent.GetComponent<CTFAgent>().EndEpisode();  // Finish episode
    }

    // agent found flag
    public void EnteredBase(GameObject agent) {
        // TODO: check stuff
        if (agent.GetComponent<CTFAgent>().bringingFlag) {  // If the agent is bringing a flag
            Debug.Log("win!");
            agent.GetComponent<CTFAgent>().SetReward(1.0f);  // Give reward
            agent.GetComponent<CTFAgent>().EndEpisode();  // Finish episode
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - episodeBeginTime > timeLimit)
        {
            blueAgent.GetComponent<CTFAgent>().EndEpisode();
        }
    }
}
