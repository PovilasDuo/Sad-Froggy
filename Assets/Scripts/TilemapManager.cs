using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapManager : MonoBehaviour
{
    [Header("Tilemap")]
    public Tilemap tilemap;
    public int tilesNumberX = 11;
    public int tilesNumberZ = 11;
    private int tileSize = 10;

    [Header("Obstacles")]
    public GameObject roadTile;
    public GameObject riverTile;
    public GameObject grassTile;
    public GameObject mushroom;
    public GameObject rock;

    [Header("Obstacle counts")]
    public int roadCount = 2;
    public int riverCount = 2;
    public int rockCount = 5;
    public int mushroomCount = 7;

    private bool[] occupiedRows;
    private bool[][] occupiedTiles;

    void Start()
    {
        tilemap = GetComponent<Tilemap>();
        occupiedRows = new bool[tilesNumberZ];
        occupiedTiles = new bool[tilesNumberX][];
        for (int i = 0; i < tilesNumberX; i++)
        {
            occupiedTiles[i] = new bool[tilesNumberZ];
        }

        SetTilemapSize();
        SetTilemapPosition();

        CreateLinearObstacles(roadTile, roadCount);
        CreateLinearObstacles(riverTile, riverCount);
        CreateObstacles(mushroom, mushroomCount);
        CreateObstacles(rock, rockCount);

        CreateBase();
    }

    /// <summary>
    /// sets prefered tilemap size
    /// </summary>
    private void SetTilemapSize()
    {
        tilemap.size = new Vector3Int(tilesNumberX, 1, tilesNumberZ);
        tilemap.tileAnchor = new Vector3(0f, 0f, 0f);
    }

    /// <summary>
    /// set tilemap in the middle
    /// </summary>
    private void SetTilemapPosition()
    {
        tilemap.transform.position = new Vector3(-tileSize * tilesNumberX / 2 + tileSize/2, 0, 0);
    }

    /// <summary>
    /// creates linear obstacles like roads
    /// </summary>
    /// <param name="obstacle">square obstacle object</param>
    /// <param name="obstacleCount">count of obstacles (e.g. roads)</param>
    public void CreateLinearObstacles(GameObject obstacle, int obstacleCount)
    {
        Vector3 tilemapOrigin = tilemap.transform.position;

        for (int c = 0; c < obstacleCount; c++)
        {
            int obstaclePosition = GetRandomEmptyRow();
            occupiedRows[obstaclePosition] = true;

            for (int x = 0; x < tilesNumberX; x++)
            {
                Vector3 worldPos = tilemapOrigin + new Vector3(x * tileSize, 0, obstaclePosition * tileSize);
                Instantiate(obstacle, worldPos, Quaternion.identity);
            }
        }
    }

    /// <summary>
    /// creates simple obstacles
    /// </summary>
    /// <param name="obstacle">obstacle object</param>
    /// <param name="obstacleCount">count of obstacles (e.g. rocks)</param>
    public void CreateObstacles(GameObject obstacle, int obstacleCount)
    {
        Vector3 tilemapOrigin = tilemap.transform.position;

        for (int c = 0; c < obstacleCount; c++)
        {
            Vector3 obstaclePosition = GetRandomCell();
            occupiedTiles[(int)obstaclePosition.x][(int)obstaclePosition.z] = true;

            Vector3 worldPos = tilemapOrigin + new Vector3(obstaclePosition.x * tileSize, obstacle.GetComponent<Renderer>().bounds.size.y/2, obstaclePosition.z * tileSize);
            Instantiate(obstacle, worldPos, Quaternion.identity);
        }
    }

    /// <summary>
    /// creates base under obstacles(e.g. grass)
    /// </summary>
    public void CreateBase()
    {
        Vector3 tilemapOrigin = tilemap.transform.position;

        for (int z = 0; z < tilesNumberZ; z++)
        {
            if (occupiedRows[z] == false)
            {
                for (int x = 0; x < tilesNumberX; x++)
                {
                    Vector3 worldPos = tilemapOrigin + new Vector3(x * tileSize, 0, z * tileSize);
                    Instantiate(grassTile, worldPos, Quaternion.identity);
                }
            }
        }
    }

    /// <summary>
    /// returns row where no other obstacles exist
    /// </summary>
    /// <returns>index of free row</returns>
    private int GetRandomEmptyRow()
    {
        int row;
        do
        {
            row = Random.Range(1, tilesNumberZ);
        } while (occupiedRows[row]);

        return row;
    }


    /// <summary>
    /// returns cell where no other obstacles exist
    /// </summary>
    /// <returns>coordinates of free cell</returns>
    private Vector3 GetRandomCell()
    {
        int cellX, cellZ;
        do
        {
            cellX = Random.Range(0, tilesNumberX);
            cellZ = Random.Range(1, tilesNumberX);
        } while (occupiedTiles[cellX][cellZ]);

        return new Vector3(cellX, 0, cellZ);
    }
}
