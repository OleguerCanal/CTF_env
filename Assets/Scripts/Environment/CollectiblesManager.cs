using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectiblesManager : MonoBehaviour
{
    
    public GameObject collectiblePrefab;

    private List<GameObject> fixedCollectibles;
    private List<GameObject> randomCollectibles;

    void Start()
    {
        fixedCollectibles = Utilities.GetChildrenWithTag(gameObject, "collectible");
        randomCollectibles = new List<GameObject>();
    }

    public void InstantiateCollectibles(List<Cell> collectiblesPositions) 
    {
        foreach (Cell c in collectiblesPositions)
        {
            Transform instantiatedCollectible = Instantiate(collectiblePrefab);
            instantiatedCollectible.parent = this.transform;  // Should be attached to the enemiesHolder
            instantiatedCollectible.localPosition = new Vector3(c.x, 1, c.z);
            randomizedEnemies.Add(instantiatedCollectible);
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
