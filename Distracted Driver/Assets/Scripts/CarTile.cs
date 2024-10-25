using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarTile : MonoBehaviour
{
    bool clicked = false;
    [SerializeField] float scale;
    [SerializeField] int column;
    [SerializeField] int row;

    // Start is called before the first frame update
    void Start()
    {
        //Set size of tiles
        transform.localScale = new Vector3(scale, scale, scale);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnMouseOver()
    {
        transform.localScale = new Vector3(scale * .875f, scale * .875f, scale * .875f);

        if (Input.GetButtonDown("Select"))
        {
            if (clicked)
            {
                clicked = false;
                gameObject.GetComponent<Renderer>().material.SetColor("_Color", new Color(1.0f, 1.0f, 1.0f, 1.0f));
            }
            else
            {
                clicked = true;
                gameObject.GetComponent<Renderer>().material.SetColor("_Color", new Color(1, 1, 1, .75f));
            }
        }
    }

    private void OnMouseExit()
    {

        transform.localScale = new Vector3(scale, scale, scale);
    }

    public void SetPosition(int col, int ro)
    {
        column = col;
        row = ro;
    }

    public int GetRow()
    {
        return row;
    }

    public int GetColumn()
    {
        return column;
    }
}
