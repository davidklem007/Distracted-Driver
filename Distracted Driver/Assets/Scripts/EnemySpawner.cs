using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemySpawner : MonoBehaviour
{
    GameObject enemyPrefab;
    GameObject enemyPrefab2;
    GameObject[] enemyPrefabs;
    float rightBounds = -2.72f;
    float leftBounds = -6.14f;
    float middleBounds = -4.43f;
    float spawnRate;
    //speed is negative for enemy car
    float enemySpeed = 3;
    float differencer = .03f;

    bool scoreActive = false;
    bool spawnActive = false;

    bool stop = false;

    // Start is called before the first frame update
    void Start()
    {
        enemyPrefabs = Resources.LoadAll<GameObject>("Enemy Cars");

        spawnRate = 5.2f / enemySpeed;

        EventManager.GameOver.AddListener(Stop);
    }

    // Update is called once per frame
    void Update()
    {
        Random.InitState((int) System.DateTime.Now.Ticks);

        if (!stop)
        {
            if (!spawnActive)
            {
                DOVirtual.DelayedCall(spawnRate, SpawnEnemy)
                    .OnStart(() =>
                    {
                        spawnActive = true;

                    })
                    .OnComplete(() =>
                    {
                        //update spawnrate
                        spawnRate = 5.2f / enemySpeed;
                        spawnActive = false;
                    });
            }
            if (!scoreActive)
            {
                DOVirtual.DelayedCall(.1f, () => GameManager.gameManager.IncreaseScore(enemySpeed))
                    .OnStart(() =>
                    {
                        scoreActive = true;

                    })
                    .OnComplete(() =>
                    {
                        scoreActive = false;
                    });
            }
        }
    }

    //1 or 2 random enemy prefabs spawn in 1 or 2 different lanes
    void SpawnEnemy()
    {
        enemySpeed = GameManager.gameManager.GetEnemySpeed();

        //generate different random speeds that are different by up to differencer*2 for both enemy cars 
        float speed1 = Random.Range(enemySpeed - (enemySpeed * differencer), enemySpeed + (enemySpeed * differencer/4));
        float speed2 = Random.Range(enemySpeed - (enemySpeed * differencer), enemySpeed + (enemySpeed * differencer/4));

        enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        enemyPrefab2 = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

        enemyPrefab.GetComponent<EnemyCar>().SetSpeed(speed1);
        enemyPrefab2.GetComponent<EnemyCar>().SetSpeed(speed2);

        int lane = Random.Range(1, 7);

        if (!stop)
        {
            //instantiate enemy prefabs in random lanes
            switch (lane)
            {
                case 1:
                    Instantiate(enemyPrefab, new Vector3(leftBounds, 9.5f, 0), Quaternion.identity);
                    enemySpeed = speed1;
                    break;
                case 2:
                    Instantiate(enemyPrefab, new Vector3(middleBounds, 9.5f, 0), Quaternion.identity);
                    enemySpeed = speed1;
                    break;
                case 3:
                    Instantiate(enemyPrefab, new Vector3(rightBounds, 9.5f, 0), Quaternion.identity);
                    enemySpeed = speed1;
                    break;
                case 4:
                    Instantiate(enemyPrefab, new Vector3(rightBounds, 9.5f, 0), Quaternion.identity);
                    Instantiate(enemyPrefab2, new Vector3(leftBounds, 9.5f, 0), Quaternion.identity);
                    //set speed to generated speeds average
                    enemySpeed = (speed1 + speed2) / 2;
                    break;
                case 5:
                    Instantiate(enemyPrefab, new Vector3(rightBounds, 9.5f, 0), Quaternion.identity);
                    Instantiate(enemyPrefab2, new Vector3(middleBounds, 9.5f, 0), Quaternion.identity);
                    enemySpeed = (speed1 + speed2) / 2;
                    break;
                case 6:
                    Instantiate(enemyPrefab, new Vector3(leftBounds, 9.5f, 0), Quaternion.identity);
                    Instantiate(enemyPrefab2, new Vector3(middleBounds, 9.5f, 0), Quaternion.identity);
                    enemySpeed = (speed1 + speed2) / 2;
                    break;

            }
        }
    }

    void Stop()
    {
        stop = true;
    }

}
