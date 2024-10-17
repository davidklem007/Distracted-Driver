using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCar : MonoBehaviour
{
    [SerializeField] float speed;
    bool stop = false;

    // Start is called before the first frame update
    void Start()
    {
        EventManager.GameOver.AddListener(Stop);
    }

    // Update is called once per frame
    void Update()
    {
        if (!stop)
        {
            transform.Translate(new Vector3(0, -speed, 0) * Time.deltaTime);
        }
        if(transform.position.y < -6)
        {
            Destroy(gameObject);
        }
    }

    void Stop()
    {
        stop = true;
    }
}
