using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class EnemyController : MonoBehaviour
{
    private int radius = 6;
    private float speed; // Between 0, 1
    private List<GameObject> agents;
    private Vector3 initialPosition;
    private GameManager gameManager; // GameManager associated with this TrainingArea

    void Start()
    {
        gameManager = transform.parent.parent.gameObject.GetComponent<GameManager>();
        agents = Utilities.GetChildrenWithTag(this.transform.parent.parent.gameObject, "agent");
        speed = Academy.Instance.EnvironmentParameters.GetWithDefault("enemySpeed", 0.03f);
        initialPosition = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        foreach (GameObject agent in agents)
        {
            if (Vector3.Distance(agent.transform.position, this.transform.position) < radius) {
                this.transform.position = (1.0f - speed)*this.transform.position + speed*agent.transform.position; 
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 6);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "agent") {
            Debug.Log("Enemy killed agent!");
            gameManager.DeathByEnemy(other.gameObject);
        }
    }

    public void ResetPosition() {
        this.transform.position = initialPosition;
    }
}
