using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarTile : MonoBehaviour
{
    bool clicked = false;
    [SerializeField] float scale;

    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = new Vector3(scale, scale, scale);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnMouseOver()
    {
        transform.localScale = new Vector3(scale * .875f, scale * .875f, scale*.875f);
        gameObject.GetComponent<Renderer>().material.SetColor("_Color", new Color(1.0f, 1.0f, 1.0f, 0.75f));

        if (Input.GetButtonDown("Select"))
        {
            if (clicked)
            {
                clicked = false;
            }
            else
            {
                clicked = true;
            }
        }
    }

    private void OnMouseExit()
    {

        if (!clicked)
        {
            transform.localScale = new Vector3(scale, scale, scale);
            gameObject.GetComponent<Renderer>().material.SetColor("_Color", new Color(1.0f, 1.0f, 1.0f, 1.0f));
        }
    }
}
