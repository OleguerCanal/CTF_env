using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    private GameManager gameManager; // GameManager associated with this TrainingArea

    // Start is called before the first frame update
    void Start()
    {
        gameManager = transform.parent.gameObject.GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "agent") {
            gameManager.EnteredBase(other.gameObject);
        }
    }
}
