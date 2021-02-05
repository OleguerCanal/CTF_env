using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class CTFAgent : Agent
{
    public Transform Target;
    public float timeLimit;

    private Rigidbody rBody;
    private float episodeBeginTime;

    void Start ()
    {
        rBody = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        int target_radius = 12;
        int player_radius = 12;

        // Move the target to a new spot
        Target.localPosition = new Vector3(Random.value * target_radius - target_radius/2,
                                           1.0f,
                                           Random.value * target_radius - target_radius/2);
        transform.localPosition = new Vector3(Random.value * player_radius - player_radius/2,
                                            1.0f,
                                            Random.value * player_radius - player_radius/2);

        episodeBeginTime = Time.time;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Target and Agent positions
        sensor.AddObservation(Target.localPosition.x - this.transform.localPosition.x);
        sensor.AddObservation(Target.localPosition.z - this.transform.localPosition.z);

        // Agent velocity
        sensor.AddObservation(rBody.velocity.x);
        sensor.AddObservation(rBody.velocity.z);
    }

    public float forceMultiplier = 0.1f;
    public override void OnActionReceived(float[] vectorAction)
    {
        // Actions, size = 2
        Vector3 controlSignal = new Vector3(vectorAction[0],
                                            0.0f,
                                            vectorAction[1]);
        rBody.AddForce(controlSignal * forceMultiplier, ForceMode.VelocityChange);

        // Rewards
        float distanceToTarget = Vector3.Distance(this.transform.localPosition, Target.localPosition);

        // Reached target
        if (distanceToTarget < 1.0f)
        {
            SetReward(1.0f);
            EndEpisode();
        }

        // Time limit
        if (Time.time - episodeBeginTime > timeLimit)
        {
            EndEpisode();
        }

        SetReward(-0.01f);
    }

    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = -Input.GetAxis("Vertical");
        actionsOut[1] = Input.GetAxis("Horizontal");
    }

}
