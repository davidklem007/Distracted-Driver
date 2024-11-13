using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreText1;
    [SerializeField] TextMeshProUGUI scoreText2;
    static List<int> scores = null;
    int curScore = 0;

    public static MenuManager menuManager;

    private void Awake()
    {
        menuManager = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
        if(scores == null)
        {
            scores = new List<int>();

            for (int i = 0; i < 10; i++)
            {
                scores.Add(0);
            }
        }

        UpdateScores(GameManager.gameManager.GetScore());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateScores(int score)
    {
        curScore = score;

        int oldScore = scores.Find(num => num < curScore);

        Debug.Log(scores.IndexOf(oldScore));

        scores.Insert(scores.IndexOf(oldScore), curScore);

        scoreText1.text = string.Format("	   High Scores{0}1. {1}{0}2. {2}{0}3. {3}{0}4. {4}{0}5. {5}{0}	   You got {6}", System.Environment.NewLine, scores[0], scores[1], scores[2], scores[3], scores[4], curScore);
        scoreText2.text = string.Format("{0}6. {1}{0}7. {2}{0}8. {3}{0}9. {4}{0}10. {5}{0}", System.Environment.NewLine, scores[5], scores[6], scores[7], scores[8], scores[9]);
    }

    public void Restart()
    {
        SceneManager.LoadScene("Main Scene");
    }
}
