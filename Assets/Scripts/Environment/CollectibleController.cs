using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleController : MonoBehaviour
{
    private GameManager gameManager; // GameManager associated with this TrainingArea

    void Start()
    {
        gameManager = transform.parent.parent.gameObject.GetComponent<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "agent") {
            other.GetComponent<CTFAgent>().OnCollectedCollectible();
            gameObject.SetActive(false);
        }
    }
}

