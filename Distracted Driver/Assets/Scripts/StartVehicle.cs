using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class StartVehicle : MonoBehaviour
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
        Debug.Log("aosijnvfdnb");
        transform.localScale = new Vector3(scale * 1.15f, scale * 1.15f, scale * 1.15f);

        if (Input.GetButtonDown("Select"))
        {

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

    public void Select()
    {
        clicked = true;
        TileManager.tileManager.amtClicked++;
        gameObject.GetComponent<Renderer>().material.SetColor("_Color", new Color(1, 1, 1, .75f));
    }

    public void Deselect()
    {
        clicked = false;
        TileManager.tileManager.amtClicked--;
        if(TileManager.tileManager.amtClicked < 0)
        {
            TileManager.tileManager.amtClicked = 0;
        }
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
