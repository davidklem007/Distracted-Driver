using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    //
    //
    //!!to do: create prefab variants for all different enemies

    [SerializeField] GameObject enemyPrefab;
    float rightBounds = -2.72f;
    float leftBounds = -6.14f;
    float middleBounds = -4.43f;
    [SerializeField] float spawnRate;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("SpawnEnemy", 2, spawnRate);
        EventManager.GameOver.AddListener(Stop);
    }

    // Update is called once per frame
    void Update()
    {
        Random.InitState((int) System.DateTime.Now.Ticks);
    }

    void SpawnEnemy()
    {
        int lane = Random.Range(1, 7);

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
                Instantiate(enemyPrefab, new Vector3(leftBounds, 9.5f, 0), Quaternion.identity);
                break;
            case 5:
                Instantiate(enemyPrefab, new Vector3(rightBounds, 9.5f, 0), Quaternion.identity);
                Instantiate(enemyPrefab, new Vector3(middleBounds, 9.5f, 0), Quaternion.identity);
                break;
            case 6:
                Instantiate(enemyPrefab, new Vector3(leftBounds, 9.5f, 0), Quaternion.identity);
                Instantiate(enemyPrefab, new Vector3(middleBounds, 9.5f, 0), Quaternion.identity);
                break;

        }
    }

    void Stop()
    {
        CancelInvoke();
    }
}
