using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities
{
    public static List<GameObject> GetChildrenWithTag(GameObject parent, string tag)
    {
        List<GameObject> children = new List<GameObject>();
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            Transform child = parent.transform.GetChild(i);
            if (child.tag == tag)
            {
                children.Add(child.gameObject);
            }
        }
        return children;
    }
}