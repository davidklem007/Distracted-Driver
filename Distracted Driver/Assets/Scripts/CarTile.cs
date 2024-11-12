using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CarTile : MonoBehaviour
{
    bool clicked = false;
    [SerializeField] float scale;
    [SerializeField] int row;
    [SerializeField] int column;
    [SerializeField] int num;

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
        transform.localScale = new Vector3(scale * 1.15f, scale * 1.15f, scale * 1.15f);

        if (Input.GetButtonDown("Select") && !TileManager.tileManager.moving && !TileManager.tileManager.stop)
        {
            //if tile is selected and clicked on, deselect it
            if (clicked)
            {
                Deselect();
            }
            //if tile isn't already selected and clicked on, and up to one other tile is selected
            else if(!clicked && TileManager.tileManager.amtClicked <= 1)
            {
                //if tile clicked is second tile selected
                if(TileManager.tileManager.amtClicked == 1)
                {
                    //get other tile that's selected
                    CarTile otherTile = TileManager.tileManager.FindOther();

                    //if tile is adjacent swap tiles
                    //then get list of all matching tiles, then use that list to get match 3s
                    if (TileManager.tileManager.IsAdjacent(this, otherTile))
                    {
                        Select();
                        Tween swap1 = TileManager.tileManager.SwapTiles(this, otherTile);

                        List<GameObject> otherMatch3 = TileManager.tileManager.Match3(TileManager.tileManager.AdjacentMatches(otherTile));
                        List<GameObject> thisMatch3 = TileManager.tileManager.Match3(TileManager.tileManager.AdjacentMatches(this));

                        /*
                        foreach (GameObject g in otherMatch3)
                        {
                            Debug.Log("other: (" + g.GetComponent<CarTile>().GetRow() + ", " + g.GetComponent<CarTile>().GetColumn() + ")");
                        }

                        foreach (GameObject g in thisMatch3)
                        {
                            Debug.Log("this: (" + g.GetComponent<CarTile>().GetRow() + ", " + g.GetComponent<CarTile>().GetColumn() + ")");
                        }

                        */
                        
                        if(otherMatch3.Count == 0 && thisMatch3.Count == 0)
                        {
                            DOVirtual.DelayedCall(0.3f, () => TileManager.tileManager.SwapTiles(this, otherTile));
                        }
                        else
                        {
                            StartCoroutine(ReplaceMatch3sAndDecreaseSpeed(0.3f, swap1));
                        }
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

    IEnumerator ReplaceMatch3sAndDecreaseSpeed(float delay, Tween tween)
    {
        Debug.Log("1 (" + row + ", " + column + ")");
        yield return StartCoroutine(TileManager.tileManager.ReplaceMatch3s(delay, tween));
        Debug.Log("2 (" + row + ", " + column + ")");
        for (int i = 0; i < TileManager.tileManager.GetMatchesCount(); i++)
        {
            GameManager.gameManager.DecreaseSpeed();
            Debug.Log("Sped decreaz (" + row + ", " + column + ")");
        }
        Debug.Log("3 (" + row + ", " + column + ")");
    }
}
