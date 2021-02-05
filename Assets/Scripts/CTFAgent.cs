using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class CTFAgent : Agent
{
    private GameManager gameManager; // GameManager associated with this TrainingArea
    private Rigidbody rBody;

    public bool bringingFlag = false;

    void Start ()
    {
        rBody = GetComponent<Rigidbody>();
        gameManager = transform.parent.gameObject.GetComponent<GameManager>();
    }

    public override void OnEpisodeBegin()
    {
        gameManager.ResetGame();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Flag relative position
        sensor.AddObservation(gameManager.greenFlag.transform.localPosition.x - this.transform.localPosition.x);
        sensor.AddObservation(gameManager.greenFlag.transform.localPosition.z - this.transform.localPosition.z);

        // Base relative position
        sensor.AddObservation(gameManager.blueBase.transform.localPosition.x - this.transform.localPosition.x);
        sensor.AddObservation(gameManager.blueBase.transform.localPosition.z - this.transform.localPosition.z);

        // Agent velocity
        // sensor.AddObservation(rBody.velocity.x); // TODO: transform to local coordinates
        // sensor.AddObservation(rBody.velocity.z);

        // Bringing flag
        if (bringingFlag) {
            sensor.AddObservation(1);
        }
        else {
            sensor.AddObservation(0);
        }
    }

    public float forceMultiplier = 0.1f;
    public override void OnActionReceived(float[] vectorAction)
    {
        // Actions, size = 2
        Vector3 controlSignal = new Vector3(vectorAction[0],
                                            0.0f,
                                            vectorAction[1]);
        rBody.AddForce(controlSignal * forceMultiplier, ForceMode.VelocityChange);

        // Rewards: Time penalty
        SetReward(-0.01f);
    }

    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = -Input.GetAxis("Vertical");
        actionsOut[1] = Input.GetAxis("Horizontal");
    }

}
