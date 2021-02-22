using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesManager : MonoBehaviour
{
    public List<GameObject> enemies;
    void Start()
    {
        enemies = Utilities.GetChildrenWithTag(this.gameObject, "enemy");
    }

    public void ResetPositions() {
        foreach (GameObject enemy in enemies) {
            enemy.GetComponent<EnemyController>().ResetPosition();
        }
    }
}
