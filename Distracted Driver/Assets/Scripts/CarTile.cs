using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarTile : MonoBehaviour
{
    [SerializeField] GameObject tileManagerObj;
    TileManager tileManager;
    bool clicked = false;
    [SerializeField] float scale;
    [SerializeField] int column;
    [SerializeField] int row;
    List<GameObject> tiles;
    int amtClicked;
    int index;

    // Start is called before the first frame update
    void Start()
    {
        tiles = TileManager.tileManager.tiles;

        index = tiles.IndexOf(gameObject);
        amtClicked = TileManager.tileManager.amtClicked;

        //Set size of tiles
        transform.localScale = new Vector3(scale, scale, scale);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Bart :)  " + amtClicked);
    }

    private void OnMouseOver()
    {
        transform.localScale = new Vector3(scale * 1.15f, scale * 1.15f, scale * 1.15f);

        if (Input.GetButtonDown("Select"))
        {
            amtClicked = TileManager.tileManager.amtClicked;
            if (clicked)
            {
                clicked = false;
                TileManager.tileManager.amtClicked--;
                gameObject.GetComponent<Renderer>().material.SetColor("_Color", new Color(1.0f, 1.0f, 1.0f, 1.0f));
            }
            else if(!clicked && amtClicked <= 1)
            {
                if(amtClicked == 1)
                {
                    if (TileManager.tileManager.isAdjacent(this))
                    {
                        clicked = true;
                        TileManager.tileManager.amtClicked++;
                        gameObject.GetComponent<Renderer>().material.SetColor("_Color", new Color(1, 1, 1, .75f));
                    }
                    else
                    {
                        clicked = true;
                        TileManager.tileManager.amtClicked++;
                        gameObject.GetComponent<Renderer>().material.SetColor("_Color", new Color(1, 1, 1, .75f));
                        StartCoroutine(TileManager.tileManager.DeselectAll());
                    }

                }
                else
                {
                    clicked = true;
                    TileManager.tileManager.amtClicked++;
                    gameObject.GetComponent<Renderer>().material.SetColor("_Color", new Color(1, 1, 1, .75f));
                }
            }
        }
    }

    private void OnMouseExit()
    {

        transform.localScale = new Vector3(scale, scale, scale);
    }

    public void SetPosition(int ro, int col)
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

    public void Deselect()
    {
        clicked = false;
        TileManager.tileManager.amtClicked--;
        gameObject.GetComponent<Renderer>().material.SetColor("_Color", new Color(1.0f, 1.0f, 1.0f, 1.0f));
    }

    public bool isClicked()
    {
        return clicked;
    }
}
