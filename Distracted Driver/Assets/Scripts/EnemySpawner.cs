using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    //
    //
    //!!to do: create prefab variants for all different enemies

    GameObject enemyPrefab;
    GameObject enemyPrefab2;
    GameObject[] enemyPrefabs;
    float rightBounds = -2.72f;
    float leftBounds = -6.14f;
    float middleBounds = -4.43f;
    [SerializeField] float spawnRate;

    // Start is called before the first frame update
    void Start()
    {
        enemyPrefabs = Resources.LoadAll<GameObject>("Enemy Cars");

        InvokeRepeating("SpawnEnemy", 2, spawnRate);
        EventManager.GameOver.AddListener(Stop);
    }

    // Update is called once per frame
    void Update()
    {
        Random.InitState((int) System.DateTime.Now.Ticks);
    }

    //1 or 2 random enemy prefabs spawn in 1 or 2 different lanes
    void SpawnEnemy()
    {
        enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        enemyPrefab2 = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

        int lane = Random.Range(1, 7);

        //instantiate enemy prefabs in random lanes
        switch (lane)
        {
            case 1:
                Instantiate(enemyPrefab, new Vector3(leftBounds, 9.5f, 0), Quaternion.identity);
                break;
            case 2:
                Instantiate(enemyPrefab, new Vector3(middleBounds, 9.5f, 0), Quaternion.identity);
                break;
            case 3:
                Instantiate(enemyPrefab, new Vector3(rightBounds, 9.5f, 0), Quaternion.identity);
                break;
            case 4:
                Instantiate(enemyPrefab, new Vector3(rightBounds, 9.5f, 0), Quaternion.identity);
                Instantiate(enemyPrefab2, new Vector3(leftBounds, 9.5f, 0), Quaternion.identity);
                break;
            case 5:
                Instantiate(enemyPrefab, new Vector3(rightBounds, 9.5f, 0), Quaternion.identity);
                Instantiate(enemyPrefab2, new Vector3(middleBounds, 9.5f, 0), Quaternion.identity);
                break;
            case 6:
                Instantiate(enemyPrefab, new Vector3(leftBounds, 9.5f, 0), Quaternion.identity);
                Instantiate(enemyPrefab2, new Vector3(middleBounds, 9.5f, 0), Quaternion.identity);
                break;

        }
    }

    void Stop()
    {
        CancelInvoke();
    }
}
