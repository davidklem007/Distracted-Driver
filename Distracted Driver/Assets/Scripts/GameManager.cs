using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class GameManager : MonoBehaviour
{

    [SerializeField] GameObject restartButton;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] Image snail;
    [SerializeField] Image fast;

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
    }

    public void Menu()
    {
        DOTween.CompleteAll();
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }

    public int IncreaseScore()
    {
        if (!stop)
        {
            for (int i = 0; i < (int) enemySpeed; i++)
            {
                score++;
                scoreText.text = string.Format("Score: {0}       Speed: {1:#00}mph", score, enemySpeed/3*60);
                if (score % 15 == 0 && score > 100)
                {
                    IncreaseSpeed(0.05f, false);
                }
            }

        }

        return score;
    }

    public float DecreaseSpeed(float increment = 0.03f)
    {
        increment += ((enemySpeed / 3) - 1) / 25;

        enemySpeed -= enemySpeed * increment;

        if (enemySpeed < 3) enemySpeed = 3;

        GameObject[] enemis = GameObject.FindGameObjectsWithTag("Enemy");

        foreach(GameObject obj in enemis)
        {
            obj.GetComponent<EnemyCar>().SetSpeed(enemySpeed);
        }

        if (!stop)
        {
            Sequence fade = DOTween.Sequence().Pause();

            fade.Append(snail.DOFade(1, .3f).SetEase(Ease.OutSine));
            fade.AppendInterval(.5f);
            fade.Append(snail.DOFade(0, .3f).SetEase(Ease.OutSine));

            fade.Play();
        }

        return enemySpeed;
    }

    public float IncreaseSpeed(float increment = 0.25f, bool exp = true)
    {
        if (exp)
        {
            enemySpeed += enemySpeed * increment;
        }
        else
        {
            enemySpeed += increment;
        }

        GameObject[] enemis = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject obj in enemis)
        {
            obj.GetComponent<EnemyCar>().SetSpeed(enemySpeed);
        }

        if (!stop && increment >= 0.3f)
        {
            Sequence fade = DOTween.Sequence().Pause();

            fade.Append(fast.DOFade(1, .3f).SetEase(Ease.OutSine));
            fade.AppendInterval(.5f);
            fade.Append(fast.DOFade(0, .3f).SetEase(Ease.OutSine));

            fade.Play();
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
