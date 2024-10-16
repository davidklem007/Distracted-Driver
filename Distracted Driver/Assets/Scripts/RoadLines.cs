using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadLines : MonoBehaviour
{
    [SerializeField] float speed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Scroll();
        if(transform.position.y < -9.86f)
        {
            transform.position = new Vector3(-4.45f, 9.86f, 0);
        }
    }

    void Scroll()
    {
        transform.Translate(Vector2.down * speed * Time.deltaTime);
    }
}
