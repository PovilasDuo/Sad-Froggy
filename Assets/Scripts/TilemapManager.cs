using System.Collections;
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
	public GameObject border;

	[Header("Obstacle counts")]
	public int roadCount = 2;
	public int riverCount = 2;
	public int rockCount = 5;
	public int tadpoleCount = 7;

	[Header("Doors")]
	public GameObject exitDoor;
	public GameObject nextLevelDoor;

	private bool[] occupiedRows;
	private bool[][] occupiedTiles;

	private int borderNumber = 0;
	private int borderCount = 0;
	private int exitGap, nextLevelGap;

	public GameObject levelGameobject;
	private float levelNr = 1;


	private void Awake()
	{
		levelGameobject = new GameObject();
		levelGameobject.name = "Level " + levelNr.ToString();
	}
	void Start()
	{

		tilemap = GetComponent<Tilemap>();
	}

	/// <summary>
	/// Generates a map of default size
	/// </summary>
	public void GenerateMap()
	{
		ClearOccupation();
		SetTilemapSize(tilesNumberX, tilesNumberZ);
		SetTilemapPosition();

		CreateLinearObstacles(roadTile, roadCount);
		CreateLinearObstacles(riverTile, riverCount);
		CreateObstacles(rock, rockCount);

		CreateCollectibles(tadpole, tadpoleCount);
		SpawnKeyRandom();

		CreateBase();
		CreateBorders(true, true, true, true);
		PrepareNextLevel();
	}

	public void GenerateMap(Vector3 position)
	{
		ClearOccupation();
		SetTilemapSize(tilesNumberX, tilesNumberZ);
		SetTilemapPosition();

		CreateLinearObstacles(roadTile, roadCount);
		CreateLinearObstacles(riverTile, riverCount);
		CreateObstacles(rock, rockCount);

		CreateCollectibles(tadpole, tadpoleCount);
		SpawnKeyRandom();

		CreateBase();

		if ((position.x >= 0 && position.z >= 0) && (position.x > position.z))
		{
			Debug.Log(position.x + " and " + position.z);
			Debug.Log("right");
			levelGameobject.transform.position = new Vector3(levelGameobject.transform.position.x + (tilesNumberX + 1) * tileSize, 0, levelGameobject.transform.position.z);
			StartCoroutine(SmoothCameraMovement(new Vector3(Camera.main.transform.position.x + (tilesNumberX + 1) * tileSize * 1f, Camera.main.transform.position.y, Camera.main.transform.position.z), 3f));
			CreateBorders(true, true, true, false);
		}
		else if (position.x < 0 && position.z >= 0)
		{
			Debug.Log("left");
			levelGameobject.transform.position = new Vector3(levelGameobject.transform.position.x - (tilesNumberX + 1) * tileSize, 0, levelGameobject.transform.position.z);
			StartCoroutine(SmoothCameraMovement(new Vector3(Camera.main.transform.position.x - (tilesNumberX + 1) * tileSize * 1f, Camera.main.transform.position.y, Camera.main.transform.position.z), 3f));
			CreateBorders(true, true, false, true);
		}
		else if (position.z >= 0 && position.z > position.x)
		{
			Debug.Log("top"); //neveikia
			levelGameobject.transform.position = new Vector3(levelGameobject.transform.position.x, 0, levelGameobject.transform.position.z + (tilesNumberZ + 1) * tileSize);
			StartCoroutine(SmoothCameraMovement(new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z + (tilesNumberX + 1) * tileSize * 1f), 3f));
			CreateBorders(true, false, true, true);
		}
		else// if (position.x <= 0 && position.z <= 0)
		{
			Debug.Log("bottom"); //neveikia
			levelGameobject.transform.position = new Vector3(levelGameobject.transform.position.x, 0, levelGameobject.transform.position.z - (tilesNumberZ + 1) * tileSize);
			StartCoroutine(SmoothCameraMovement(new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z - (tilesNumberX + 1) * tileSize * 1f), 3f));
			CreateBorders(false, true, true, true);
		}
		PrepareNextLevel();
	}

	IEnumerator SmoothCameraMovement(Vector3 targetPosition, float duration)
	{
		Vector3 startPosition = Camera.main.transform.position;
		float startTime = Time.time;

		while (Time.time - startTime < duration)
		{
			float t = (Time.time - startTime) / duration;
			Camera.main.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
			yield return null; // Wait for the next frame
		}

		// Ensure that the camera reaches the exact target position
		Camera.main.transform.position = targetPosition;
	}


	public int CalculatePosition(Vector3 position)
	{
		if (levelNr == 1)
		{
			if (position.x >= 0 && position.z >= 0)
			{
				return (tilesNumberX + 2) * tileSize;
			}
			else if (position.x <= 0 && position.z >= 0)
			{
				return -(tilesNumberX + 2) * tileSize;
			}
			else if (position.z < 0)
			{
				return -(tilesNumberZ + 2) * tileSize;
			}
		}
		return 0;
	}

	private void ClearOccupation()
	{
		occupiedRows = new bool[tilesNumberZ];
		occupiedTiles = new bool[tilesNumberX][];
		for (int i = 0; i < tilesNumberX; i++)
		{
			occupiedTiles[i] = new bool[tilesNumberZ];
		}
		borderCount = 0;
		borderNumber = 0;
	}

	private void PrepareNextLevel()
	{
		Vector3 position = levelGameobject.transform.position;
		levelNr++;
		levelGameobject = new GameObject();
		levelGameobject.name = "Level " + levelNr.ToString();
		levelGameobject.transform.position = position;
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
		tilemap.transform.position = new Vector3(-tileSize * tilesNumberX / 2 + tileSize / 2, 0, 0);
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
				occupiedTiles[x][obstaclePosition] = true;
				Vector3 worldPos = tilemapOrigin + new Vector3(x * tileSize, 0, obstaclePosition * tileSize);

				GameObject go = Instantiate(obstacle, worldPos, Quaternion.identity);
				go.transform.SetParent(levelGameobject.transform, false);
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

			Vector3 worldPos = tilemapOrigin + new Vector3(obstaclePosition.x * tileSize, obstacle.GetComponent<Renderer>().bounds.size.y / 2, obstaclePosition.z * tileSize);

			GameObject go = Instantiate(obstacle, worldPos, Quaternion.identity);
			go.transform.SetParent(levelGameobject.transform, false);
		}
	}

	public void CreateCollectibles(GameObject obstacle, int obstacleCount)
	{
		Vector3 tilemapOrigin = tilemap.transform.position;

		for (int c = 0; c < obstacleCount; c++)
		{
			Vector3 obstaclePosition = GetRandomCell();
			occupiedTiles[(int)obstaclePosition.x][(int)obstaclePosition.z] = true;

			Vector3 worldPos = tilemapOrigin + new Vector3(obstaclePosition.x * tileSize, 2, obstaclePosition.z * tileSize);

			GameObject go = Instantiate(obstacle, worldPos, Quaternion.identity);
			go.transform.SetParent(levelGameobject.transform, false);
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
					go.transform.SetParent(levelGameobject.transform, false);
				}
			}
		}
	}

	public void CreateBorders(bool top, bool bottom, bool right, bool left)
	{
		Vector3 tilemapOrigin = tilemap.transform.position;
		Vector3Int mapSize = tilemap.size;

		borderCount = (tilesNumberX + tilesNumberZ) * 2 + 2;
		GetRandomGaps(top, bottom, right, left);

		if (top)
		{
			for (int x = -1; x <= mapSize.x; x++)
			{
				CreateBorderTile(x * tileSize, 0, mapSize.z * tileSize, tilemapOrigin); // Bottom border
			}
		}

		if (right)
		{
			for (int z = mapSize.z; z >= 0; z--)
			{
				CreateBorderTile(mapSize.x * tileSize, 0, z * tileSize, tilemapOrigin); // Right border
			}
		}

		if (bottom)
		{
			for (int x = mapSize.x; x >= -1; x--)
			{
				CreateBorderTile(x * tileSize, 0, -tileSize, tilemapOrigin); // Top border
			}
		}


		if (left)
		{
			for (int z = 0; z <= mapSize.z; z++)
			{
				CreateBorderTile(-tileSize, 0, z * tileSize, tilemapOrigin); // Left border
			}
		}
	}

	private void CreateBorderTile(float x, float y, float z, Vector3 origin)
	{
		borderNumber++;
		Vector3 worldPos = new Vector3(x, y, z) + origin;
		GameObject go = Instantiate(grassTile, worldPos, Quaternion.identity);
		go.transform.SetParent(levelGameobject.transform, false);

		if (borderNumber == exitGap)
		{
			worldPos.y += 3; // TODO remove when we have a model for door
			go = Instantiate(exitDoor, worldPos, Quaternion.identity);
		}
		else if (borderNumber == nextLevelGap)
		{
			worldPos.y += 3; // TODO remove when we have a model for door
			go = Instantiate(nextLevelDoor, worldPos, Quaternion.identity);
		}
		else
		{
			go = Instantiate(border, worldPos, Quaternion.identity);
		}
		go.transform.SetParent(levelGameobject.transform, false);
	}


	private void GetRandomGaps(bool top, bool bottom, bool right, bool left)
	{
		int randomN = (int)Mathf.Round(Random.Range(1f, 8f));
		int addative = 2;
		//if (!top)
		//{
		//	randomN = (int)Mathf.Round(Random.Range(3f, 6f));
		//	addative = 2;
		//}
		//else if (!bottom)
		//{
		//	if (Random.value < 0.5f) randomN = (int)Mathf.Round(Random.Range(1f, 2f));
		//	else randomN = (int)Mathf.Round(Random.Range(7f, 8f));
		//	addative = 2;
		//}
		//else if (!right)
		//{
		//	randomN = (int)Mathf.Round(Random.Range(5f, 8f));
		//	addative = 2;
		//}
		//else if (!left)
		//{
		//	randomN = (int)Mathf.Round(Random.Range(1f, 4f));
		//	addative = 2;
		//}


		if (!top || !bottom || !right || !left)
		{
			randomN = (int)Mathf.Round(Random.Range(1f, 4f));
			addative = 2;
		}

		float[] multiplactionOdd = new float[] { 3.5f, 6f, 8f };
		float[] multiplcationNOdd = new float[] { 3.5f, 5.5f, 7.5f };
		float[] multiplcationEven = new float[] { 3f, 5f, 6.5f };
		var multiplication = new float[3];

		if ((tilesNumberZ % 2 == 0 && tilesNumberX % 2 != 0) || (tilesNumberZ % 2 != 0 && tilesNumberX % 2 == 0)) multiplication = multiplactionOdd;
		else if (tilesNumberZ % 2 != 0 && tilesNumberX % 2 != 0) multiplication = multiplcationNOdd;
		else multiplication = multiplcationEven;

		if (randomN == 1)
		{
			exitGap = Mathf.CeilToInt((tilesNumberX + addative) / 2f);
			nextLevelGap = Mathf.CeilToInt((tilesNumberX + addative) / 2 * multiplication[0]);
		}
		else if (randomN == 2)
		{
			nextLevelGap = Mathf.CeilToInt((tilesNumberX + addative) / 2f);
			exitGap = Mathf.CeilToInt((tilesNumberX + addative) / 2 * multiplication[0]);
		}
		else if (randomN == 3)
		{
			nextLevelGap = Mathf.CeilToInt((tilesNumberX + addative) / 2 * multiplication[0]);
			exitGap = Mathf.CeilToInt((tilesNumberX + addative) / 2 * multiplication[1]);
		}
		else if (randomN == 4)
		{
			exitGap = Mathf.CeilToInt((tilesNumberX + addative) / 2 * multiplication[0]);
			nextLevelGap = Mathf.CeilToInt((tilesNumberX + addative) / 2 * multiplication[1]);
		}
		else if (randomN == 5)
		{
			nextLevelGap = Mathf.CeilToInt((tilesNumberX + addative) / 2 * multiplication[2]);
			exitGap = Mathf.CeilToInt((tilesNumberX + addative) / 2 * multiplication[1]);
		}
		else if (randomN == 6)
		{
			exitGap = Mathf.CeilToInt((tilesNumberX + addative) / 2 * multiplication[2]);
			nextLevelGap = Mathf.CeilToInt((tilesNumberX + addative) / 2 * multiplication[1]);
		}
		else if (randomN == 7)
		{
			nextLevelGap = Mathf.CeilToInt((tilesNumberX + addative) / 2 * multiplication[2]);
			exitGap = Mathf.CeilToInt((tilesNumberX + addative) / 2f);
		}
		else if (randomN == 8)
		{
			exitGap = Mathf.CeilToInt((tilesNumberX + addative) / 2 * multiplication[2]);
			nextLevelGap = Mathf.CeilToInt((tilesNumberX + addative) / 2f);
		}
		Debug.Log("random: " + randomN + "; " + "exitGap: " + exitGap + " nextLevelGap: " + nextLevelGap + " addative:" + addative);
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
			cellZ = Random.Range(1, tilesNumberZ);
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
		go.transform.SetParent(levelGameobject.transform, false);
	}
}
