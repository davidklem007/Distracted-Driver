using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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
        CarTile otherTile = tiles.Find(obj => obj.GetComponent<CarTile>().isClicked()).GetComponent<CarTile>();
        return otherTile;
    }

    public bool IsAdjacent(CarTile thisTile, CarTile otherTile)
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
        //thisTile is not directly perpendicularly adjacent to otherTile
        else
        {
            return false;
        }

    }

    List<GameObject> GetAdjacent(CarTile carTile)
    {
        List<GameObject> output = new List<GameObject>();

        foreach(GameObject obj in tiles)
        {
            CarTile curTile = obj.GetComponent<CarTile>();

            if (curTile.Equals(carTile))
            {
                continue;
            }
            if(IsAdjacent(carTile, curTile))
            {
                output.Add(obj);
            }
        }

        return output;
    }

    GameObject GetHorizontal(GameObject obj, int direction = 1)
    {
        CarTile carTile = obj.GetComponent<CarTile>();
        return tiles.Find(tile => tile.GetComponent<CarTile>().GetColumn() == carTile.GetColumn() + direction);
    }

    GameObject GetVertical(GameObject obj, int direction = 1)
    {
        CarTile carTile = obj.GetComponent<CarTile>();
        return tiles.Find(tile => tile.GetComponent<CarTile>().GetRow() == carTile.GetRow() + direction);
    }

    //Shake and deselect both tiles
    public void DeselectAll(CarTile tile1, CarTile tile2)
    {
        tile1.transform.DOShakePosition(0.3f, new Vector3(.07f, 0, 0), 30)
            .OnComplete(tile1.GetComponent<CarTile>().Deselect);
        tile2.transform.DOShakePosition(0.3f, new Vector3(.07f, 0, 0), 30)
            .OnComplete(tile2.GetComponent<CarTile>().Deselect);
    }

    //Tiles swap positions, and deselected
    public void SwapTiles(CarTile tile1, CarTile tile2)
    {
        GameObject objTile1 = tile1.gameObject;
        GameObject objTile2 = tile2.gameObject;

        Vector3 pos1 = objTile1.transform.position;
        int row1 = tile1.GetRow();
        int col1 = tile1.GetColumn();

        tile1.SetCar(tile2.GetRow(), tile2.GetColumn(), tile1.GetNum());
        objTile1.transform.DOMove(objTile2.transform.position, 0.2f)
            .SetEase(Ease.OutCubic)
            .OnComplete(tile1.GetComponent<CarTile>().Deselect);

        tile2.SetCar(row1, col1, tile2.GetNum());
        objTile2.transform.DOMove(pos1, 0.2f)
            .SetEase(Ease.OutCubic)
            .OnComplete(tile2.GetComponent<CarTile>().Deselect);
    }

    //if match, delete and replace with new, if not deselect all
    public List<GameObject> Matches(CarTile thisTile, HashSet<GameObject> visited = null)
    {
        if (visited == null)
        {
            visited = new HashSet<GameObject>();
        }

        List<GameObject> matches = new List<GameObject>();

        foreach (GameObject objCar in GetAdjacent(thisTile))
        {
            CarTile carTile = objCar.GetComponent<CarTile>();

            if (carTile.GetNum() == thisTile.GetNum() && !visited.Contains(objCar))
            {
                matches.Add(objCar);
                visited.Add(objCar);
            }
        }

        if(matches.Count <= 0)
        {
            return new List<GameObject>();
        }
        else
        {
            foreach (GameObject objCar in new List<GameObject>(matches))
            {

                foreach (GameObject objOtherCar in Matches(objCar.GetComponent<CarTile>(), visited))
                {
                    if (!matches.Contains(objOtherCar))
                    {
                        matches.Add(objOtherCar);
                    }
                }
            }
            return matches;
        }
    }

    public bool Match3(List<GameObject> matches)
    {
        List<GameObject> matches3 = new List<GameObject>();

        int matched = 0;

        //sort by row, then find match 3s in rows
        matches.Sort((tile1, tile2) => tile1.GetComponent<CarTile>().GetColumn().CompareTo(tile2.GetComponent<CarTile>().GetColumn()));
        foreach (GameObject g in matches)
        {
            Debug.Log("columnsort: (" + g.GetComponent<CarTile>().GetRow() + ", " + g.GetComponent<CarTile>().GetColumn() + ")");
        }

        for (int i = 0; i < matches.Count - 1; i++)
        {
            if (matches[i + 1].Equals(GetHorizontal(matches[i])) && GetHorizontal(matches[i]) != null)
            {
                if (matched == 0)
                {
                    matched += 2;
                }
                else
                {
                    matched++;
                }
            }
            else if(GetHorizontal(matches[i]) == null)
            {
                if (matched >= 3)
                {
                    for (int x = 0; x < matched; x++)
                    {
                        if (!matches3.Contains(matches[i - x]))
                        {
                            matches3.Add(matches[i - x]);
                        }
                    }
                }
                matched = 0;
            }
            else
            {
                if (matched >= 3)
                {
                    for (int x = 0; x < matched; x++)
                    {
                        if (!matches3.Contains(matches[i - x]))
                        {
                            matches3.Add(matches[i - x]);
                        }
                    }
                }
                matched = 0;
            }
        }


        //sort by column, then find match 3s in columns
        matches.Sort((tile1, tile2) => tile1.GetComponent<CarTile>().GetRow().CompareTo(tile2.GetComponent<CarTile>().GetRow()));
        foreach (GameObject g in matches)
        {
            Debug.Log("rowsort: (" + g.GetComponent<CarTile>().GetRow() + ", " + g.GetComponent<CarTile>().GetColumn() + ")");
        }

        for (int i = 0; i < matches.Count - 1; i++)
        {
            if (matches[i + 1].Equals(GetVertical(matches[i])))
            {
                if (matched == 0)
                {
                    matched += 2;
                }
                else
                {
                    matched++;
                }
            }
            else
            {
                if (matched >= 3)
                {
                    for (int x = 0; x < matched; x++)
                    {
                        if (!matches3.Contains(matches[i - x]))
                        {
                            matches3.Add(matches[i - x]);
                        }
                    }
                }
                matched = 0;
            }
        }

        foreach (GameObject g in matches3)
        {
            Debug.Log("(" + g.GetComponent<CarTile>().GetRow() + ", " + g.GetComponent<CarTile>().GetColumn() + ")");
        }

        return false;
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
