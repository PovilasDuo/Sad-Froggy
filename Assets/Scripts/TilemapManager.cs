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
    public GameObject tadpole;
    public GameObject rock;
    public GameObject key;

    [Header("Obstacle counts")]
    public int roadCount = 2;
    public int riverCount = 2;
    public int rockCount = 5;
    public int tadpoleCount = 7;

    private bool[] occupiedRows;
    private bool[][] occupiedTiles;

    void Start()
    {
        tilemap = GetComponent<Tilemap>();
    }

	/// <summary>
	/// Generates a map of default size
	/// </summary>
	public void GenerateMap()
    {
		occupiedRows = new bool[tilesNumberZ];
		occupiedTiles = new bool[tilesNumberX][];
		for (int i = 0; i < tilesNumberX; i++)
		{
			occupiedTiles[i] = new bool[tilesNumberZ];
		}

		SetTilemapSize(tilesNumberX, tilesNumberZ);
		SetTilemapPosition();

		CreateLinearObstacles(roadTile, roadCount);
		CreateLinearObstacles(riverTile, riverCount);
		CreateObstacles(tadpole, tadpoleCount);
		CreateObstacles(rock, rockCount);
        SpawnKeyRandom();

		CreateBase();
	}

	/// <summary>
	/// Generates a map of desired size
	/// </summary>
	/// <param name="sizeX">Tiles Number X</param>
	/// <param name="sizeZ">Tiles Number Z</param>
	/// <param name="roadCount">Number of roads</param>
	/// <param name="riverCount">Number of rivers</param>
	/// <param name="rockCount">Number of rocks</param>
	/// <param name="tadpoleCount">Number of tadpoles</param>
	public void GenerateMap(int sizeX, int sizeZ, int roadCount, int riverCount, int rockCount, int tadpoleCount)
	{
		occupiedRows = new bool[sizeZ];
		occupiedTiles = new bool[sizeX][];
		for (int i = 0; i < sizeX; i++)
		{
			occupiedTiles[i] = new bool[sizeZ];
		}

		SetTilemapSize(sizeX, sizeZ);
		SetTilemapPosition();

		CreateLinearObstacles(roadTile, roadCount);
		CreateLinearObstacles(riverTile, riverCount);
		CreateObstacles(tadpole, tadpoleCount);
		CreateObstacles(rock, rockCount);
        SpawnKeyRandom();

		CreateBase();
	}

	/// <summary>
	/// sets prefered tilemap size
	/// </summary>
    /// <param name="sizeX">Tiles Number X</param>
	/// <param name="sizeZ">Tiles Number Z</param>
	private void SetTilemapSize(int sizeX, int sizeZ)
    {
        tilemap.size = new Vector3Int(sizeX, 1, sizeZ);
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

                GameObject go = Instantiate(obstacle, worldPos, Quaternion.identity);
                go.transform.SetParent(this.transform, true);
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

			GameObject go = Instantiate(obstacle, worldPos, Quaternion.identity);
			go.transform.SetParent(this.transform, true);
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

					GameObject go = Instantiate(grassTile, worldPos, Quaternion.identity);
					go.transform.SetParent(this.transform, true);
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

    /// <summary>
    /// Spawns a key in a random position
    /// </summary>
    public void SpawnKeyRandom()
    {
		Vector3 tilemapOrigin = tilemap.transform.position;
		Vector3 obstaclePosition = GetRandomCell();
		occupiedTiles[(int)obstaclePosition.x][(int)obstaclePosition.z] = true;

		Vector3 worldPos = tilemapOrigin + new Vector3(obstaclePosition.x * tileSize, key.GetComponent<Renderer>().bounds.size.y / 2, obstaclePosition.z * tileSize);

		GameObject go = Instantiate(key, worldPos, Quaternion.identity);
		go.transform.SetParent(this.transform, true);
	}
}
