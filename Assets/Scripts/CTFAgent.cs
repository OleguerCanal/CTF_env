using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class CTFAgent : Agent
{
    private GameManager gameManager; // GameManager associated with this TrainingArea
    private Rigidbody rBody;
    private float time_reward = -0.01f;  // Default value

    void Start ()
    {
        rBody = GetComponent<Rigidbody>();
        gameManager = transform.parent.gameObject.GetComponent<GameManager>();
        time_reward = Academy.Instance.EnvironmentParameters.GetWithDefault("time_reward", time_reward);
        Debug.Log(time_reward);
    }

    public override void OnEpisodeBegin()
    {
        gameManager.ResetGame();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // FinalGoal relative position
        sensor.AddObservation(gameManager.finalGoal.transform.localPosition.x - this.transform.localPosition.x);
        sensor.AddObservation(gameManager.finalGoal.transform.localPosition.z - this.transform.localPosition.z);
    }

    public float forceMultiplier = 0.1f;
    public override void OnActionReceived(float[] vectorAction)
    {
        // Actions, size = 2
        Vector3 controlSignal = new Vector3(vectorAction[0],
                                            0.0f,
                                            vectorAction[1]);
        rBody.AddForce(controlSignal * forceMultiplier, ForceMode.VelocityChange);
    }

    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = -Input.GetAxis("Vertical");
        actionsOut[1] = Input.GetAxis("Horizontal");
    }

    void Update()
    {
        SetReward(time_reward);  // Time penalty
    }

}
