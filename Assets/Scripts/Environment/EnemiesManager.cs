using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesManager : MonoBehaviour
{
    public GameObject enemyPrefab;

    private List<GameObject> fixedEnemies;
    private List<GameObject> randomizedEnemies = new List<GameObject>();

    void Start()
    {
        fixedEnemies = Utilities.GetChildrenWithTag(this.gameObject, "enemy");
    }

    public void InstantiateEnemies(List<Cell> enemiesPositions)
    {
        randomizedEnemies = new List<GameObject>();
        foreach (Cell c in enemiesPositions)
        {
            GameObject instantiatedEnemy = Instantiate(enemyPrefab);
            instantiatedEnemy.transform.parent = this.transform;  // Should be attached to the enemiesHolder
            instantiatedEnemy.transform.localPosition = new Vector3(c.x, 1, c.z);
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

    public bool EnemyClose(Vector3 pos, float threshold)
    {
        foreach (GameObject enemy in fixedEnemies)
        {
            if (Vector3.Distance(enemy.transform.position, pos) < threshold)
            {
                return true;
            }
        }
        foreach (GameObject enemy in randomizedEnemies)
        {
            if (Vector3.Distance(enemy.transform.position, pos) < threshold)
            {
                return true;
            }
        }
        return false;
    }

}
