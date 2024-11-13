using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TileManager : MonoBehaviour
{

    //[rows][columns]
    public List<GameObject> tiles;
    public int amtClicked = 0;
    public bool moving = false;
    public bool stop = false;

    int match3sCount = 0;

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
        EventManager.GameOver.AddListener(Stop);

        tilePrefabs = Resources.LoadAll<GameObject>("Car Tiles");

        tiles = new List<GameObject>();

        GridAssign();
    }

    // Update is called once per frame
    void Update()
    {
        if(amtClicked == 2)
        {
            amtClicked = 0;
            StartCoroutine(ManageClicks());
        }
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

        //remove match 3s
        StartCoroutine(ReplaceMatch3sAtStart());
        match3sCount = 0;
    }

    Vector3 Offset(Vector3 vector)
    {
        return new Vector3(vector.x + xOffset, vector.y - yOffset, vector.x);
    }

    //find clicked car tile
    CarTile FindOther()
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

    public int GetMatchesCount()
    {
        int temp = match3sCount;
        match3sCount = 0;
        return temp;
    }

    //Returns list of Cartile gameobjects next to given cartile
    //loops through all tiles, adding adjacent ones to the list, and skips the given cartile
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

    GameObject GetHorizontalAdjacent(GameObject obj, int direction = 1)
    {
        CarTile carTile = obj.GetComponent<CarTile>();
        //Find tile in column + direction, and in same row
        return tiles.Find(tile => tile.GetComponent<CarTile>().GetColumn() == carTile.GetColumn() + direction && tile.GetComponent<CarTile>().GetRow() == carTile.GetRow());
    }

    GameObject GetVerticalAdjacent(GameObject obj, int direction = 1)
    {
        CarTile carTile = obj.GetComponent<CarTile>();
        //Find tile in row + direction, and in same column
        return tiles.Find(tile => tile.GetComponent<CarTile>().GetRow() == carTile.GetRow() + direction && tile.GetComponent<CarTile>().GetColumn() == carTile.GetColumn());
    }

    //Shake and deselect both tiles
    Sequence DeselectAll(CarTile tile1 = null, CarTile tile2 = null)
    {
        Sequence deselect = DOTween.Sequence()
            .OnStart(() =>
            {
                moving = true;
            })
            .OnComplete(() =>
            {
                tile1.GetComponent<CarTile>().Deselect();
                tile2.GetComponent<CarTile>().Deselect();
                moving = false;
            });

        deselect.Insert(0, tile1.transform.DOShakePosition(0.3f, new Vector3(.07f, 0, 0), 30));

        deselect.Insert(0, tile2.transform.DOShakePosition(0.3f, new Vector3(.07f, 0, 0), 30));

        return deselect;
    }

    //Tiles swap positions, and deselected
    Sequence SwapTiles(CarTile tile1, CarTile tile2, bool kill = true)
    {
        GameObject objTile1 = tile1.gameObject;
        GameObject objTile2 = tile2.gameObject;

        Vector3 pos1 = objTile1.transform.position;
        int row1 = tile1.GetRow();
        int col1 = tile1.GetColumn();

        tile1.SetCar(tile2.GetRow(), tile2.GetColumn(), tile1.GetNum());
        tile2.SetCar(row1, col1, tile2.GetNum());

        Sequence move = DOTween.Sequence().Pause()
            .OnStart(() =>
            {
                moving = true;
            })
            .OnComplete(() =>
            {
                tile1.GetComponent<CarTile>().Deselect();
                tile2.GetComponent<CarTile>().Deselect();
            })
            .SetAutoKill(kill);

        move.Insert(0, objTile1.transform.DOMove(objTile2.transform.position, 0.2f).SetEase(Ease.OutCubic));

        move.Insert(0, objTile2.transform.DOMove(pos1, 0.2f).SetEase(Ease.OutCubic));



        return move;
    }

    //returns list of all connected, matching tiles of thisTile
    public List<GameObject> AdjacentMatches(CarTile thisTile, HashSet<GameObject> visited = null)
    {
        //sets up HashSet to keep track of all tiles checked
        if (visited == null)
        {
            visited = new HashSet<GameObject>();
        }

        List<GameObject> matches = new List<GameObject>();

        //loops through all adjacent tiles
        foreach (GameObject objCar in GetAdjacent(thisTile))
        {
            CarTile carTile = objCar.GetComponent<CarTile>();

            //if the num is the same and tile has not been checked, add to visited and total matches list
            if (carTile.GetNum() == thisTile.GetNum() && !visited.Contains(objCar))
            {
                matches.Add(objCar);
                visited.Add(objCar);
            }
        }

        if(matches.Count <= 0)
        {
            return matches;
        }
        else
        {
            //For every adjacent match, check their adjacent matches and so on
            foreach (GameObject objCar in new List<GameObject>(matches))
            {

                foreach (GameObject objOtherCar in AdjacentMatches(objCar.GetComponent<CarTile>(), visited))
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

    //returns list of all tiles that are a set of 3 or more in the list given
    List<GameObject> Match3rd(List<GameObject> matches)
    {
        List<GameObject> matches3 = new List<GameObject>();

        //counts tiles for a set of matched tiles
        int matched = 0;

        //Sort row by row, with columns in order
        matches.Sort((tile1, tile2) =>
        {
            CarTile carTile1 = tile1.GetComponent<CarTile>();
            CarTile carTile2 = tile2.GetComponent<CarTile>();

            //compare by row
            int rowComparison = carTile1.GetRow().CompareTo(carTile2.GetRow());

            if (rowComparison == 0)
            {
                //if rows are the same, compare by column
                return carTile1.GetColumn().CompareTo(carTile2.GetColumn());
            }
            else
            {
                //if not same row return the row comparison result
                return rowComparison;
            }
        });

        //After matches sorted by row, go through, checking 2 matches at a time
        for (int i = 0; i < matches.Count - 1; i++)
        {
            //get horizontally adjacent tile
            GameObject horizontal = GetHorizontalAdjacent(matches[i]);

            //if there is a horizontally adjacent tile 
            if (horizontal != null)
            {
                //check if the horizontally adjacent tile is the same as the next tile in the list of matching tiles
                if (matches[i + 1].GetComponent<CarTile>().Equals(GetHorizontalAdjacent(matches[i]).GetComponent<CarTile>()))
                {
                    //if there are no previous matching tiles in the row connected to the current tile,
                    //add 2 to matched tiles count since we are counting 2 at a time we count those two tiles
                    //if there are already matching tiles in the set, just add 1 because of the 2 that are checked, one of them was already counted for
                    if (matched == 0)
                    {
                        matched += 2;
                    }
                    else
                    {
                        matched++;
                    }
                }
                //if the two tiles checked aren't matching and in the same row
                else
                {
                    //this is to check the tiles that came before
                    //if there were at least three matching tiles before, add all of them to the matches3 list if they are not already in that list
                    //then reset the matched count because we will check a new row next
                    if (matched >= 3)
                    {
                        match3sCount++;
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
            //if there isn't a horizontally adjacent tile (current tile is on an edge)
            else
            {
                //since we are at the end of the row now, add previous tiles to the match3 list if there was a match3
                //and reset matched count to zero because we will start in a new row next
                if (matched >= 3)
                {
                    match3sCount++;
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

        //after going through all the tiles, if there was a match3 at the end of the row this checks that and adds the matching tiles
        if (matched >= 3)
        {
            match3sCount++;
            for (int x = 0; x < matched; x++)
            {
                if (!matches3.Contains(matches[matches.Count - 1 - x]))
                {
                    matches3.Add(matches[matches.Count - 1 - x]);
                }
            }
        }

        //reset the count to check by column now
        matched = 0;


        //sort column by column, with rows in order
        matches.Sort((tile1, tile2) =>
        {
            CarTile carTile1 = tile1.GetComponent<CarTile>();
            CarTile carTile2 = tile2.GetComponent<CarTile>();

            //compare by column
            int columnComparison = carTile1.GetColumn().CompareTo(carTile2.GetColumn());

            if (columnComparison == 0)
            {
                //if columns are the same, compare by row
                return carTile1.GetRow().CompareTo(carTile2.GetRow());
            }
            else
            {
                //if not the same column return the column comparison result
                return columnComparison;
            }
        });

        //basically the same as the row check but going by column and using vertically adjacent tiles instead
        for (int i = 0; i < matches.Count - 1; i++)
        {
            GameObject vertical = GetVerticalAdjacent(matches[i]);

            if(vertical != null)
            {
                if (matches[i + 1].GetComponent<CarTile>().Equals(GetVerticalAdjacent(matches[i]).GetComponent<CarTile>()))
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
                        match3sCount++;
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
            else
            {
                if (matched >= 3)
                {
                    match3sCount++;
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

        if (matched >= 3)
        {
            for (int x = 0; x < matched; x++)
            {
                match3sCount++;
                if (!matches3.Contains(matches[matches.Count - 1 - x]))
                {
                    matches3.Add(matches[matches.Count - 1 - x]);
                }
            }
        }

        /*
        Debug.Log("count3: " + matches3.Count);

        foreach (GameObject g in matches3)
        {
            Debug.Log("(" + g.GetComponent<CarTile>().GetRow() + ", " + g.GetComponent<CarTile>().GetColumn() + ")");
        }
        */

        return matches3;
    }

    List<GameObject> Match3(List<GameObject> matches)
    {
        List<GameObject> matches3 = new List<GameObject>();
        int matched = 0;

        // Sort by row, then by column
        matches.Sort((tile1, tile2) =>
        {
            CarTile carTile1 = tile1.GetComponent<CarTile>();
            CarTile carTile2 = tile2.GetComponent<CarTile>();
            int rowComparison = carTile1.GetRow().CompareTo(carTile2.GetRow());
            return rowComparison == 0 ? carTile1.GetColumn().CompareTo(carTile2.GetColumn()) : rowComparison;
        });

        // Horizontal check for groups of 3 or more
        for (int i = 0; i < matches.Count - 1; i++)
        {
            GameObject horizontal = GetHorizontalAdjacent(matches[i]);

            if (horizontal != null && matches[i + 1] == horizontal)
            {
                matched = (matched == 0) ? 2 : matched + 1;
            }
            else
            {
                if (matched >= 3)
                {
                    AddMatchGroup(matches, matches3, i, matched);
                }
                matched = 0;
            }
        }
        if (matched >= 3) AddMatchGroup(matches, matches3, matches.Count - 1, matched);
        matched = 0;

        // Sort by column, then by row for vertical check
        matches.Sort((tile1, tile2) =>
        {
            CarTile carTile1 = tile1.GetComponent<CarTile>();
            CarTile carTile2 = tile2.GetComponent<CarTile>();
            int colComparison = carTile1.GetColumn().CompareTo(carTile2.GetColumn());
            return colComparison == 0 ? carTile1.GetRow().CompareTo(carTile2.GetRow()) : colComparison;
        });

        // Vertical check for groups of 3 or more
        for (int i = 0; i < matches.Count - 1; i++)
        {
            GameObject vertical = GetVerticalAdjacent(matches[i]);

            if (vertical != null && matches[i + 1] == vertical)
            {
                matched = (matched == 0) ? 2 : matched + 1;
            }
            else
            {
                if (matched >= 3)
                {
                    AddMatchGroup(matches, matches3, i, matched);
                }
                matched = 0;
            }
        }
        if (matched >= 3) AddMatchGroup(matches, matches3, matches.Count - 1, matched);

        return matches3;
    }

    // Helper function to add matches and increment count
    private void AddMatchGroup(List<GameObject> matches, List<GameObject> matches3, int endIndex, int matchedCount)
    {
        match3sCount++;
        for (int x = 0; x < matchedCount; x++)
        {
            if (!matches3.Contains(matches[endIndex - x]))
            {
                matches3.Add(matches[endIndex - x]);
            }
        }
    }


    //sequence for replacing a tile
    Sequence ReplaceSequence(GameObject tile, float delay = 0)
    {
        //get position and coordinates of tile
        Vector3 position = tile.transform.position;
        int row = tile.GetComponent<CarTile>().GetRow();
        int col = tile.GetComponent<CarTile>().GetColumn();

        //create sequence
        Sequence replace = DOTween.Sequence().Pause();

        replace.Append(
            //remove tile from tiles list, then make sure it's deselecte to account for amtClicked, then destroy
            DOVirtual.DelayedCall(delay, () =>
            {
                tiles.Remove(tile);
                if (tile.GetComponent<CarTile>().isClicked())
                {
                    tile.GetComponent<CarTile>().Deselect();
                }
                Destroy(tile);
            })
        );

        //spawn random new tile with same coordinates and position as old tile
        replace.Append(
            DOVirtual.DelayedCall(delay, () =>
            {
                int carNum = Random.Range(0, tilePrefabs.Length);
                GameObject carTile = Instantiate(tilePrefabs[carNum], position, Quaternion.identity);
                carTile.GetComponent<CarTile>().SetCar(row, col, carNum);
                tiles.Add(carTile);
            })
        );

        return replace;
    }
    

    //sequence to replace a list of cartiles, each deleted at delay time given
    Sequence ReplaceList(List<GameObject> list, float delay = 0)
    {
        Sequence replace = DOTween.Sequence().Pause();

        if (list != null)
        {
            if (list.Count > 0)
            {
                foreach (GameObject obj in list)
                {
                    replace.Insert(0, ReplaceSequence(obj, delay));
                }
            }
        }

        return replace;
    }

    //replace all match 3s, at delay specified
    IEnumerator ReplaceMatch3s(float delay = 0)
    {
        //for every different tile type
        for (int i = 0; i < tilePrefabs.Length; i++)
        {
            //get all tiles of the tile type i
            List<GameObject> matches = tiles.FindAll(tile => tile.GetComponent<CarTile>().GetNum() == i);
            //get all match 3s of those tiles
            List<GameObject> match3 = Match3(matches);

            //if match 3s found, repeat process again
            if (match3.Count > 0)
            {
                Sequence replaceList = ReplaceList(match3, delay);
                replaceList.Play();

                if (replaceList != null && replaceList.IsActive())
                {
                    yield return replaceList.WaitForCompletion();
                }

                yield return StartCoroutine(ReplaceMatch3s(delay));
                
            }
        }
    }

    IEnumerator ReplaceMatch3sAtStart()
    {
        yield return StartCoroutine(ReplaceMatch3s());
        GetMatchesCount();
    }

    public IEnumerator ManageClicks()
    {
        //if tile clicked is second tile selecte
        //get tiles selected
        List<GameObject> selected = tiles.FindAll(tile => tile.GetComponent<CarTile>().isClicked());

        CarTile carTile1 = selected[0].GetComponent<CarTile>();
        CarTile carTile2 = selected[1].GetComponent<CarTile>();

        //if tile is adjacent swap tiles
        //then get list of all matching tiles, then use that list to get match 3s
        if (IsAdjacent(carTile1, carTile2))
        {

            Sequence swap1 = DOTween.Sequence().SetAutoKill(false);

            yield return swap1.Append(SwapTiles(carTile1, carTile2)).WaitForCompletion();

            swap1.Kill();

            List<GameObject> otherMatch3 = Match3(AdjacentMatches(carTile1));
            List<GameObject> thisMatch3 = Match3(AdjacentMatches(carTile2));

            if (otherMatch3.Count == 0 && thisMatch3.Count == 0)
            {
                yield return new WaitForSeconds(0.15f);

                Sequence swap2 = DOTween.Sequence().SetAutoKill(false);

                yield return swap2.Append(SwapTiles(carTile1, carTile2)).WaitForCompletion();

                swap2.Kill();

                moving = false;

            }
            else
            {
                yield return StartCoroutine(ReplaceMatch3s(0.3f));

                moving = false;

                int num = GetMatchesCount() - 1;

                for (int i = 0; i < num; i++)
                {
                    GameManager.gameManager.DecreaseSpeed();
                }
                GetMatchesCount();
            }
        }
        //if this tile is not adjacent, deselect all
        else
        {
            yield return new WaitForSeconds(0.3f);

            yield return DeselectAll(carTile1, carTile2).WaitForCompletion();
        }


    }

    public void Reset()
    {
        if (!stop && !moving)
        {
            foreach (GameObject obj in tiles)
            {

                Destroy(obj);
            }

            tiles.Clear();

            GridAssign();

            GameManager.gameManager.IncreaseSpeed();
        }
    }

    void Stop()
    {
        stop = true;
    }

}
