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

                carTile.GetComponent<CarTile>().SetPosition(i, j);

                tiles.Add(carTile);
            }
        }
    }

    Vector3 Offset(Vector3 vector)
    {
        return new Vector3(vector.x + xOffset, vector.y - yOffset, vector.x);
    }

    public bool isAdjacent(CarTile newTile)
    {
        CarTile oldTile = tiles.Find(clicked).GetComponent<CarTile>();
        int newColumn = newTile.GetColumn();
        int newRow = newTile.GetRow();
        int oldColumn = oldTile.GetColumn();
        int oldRow = oldTile.GetRow();

        //if newTile in the same column and one row above or below of oldTile
        if (newColumn == oldColumn && (newRow == oldRow + 1 || newRow == oldRow - 1))
        {
            return true;
        }
        //if newTile in same row and one column to the left or right of oldTile
        else if (newRow == oldRow && (newColumn == oldColumn + 1 || newColumn == oldColumn - 1))
        {
            return true;
        }
        //newTile is not directly perpendicularly adjacent to oldTile
        {
            return false;
        }


    }

    bool IsMatch(CarTile tile1, CarTile tile2)
    {
        return false;
    }

    public IEnumerator DeselectAll()
    {
        yield return new WaitForSeconds(0.5f);
        foreach(GameObject g in tiles.FindAll(clicked))
        {
            g.GetComponent<CarTile>().Deselect();
        }
    }

}
