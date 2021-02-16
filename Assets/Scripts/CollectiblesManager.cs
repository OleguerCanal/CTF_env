using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectiblesManager : MonoBehaviour
{
    private List<GameObject> collectibles;
    private string collectible_tag = "collectible";

    // Start is called before the first frame update
    void Start()
    {
        collectibles = Utilities.GetChildrenWithTag(gameObject, collectible_tag);
    }

    public void SetAllCollectiblesActive()
    {
        foreach (var collectible in collectibles)
        {
            collectible.SetActive(true);
        }
    }
}
