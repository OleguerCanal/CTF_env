using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WallManager : MonoBehaviour
{

    public Transform wallPrefab;

    private List<GameObject> randomWalls = new List<GameObject>();

    public void InstantiateWalls(List<Wall> walls) {
        randomWalls = new List<GameObject>();
        foreach (Wall w in walls)
        {
            Transform instantiatedWall = Instantiate(wallPrefab);
            instantiatedWall.parent = this.transform;  // Should be attached to the wallHolder
            instantiatedWall.localScale = new Vector3(0.75f, 5, w.length);
            instantiatedWall.rotation = Quaternion.Euler(0, 90 - w.theta, 0);
            instantiatedWall.localPosition = new Vector3(w.cell.x, 2, w.cell.z);
            randomWalls.Add(instantiatedWall.gameObject);
        }
    }

    public void DestroyRandomizedWalls() 
    {
        foreach (GameObject wall in randomWalls)
        {
            Destroy(wall);
        }
    }
}