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
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public int IncreaseScore(float enemSped)
    {
        if (!stop)
        {
            for (int i = 0; i < (int) enemSped; i++)
            {
                score++;
                scoreText.text = string.Format("Score: {0}       Speed: {1:#.00}x", score, enemySpeed - 2);
                if (score % 250 == 0 && score > 10)
                {
                    Debug.Log("yurrr");
                    enemySpeed = enemSped + (enemSped * .15f);

                }
            }

        }

        return score;
    }

    public float DecreaseSpeed(float increment = 0.03f)
    {
        enemySpeed -= enemySpeed * increment;

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
