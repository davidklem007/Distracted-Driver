using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    //[rows][columns]
    [SerializeField] int rows;
    [SerializeField] int columns;
    List<GameObject> tiles;
    GameObject[] tilePrefabs;

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
        for(int i = 0; i < rows; i++)
        { 
            for(int j = 0; j < columns; j++)
            {
                int carNum = Random.Range(0, tilePrefabs.Length);
                GameObject carTile = Instantiate(tilePrefabs[carNum], new Vector3(j, i, 0), Quaternion.identity);
                carTile.GetComponent<CarTile>().SetPosition(i, j);
                tiles.Add(carTile);
            }
        }
    }
}
