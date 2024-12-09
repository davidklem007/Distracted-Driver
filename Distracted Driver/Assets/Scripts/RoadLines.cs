using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadLines : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] GameObject RoadLines1;
    bool stop = false;
    Vector3 startPos;
    Vector3 startPos1;

    // Start is called before the first frame update
    void Start()
    {
        EventManager.GameOver.AddListener(Stop);
        startPos = transform.position;
        startPos1 = RoadLines1.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!stop)
        {
            Scroll();
        }
    }

    void Scroll()
    {
        if(Vector3.Distance(transform.position, RoadLines1.transform.position) > 10f || Vector3.Distance(transform.position, RoadLines1.transform.position) < 9.6f)
        {
            transform.position = startPos;
            RoadLines1.transform.position = startPos1;
        }
        else if (transform.position.y < -9.86f)
        {
            transform.position = new Vector3(-4.45f, 9.86f, 0);
        }
        else if(RoadLines1.transform.position.y < -9.86f)
        {
            RoadLines1.transform.position = new Vector3(-4.45f, 9.86f, 0);
        }
        else
        {
            transform.Translate(Vector2.down * speed * Time.deltaTime);
            RoadLines1.transform.Translate(Vector2.down * speed * Time.deltaTime);
        }
    }

   void Stop()
   {
        stop = true; 
   }
}
