using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Fader : MonoBehaviour
{
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
        gameObject.GetComponent<Image>().DOFade(1, 1f)
            .OnKill(() =>
            {
                GameManager.gameManager.Menu();
            });
    }




}
