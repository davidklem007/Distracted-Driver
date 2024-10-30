using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{

    //[rows][columns]
    public List<GameObject> tiles;
    public int amtClicked = 0;

    [SerializeField] int rows;
    [SerializeField] int columns;
    GameObject[] tilePrefabs;
    [SerializeField] GameObject bounds;

    [SerializeField] float spacing;
    [SerializeField] float xOffset;
    [SerializeField] float yOffset;
    System.Predicate<GameObject> clicked;

    public static TileManager tileManager;

    private void Awake()
    {
        tileManager = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        tilePrefabs = Resources.LoadAll<GameObject>("Car Tiles");

        tiles = new List<GameObject>();

        clicked = (GameObject g) =>
        {
            return g.GetComponent<CarTile>().isClicked();
        };

        GridAssign();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void GridAssign()
    {
        //gets x distance and y distance of bounds, taking into account x and y offsets
        float xBounds = Vector3.Distance(new Vector3(bounds.transform.position.x - xOffset, transform.position.y - yOffset, transform.position.z), Offset(transform.position));
        float yBounds = Vector3.Distance(new Vector3(transform.position.x + xOffset, bounds.transform.position.y + yOffset, transform.position.z), Offset(transform.position));


        //spacing intervals for x and y
        float xSpacing = xBounds / (columns - 1);
        float ySpacing = yBounds / (rows - 1);

        //chooses random car tile prefab to instantiate
        //instantiates all car tiles evenly
        //adds each car tile to the tiles list and sets their row and column
        for (int i = 0; i < rows; i++)
        { 
            for(int j = 0; j < columns; j++)
            {
                int carNum = Random.Range(0, tilePrefabs.Length);

                Vector3 position = transform.TransformPoint(new Vector3((j * xSpacing) + xOffset, (-i * ySpacing) - yOffset, 0));

                GameObject carTile = Instantiate(tilePrefabs[carNum], position, Quaternion.identity);

                carTile.GetComponent<CarTile>().SetCar(i, j, carNum);

                tiles.Add(carTile);
            }
        }
    }

    Vector3 Offset(Vector3 vector)
    {
        return new Vector3(vector.x + xOffset, vector.y - yOffset, vector.x);
    }

    

    //find clicked car tile
    public CarTile FindOther()
    {
        CarTile otherTile = tiles.Find(clicked).GetComponent<CarTile>();
        return otherTile;
    }

    public bool isAdjacent(CarTile thisTile, CarTile otherTile)
    {
        int thisColumn = thisTile.GetColumn();
        int thisRow = thisTile.GetRow();
        int otherColumn = otherTile.GetColumn();
        int otherRow = otherTile.GetRow();

        //if thisTile in the same column and one row above or below of otherTile
        if (thisColumn == otherColumn && (thisRow == otherRow + 1 || thisRow == otherRow - 1))
        {
            return true;
        }
        //if thisTile in same row and one column to the left or right of otherTile
        else if (thisRow == otherRow && (thisColumn == otherColumn + 1 || thisColumn == otherColumn - 1))
        {
            return true;
        }
        //thisTile is not directly perpendicularly adjacent to otherTile, deselect all
        {
            StartCoroutine(DeselectAll(thisTile, otherTile));
            return false;
        }


    }

    //if match, delete and replace with new, if not deselect all
    public bool CheckMatch(CarTile tile1, CarTile tile2)
    {
        if(tile1.GetNum() == tile2.GetNum())
        {
            StartCoroutine(ReplaceTile(tile1.gameObject));
            StartCoroutine(ReplaceTile(tile2.gameObject));
            return true;
        }
        else
        {
            StartCoroutine(DeselectAll(tile1, tile2));
            return false;
        }
    }

    public IEnumerator DeselectAll(CarTile tile1, CarTile tile2)
    {
        yield return new WaitForSeconds(0.5f);
        tile1.GetComponent<CarTile>().Deselect();
        tile2.GetComponent<CarTile>().Deselect();
    }

    //delete tile input and replace with new tile
    IEnumerator ReplaceTile(GameObject tile)
    {
        Vector3 position = tile.transform.position;
        int row = tile.GetComponent<CarTile>().GetRow();
        int col = tile.GetComponent<CarTile>().GetColumn();

        yield return new WaitForSeconds(0.5f);

        tiles.Remove(tile);
        tile.GetComponent<CarTile>().Deselect();
        Destroy(tile);

        yield return new WaitForSeconds(0.5f);

        int carNum = Random.Range(0, tilePrefabs.Length);

        GameObject carTile = Instantiate(tilePrefabs[carNum], position, Quaternion.identity);

        carTile.GetComponent<CarTile>().SetCar(row, col, carNum);

        tiles.Add(carTile);
    }


}
