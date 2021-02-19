using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectiblesManager : MonoBehaviour
{
    
    public GameObject collectiblePrefab;

    private List<GameObject> collectibles;
    private string collectible_tag = "collectible";

    // Start is called before the first frame update
    void Start()
    {
        collectibles = Utilities.GetChildrenWithTag(gameObject, collectible_tag);
    }

    public void InstantiateCollectibles(int n, float radius) 
    {
        for (int i = 0; i < n; i++) {
            Vector3 pos = new Vector3(Random.value * radius - radius/2,
                                      1.0f,
                                      Random.value * radius - radius/2);
            var instantiatedCollectible = Instantiate(collectiblePrefab, pos, Quaternion.identity);
            instantiatedCollectible.transform.parent = gameObject.transform;
        }
        collectibles = Utilities.GetChildrenWithTag(gameObject, collectible_tag);
    }

    public void DestroyCollectibles() 
    {
        foreach (var collectible in collectibles)
        {
            Destroy(collectible);
        }
    }

    public void SetAllCollectiblesActive()
    {
        foreach (var collectible in collectibles)
        {
            collectible.SetActive(true);
        }
    }
}
