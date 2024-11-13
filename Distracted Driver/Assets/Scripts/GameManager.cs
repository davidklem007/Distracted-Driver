using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{

    [SerializeField] GameObject restartButton;
    [SerializeField] TextMeshProUGUI scoreText;
    int score = 0;
    float enemySpeed = 3;

    bool stop = false;

    public static GameManager gameManager;

    private void Awake()
    {
        gameManager = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        EventManager.GameOver.AddListener(GameOver);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void GameOver()
    {
        stop = true;
        restartButton.SetActive(true);
    }

    public void Restart()
    {
        if (!TileManager.tileManager.moving)
        {
            SceneManager.LoadScene("Main Scene");
        }
    }

    public int IncreaseScore()
    {
        if (!stop)
        {
            for (int i = 0; i < (int) enemySpeed; i++)
            {
                score++;
                scoreText.text = string.Format("Score: {0}       Speed: {1:#.00}x", score, enemySpeed/3);
                if (score % 100 == 0 && score > 150)
                {
                    Debug.Log("yurrr");
                    IncreaseSpeed(0.05f);
                }
            }

        }

        return score;
    }

    public float DecreaseSpeed(float increment = 0.03f)
    {
        enemySpeed -= enemySpeed * increment;

        if (enemySpeed < 3) enemySpeed = 3;

        GameObject[] enemis = GameObject.FindGameObjectsWithTag("Enemy");

        foreach(GameObject obj in enemis)
        {
            obj.GetComponent<EnemyCar>().SetSpeed(enemySpeed);
        }

        return enemySpeed;
    }

    public float IncreaseSpeed(float increment = 0.25f)
    {
        enemySpeed += enemySpeed * increment;

        GameObject[] enemis = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject obj in enemis)
        {
            obj.GetComponent<EnemyCar>().SetSpeed(enemySpeed);
        }

        return enemySpeed;
    }

    public int GetScore()
    {
        return score;
    }

    public float GetEnemySpeed()
    {
        return enemySpeed;
    }
}
