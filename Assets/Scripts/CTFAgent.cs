using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class CTFAgent : Agent
{
    private GameManager gameManager; // GameManager associated with this TrainingArea
    private Rigidbody rBody;

    private EnemiesManager enemiesManager;
    
    // Counters
    private int maxFrames = 3500;  // Time limit in seconds
    private int frameCounter;
    private int episodeCounter;

    // Rewards
    private float timeReward;
    private float finishReward;
    private float notFinishedReward;
    private float collectableReward;
    private float enemyCloseReward;
    private float deathByEnemyReward;
    private int levelDifficulty;
    private int mapRepetitions;

    // Logger
    public bool log = false;
    public string logPath = "Assets/Resources/cmplx_completionist.txt";
    private int logFrequency = 100;
    private Logger analyticsLogger;

    void Start ()
    {
        rBody = GetComponent<Rigidbody>();
        gameManager = transform.parent.gameObject.GetComponent<GameManager>();
        enemiesManager = gameManager.enemiesHolder.GetComponent<EnemiesManager>();
        
        // logFrequency = (int) Academy.Instance.EnvironmentParameters.GetWithDefault("logFrequency", 1.0f);        
        analyticsLogger = new Logger(logPath, logFrequency, log);
        episodeCounter = 0;
    }

    public override void OnEpisodeBegin()
    {
        // Read rewards
        timeReward = Academy.Instance.EnvironmentParameters.GetWithDefault("timeReward", -0.01f);
        finishReward = Academy.Instance.EnvironmentParameters.GetWithDefault("finishReward", 1.0f);
        notFinishedReward = Academy.Instance.EnvironmentParameters.GetWithDefault("notFinishedReward", -1.0f);
        collectableReward = Academy.Instance.EnvironmentParameters.GetWithDefault("collectableReward", 0.5f);
        enemyCloseReward = Academy.Instance.EnvironmentParameters.GetWithDefault("enemyCloseReward", 0.0f);
        deathByEnemyReward = Academy.Instance.EnvironmentParameters.GetWithDefault("deathByEnemyReward", -2.0f);
        levelDifficulty = (int) Academy.Instance.EnvironmentParameters.GetWithDefault("levelDifficulty", 10.0f);
        mapRepetitions = (int) Academy.Instance.EnvironmentParameters.GetWithDefault("mapRepetitions", 1.0f);

        
        if (episodeCounter % mapRepetitions == 0)
        {
            Debug.Log("New map with difficulty: " + levelDifficulty);
            gameManager.DestroyMap();
            // UnityEngine.Random.Range(levelDifficulty - 3, levelDifficulty)
            gameManager.BuildMap(levelDifficulty);
        }
        else
        {
            Debug.Log("Map reset");
            gameManager.ResetMap();
        }
        
        // gameManager.ResetMap();
        
        frameCounter = 0;
        episodeCounter += 1;
        analyticsLogger.StartEpisode(episodeCounter);
    }

    public void OnEpisodeEnd()
    {
        base.EndEpisode(); // Call the Agent.EndEpisode
        analyticsLogger.EndEpisode(episodeCounter);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // FinalGoal relative position
        sensor.AddObservation(rBody.velocity.x);
        sensor.AddObservation(rBody.velocity.z);
        float time_proportion = 1- ((float) frameCounter)/((float) maxFrames);
        sensor.AddObservation(time_proportion);
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        float forceMultiplier = 10.0f;
        Vector3 controlSignal = new Vector3(vectorAction[0] - 1,
                                            0.0f,
                                            vectorAction[1] - 1);
        rBody.AddForce(controlSignal * forceMultiplier, ForceMode.VelocityChange);
    }

    public void OnTouchedFinish()
    {
        Debug.Log("Done");
        SetReward(finishReward);  // Give reward
        OnEpisodeEnd();
    }
    
    public void OnCollectedCollectible()
    {
        Debug.Log("Collected collectible!");
        SetReward(collectableReward);  // Give reward
    }
    
    public void OnTouchedByEnemy()
    {
        Debug.Log("Enemy killed agent!");
        SetReward(deathByEnemyReward);  // Give reward
        OnEpisodeEnd();
    }

    public override void Heuristic(float[] actionsOut)
    {
        int vert = 1;
        int hor = 1;
        if (Input.GetAxis("Vertical") > 0.1) vert = 2; 
        else if (Input.GetAxis("Vertical") < -0.1) vert = 0;
        if (Input.GetAxis("Horizontal") > 0.1) hor = 2;
        else if (Input.GetAxis("Horizontal") < -0.1) hor = 0;
        actionsOut[0] = hor;
        actionsOut[1] = vert;
    }

    void Update()
    {
        frameCounter += 1;
        SetReward(timeReward);  // Time penalty
        if (enemyCloseReward != 0.0f && enemiesManager.EnemyClose(this.transform.position, 7.0f))
        {
            SetReward(enemyCloseReward);
        }
        analyticsLogger.AddStep(episodeCounter, frameCounter, this.transform.localPosition);
        if (frameCounter >= maxFrames)
        {
            SetReward(notFinishedReward);  // Give reward
            OnEpisodeEnd();
        }
    }

}
