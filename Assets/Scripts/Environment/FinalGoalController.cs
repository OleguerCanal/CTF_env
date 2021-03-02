using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalGoalController : MonoBehaviour
{
    private GameManager gameManager; // GameManager associated with this TrainingArea

    // Start is called before the first frame update
    void Start()
    {
        gameManager = transform.parent.gameObject.GetComponent<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "agent")
        {
            other.GetComponent<CTFAgent>().OnTouchedFinish();
        }
    }
}
