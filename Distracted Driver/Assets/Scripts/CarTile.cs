using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarTile : MonoBehaviour
{
    TileManager tileManager;
    bool clicked = false;
    [SerializeField] float scale;
    [SerializeField] int column;
    [SerializeField] int row;
    [SerializeField] int num;
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
                //if tile clicked is second tile selected, check if adjancent and select or deselect accordingly
                if(amtClicked == 1)
                {
                    CarTile otherTile = TileManager.tileManager.FindOther();

                    if (TileManager.tileManager.isAdjacent(this, otherTile))
                    {
                        if(TileManager.tileManager.IsMatch(this, otherTile))
                        {
                            Debug.Log("U got a match :D");
                            clicked = true;
                            TileManager.tileManager.amtClicked++;
                            gameObject.GetComponent<Renderer>().material.SetColor("_Color", new Color(1, 1, 1, .75f));
                        }
                        else
                        {
                            Debug.Log("Not a Match :(");
                            clicked = true;
                            TileManager.tileManager.amtClicked++;
                            gameObject.GetComponent<Renderer>().material.SetColor("_Color", new Color(1, 1, 1, .75f));
                            StartCoroutine(TileManager.tileManager.DeselectAll(this, otherTile));
                        }

                    }
                    else
                    {
                        clicked = true;
                        TileManager.tileManager.amtClicked++;
                        gameObject.GetComponent<Renderer>().material.SetColor("_Color", new Color(1, 1, 1, .75f));
                        StartCoroutine(TileManager.tileManager.DeselectAll(this, otherTile));
                    }

                }
                //if no other tiles clicked, select tile
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

    public void SetCar(int ro, int col, int nu)
    {
        column = col;
        row = ro;
        num = nu;
    }

    public int GetRow()
    {
        return row;
    }

    public int GetColumn()
    {
        return column;
    }

    //returns car color/type number
    public int GetNum()
    {
        return num;
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
