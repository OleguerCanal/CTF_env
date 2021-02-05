using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagController : MonoBehaviour
{
    public GameObject goToFollow = null;

    private GameManager gameManager; // GameManager associated with this TrainingArea
    private float displacement = 1;
    
    // Start is called before the first frame update
    void Start()
    {
        gameManager = transform.parent.gameObject.GetComponent<GameManager>();
    }

    public void Reset(Vector3 pos) {
        transform.localPosition = pos;
    }

    public void StartFollowing(GameObject go)
    {
        goToFollow = go;
    }

    public void StopFollowing()
    {
        goToFollow = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (goToFollow != null)
        {
            transform.localPosition = new Vector3(
                goToFollow.transform.localPosition.x,
                goToFollow.transform.localPosition.y + displacement,
                goToFollow.transform.localPosition.z);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "agent") {
            gameManager.FlagFound(other.gameObject);
        }
    }
}
