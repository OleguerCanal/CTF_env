using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesManager : MonoBehaviour
{
    public GameObject enemyPrefab;

    private List<GameObject> fixedEnemies;
    private List<GameObject> randomizedEnemies;

    void Start()
    {
        fixedEnemies = Utilities.GetChildrenWithTag(this.gameObject, "enemy");
        randomizedEnemies = new List<GameObject>();
    }

    public void InstantiateEnemies(List<Cell> enemiesPositions)
    {
        foreach (Cell c in enemiesPositions)
        {
            Transform instantiatedEnemy = Instantiate(enemyPrefab);
            instantiatedEnemy.parent = this.transform;  // Should be attached to the enemiesHolder
            instantiatedEnemy.localPosition = new Vector3(c.x, 1, c.z);
            randomizedEnemies.Add(instantiatedEnemy);
        }
    }

    public void DestroyRandomizedEnemies()
    {
        foreach (GameObject enemy in randomizedEnemies)
        {
            Destroy(enemy);
        }
        randomizedEnemies = new List<GameObject>();
    }

    public void ResetEnemyPositions()
    {
        foreach (GameObject enemy in fixedEnemies)
        {
            enemy.GetComponent<EnemyController>().ResetPosition();
        }

        foreach (GameObject enemy in randomizedEnemies)
        {
            enemy.GetComponent<EnemyController>().ResetPosition();
        }
    }

}
