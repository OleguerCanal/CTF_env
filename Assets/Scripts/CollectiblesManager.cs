using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectiblesManager : MonoBehaviour
{
    
    public GameObject collectiblePrefab;

    private List<GameObject> fixedCollectibles;
    private List<GameObject> randomCollectibles;
    private string collectible_tag = "collectible";

    // Start is called before the first frame update
    void Start()
    {
        fixedCollectibles = Utilities.GetChildrenWithTag(gameObject, collectible_tag);
        randomCollectibles = new List<GameObject>();
    }

    public void InstantiateCollectibles(int n, float radius) 
    {
        for (int i = 0; i < n; i++) {
            Vector3 pos = new Vector3(Random.value * radius - radius/2,
                                      1.0f,
                                      Random.value * radius - radius/2);
            var instantiatedCollectible = Instantiate(collectiblePrefab, pos, Quaternion.identity);
            instantiatedCollectible.transform.parent = gameObject.transform;
            randomCollectibles.Add(instantiatedCollectible);
        }
    }

    public void DestroyRandomizedCollectibles() 
    {
        foreach (var collectible in randomCollectibles)
        {
            Destroy(collectible);
        }
    }

    public void SetAllCollectiblesActive()
    {
        foreach (var collectible in fixedCollectibles)
        {
            collectible.SetActive(true);
        }
    }
}
