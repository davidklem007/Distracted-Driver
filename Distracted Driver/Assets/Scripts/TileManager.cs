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

    [SerializeField] float xOffset;
    [SerializeField] float yOffset;
    float xSpacing;
    float ySpacing;

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
        if (amtClicked == 2 && !moving)
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
        xSpacing = xBounds / (columns - 1);
        ySpacing = yBounds / (rows - 1);

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
            .OnKill(() =>
            {
                moving = false;
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
    List<GameObject> Match3(List<GameObject> matches)
    {
        List<GameObject> matches3 = new List<GameObject>();

        //counts tiles for a set of matched tiles
        int matchingTileCount = 0;

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
                    if (matchingTileCount == 0)
                    {
                        matchingTileCount += 2;
                    }
                    else
                    {
                        matchingTileCount++;
                    }
                }
                //if the two tiles checked aren't matching and in the same row
                else
                {
                    //this is to check the tiles that came before
                    //if there were at least three matching tiles before, add all of them to the matches3 list if they are not already in that list
                    //then reset the matched count because we will check a new row next
                    if (matchingTileCount >= 3)
                    {
                        AddMatchGroup(matches, matches3, i, matchingTileCount);
                    }
                    matchingTileCount = 0;
                }
            }
            //if there isn't a horizontally adjacent tile (current tile is on an edge)
            else
            {
                //since we are at the end of the row now, add previous tiles to the match3 list if there was a match3
                //and reset matched count to zero because we will start in a new row next
                if (matchingTileCount >= 3)
                {
                    AddMatchGroup(matches, matches3, i, matchingTileCount);
                }
                matchingTileCount = 0;
            }
        }

        //after going through all the tiles, if there was a match3 at the end of the row this checks that and adds the matching tiles
        if (matchingTileCount >= 3)
        {
            AddMatchGroup(matches, matches3, matches.Count - 1, matchingTileCount);
        }

        //reset the count to check by column now
        matchingTileCount = 0;


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
                    if (matchingTileCount == 0)
                    {
                        matchingTileCount += 2;
                    }
                    else
                    {
                        matchingTileCount++;
                    }
                }
                else
                {
                    if (matchingTileCount >= 3)
                    {
                        AddMatchGroup(matches, matches3, i, matchingTileCount);
                    }
                    matchingTileCount = 0;
                }
            }
            else
            {
                if (matchingTileCount >= 3)
                {
                    AddMatchGroup(matches, matches3, i, matchingTileCount);
                }
                matchingTileCount = 0;
            }
        }

        if (matchingTileCount >= 3)
        {
            for (int x = 0; x < matchingTileCount; x++)
            {
                AddMatchGroup(matches, matches3, matches.Count - 1, matchingTileCount);
            }
        }

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
        int curCount = 0;

        Sequence replaceList = DOTween.Sequence().Pause()
            .OnStart(() => { moving = true; })
            .OnKill(() => { moving = false; });
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
                curCount += match3.Count;
                replaceList.Insert(0, ReplaceList(match3, delay));
            }
        }

        yield return replaceList.Play().WaitForCompletion();


        //yield return new WaitUntil(() => moving == false);
        if (curCount > 0)
        {
            yield return StartCoroutine(ReplaceMatch3s(delay));
        }
    }

    //dynamic tile replacing
    IEnumerator ReplaceMatch3sDynamic()
    {
        //stores all tiles being deleted
        List<GameObject> deleted = new List<GameObject>();
        //stores all unique columns that have tiles being deleted
        List<int> deletedCols = new List<int>();

        int curCount = 0;

        Sequence replaceList = DOTween.Sequence().Pause().SetAutoKill(false)
            .OnStart(() => { moving = true; })
            .OnKill(() => { moving = false; });
        //for every different tile type
        for (int i = 0; i < tilePrefabs.Length; i++)
        {
            //get all tiles of the tile type i
            List<GameObject> matches = tiles.FindAll(tile => tile.GetComponent<CarTile>().GetNum() == i);
            //get all match 3s of those tiles
            List<GameObject> match3 = Match3(matches);

            //for every tile in a set of 3
            foreach (GameObject obj in match3)
            {
                //add tile to deleted list if it is not already there, and add column to deleted column list if not already there
                if (!deleted.Contains(obj))
                {
                    deleted.Add(obj);
                    if (!deletedCols.Contains(obj.GetComponent<CarTile>().GetColumn()))
                    {
                        deletedCols.Add(obj.GetComponent<CarTile>().GetColumn());
                    }
                }
            }
        }

        //update count of tiles to be deleted
        curCount = deleted.Count;

        yield return new WaitUntil(() => !moving);

        //add ReplaceCol sequence for each column with tiles being deleted
        for(int i = 0; i < deletedCols.Count; i++)
        {
            replaceList.Join(ReplaceCol(deleted.FindAll(obj => obj.GetComponent<CarTile>().GetColumn() == deletedCols[i]), deletedCols[i]));
        }

        yield return replaceList.Play().WaitForCompletion();

        replaceList.Kill();

        yield return new WaitUntil(() => !moving);
        if (curCount > 0)
        {
            //if there was at least one set of 3, restart coroutine to replace any new sets of 3
            yield return StartCoroutine(ReplaceMatch3sDynamic());
        }
    }

    //replace all tiles in list from column col
    Sequence ReplaceCol(List<GameObject> list, int col)
    {
        Sequence colReplace = DOTween.Sequence().Pause();

        //x position of the column
        float xPos = list[0].transform.position.x;

        int maxHeight = -1;

        //find the highest tile being deleted
        foreach(GameObject obj in list)
        {
            if(obj.GetComponent<CarTile>().GetRow() > maxHeight)
            {
                maxHeight = obj.GetComponent<CarTile>().GetRow();
            }
        }


        int minHeight = rows;

        //find the highest tile being deleted
        foreach (GameObject obj in list)
        {
            if (obj.GetComponent<CarTile>().GetRow() < minHeight)
            {
                minHeight = obj.GetComponent<CarTile>().GetRow();
            }
        }

        //stores every tile in the column that needs to be moved
        List<GameObject> toMove = new List<GameObject>();

        //all tiles that are not in list to be deleted, are in the given column, and are below (visually) the highest row tile being deleted
        //in other words, these are all the current tiles that will need to be moved to a new position
        List<GameObject> rest = tiles.FindAll(obj => !list.Contains(obj) && obj.GetComponent<CarTile>().GetColumn() == col && obj.GetComponent<CarTile>().GetRow() > maxHeight);

        //find all stranded tiles
        List<GameObject> island = tiles.FindAll(obj => !list.Contains(obj) && obj.GetComponent<CarTile>().GetColumn() == col && obj.GetComponent<CarTile>().GetRow() < maxHeight && obj.GetComponent<CarTile>().GetRow() > minHeight);

        foreach (GameObject obj in island)
        {
            int row = obj.GetComponent<CarTile>().GetRow();
            int num = obj.GetComponent<CarTile>().GetNum();

            int newRow = row - list.FindAll(tile => tile.GetComponent<CarTile>().GetRow() < row).Count;

            obj.GetComponent<CarTile>().SetCar(newRow, col, num);
            toMove.Add(obj);
        }
        

        //set the new row and column for the tiles in rest, then add them to toMove list
        foreach (GameObject obj in rest)
        {
            int row = obj.GetComponent<CarTile>().GetRow();
            int num = obj.GetComponent<CarTile>().GetNum();
            obj.GetComponent<CarTile>().SetCar(row - list.Count, col, num);
            toMove.Add(obj);
        }

        //hide tiles to be deleted and remove them from the tiles list
        foreach (GameObject obj in list)
        {
            obj.SetActive(false);
            tiles.Remove(obj);
        }

        //spawns new tiles off screen
        //sets their new car coordinates and add them to tiles list and then toMove list
        for(int i = 0; i < list.Count; i++)
        {
            Vector3 position = new Vector3(xPos, -5.415881f, 0);

            int carNum = Random.Range(0, tilePrefabs.Length);
            GameObject carTile = Instantiate(tilePrefabs[carNum], position, Quaternion.identity);

            int newRow = rows - 1 - i;

            carTile.GetComponent<CarTile>().SetCar(rows - 1 - i, col, carNum);

            tiles.Add(carTile);
            toMove.Add(carTile);

        }

        //sort toMove list by row, lowest to highest row num
        toMove.Sort((tile1, tile2) =>
        {
            CarTile carTile1 = tile1.GetComponent<CarTile>();
            CarTile carTile2 = tile2.GetComponent<CarTile>();
            int rowComparison = carTile1.GetRow().CompareTo(carTile2.GetRow());
            return rowComparison;
        });

        //move each tile in toMove to their new position
        for (int i = 0; i < toMove.Count; i++)
        {
            int row = toMove[i].GetComponent<CarTile>().GetRow();
            float yPos = transform.TransformPoint(new Vector3(xPos, (-row * ySpacing) - yOffset, 0)).y;

            Vector3 position = new Vector3(xPos, yPos, 0);

            colReplace.Join(toMove[i].transform.DOMoveY(yPos, 0.5f + (i*0.07f)).SetEase(Ease.InOutQuad));
        }

        //delete all tiles in deleted list to free up memory
        foreach(GameObject obj in list)
        {
            Destroy(obj);
        }



        return colReplace;

    }

    IEnumerator ReplaceMatch3sAtStart()
    {
        yield return StartCoroutine(ReplaceMatch3s());
        GetMatchesCount();
    }

    public IEnumerator ManageClicks()
    {
        yield return new WaitUntil(() => moving == false);
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

            //run match3 check to have match3sCount
            Match3(AdjacentMatches(carTile1));
            Match3(AdjacentMatches(carTile2));

            if (match3sCount == 0)
            {
                yield return new WaitForSeconds(0.1f);

                Sequence swap2 = DOTween.Sequence().SetAutoKill(false);

                yield return swap2.Append(SwapTiles(carTile1, carTile2)).WaitForCompletion();

                swap1.Kill();

                swap2.Kill();
                

            }
            else
            {
                swap1.Kill();

                yield return StartCoroutine(ReplaceMatch3sDynamic());

                int num = GetMatchesCount() - 1;

                for (int i = 0; i < num; i++)
                {
                    GameManager.gameManager.DecreaseSpeed();
                }
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

            GameManager.gameManager.IncreaseSpeed(0.45f, false);
        }
    }

    void Stop()
    {
        stop = true;
    }

}
