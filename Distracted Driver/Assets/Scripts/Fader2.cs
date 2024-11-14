using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class Fader2 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Image>().DOFade(0, .5f)
            .SetEase(Ease.InExpo)
            .OnKill(() =>
            {
                gameObject.SetActive(false);
            });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
