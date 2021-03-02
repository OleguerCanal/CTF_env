using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectiblesManager : MonoBehaviour
{
    
    public GameObject collectiblePrefab;

    private List<GameObject> fixedCollectibles;
    private List<GameObject> randomCollectibles = new List<GameObject>();

    void Start()
    {
        fixedCollectibles = Utilities.GetChildrenWithTag(gameObject, "collectible");
    }

    public void InstantiateCollectibles(List<Cell> collectiblesPositions) 
    {
        randomCollectibles = new List<GameObject>();
        foreach (Cell c in collectiblesPositions)
        {
            GameObject instantiatedCollectible = Instantiate(collectiblePrefab);
            instantiatedCollectible.transform.parent = this.transform;  // Should be attached to the collectiblesHolder
            instantiatedCollectible.transform.localPosition = new Vector3(c.x, 1, c.z);
            randomCollectibles.Add(instantiatedCollectible);
        }
    }

    public void DestroyRandomizedCollectibles() 
    {
        foreach (GameObject collectible in randomCollectibles)
        {
            Destroy(collectible);
        }
        randomCollectibles = new List<GameObject>();
    }

    public void SetAllCollectiblesActive()
    {
        foreach (GameObject collectible in fixedCollectibles)
        {
            collectible.SetActive(true);
        }
        foreach (GameObject collectible in randomCollectibles)
        {
            collectible.SetActive(true);
        }
    }
}
