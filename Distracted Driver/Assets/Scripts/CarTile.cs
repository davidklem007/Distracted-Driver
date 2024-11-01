using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CarTile : MonoBehaviour
{
    TileManager tileManager;
    bool clicked = false;
    [SerializeField] float scale;
    [SerializeField] int row;
    [SerializeField] int column;
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
            //get number of selected tiles
            amtClicked = TileManager.tileManager.amtClicked;
            //if tile is selected and clicked on, deselect it
            if (clicked)
            {
                Deselect();
            }
            //if tile isn't already selected and clicked on, and up to one other tile is selected
            else if(!clicked && amtClicked <= 1)
            {
                //if tile clicked is second tile selected
                if(amtClicked == 1)
                {
                    //get other tile that's selected
                    CarTile otherTile = TileManager.tileManager.FindOther();

                    //if tile is adjacent, get list of all matching tiles, then use that list to get match 3s
                    if (TileManager.tileManager.IsAdjacent(this, otherTile))
                    {
                        Select();
                        TileManager.tileManager.SwapTiles(this, otherTile);
                        
                        TileManager.tileManager.Match3(TileManager.tileManager.Matches(otherTile));
                        TileManager.tileManager.Match3(TileManager.tileManager.Matches(this));
                    }
                    //if this tile is not adjacent, deselect all
                    else
                    {
                        Select();
                        DOVirtual.DelayedCall(0.3f, ()=> TileManager.tileManager.DeselectAll(this, otherTile));
                    }

                }
                //if no other tiles clicked, select tile
                else
                {
                    Select();
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

    void Select()
    {
        clicked = true;
        TileManager.tileManager.amtClicked++;
        gameObject.GetComponent<Renderer>().material.SetColor("_Color", new Color(1, 1, 1, .75f));
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

    public bool Equals(CarTile other)
    {
        return row == other.GetRow() && column == other.GetColumn() && num == other.GetNum();
    }
}
