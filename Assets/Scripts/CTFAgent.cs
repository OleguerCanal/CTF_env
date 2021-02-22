using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class CTFAgent : Agent
{
    private GameManager gameManager; // GameManager associated with this TrainingArea
    private Rigidbody rBody;
    private float timeReward;
    private float enemyCloseReward;
    private EnemiesManager enemiesManager;
    
    // Counters
    private int frameCounter;
    private int episodeCounter;

    // Logger
    // private Logger analyticsLogger;
    private string logPath = "Assets/Resources/cmplx_completionist.txt";
    private int logFrequency;

    void Start ()
    {
        rBody = GetComponent<Rigidbody>();
        gameManager = transform.parent.gameObject.GetComponent<GameManager>();
        enemiesManager = gameManager.enemiesHolder.GetComponent<EnemiesManager>();
        timeReward = Academy.Instance.EnvironmentParameters.GetWithDefault("timeReward", -0.01f);
        enemyCloseReward = Academy.Instance.EnvironmentParameters.GetWithDefault("enemyCloseReward", 0.0f);
        logFrequency = (int) Academy.Instance.EnvironmentParameters.GetWithDefault("logFrequency", 1.0f);
        // analyticsLogger = new Logger();
        episodeCounter = 0;
    }

    public override void OnEpisodeBegin()
    {
        gameManager.ResetGame();
        
        frameCounter = 0;
        episodeCounter += 1;

        // if (episodeCounter % logFrequency == 0)
        // {
        //     analyticsLogger.StartEpisode();
        // }
    }

    public void OnEpisodeEnd()
    {
        base.EndEpisode(); // Call the Agent.EndEpisode
        // if (episodeCounter % logFrequency == 0)
        // {
        //     analyticsLogger.EndEpisode();
        //     analyticsLogger.Save(logPath); // Do this with a certain frequency
        // }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // FinalGoal relative position
        sensor.AddObservation(rBody.velocity.x);
        sensor.AddObservation(rBody.velocity.z);
        float time_proportion = 1- ((float) frameCounter)/((float) gameManager.maxFrames);
        sensor.AddObservation(time_proportion);
    }

    public float forceMultiplier = 0.1f;
    public override void OnActionReceived(float[] vectorAction)
    {
        Vector3 controlSignal = new Vector3(vectorAction[0] - 1,
                                            0.0f,
                                            vectorAction[1] - 1);
        rBody.AddForce(controlSignal * forceMultiplier, ForceMode.VelocityChange);
    }

    public override void Heuristic(float[] actionsOut)
    {
        int vert = 1;
        int hor = 1;
        if (-Input.GetAxis("Vertical") > 0.1) vert = 2; 
        else if (-Input.GetAxis("Vertical") < -0.1) vert = 0;
        if (Input.GetAxis("Horizontal") > 0.1) hor = 2;
        else if (Input.GetAxis("Horizontal") < -0.1) hor = 0;
        actionsOut[0] = vert;
        actionsOut[1] = hor;
    }

    void Update()
    {
        SetReward(timeReward);  // Time penalty
        if (enemyCloseReward != 0.0f) {
            foreach (GameObject enemy in enemiesManager.enemies)
            {
                if (Vector3.Distance(enemy.transform.position, this.transform.position) < 7.0f) {
                    Debug.Log("Enemy Close!");
                    SetReward(enemyCloseReward);
                }
            }
        }
        frameCounter += 1;
        // if (episodeCounter % logFrequency == 0)
        // {
        //     if (frameCounter % 5 == 0)
        //     {
        //         analyticsLogger.episode.trajectory.Add(this.transform.localPosition);
        //     }
        // }
    }

}
